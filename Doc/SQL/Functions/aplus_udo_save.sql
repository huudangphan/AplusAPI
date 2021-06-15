-- FUNCTION: aplus_udo_save(character, integer)

-- DROP FUNCTION aplus_udo_save(character, integer);

CREATE OR REPLACE FUNCTION aplus_udo_save(
	_trans_type character,
	_user_sign integer)
    RETURNS TABLE(msgcode integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	_code varchar;
	_header_table varchar;
BEGIN
	--Check type
	IF((_trans_type <> 'A' And _trans_type <> 'U') Or _trans_type = '')
	THEN
		RETURN QUERY SELECT Cast(13 as int) as msg_code, Cast('Trans_type is invalid.' as varchar) as msg;
		Return;
	END IF;
	--Kiểm tra oudo
	if EXISTS(Select 1 from "_temp_user_object" where code isnull or code = '') THEN
 		RETURN QUERY SELECT Cast(10170 as int) as msg_code, Cast('Udo Code is empty.' as varchar) as msg;
		Return;
 	end if;
	--Kiểm tra header_table
	if EXISTS(Select 1 from "_temp_user_object" where header_table isnull or header_table = '') THEN
 		RETURN QUERY SELECT Cast(10171 as int) as msg_code, Cast('Udo header_table is empty.' as varchar) as msg;
		Return;
 	end if;
	if NOT EXISTS(Select 1 from user_table where table_name = (Select header_table from "_temp_user_object")) THEN
 		RETURN QUERY SELECT Cast(10163 as int) as msg_code, Cast('Udo header_table is not exists.' as varchar) as msg;
		Return;
 	end if;
	--Kiểm tra object_type
	if EXISTS(Select 1 from "_temp_user_object" where object_type isnull or object_type = '') THEN
 		RETURN QUERY SELECT Cast(10172 as int) as msg_code, Cast('Udo object_type is empty.' as varchar) as msg;
		Return;
 	end if;
	
	--Kiểm tra type
	if EXISTS(Select 1 from "_temp_user_object" where type isnull or type = '') THEN
 		RETURN QUERY SELECT Cast(10173 as int) as msg_code, Cast('Udo type is empty.' as varchar) as msg;
		Return;
 	end if;
	if (NOT EXISTS(Select 1 from "_temp_user_object" where type IN ('Document','MasterData'))) THEN
 		RETURN QUERY SELECT Cast(10174 as int) as msg_code, Cast('Udo type must be ''Document'' or ''MasterData''.' as varchar) as msg;
		Return;
 	end if;
	--Lấy udo code
	Select x.code,x.header_table into _code,_header_table from "_temp_user_object" x;
	
 	Delete From "_temp_user_object_child" where code isnull or code = '' or child_table isnull or child_table = '';
	Delete From "_temp_user_object_search" where code isnull or code = '' or col_code isnull or col_code = '';
	Delete From "_temp_user_object_default" where code isnull or code = '' or col_code isnull or col_code = '';
	Delete From "_temp_user_object_child_default" where code isnull or code = '' or child_table isnull or child_table = '' or col_code isNull or col_code ='';
	
	--Check udo1
	if EXISTS (Select 1 from "_temp_user_object_child") then
		if Exists (Select 1 From "_temp_user_object_child" x where x.code NOT LIKE _code)
		THEN
 			RETURN QUERY SELECT Cast(10175 as int) as msg_code, Cast('Code in udo_child is invalid.' as varchar) as msg;
			Return;
 		end if;
		if Exists (Select 1 from "_temp_user_object_child" x Where x.child_table NOT IN (Select y.table_name from user_table y))Then
			RETURN QUERY SELECT Cast(10163 as int) as msg_code, Cast('Table is invalid.' as varchar) as msg;
			Return;
		End if;	
	End if;
	--Check udo2
	_header_table := 'u_'||_header_table;
	If EXISTS (Select 1 from "_temp_user_object_search") Then
		if Exists (Select 1 From "_temp_user_object_search" x where x.code NOT LIKE _code)
		THEN
 			RETURN QUERY SELECT Cast(10176 as int) as msg_code, Cast('Code in default_find is invalid.' as varchar) as msg;
			Return;
 		end if;
		If Exists(Select 1 from "_temp_user_object_search" x Where x.col_code NOT IN (SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=_header_table)) 
		Then
			RETURN QUERY SELECT Cast(10178 as int) as msg_code, Cast('Some columns is not in table: '||_header_table||'.' as varchar) as msg;
			Return;
		End if;
	End if;
	--Check udo3
	if EXISTS (Select 1 from "_temp_user_object_default") then
		if Exists (Select 1 From "_temp_user_object_default" x where x.code NOT LIKE _code)
		THEN
 			RETURN QUERY SELECT Cast(10177 as int) as msg_code, Cast('Code in header_view is invalid.' as varchar) as msg;
			Return;
 		end if;
		If Exists(Select 1 from "_temp_user_object_default" x Where x.col_code NOT IN (SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = _header_table)) 
		Then
			RETURN QUERY SELECT Cast(10178 as int) as msg_code, Cast('Some column is not in table: '||_header_table||'.' as varchar) as msg;
			Return;
		End if;
	End if;
	--Check udo4
	if EXISTS (Select 1 from "_temp_user_object_child_default") then
		--Check Code
		if Exists (Select 1 From "_temp_user_object_child_default" x where x.code NOT LIKE _code)
		THEN
 			RETURN QUERY SELECT Cast(10179 as int) as msg_code, Cast('Code in child_view is invalid.' as varchar) as msg;
			Return;
 		end if;
		--Check child table
 		If Cast((Select Count(1) from "_temp_user_object_child" x left join "_temp_user_object_child_default" y on x.code = y.code And x.child_table = y.child_table
				 where y.child_table is null) as int) != 0
 		Then
 			RETURN QUERY SELECT Cast(10180 as int) as msg_code, Cast('Some tables in child_view are invalid' as varchar) as msg;
 			Return;
 		End if;
		--Check column
		If Exists(Select 1 from "_temp_user_object_child_default" x Where x.col_code NOT IN (SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS 
																		WHERE TABLE_NAME = 'u_'||Cast((x.child_table) as varchar))) 
		Then
			RETURN QUERY SELECT Cast(10178 as int) as msg_code, Cast('Some columns is not in table_child.' as varchar) as msg;
			Return;
		End if;
	End if;
	
	IF _trans_type LIKE 'A' THEN
		--Kiểm tra code exists
		if EXISTS(Select 1 from user_object x where x.code = _code ) THEN
 			RETURN QUERY SELECT Cast(10181 as int) as msg_code, Cast('Udo has been existed.' as varchar) as msg;
			Return;
 		end if;
		--Kiểm tra table in_use
		if EXISTS(Select 1 from user_object x where x.header_table = (Select header_table from "_temp_user_object")) THEN
 			RETURN QUERY SELECT Cast(10183 as int) as msg_code, Cast('Udo header_table has been used in another Object.' as varchar) as msg;
			Return;
 		end if;
		--Kiểm tra object_type
		if EXISTS(Select 1 from user_object x where x.object_type = (Select object_type from "_temp_user_object")) THEN
 			RETURN QUERY SELECT Cast(10184 as int) as msg_code, Cast('Udo object_type has been used in another Object.' as varchar) as msg;
			Return;
 		end if;
		
		--Insert vào APZ_OUDO
		Update "_temp_user_object" set mng_series = case when (mng_series isnull or mng_series = '') Then 'N' End,
								can_delete = case when (can_delete isnull or can_delete = '') Then 'Y' End,
								can_close = case when (can_close isnull or can_close = '') Then 'Y' End,
								can_cancel = case when (can_cancel isnull or can_cancel = '') Then 'Y' End,
								can_find = case when (can_find isnull or can_find = '') Then 'Y' End,
								menu_item = case when (menu_item isnull or menu_item = '') Then 'Y' End,
								menu_type = case when (menu_type isnull or menu_type = '') Then 'H' End,
								user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int);
		Insert into user_object select * from "_temp_user_object";
		--Insert vào APZ_UDO1
		If EXISTS (Select 1 from "_temp_user_object_child") Then
			Update "_temp_user_object_child" x set child_num = y.seqnum,
								code = _code,
								user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			from (select x1.*, row_number() over () as seqnum from "_temp_user_object_child" x1) y
    		where y.code = x.code And x.child_table = y.child_table;
			Insert into user_object_child select * from "_temp_user_object_child";
		End if;
		--Insert vào APZ_UDO2
		If EXISTS (Select 1 from "_temp_user_object_search") Then
			Update "_temp_user_object_search" x set col_num = y.seqnum,
								code = _code,
								user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			from (select x1.*, row_number() over () as seqnum from "_temp_user_object_search" x1) y
    		where y.code = x.code And x.col_code = y.col_code;
			Insert into user_object_search select * from "_temp_user_object_search";
		End if;
		--Insert vào APZ_UDO3
		If EXISTS (Select 1 from "_temp_user_object_default") Then	
			Update "_temp_user_object_default" x set col_num = y.seqnum,
								code = _code,
								col_edit = case when (x.col_edit isnull or x.col_edit = '') Then 'Y' End,
								user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			from (select x1.*, row_number() over () as seqnum from "_temp_user_object_default" x1) y
    		where y.code = x.code And x.col_code = y.col_code;
			Insert into user_object_default select * from "_temp_user_object_default";
		End if;
		--Insert vào APZ_UDO4
		If EXISTS (Select 1 from "_temp_user_object_child_default") Then
			Update "_temp_user_object_child_default" x set code = _code,
								col_num = y.seqnum,
								col_edit = case when (x.col_edit isnull or x.col_edit = '') Then 'Y' else x.col_edit End,
								user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			from (select x1.*, row_number() over(PARTITION BY x1.child_table) as seqnum from "_temp_user_object_child_default" x1) y
    		where y.code = x.code And x.child_table = y.child_table And x.col_code = y.col_code;
			Insert into user_object_child_default select * from "_temp_user_object_child_default";
		End if;
	Else
 		--Kiểm tra code exists
		if NOT EXISTS(Select 1 from user_object x where x.code = _code) THEN
 			RETURN QUERY SELECT Cast(10182 as int) as msg_code, Cast('Udo does not exists.' as varchar) as msg;
			Return;
 		end if;
		--Kiểm tra table in_use
		if EXISTS(Select 1 from user_object x where x.header_table = (Select header_table from "_temp_user_object") AND x.code != _code) THEN
 			RETURN QUERY SELECT Cast(10183 as int) as msg_code, Cast('Udo header_table has been used in another object.' as varchar) as msg;
			Return;
 		end if;
		--Kiểm tra object_type
		if EXISTS(Select 1 from user_object x where x.object_type = (Select object_type from "_temp_user_object") AND x.code != _code) THEN
 			RETURN QUERY SELECT Cast(10184 as int) as msg_code, Cast('Udo object_type has been used in another object.' as varchar) as msg;
			Return;
 		end if;
		
		--Update dữ liệu bảng tạm
		Update "_temp_user_object" set mng_series = case when("_temp_user_object".mng_series isnull or "_temp_user_object".mng_series = '')Then user_object.mng_series Else "_temp_user_object".mng_series End,
							can_delete = case when("_temp_user_object".can_delete isnull or "_temp_user_object".can_delete = '')Then user_object.can_delete Else "_temp_user_object".can_delete End,
							can_close = case when("_temp_user_object".can_close isnull or "_temp_user_object".can_close = '')Then user_object.can_close Else "_temp_user_object".can_close End,
							can_cancel = case when("_temp_user_object".can_cancel isnull or "_temp_user_object".can_cancel = '')Then user_object.can_cancel Else "_temp_user_object".can_cancel End,
							can_find = case when("_temp_user_object".can_find isnull or "_temp_user_object".can_find = '')Then user_object.can_find Else "_temp_user_object".can_find End,
							menu_item = case when("_temp_user_object".menu_item isnull or "_temp_user_object".menu_item = '')Then user_object.menu_item Else "_temp_user_object".menu_item End,
							menu_type = case when("_temp_user_object".menu_type isnull or "_temp_user_object".menu_type = '')Then user_object.menu_type Else "_temp_user_object".mng_series End,
							user_sign = user_object.user_sign,
							create_date = user_object.create_date,
							create_time = user_object.create_time,
							user_sign2 = _user_sign,
							update_date = current_date,
							update_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
		FROM user_object 
		Where "_temp_user_object".code = user_object.code;
		Delete from user_object where code = _code;
		Insert into user_object select * from "_temp_user_object";
		--Insert vào APZ_UDO1
		If EXISTS (Select 1 from "_temp_user_object_child") Then
			--Update thông tin cũ
			Update "_temp_user_object_child" set code = _code,
								user_sign = user_object_child.user_sign,
								create_date = user_object_child.create_date,
								create_time = user_object_child.create_time,
								user_sign2 = _user_sign,
								update_date = current_date,
								update_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			FROM user_object_child 
			Where "_temp_user_object_child".code = user_object_child.code and "_temp_user_object_child".child_table = user_object_child.child_table;
			--Update thông tin row mới
			Update "_temp_user_object_child" set user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			Where user_sign isnull or create_date isnull or create_time isnull;
			--Đánh số
			Update "_temp_user_object_child" x set child_num = y.seqnum
				from (select x1.*, row_number() over() as seqnum from "_temp_user_object_child" x1) y
    			where y.code = x.code And x.child_table = y.child_table;
			Delete from user_object_child where code = _code;
			Insert into user_object_child select * from "_temp_user_object_child";
		End if;
		--Insert vào APZ_UDO2
		If EXISTS (Select 1 from "_temp_user_object_search") Then
			--Update thông tin cũ
			Update "_temp_user_object_search" set code = _code,
								user_sign = user_object_search.user_sign,
								create_date = user_object_search.create_date,
								create_time = user_object_search.create_time,
								user_sign2 = _user_sign,
								update_date = current_date,
								update_time = Cast(to_char(CURRENT_TIMESTAMP, 'hhmi') as int)
			FROM user_object_search 
			Where "_temp_user_object_search".code = user_object_search.code and "_temp_user_object_search".col_code = user_object_search.col_code;
			--Update thông tin row mới
			Update "_temp_user_object_search" set user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			Where user_sign isnull or create_date isnull or create_time isnull;
			--Đánh số
			Update "_temp_user_object_search" x set col_num = y.seqnum
				from (select x1.*, row_number() over() as seqnum from "_temp_user_object_search" x1) y
    			where y.code = x.code And x.col_code = y.col_code;
			Delete from user_object_search where code = _code;
			Insert into user_object_search select * from "_temp_user_object_search";
		End if;
		--Insert vào APZ_UDO3
		If EXISTS (Select 1 from "_temp_user_object_default") Then
			--Update thông tin cũ
			Update "_temp_user_object_default" set code = _code,
								col_edit = case when("_temp_user_object_default".col_edit isnull or "_temp_user_object_default".col_edit = '') Then user_object_default.col_edit Else "_temp_user_object_default".col_edit End,
								user_sign = user_object_default.user_sign,
								create_date = user_object_default.create_date,
								create_time = user_object_default.create_time,
								user_sign2 = _user_sign,
								update_date = current_date,
								update_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			FROM user_object_default 
			Where "_temp_user_object_default".code = user_object_default.code and "_temp_user_object_default".col_code = user_object_default.col_code;
			--Update thông tin row mới
			Update "_temp_user_object_default" set col_edit = case when(col_edit isnull or col_edit = '') Then 'Y' End,
								user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			Where user_sign isnull or create_date isnull or create_time isnull;
			--Đánh số
			Update "_temp_user_object_default" x set col_num = y.seqnum
				from (select x1.*, row_number() over() as seqnum from "_temp_user_object_default" x1) y
    			where y.code = x.code And x.col_code = y.col_code;
			Delete from user_object_default where code = _code;
			Insert into user_object_default select * from "_temp_user_object_default";
		End if;
		--Insert vào APZ_UDO4
		If EXISTS (Select 1 from "_temp_user_object_child_default") Then
			--Update thông tin cũ
			Update "_temp_user_object_child_default" set code = _code,
								col_edit = case when("_temp_user_object_child_default".col_edit isnull or "_temp_user_object_child_default".col_edit = '') Then user_object_child_default.col_edit Else "_temp_user_object_child_default".col_edit End,
								user_sign = user_object_child_default.user_sign,
								create_date = user_object_child_default.create_date,
								create_time = user_object_child_default.create_time,
								user_sign2 = _user_sign,
								update_date = current_date,
								update_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			FROM user_object_child_default 
			Where "_temp_user_object_child_default".code = user_object_child_default.code and "_temp_user_object_child_default".child_table = user_object_child_default.child_table and "_temp_user_object_child_default".col_code = user_object_child_default.col_code;
			--Update thông tin row mới
			Update "_temp_user_object_child_default" set col_edit = case when(col_edit isnull or col_edit = '') Then 'Y' else col_edit End,
								user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int)
			Where user_sign isnull or create_date isnull or create_time isnull;	
			--Đánh số
			Update "_temp_user_object_child_default" x set col_num = y.seqnum
				from (select x1.*, row_number() over(PARTITION BY x1.child_table) as seqnum from "_temp_user_object_child_default" x1) y
    			where y.code = x.code And x.child_table = y.child_table And x.col_code = y.col_code;
			Delete from user_object_child_default where code = _code;
			Insert into user_object_child_default select * from "_temp_user_object_child_default";
		End if;
	END IF;
	
	RETURN QUERY SELECT Cast(200 as int) as msg_code, Cast('Success.' as varchar) msg;
	Return;
END;
$BODY$;
