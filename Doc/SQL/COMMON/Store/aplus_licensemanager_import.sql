create or replace function aplus_licensemanager_import(_bpcode character varying(50), _install_number character varying(100), _hardware_key character varying(100)
													  , _license_info text, _file_id character varying(50), _license_type character varying(10)
													   , _date_request text)
	returns table (msg_code int, message character varying(254))
	language 'plpgsql'
as $BODY$
declare
	_oldinstall_number character varying(50);
BEGIN
	---------------------- Lấu dữ liệu installNumber của license cũ
	_oldinstall_number = coalesce((select install_number from license_info where hardware_key = _hardware_key), '');

	----------------------- Xóa dữ liệu mapping các license type không có trong dữ liệu của license moi
	delete from user_license a where install_number = _oldinstall_number and not exists(select 1 from _temp_license_type x where x.license_type = a.license_type);

	----------------------- Lấy thông tin license type cũ và số lượng đã mapping
	create local temporary table _temp_old_lic as
		select license_type, user_code, row_number() over(partition by license_type order by 1) count_id from user_license where install_number = _oldinstall_number;

	----------------------- Xóa dữ liệu mapping của các license type có số lượng mapping lớn hơn số lượng trong license mới
	delete from user_license a 
		using _temp_old_lic b, _temp_license_type c
			where a.license_type = b.license_type and a.license_type = c.license_type and a.user_code = a.user_code and b.count_id > c.amount;

	----------------------- Xóa dữ liệu license cũ
    DELETE From license_info  where hardware_key = _hardware_key;
	----------------------- Xóa dữ liệu token cũ yêu cầu login lại sau khi mapping license
    DELETE FROM user_token;

	------------------------ INSERT dữ liệu license mới
    INSERT INTO license_info (bp_code,install_number,hardware_key,license_info, file_id, license_type, date_request) 
		values (_bpcode, _install_number, _hardware_key, _license_info, _file_id, _license_type, _date_request);

	------------------------ Update dữ liệu mapping license cũ sang license mới
	update user_license set install_number = _install_number where install_number = _oldinstall_number;
	
	return query select 200 :: int, '' :: character varying;

END; $BODY$