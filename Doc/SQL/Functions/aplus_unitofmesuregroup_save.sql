-- FUNCTION: aplus_unitofmesuregroup_save(character varying, integer)

-- DROP FUNCTION aplus_unitofmesuregroup_save(character varying, integer);

CREATE OR REPLACE FUNCTION aplus_unitofmesuregroup_save(
	_trans_type character varying,
	_user_sign integer)
    RETURNS TABLE(msg_code integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	msg varchar;
	user_create smallint;
	date_create date;
	time_create int;
	entry integer;
	check_ouom integer;
	count_ugp1 integer;
	_same integer = 0;
BEGIN
	--Check base_uom exists in apz_ouom
	If (Select ug.base_uom from "_temp_ougp" ug) <> '' Then
		if NOT EXISTS(Select 1 from "_temp_ougp" x join apz_ouom y on x.base_uom = y.code) Then
			RETURN QUERY SELECT Cast(10134 as int) as msg_code, Cast('' as varchar) as msg;
			Return;
		End if;
	End if;
	--Delete empty row, Check temnp_ugp1
	Delete from "_temp_ugp1" where uom_code like null or uom_code like '';
	Select count(u1.uom_code) into count_ugp1 from "_temp_ugp1" u1;
	if count_ugp1 = 0 Then
		RETURN QUERY SELECT Cast(10130 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
	End if;
	
	--Check ugp1 exists in apz_ouom
	Select count(1) into check_ouom from "_temp_ugp1" u1 join apz_ouom uo on u1.uom_code = uo.code ;
	if check_ouom <> count_ugp1 Then
		RETURN QUERY SELECT Cast(10134 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
	End if;
	
	--Switch transtype
	if _trans_type like 'A' Then
		--create new ugp_entry
		Select Max(ug.ugp_entry) into entry from unit_group ug;
		entry := entry + 1;
		if (entry is null) Then
			entry := 1;
		End if;
		Update "_temp_ougp" Set ugp_entry = entry, 
								create_date = Current_date,
								create_time = Cast(to_char(current_timestamp, 'hh24mi') as int),
								status = 'A', 
								user_sign = _user_sign;
		Update "_temp_ugp1" Set ugp_entry = entry;
		
		--Return new id
		msg := entry;
	else
		Select tu.ugp_entry into entry from "_temp_ougp" tu;
		If (Select Count(1) from "_temp_ugp1" x join unit_group_1 y on x.uom_code = y.uom_code) != count_ugp1
		Then
			_same := 1;
		End If;
		--Check ugp_entry
		If ((entry is Null) or (entry = 0)) Then
			msg := '';
			RETURN QUERY SELECT Cast(10130 as int) as msg_code, msg;
			Return;
		End if;
		--Check ugp_entry exists
		If Not exists(Select * from unit_group ug where ug.ugp_entry = entry)
		Then
			msg := '';
			RETURN QUERY SELECT Cast(10134 as int) as msg_code, msg;
			Return;
		End if;
		--Check ugp1 has been used in item APZ_OITM sal
		if Exists (Select (1) from "_temp_ugp1" u1 join apz_oitm it on u1.uom_code = it.sal_unit_msr) Then
 			if (_same = 1) Then
				msg := '';
				RETURN QUERY SELECT Cast(10133 as int) as msg_code,msg;
				Return;
			End if;
 		End if;
		--Check ugp1 has been used in item APZ_OITM inv
		if Exists(Select (1) from "_temp_ugp1" u1 join apz_oitm it on u1.uom_code = it.invn_unit_msr) Then
 			if (_same = 1) Then
				msg := '';
				RETURN QUERY SELECT Cast(10133 as int) as msg_code,msg;
				Return;
			End if;
 		End if;
		--Check ugp1 has been used in item APZ_ITM1
 		if Exists(Select (1) from "_temp_ugp1" u1, apz_itm1 it1 where u1.uom_code = it1.uom_code) Then
 			if (_same = 1) Then
				msg := '';
				RETURN QUERY SELECT Cast(10133 as int) as msg_code,msg;
				Return;
			End if;
 		End if;
		
		--temp_ougp
		SELECT x.create_date, x.create_time,x.user_sign INTO date_create, time_create,user_create FROM unit_group x WHERE x.ugp_entry = entry;
		Update "_temp_ougp" 
		set create_date = date_create,
			create_time = time_create,
			user_sign = user_create,
			update_date = Current_Date,
			update_time = Cast(to_char(current_timestamp, 'hh24mi') as int),
			user_sign2 = _user_sign;
		
		--temp_ugp1
		Create LOCAL TEMP TABLE "_temp_dataup" as 
			Select row_number() over (order by x.uom_code desc) row_num, x.* from "_temp_ugp1" x;
		
		UPDATE "_temp_ugp1"
		SET ugp_entry=entry,
    	line_num = subquery.row_num
		FROM (SELECT row_num, uom_code, ugp_entry
      		FROM "_temp_dataup") AS subquery
		WHERE "_temp_ugp1".uom_code = subquery.uom_code;
		
		Delete from unit_group ug where ug.ugp_entry = entry;
		Delete from unit_group_1 u1 where u1.ugp_entry = entry;
		msg := 'Success.';
	End if;
	
	Insert into unit_group Select * from "_temp_ougp";
	Insert into unit_group_1 Select * from "_temp_ugp1";
	RETURN QUERY SELECT Cast(200 as int) as msg_code, Cast(msg as varchar) as msg;
	Return;
END;
$BODY$;
