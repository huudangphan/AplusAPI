-- FUNCTION: aplus_user_updateconfig()

-- DROP FUNCTION aplus_user_updateconfig();

CREATE OR REPLACE FUNCTION aplus_user_updateconfig(
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
	If (NOT EXISTS (SELECT 1 FROM user_info WHERE 
					user_code = (SELECT DISTINCT user_code FROM "_temp_user_config_2") )) THEN
		RETURN QUERY SELECT Cast(10101 as int) as msg_code,Cast('' as varchar)as msg;
		Return;
	END IF;
	--Xóa row rỗng
	Delete From "_temp_user_config_2" where user_code = '' or code = '';
	If Not Exists(Select 1 from "_temp_user_config_2") Then
		RETURN QUERY SELECT Cast(10108 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
	End if;
	--Lấy thông tin user_id
 	SELECT x.user_id into iduser FROM user_info x 
 	where x.user_code = (SELECT DISTINCT y.user_code FROM "_temp_user_config_2" y);
	
	--Tạo bảng data
	Create LOCAL TEMP TABLE "#temp_data" as 
			Select iduser user_id, t3.code,t3.name from "_temp_user_config_2" t3;
			
	if (Exists (Select 1 From user_config_2 where user_id = iduser))
	Then
		Delete From user_config_2 where user_id = iduser And code In (Select x.code From "#temp_data" x join user_config_2 y on x.user_id = y.user_id And x.code = y.code);
	End If;
	Insert Into user_config_2 select * from "#temp_data";
	
	RETURN QUERY SELECT Cast(200 as int) msg_code, Cast('Success.' as character varying) msg;
	Return;
END;
$BODY$;
