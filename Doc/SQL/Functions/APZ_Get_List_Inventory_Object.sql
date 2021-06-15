 create or replace function APZ_Get_List_Inventory_Object()
 returns table (obj_type character varying(20))
 language 'plpgsql'
 as $body$
 begin
 	create local temp table "#tbl" (obj_type character varying(20));
	insert into "#tbl" values ('18'), ('20'), ('21'), ('59'), ('60'), ('67'), ('147065');
	return query select * from "#tbl";
 end $body$