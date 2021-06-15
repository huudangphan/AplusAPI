create or replace function aplus_licensemanager_mappinguserlicense(_hwk character varying(50))
	returns table (msg_code int, message character varying(254))
	language 'plpgsql'
AS $BODY$
declare
	_install_number character varying(50);
BEGIN

	_install_number = coalesce((select install_number from license_info where hardware_key = _hwk limit 1), '');
	----------------------------------------------------------- VALIDATE THÔNG TIN -----------------------------------------------------------------------
	if _install_number = '' then
		return query select 1000503::int, N'Hệ thống chưa có giấy phép' :: character varying;
		return;
	END IF;
	
	delete from _temp_user_lic where coalesce(user_code,'') = '' or coalesce(license_type, '') = '';
	delete from _temp_user where coalesce(user_code, '') = '';
	if not exists(select 1 from _temp_user) then
		return query select 23 :: integer, N'Chưa cung cấp dữ liệu giấy phép của người dùng' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_user_lic group by user_code, license_type having count(1) > 1) then
		return query select 1001101 :: int, N'Thông tin giấy phép của một người dùng bị trùng lặp' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_user_lic a left join _temp_lic_info b on a.license_type = b.license_type where b.license_type is null) then
		return query select 1000506 :: integer, N'Thông tin giấy phép gán cho người dùng không chính xác' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_user_lic a left join _temp_user b on a.user_code = b.user_code where b.user_code is null) then
		return query select 1000505 :: integer, N'Thông tin gán giấy phép không đúng' :: character varying;
		return;
	end if;
	
	--xóa dữ liệu mapping giấy phép cũ của người dùng trên hệ thống
	delete from user_license a using _temp_user b where a.user_code = b.user_code and install_number = _install_number;
	--kiểm tra số lượng giấy phép đã mapping trên hệ thống
	create local temporary table _temp_mapping_amount as select license_type, count(1) amount from _temp_user_lic group by license_type;
	create local temporary table _temp_local_mapping as select license_type, count(1) amount from user_license where install_number = _install_number group by license_type;
	if exists(select 1 from _temp_lic_info a inner join _temp_mapping_amount b on a.license_type = b.license_type 
			  								 left join _temp_local_mapping c on a.license_type = c.license_type
					where coalesce(a.amount, 0) - coalesce(b.amount, 0) - coalesce(c.amount,0) < 0) then
		return query select 1000507 :: integer, N'Số lượng giấy phép sử dụng nhiều hơn số lượng giấy phép được cấp' :: character varying;
		return;		
	end if;

	----------------------------------------------------------- END Validate infomation ------------------------------------------------------------------

	insert into user_license(install_number, license_type, user_code) 
		select _install_number, license_type, user_code from _temp_user_lic;

	return query select 200 :: integer, '' :: character varying;

END; $BODY$