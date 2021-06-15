-- FUNCTION: aplus_businesspartnermasterdata_save(character varying, integer)

-- DROP FUNCTION aplus_businesspartnermasterdata_save(character varying, integer);

CREATE OR REPLACE FUNCTION aplus_businesspartnermasterdata_save(
	_trans_type character varying,
	_user_sign integer)
    RETURNS TABLE(msg_code integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	msg varchar = 'Success';
	newid int;
	newcode varchar;
	adres_count int;
	adres_check int;
	_user_sign1 int;
	_create_date date;
	_create_time int;
BEGIN
	--Check card_type
 	If ((Select ot.card_type from "_temp_partner" ot) Like '') 
	Then
 		RETURN QUERY SELECT Cast(10141 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End if;
 	If Exists (Select ot.card_type from "_temp_partner" ot where ot.card_type NOT IN ('S','C','L')) 
	Then
 		RETURN QUERY SELECT Cast(10142 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End if;
 	--Check GroupBP
 	If Not EXISTS(Select 1 from partner_group cg Where cg.code = (Select ot.group_code from "_temp_partner" ot)) 
	Then
 		RETURN QUERY SELECT Cast(10143 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End If;
 	--Check price List 
 	If (Not EXISTS(Select 1 from APZ_OPLN pl Where pl.doc_entry = (Select ot.list_num from "_temp_partner" ot)))
 	Then
 		RETURN QUERY SELECT Cast(10144 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End If;
 	--Check VAT Group
 	If (Not EXISTS(Select 1 from APZ_OVTG ov Where ov.code = (Select ot.vat_group from "_temp_partner" ot))) 
 	Then
 		RETURN QUERY SELECT Cast(10145 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End If;
	--Xử lý temp_crd1
	If Exists (Select 1 from "_temp_partner_address" Where address = '' Or adres_type = '') Then
		RETURN QUERY SELECT Cast(10146 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
	End If;
 	If Exists (Select adres_type from "_temp_partner_address" where adres_type NOT IN ('S','B')) 
 	Then
 		RETURN QUERY SELECT Cast(10147 as int) as msg_code,Cast('' as varchar)as msg;
 		Return;
 	End If;	
	Delete from "_temp_partner_address" where address = '' And adres_type = '';
	--Check default partner_address
	If Cast((Select Count(1) from "_temp_partner_address" x Where x.is_def = 'Y') as int) > 1
	Then
		RETURN QUERY SELECT Cast(10150 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End If;
	
	--Lưu Data
	If _trans_type = 'A' Then
		--Tạo mới card_code
		If ((Select ot.card_type from "_temp_partner" ot) LIKE 'C' )
		Then
			select max(cast(substring(oc.card_code, 3, length(oc.card_code)-2) as integer)) into newid from apz_ocrd oc where oc.card_type LIKE 'C';
			if newid is null then
				newid := 10000;
			Else
				newid := newid +1;
				if newid < 10000 Then
					newid := 10000;
				End if;
			End if;
			newcode := 'KH'||cast(newid as varchar);
		ELSIF (Select ot.card_type from "_temp_partner" ot) LIKE 'S'
		Then
			select max(cast(substring(oc.card_code, 4, length(oc.card_code)-3) as integer)) into newid from apz_ocrd oc where oc.card_type LIKE 'S';
			if newid is null then
				newid := 10000;
			Else
				newid := newid +1;
				if newid < 10000 Then
					newid := 10000;
				End if;
			End if;
			newcode := 'NCC'||cast(newid as varchar);
		Else
			select max(cast(substring(oc.card_code, 4, length(oc.card_code)-3) as integer)) into newid from apz_ocrd oc where oc.card_type LIKE 'L';
			if newid is null then
				newid := 10000;
			Else
				newid := newid +1;
				if newid < 10000 Then
					newid := 10000;
				End if;
			End if;
			newcode := 'KHT'||cast(newid as varchar);
		End if;
 		Update "_temp_partner" Set card_code = newcode, 
								status = 'A',
								create_date = Current_date, 
								create_time = Cast(to_char(current_timestamp, 'hh24mi') as integer),
								user_sign = _user_sign;
								
 		--Xử lý thông tin bảng Address
		If Exists (Select 1 from "_temp_partner_address") Then
			Create LOCAL TEMP TABLE "#temp_dataadd" as 
				Select row_number() over (order by "address" desc) row_num, * from "_temp_partner_address";
			UPDATE "_temp_partner_address"
			SET line_num = tempdata.row_num
    		FROM (SELECT * FROM "#temp_dataadd") AS tempdata WHERE "_temp_partner_address".address=tempdata.address;
			Update "_temp_partner_address" Set card_code = newcode, 
									create_date = Current_date,
									create_time = Cast(to_char(current_timestamp, 'hh24mi') as integer),
									user_sign = _user_sign;
		End if;
			
		Insert into partner Select * from "_temp_partner";
		Insert into partner_address Select * from "_temp_partner_address";
 	Else
 		--Check Card_code
 		If Exists (Select 1 from "_temp_partner" where card_code isnull or card_code = '') Then
			RETURN QUERY SELECT Cast(10140 as int) as msg_code,Cast('' as varchar)as msg;
			Return;
		End If;
		Select card_code into newcode from "_temp_partner";
		If NOT EXISTS(Select 1 from partner x Where x.card_code = newcode) 
		Then
			RETURN QUERY SELECT Cast(10148 as int) as msg_code,Cast('' as varchar)as msg;
			Return;
 		End If;
		--Check Card_type
		If NOT EXISTS(Select 1 from partner x join "_temp_partner" y on x.card_code = y.card_code And x.card_type = y.card_type) Then
			RETURN QUERY SELECT Cast(10141 as int) as msg_code,Cast('' as varchar)as msg;
			Return;
 		End If;
		--Cập nhật thông tin BP cũ vào bảng tạm
		Select x.user_sign, x.create_date,x.create_time into _user_sign1,_create_date,_create_time from partner x where x.card_code = newcode;
 		Update "_temp_partner"
  		Set card_code = newcode,
 			user_sign = _user_sign1,
  			create_date = _create_date,
			create_time = create_time,
  			update_date = Current_date,
			update_time = Cast(to_char(current_timestamp, 'hh24mi') as integer),
  			user_sign2 = _user_sign
		WHERE 1 = 1;
		--Xử lý thông tin bảng crd1
  		Update "_temp_partner_address"
   		Set card_code = newcode,
  			user_sign = y.user_sign,
   			create_date = y.create_date,
			create_time = y.create_time,
   			update_date = Current_date,
			update_time = Cast(to_char(current_timestamp, 'hh24mi') as integer),
   			user_sign2 = _user_sign
  		From (select * from partner_address where card_code = newcode) as y
  		Where 1 = 1;
-- 		--Update create line mới
-- 		Update "_temp_partner_address" 
--  		Set create_date = Current_date,
--  			user_sign = _user_sign
--  		Where line_num = 0;
		--Đánh số line_num
 		Create LOCAL TEMP TABLE "#temp_dataadd" as 
 				Select row_number() over (order by "create_date" asc) row_num, * from "_temp_partner_address";
 		UPDATE "_temp_partner_address"
 		SET line_num = tempdata.row_num
     	FROM (SELECT row_num, address FROM "#temp_dataadd") AS tempdata WHERE "_temp_partner_address".address=tempdata.address;
  		
		Delete from partner where card_code = newcode;
		Delete from partner_address where card_code = newcode;
		Insert into partner Select * from "_temp_partner";
		Insert into partner_address Select * from "_temp_partner_address";
		
	End if;
	RETURN QUERY SELECT Cast(200 as int) as msg_code,Cast(newcode as varchar)as msg;
	Return;
END;
$BODY$;
