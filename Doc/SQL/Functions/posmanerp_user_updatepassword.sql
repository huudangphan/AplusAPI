-- FUNCTION: POSMAN_TEST.aplus_user_updatepassword(character varying, character varying)

-- DROP FUNCTION "POSMAN_TEST".aplus_user_updatepassword(character varying, character varying);

CREATE OR REPLACE FUNCTION "POSMAN_TEST".aplus_user_updatepassword(
	usercode character varying,
	newpassword character varying)
    RETURNS TABLE(msgcode integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    MsgCode INTEGER = 0;
	Msg VARCHAR;
BEGIN
	--
	If(NOT EXISTS(SELECT * FROM apz_ousr WHERE user_code = usercode))
	THEN
		MsgCode := 100001;
		Msg := 'User does not exists.';
	END IF;
    
	UPDATE APZ_OUSR SET "password" = newpassword
	WHERE User_Code = userCode;
	
	RETURN QUERY SELECT
        MsgCode,
        Msg;
END;
$BODY$;

ALTER FUNCTION "POSMAN_TEST".aplus_user_updatepassword(character varying, character varying)
    OWNER TO "POSMAN";
