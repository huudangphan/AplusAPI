-- FUNCTION: aplus_user_save(character, integer)

-- DROP FUNCTION aplus_user_save(character, integer);

CREATE OR REPLACE FUNCTION aplus_user_save(
	_trans_type character,
	_user_sign integer)
    RETURNS TABLE(msgcode integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	id_user INTEGER = 0;
	date_create Date;
	time_create int;
	user_create smallint;
	_user_type varchar;
	_status varchar;
BEGIN
	--Check type
	IF((_trans_type <> 'A' And _trans_type <> 'U') Or _trans_type = '')
	THEN
		RETURN QUERY SELECT Cast(10102 as int) as msg_code;
		Return;
	END IF;
	--Validate email
-- 	if Exists (Select email from "_temp_ousr" where email is not null or email != '') Then
-- 		if EXISTS (SELECT 1 FROM apz_ousr WHERE email = (Select email from "_temp_ousr") And user_code NOT LIKE (Select user_code from "_temp_ousr")) THEN
--  			RETURN QUERY SELECT Cast(10110 as int) as msg_code, Cast('' as varchar) as msg;
-- 			Return;
--  		end if;
-- 	End if;
-- 	--Validate phone number	
-- 	if Exists (Select phone1 from "_temp_ousr" where phone1 != '' or phone1 is not null) Then
-- 		if EXISTS (SELECT 1 FROM apz_ousr WHERE user_code NOT LIKE (Select user_code from "_temp_ousr") And
-- 			   (phone1 LIKE (Select phone1 from "_temp_ousr") Or phone2 LIKE (Select phone1 from "_temp_ousr"))  )
-- 		THEN
-- 			RETURN QUERY SELECT Cast(10111 as int) as msg_code, Cast('' as varchar) as msg;
-- 			Return;
--  		end if;
-- 	end if;
--  	if (Select phone2 from "_temp_ousr") NOT Like '' Then
--  		if EXISTS (SELECT 1 FROM apz_ousr WHERE user_code NOT LIKE (Select user_code from "_temp_ousr") And
-- 			   (phone1 LIKE (Select phone2 from "_temp_ousr") Or phone2 LIKE (Select phone2 from "_temp_ousr"))  ) 
--  		THEN
--   			RETURN QUERY SELECT Cast(10111 as int) as msg_code, Cast('' as varchar) as msg;
-- 			Return;
--   		end if;
--  	End If;
	--Validate Company, Warehouse
	if Exists (Select 1 from "_temp_user_company" where company_code NOT IN (Select code from apz_obpl)) Then
		RETURN QUERY SELECT Cast(10124 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
	End if;
	if EXISTS (Select 1 from "_temp_user_config_1") Then
		If Exists(Select 1 from "_temp_user_config_1" where default_company is NOT null AND default_company != '') 
		Then
			if EXISTS (Select 1 from "_temp_user_config_1" where default_company NOT IN (Select code from apz_obpl)) Then
				RETURN QUERY SELECT Cast(10124 as int) as msg_code, Cast('' as varchar) as msg;
				Return;
			End if;
		End if;	
		If Exists(Select 1 from "_temp_user_config_1" where default_warehouse is NOT null AND default_warehouse != '') 
		Then	
			if EXISTS (Select 1 from "_temp_user_config_1" where default_warehouse NOT IN (Select whs_code from apz_owhs)) Then
				RETURN QUERY SELECT Cast(10125 as int) as msg_code, Cast('' as varchar) as msg;
				Return;
			End if;
		End if;
	End if;
	
	--Get status, get user_type
	Select status,user_type into _status,_user_type from "_temp_user_info";
	
	IF _trans_type LIKE 'A' THEN
		--Kiểm tra user tồn tại
 		if EXISTS (SELECT 1 FROM user_info WHERE user_code = (Select user_code from "_temp_user_info")) THEN
 			RETURN QUERY SELECT Cast(10112 as int) as msg_code, Cast('' as varchar) as msg;
			Return;
 		end if;
		--Lấy Id User mới nhất
		Select max(user_id) INTO id_user From user_info;
		id_user := id_user + 1;
		Update "_temp_user_info" Set user_id = Cast(id_user as int), 
								password = '',
								user_sign = _user_sign, 
								status = case when _status LIKE '' or _status isnull then 'A' else _status end,
								user_type = case when _user_type LIKE '' or _user_type isnull then 'N' else _user_type end,
								create_date = current_date,
								create_time = cast(to_char(current_timestamp, 'hh24mi') as int);
 		INSERT INTO user_info
 		Select ou.* 
 		from "_temp_user_info" ou;
		
 		--insert vào các table usr
 		insert into user_company (user_id, company_code,company_name,create_date,create_time)
 		Select Cast(id_user as int), u1.company_code, ob.name, current_date, Cast(to_char(current_timestamp, 'hh24mi') as int) 
		from "_temp_user_company" u1 join apz_obpl ob on u1.company_code = ob.code;
		
		If EXISTS (SELECT 1 FROM "_temp_user_config_1") THEN
			If (Select Count(1) FROM "_temp_user_config_1") > 0 Then 
				insert into user_config_1 (user_id, default_company, default_warehouse,create_date,create_time)
 				Select Cast(id_user as int), u2.default_company, u2.default_warehouse, current_date, Cast(to_char(current_timestamp, 'hh24mi') as int) 
				from "_temp_user_config_1" u2;
			Else
				insert into user_config_1 (user_id, default_company, default_warehouse,create_date,create_time)
 				Values (Cast(id_user as int), '' , '', current_date, Cast(to_char(current_timestamp, 'hh24mi') as int));
			End if;
		Else
			insert into user_config_1 (user_id, default_company, default_warehouse,create_date,create_time)
 			Values (Cast(id_user as int), '' , '', current_date, Cast(to_char(current_timestamp, 'hh24mi') as int));
		End if;
		
		RETURN QUERY SELECT Cast(200 as int) as msg_code, Cast(id_user as varchar) as msg;
		Return;
	End if;
	IF _trans_type LIKE 'U' Then
 		if (NOT EXISTS(SELECT 1 FROM user_info WHERE user_code = (Select user_code from "_temp_user_info")) )
		then
			RETURN QUERY SELECT Cast(10109 as int) as msg_code, Cast('' as varchar) as msg;
			Return;
		end if;	
-- 		if (EXISTS(SELECT 1 FROM apz_ousr WHERE user_code = (Select user_code from "_temp_ousr") And status = 'I' ) )
-- 		then
-- 			RETURN QUERY SELECT Cast(10115 as int) as msg_code, Cast('' as varchar) as msg;
-- 			Return;
-- 		end if;	
		
		--Lấy thông tin user ra biến
		SELECT ou.user_id,ou.create_date,ou.user_sign,ou.create_time INTO id_user,date_create,user_create,time_create 
		FROM user_info ou 
		WHERE user_code = (Select user_code from "_temp_user_info");
		Update "_temp_user_info" 
		Set user_id = Cast(id_user as int),
			password = '',
			user_sign = user_create,  
			create_date = date_create,
			create_time = time_create,
			user_sign2 = _user_sign,
			update_date = current_date,
			update_time = Cast(to_char(current_timestamp, 'hh24mi') as int);
		Delete from user_info where user_code = (Select user_code from "_temp_user_info");
		INSERT INTO user_info Select ou.* from "_temp_user_info" ou;
		
		SELECT u1.create_date,u1.create_time INTO date_create,time_create 
		from user_company u1
		where user_id = Cast(id_user as int);
		Delete from user_company where user_id = Cast(id_user as int);
		INSERT INTO user_company (user_id, company_code, company_name,create_date,create_time,update_date,update_time)
		Select id_user,u1.company_code,ob.name,date_create,time_create, current_date,Cast(to_char(current_timestamp, 'hhmi') as int)
		from "_temp_user_company" u1 join apz_obpl ob on u1.company_code = ob.code;
		
		If EXISTS (SELECT 1 FROM "_temp_user_config_1") THEN 
			SELECT u2.create_date,u2.create_time INTO date_create,time_create 
			FROM user_config_1 u2
			WHERE user_id = Cast(id_user as int);
			Delete from user_config_1 where user_id = Cast(id_user as int);
			insert into user_config_1 (user_id, default_company,default_warehouse,create_date,create_time,update_date,update_time)
			Select id_user, u2.default_company, u2.default_warehouse,date_create,time_create,current_date, Cast(to_char(current_timestamp, 'hh24mi') as int) 
			from "_temp_user_config_1" u2;
		End if;
		
	END IF;
	RETURN QUERY SELECT Cast(200 as int) as msg_code, Cast('Success.' as varchar) as msg;
	Return;
END;
$BODY$;
