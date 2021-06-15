create or replace function aplus_formsetting_save(_user_sign smallint)
	returns table (msg_code int, mess_info character varying(5000), list_setting character varying(254))
	language 'plpgsql'
as $BODY$
declare 
	_maxEntry int;
BEGIN

	--xóa các dữ liệu setting rỗng
	delete from _form_setting_user where COALESCE(column_id, '') = '' or COALESCE(user_id, -1) < 1;
	if not exists(select 1 from _form_setting) or not exists(select 1 from _form_setting_user) 
		or exists(select 1 from _form_setting where COALESCE(form_id, '') = '' or COALESCE(item_id, '') = '' or COALESCE(language_code, '') = '') then
		return query select 10354 :: int, N'Dữ liệu cài đặt không được cung cấp đầy đủ' :: character varying, null :: character varying;
		return;
	END IF;
	
	if exists(select 1 from _form_setting group by form_id, item_id, language_code having count(1) > 1) then
		return query select 10355 :: int, N'Dữ liệu cài đặt của chức năng bị trùng. Vui lòng thử lại' :: character varying, null :: character varying;
		return;
	end if;
	
	if (select count(1) from _form_setting) = 1 then
		update _form_setting set setting_entry = 1;
		update _form_setting_user set setting_entry = 1;
	end if;
	
	if exists(select 1 from _form_setting_user group by setting_entry, column_id, user_sign having count(1) > 1) then
		return query select 10355 :: int, N'Dữ liệu cài đặt của chức năng bị trùng. Vui lòng thử lại' :: character varying, null :: character varying;
		return;
	end if;
	
	alter table _form_setting add column temp_Code integer;
	
	--lấy thông tin form setting cũ trên hệ thống
	update _form_setting aa set temp_code = b.setting_entry from form_setting b where aa.form_id = b.form_id and aa.item_id = b.item_id and aa.language_code = b.language_code;
	-- khởi tạo các entry cho các setting chưa có trên hệ thống
	select max(setting_entry) from form_setting into _maxEntry;
	_maxEntry = COALESCE(_maxEntry,0);
	update _form_setting a set temp_code = _maxEntry + b.line_num
			from (select setting_entry, row_number() over(order by setting_entry) line_num from _form_setting where temp_code is null) b where a.setting_entry = b.setting_entry;
	
	update _form_setting_user ab set setting_entry = b.temp_code from _form_setting b where ab.setting_entry = b.setting_entry;
	update _form_setting_user ac set create_date = b.create_date, create_time = b.create_time
				, user_sign = b.user_sign from form_setting_user b where ac.setting_entry = b.setting_entry and ac.user_id = b.user_id and ac.column_id = b.column_id;
	update _form_setting_user set update_date = current_date, update_time = cast(TO_CHAR(now(), 'HHmm') as smallint), user_sign2 = _user_sign;
	update _form_setting_user set create_date = current_date, create_time = cast(TO_CHAR(now(), 'HHmm') as smallint), user_sign = _user_sign where user_sign is null;
	update _form_setting set setting_entry = temp_code;
	alter table _form_setting drop column temp_code;
	
	delete from form_setting ad 
				using _form_setting b where ad.setting_entry = b.setting_entry;
	delete from form_setting_user ag
				using _form_setting_user b where ag.setting_entry = b.setting_entry and ag.user_id = b.user_id;
				
	insert into form_setting select * from _form_setting;
	insert into form_setting_user select * from _form_setting_user;
	
	return query select 200 :: int, '' :: character varying, (select string_agg(setting_entry || ',' || user_id, ';') from (select setting_entry, user_id from _form_setting_user group by setting_entry, user_id) x) :: character varying;
	
END; $BODY$