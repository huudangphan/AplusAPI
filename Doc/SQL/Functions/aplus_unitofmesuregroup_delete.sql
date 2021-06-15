-- FUNCTION: aplus_unitofmesuregroup_delete(integer)

-- DROP FUNCTION aplus_unitofmesuregroup_delete(integer);

CREATE OR REPLACE FUNCTION aplus_unitofmesuregroup_delete(
	_code integer)
    RETURNS TABLE(msg_code integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	msg varchar = '';
BEGIN
	--Check ougp has been used in item APZ_OITM
	if Exists(Select * from apz_oitm it where it.ugp_entry = _code) Then
		RETURN QUERY SELECT Cast(10130 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
	End if;
	
	--Tạo bảng tạm lấy thông tin ugp1
	Create LOCAL TEMP TABLE "#temp_ugp1" as 
			Select * from unit_group_1 u1 where u1.ugp_entry = _code;
	
	--Check ugp1 has been used in item APZ_ITM1
	if EXISTS (Select 1 from "#temp_ugp1" u1 join apz_itm1 it1 on u1.uom_code = it1.uom_code) Then
		RETURN QUERY SELECT Cast(10131 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
	End if;
	--Check ugp1 has been used in item APZ_OITM sal
 	if EXISTS (Select 1 from "#temp_ugp1" u1 join apz_oitm it on u1.uom_code = it.sal_unit_msr) Then
		RETURN QUERY SELECT Cast(10131 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
 	End if;
	--Check ugp1 has been used in item APZ_OITM inv
 	if EXISTS (Select 1 from "#temp_ugp1" u1 join apz_oitm it on u1.uom_code = it.invn_unit_msr) Then
		RETURN QUERY SELECT Cast(10131 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
 	End if;
	
	--Delete
	Delete from unit_group where ugp_entry = _code;
	Delete from unit_group_1 where ugp_entry = _code;
	
	RETURN QUERY SELECT Cast(200 as int) as msg_code, Cast('Success.' as varchar) as msg;
	Return;
END;
$BODY$;
