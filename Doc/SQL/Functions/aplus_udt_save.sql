-- FUNCTION: aplus_udt_save(character, integer)

-- DROP FUNCTION aplus_udt_save(character, integer);

CREATE OR REPLACE FUNCTION aplus_udt_save(
	_trans_type character,
	_user_sign integer)
    RETURNS TABLE(msgcode integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	_table_name varchar;
	_table_type int;
	_time_create int = 0;
	_date_create Date;
	_user_create smallint = 0;
BEGIN
	--Check type
	IF((_trans_type <> 'A' And _trans_type <> 'U') Or _trans_type = '')
	THEN
		RETURN QUERY SELECT Cast(13 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
	END IF;
	--Kiểm tra table_name empty
	if EXISTS(Select 1 from "_temp_user_table" where table_name isnull or table_name ='') THEN
 		RETURN QUERY SELECT Cast(10160 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
 	end if;
	Select x.table_name into _table_name from "_temp_user_table" x;

	IF _trans_type LIKE 'A' THEN
		--Kiểm tra table_type
		if EXISTS(Select 1 from "_temp_user_table" where table_type isnull) THEN
 			RETURN QUERY SELECT Cast(10161 as int) as msg_code, Cast('' as varchar) as msg;
			Return;
 		end if;
		if EXISTS(Select 1 from "_temp_user_table" where table_type NOT IN (0,1,2,3,4)) THEN
 			RETURN QUERY SELECT Cast(10162 as int) as msg_code, Cast('' as varchar) as msg;
			Return;
 		end if;
		Select x.table_type into _table_type from "_temp_user_table" x; 
		--Kiểm tra table_name tồn tại
		if EXISTS(Select 1 from user_table x where x.table_name = _table_name) THEN
 			RETURN QUERY SELECT Cast(10164 as int) as msg_code, Cast('' as varchar) as msg;
			Return;
 		end if;
		--Insert vào APZ_UDT
		Update "_temp_user_table" set user_sign = _user_sign,
								create_date = current_date,
								create_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int);
		Insert into user_table select * from "_temp_user_table"; 
		_table_name := 'u_' || _table_name;
		--Tạo bảng mới
		CASE 
   			WHEN _table_type = 0 THEN
				Execute 'Create table '|| _table_name ||'
				(
					code varchar(100) NOT NULL,
					name varchar(100) NOT NULL,
					PRIMARY KEY (code)
				);';
   			WHEN _table_type = 1 THEN
      			Execute 'Create table '|| _table_name || '
				(
					code varchar(50) NOT NULL,
					name varchar(100),
					doc_entry int NOT NULL,
					canceled char(1),
					object varchar(20),
					user_sign int,
					transfered char(1),
					create_date date,
					create_time smallint,
					update_date date,
					update_time smallint,
					data_source char(1),
					PRIMARY KEY (code)
				);';
			WHEN _table_type = 2 THEN
      			Execute 'Create table '|| _table_name || '
				(
					code varchar(50) NOT NULL,
					line_id int NOT NULL,
					object varchar(20) NULL,
					PRIMARY KEY (code, line_id)
				);';
			WHEN _table_type = 3 THEN
      			Execute 'Create table ' || _table_name || '
				(
					doc_entry int NOT NULL,
					doc_num int,
					period int,
					instance smallint,
					series int,
					handwrtten char(1),
					canceled char(1),
					object varchar(20),
					user_sign int,
					transfered char(1),
					status char(1),
					create_date date,
					create_time smallint,
					update_date date,
					update_time smallint,
					data_source char(1),
					request_status char(1),
					creator varchar(8),
					remark varchar,
					PRIMARY KEY (doc_entry)
				);';
			WHEN _table_type = 4 THEN
      			Execute 'Create table ' || _table_name || '
				(
					doc_entry int NOT NULL,
					line_id int NOT NULL,
					vis_order int,
					"object" varchar(20),
					PRIMARY KEY (doc_entry, line_id)
				);';
		END CASE;
	Else
 		--Kiểm tra table_name tồn tại
		if NOT EXISTS(Select 1 from user_table x where x.table_name = _table_name) THEN
 			RETURN QUERY SELECT Cast(10163 as int) as msg_code, Cast('' as varchar) as msg;
			Return;
 		end if;
		--Lấy thông cũ từ bảng oudt
		Select x.table_type,x.user_sign,x.create_date,x.create_time into _table_type,_user_create,_date_create,_time_create from user_table x where x.table_name = _table_name;
		Update "_temp_user_table" set table_type = _table_type,
								user_sign = _user_create,
								create_date = _date_create,
								create_time = _time_create,
								user_sign2 = _user_sign,
								update_date = current_date,
								update_time = Cast(to_char(CURRENT_TIMESTAMP, 'hh24mi') as int);
		Delete from user_table where table_name = _table_name;
		Insert into user_table select * from "_temp_user_table";
	END IF;
	RETURN QUERY SELECT Cast(200 as int) as msg_code, Cast('Success.' as varchar) msg;
	Return;
END;
$BODY$;
