create or replace function aplus_formsetting_savevalidvalue()
	returns table (msg_code int, mess_info character varying(254), list_setting character varying(254))
	language 'plpgsql'
as $BODY$
declare 
	_maxEntry int;
BEGIN
	--xóa các dữ liệu setting rỗng
	delete from _form_setting_value where coalesce(key_code, '') = '' or coalesce(column_id, '') = '';
	if not exists(select 1 from _form_setting)
		or exists(select 1 from _form_setting where coalesce(form_id, '') = '' or coalesce(item_id, '') = '' or coalesce(language_code, '') = '') then
		return query select 10353, 'Dữ liệu cài đặt không được cung cấp đầy đủ', null :: character varying;
		return;
	END IF;
	
	if exists(select 1 from _form_setting group by form_id, item_id, language_code having count(1) > 1) then
		return query select 10355, 'Dữ liệu cài đặt của chức năng bị trùng. Vui lòng thử lại', null :: character varying;
		return;
	end if;
	
	alter table _form_setting add column temp_Code integer;
	
	--lấy thông tin form setting cũ trên hệ thống
	update _form_setting a set temp_code = b.setting_entry from form_setting b where a.form_id = b.form_id and a.item_id = b.item_id and a.language_code = b.language_code;
	-- khởi tạo các entry cho các setting chưa có trên hệ thống
	select max(setting_entry) from form_setting into _maxEntry;
	_maxEntry = coalesce(_maxEntry,0) + 1;
	update _form_setting a set temp_code = _maxEntry + b.temp_id
			from (select setting_entry, row_number() over(order by setting_entry) temp_id from _form_setting where temp_code is null) b where a.setting_entry = b.setting_entry;
	
	update _form_setting_value a set setting_entry = b.temp_code from _form_setting b where a.setting_entry = b.setting_entry;
	update _form_setting set setting_entry = temp_code;
	alter table _form_setting drop column temp_code;
	
	delete from form_setting a 
				using _form_setting b where a.setting_entry = b.setting_entry;
	delete from form_setting_value a 
				using _form_setting b where a.setting_entry = b.setting_entry;
	
	insert into form_setting select * from _form_setting;
	insert into form_setting_value select * from _form_setting_value;
	
	return query select 200 :: integer, '' :: character varying, (select string_agg(setting_entry || ',' || column_id, ';') from (select setting_entry, column_id from _form_setting_value group by setting_entry, column_id) x) :: character varying;
	
END; $BODY$