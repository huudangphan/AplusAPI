-- FUNCTION: POSMAN_TEST.posmanerp_user_savedata(character, integer)

-- DROP FUNCTION "POSMAN_TEST".posmanerp_user_savedata(character, integer);

CREATE OR REPLACE FUNCTION "POSMAN_TEST".posmanerp_user_savedata(
	tran_type character,
	user_create integer)
    RETURNS TABLE(msgcode integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    MsgCode INTEGER = 0;
	Msg VARCHAR = '';
	id_user INTEGER = 0;
	date_create Date;
BEGIN
	--Check type
	IF((tran_type <> 'A' And tran_type <> 'U') Or tran_type = '')
	THEN
		MsgCode := 100001;
		Msg := 'Invalid Tran Type.';
		RETURN QUERY SELECT MsgCode,Msg;
	END IF;
	
	IF tran_type LIKE 'A' THEN
		--Kiểm tra user tồn tại
 		if EXISTS (SELECT * FROM apz_ousr WHERE user_code = (Select user_code from "#temp_ousr")) THEN
 			MsgCode := 100002;
			Msg := 'User has been existed.';
 		   	RETURN QUERY SELECT MsgCode,Msg;
 		end if;
		--Lấy Id User mới nhất
		Select max(user_id) INTO id_user From apz_ousr;
		id_user := id_user + 1;
 		INSERT INTO apz_ousr(user_id,user_code, user_name,email, phone1, phone2,address,branch_code,user_sign,create_date)
 		Select id_user,ou.user_code,ou.user_name,ou.email,ou.phone1,ou.phone2,ou.address,us1.branch_code,user_create,current_date 
 		from "#temp_ousr" ou, "#temp_usr1" us1;
 		--insert vào các table usr
 		insert into apz_usr1(user_id, branch_code, branch_name, assign_date) 
 		Select id_user,us1.branch_code, ob.branch_name, Current_Date from "#temp_usr1" us1 join apz_obpl ob on us1.branch_code = ob.branch_id;
 		insert into apz_usr2(user_id, default_branch, default_warehouse, assign_date) 
 		Select id_user, us2.default_branch, us2.default_warehouse, Current_Date ou from "#temp_usr2" us2;
	Else	
 		if NOT EXISTS(SELECT * FROM apz_ousr WHERE user_code = (Select user_code from "#temp_ousr"))
		then
			MsgCode := 100003;
			Msg := 'User doesn''t exists.';
		   	RETURN QUERY SELECT MsgCode,Msg;
		end if;		
		--Lấy thông tin user ra bảng tạm
		SELECT ou.user_id, ou.create_date INTO id_user,date_create FROM apz_ousr ou WHERE user_code = (Select user_code from "#temp_ousr");
		--Delete thông tin cũ
		Delete from apz_ousr where user_code = (Select user_code from "#temp_ousr");
		Delete from apz_usr1 where user_id = id_user;
	   	Delete from apz_usr2 where user_id = id_user;
		--Thêm lại thông tin mới
		INSERT INTO apz_ousr(user_id,user_code, user_name,"password",email, phone1, phone2,address,branch_code,user_sign,create_date, update_date)
		Select id_user,ou.user_code,ou.user_name,ou."password",ou.email,ou.phone1,ou.phone2,ou.address,us1.branch_code,user_create,date_create,current_date 
		from "#temp_ousr" ou, "#temp_usr1" us1;
		
		INSERT INTO apz_usr1(user_id, branch_code, branch_name, assign_date) 
		Select id_user,us1.branch_code, ob.branch_name, Current_Date from "#temp_usr1" us1 join apz_obpl ob on us1.branch_code = ob.branch_id;
		
		insert into apz_usr2(user_id, default_branch, default_warehouse, assign_date) 
		Select id_user, us2.default_branch, us2.default_warehouse, Current_Date ou from "#temp_usr2" us2;
	END IF;
	RETURN QUERY SELECT MsgCode,Msg;
END;
$BODY$;

ALTER FUNCTION "POSMAN_TEST".posmanerp_user_savedata(character, integer)
    OWNER TO "POSMAN";
