-- FUNCTION: POSMAN_TEST.posmanerp_user_updateconfiguser(character varying, character varying, character varying)

-- DROP FUNCTION "POSMAN_TEST".posmanerp_user_updateconfiguser(character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION "POSMAN_TEST".posmanerp_user_updateconfiguser(
	usercode character varying,
	configcode character varying,
	configname character varying)
    RETURNS TABLE(msgcode integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    MsgCode INTEGER = 0;
	Msg VARCHAR;
	iduser int;
BEGIN
	--
	If(NOT EXISTS(SELECT * FROM apz_ousr WHERE User_Code = usercode))
	THEN
		MsgCode := 100001;
		Msg := 'User does not exists.';
	END IF;
    
	Select User_Id into iduser FROM apz_ousr WHERE user_code = usercode;
	If(EXISTS(SELECT * FROM apz_usr3 WHERE user_id = iduser and code = configcode))
	THEN
		Delete From apz_usr3 WHERE user_id = iduser and code = configcode;
	END IF;
	
	Insert Into apz_usr3(user_id, code, "name") Values (iduser, configcode, configname);
	
	RETURN QUERY SELECT
        MsgCode,
        Msg;
END;
$BODY$;

ALTER FUNCTION "POSMAN_TEST".posmanerp_user_updateconfiguser(character varying, character varying, character varying)
    OWNER TO "POSMAN";
