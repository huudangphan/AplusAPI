create or replace function aplus_formsetting_get(_formId character varying(50), _userId int, _lngCode character varying(10), _itemId character varying(50) default '')
	returns setof refcursor
	LANGUAGE 'plpgsql'
AS $BODY$
declare
	_formData refcursor;
	_settingData refcursor;
	_validValueData refcursor;
BEGIN

	if _itemId is null then
		_itemId = '';
	end if;
	
	create local temporary table _settingTB(
		setting_entry integer,
		form_id character varying(50),
		item_id character varying(50),
		language_code character varying(5)
	);
	
	insert into _settingTB
		select setting_entry, form_id, item_id,language_code from apz_ofst where form_id = _formId and (_itemId = '' or item_id = _itemId) and language_code = _lngCode;
	
	--form data
	OPEN _formData FOR
        SELECT * from _settingTB;
    RETURN NEXT _formData;
	
	--setting data
	create local temporary table _fst1 as select a.*, null :: integer lay_order, null :: character varying(254) lay_name from apz_fst1 a inner join _settingTb b on a.setting_entry = b.setting_entry where user_id = _userId;
	if not exists(select 1 from _fst1) then
		insert into _fst1 select a.*, null :: integer lay_order, null :: character varying(254) lay_name from apz_fst1 a inner join _settingTb b on a.setting_entry = b.setting_entry where user_id = -1;
	END IF;
	
	---- update lại dữ liệu layout của các udf đã có form setting
	update _fst1 a set lay_order = d.line_num, lay_name = d.layout_name from _settingTB b, apz_oudf c, apz_udf1 d 
						where a.setting_entry = b.setting_entry and a.column_id = c.column_id and b.item_id = c.table_name 
								and c.layout_id = d.line_num and c.table_name = d.table_name;
								
	---- insert dữ liệu các udf chưa có trong form setting
	insert into _fst1(column_id, column_name, data_type, sub_type, maximum_length, is_visible, is_editable, is_required, default_value, is_udf,
    					linked_type, linked_object, setting_entry, user_id, col_fixed, col_width, col_height, is_ch_edit, is_ch_visible, lay_order, lay_name)
		select a.column_id, a.column_name, a.data_type, a.sub_type, a.size, 'Y', 'Y', a.is_required, a.default_value, 'Y',
				a.linked_type, a.linked_object, b.setting_entry, _userId, 'N', null, null, 'Y', 'Y', c.line_num, c.layout_name
					from apz_oudf a inner join _settingTB b on a.table_name = b.item_id
									left join apz_udf1 c on a.layout_id = c.line_num and a.table_name = c.table_name
						where not exists(select 1 from _fst1 x inner join _settingTB y on b.setting_entry = y.setting_entry 
										 	where x.column_id = a.column_id and y.item_id = b.item_id);
	
	update _fst1 set user_id = _userId;
	open _settingData for
		select * from _fst1;
	return next _settingData;
	
	create local temporary table _fst2 as select a.setting_entry, a.column_id, a.key_code, a.key_name from apz_fst2 a inner join _settingTB b on a.setting_entry = b.setting_entry;
	
	----------- thêm dữ liệu setting udf vào valid value -------------------------------
	insert into _fst2 select b.setting_entry, a.column_id, a.key_code, a.key_name from apz_udf2 a inner join _settingTB b on a.table_name = b.item_id;
	
	--Valiad value data
	open _validValueData for
		select * from _fst2;
	return next _validValueData;

END $BODY$;