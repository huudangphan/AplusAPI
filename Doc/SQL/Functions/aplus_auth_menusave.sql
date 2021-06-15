create or replace function aplus_auth_menusave(_user_sign integer)
	returns table (mag_code int, message character varying(254))
	language 'plpgsql'
as $BODY$
BEGIN

	if exists(select 1 from _menu where coalesce(menu_code, '') = '')
	 or exists(select 1 from _menu group by menu_code having count(1) > 1) then
		return query select 10352 :: integer, 'Dữ liệu danh sách chức năng không chính xác' :: character varying;
		return;
	END IF;

	--update lại thông tin status theo format
	update _menu set status = 'Y' where coalesce(status, '') not in ('A', 'N');
	--xóa dữ liệu menu cũ
	delete from menu;
	--xóa dữ liệu phân quyền của các chức năng bị xóa
	delete from auth a 
			using auth b left join _menu c on b.menu_code = c.menu_code where a.menu_code = b.menu_code and c.menu_code is null;
	--insert dữ liệu setting mới
	insert into menu select * from _menu;
	return query select 200 :: integer, '' :: character varying;
	
END; $BODY$