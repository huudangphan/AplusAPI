-- FUNCTION: aplus_user_deleteconfig()

-- DROP FUNCTION aplus_user_deleteconfig();

CREATE OR REPLACE FUNCTION aplus_user_deleteconfig(
	)
    RETURNS TABLE(msg_code integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	iduser INTEGER;
BEGIN
	--Check user
	If (NOT EXISTS (SELECT * FROM user_info x WHERE 
					x.user_code = (SELECT y.user_code FROM "_temp_user_config_2" y))) THEN
		RETURN QUERY SELECT Cast(10109 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
	END IF;
	
	--Lấy thông tin user_id
 	SELECT x.user_id into iduser FROM user_info x 
 	where x.user_code = (SELECT y.user_code FROM "_temp_user_config_2" y);
	
	--Check config
	If (NOT EXISTS (Select 1 From user_config_2 x Where x.user_id = iduser 
					And x.code = (Select code From "_temp_user_config_2"))) THEN
		RETURN QUERY SELECT Cast(10122 as int) as msg_code,Cast('' as varchar) as msg;
		Return;
	END IF;

 	--Xóa record
 	Delete From user_config_2 y
 	Where y.user_id = iduser 
	And y.code = (SELECT code FROM "_temp_user_config_2");
	
	RETURN QUERY SELECT Cast(200 as int) as msg_code, Cast('Success.' as varchar) as msg;
	Return;
END;
$BODY$;
