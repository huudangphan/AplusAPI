create or replace function aplus_auth_save(_user_sign integer)
	returns table (msg_code int, message character varying(254))
	language 'plpgsql'
as $BODY$
declare
	_user_id smallint;
BEGIN

	-- validate dữ liệu người dùng
	if exists(select 1 from _auth a left join user_info b on a.user_id = b.user_id where b.user_id is null) then
		return query select 10101 :: integer, 'Thông tin người dùng không chính xác' :: character varying;
		return;
	end if;
	
	--validate dữ liệu chức năng
	if exists(select 1 from _auth a left join menu b on a.menu_code = b.menu_code where b.menu_code is null) 
		or exists(select 1 from _auth group by menu_code having count(1) > 1)
	then
		return query select 10353 :: integer, 'Thông tin chức năng phân quyền không chính xác' :: character varying;
		return;
	end if;
	
	--lấy user phần quyền
	_user_id = (select user_id from _auth limit 1);
	--thêm dữ liệu các chức năng không có trong save data
	insert into _auth(user_id, menu_code, auth)
		select _user_id, a.menu_code, 'N' from menu a left join _auth b on a.menu_code = b.menu_code where b.menu_code is null;
	--update lại thông tin phân quyền theo format
	update _auth set auth = 'N' where coalesce(auth, '') not in ('N', 'F', 'R');
	--update lại dữ liệu log update
	update _auth set user_sign = _user_sign, update_date = current_date, update_time = cast(TO_CHAR(now(), 'HH24mm') as smallint);
	update _auth a set create_date = b.create_date, create_time = b.create_time from auth b where a.user_id = b.user_id and a.menu_code = b.menu_code;
	update _auth set create_date = current_date, create_time = cast(TO_CHAR(now(), 'HH24mm') as smallint) where create_date is null;
	--xóa dữ liệu phân quyền cũ của người dùng
	delete from auth where user_id = _user_id;
	--lưu dữ liệu phân quyền mới
	insert into auth
		select * from _auth;
		
	return query select 200 :: integer, '' :: character varying;
	
END; $BODY$