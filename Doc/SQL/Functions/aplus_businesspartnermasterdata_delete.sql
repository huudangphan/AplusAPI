-- FUNCTION: aplus_businesspartnermasterdata_delete(character varying)

-- DROP FUNCTION aplus_businesspartnermasterdata_delete(character varying);

CREATE OR REPLACE FUNCTION aplus_businesspartnermasterdata_delete(
	_code character varying)
    RETURNS TABLE(msg_code integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN	
	--Check Card_code
 	If NOT EXISTS(Select 1 from partner oc Where oc.card_code = _code) Then
 		RETURN QUERY SELECT Cast(10148 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End If;
	--Check APZ_OIVL Transaction
	If EXISTS(Select * from apz_oivl oc Where oc.card_code = _code) Then
 		RETURN QUERY SELECT Cast(10149 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End If;
	--Check APZ_OVPM 
	If EXISTS(Select * from apz_ovpm ov Where ov.card_code = _code) Then
		RETURN QUERY SELECT Cast(10149 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
 	End If;
	
	--Delete Data
	Delete from partner where card_code = _code;
 	Delete from partner_address where card_code = _code;
	RETURN QUERY SELECT Cast(200 as int) as msg_code,Cast('Success.' as varchar)as msg;
	Return;
END;
$BODY$;
