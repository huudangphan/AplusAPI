-- FUNCTION: aplus_udt_delete(character varying)

-- DROP FUNCTION aplus_udt_delete(character varying);

CREATE OR REPLACE FUNCTION aplus_udt_delete(
	_table_name character varying)
    RETURNS TABLE(msg_code integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
begin
	--Check table_name
    If NOT EXISTS(Select 1 from user_table x where x.table_name = _table_name) THEN
 		RETURN QUERY SELECT Cast(10163 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
 	End if;

	--Check udo
	--Udo Header
	If EXISTS(Select 1 from user_object x join user_table y on x.header_table = y.table_name where x.header_table = _table_name)
	THEN
 		RETURN QUERY SELECT Cast(10165 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
 	End if;
	--Udo Child
	If EXISTS(Select 1 from user_object_child x join user_table y on x.child_table = y.table_name where x.child_table = _table_name)
	THEN
 		RETURN QUERY SELECT Cast(10165 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
 	End if;
	
	--Check udf
	If EXISTS(Select 1 from apz_oudf x join user_table y on x.linked_object = y.table_name where x.linked_type = '00' And x.linked_object = _table_name)
	THEN
 		RETURN QUERY SELECT Cast(10165 as int) as msg_code, Cast('' as varchar) as msg;
		Return;
 	End if;
	
	--Delete record from table
    delete from user_table where table_name = _table_name;
	--Drop table in database
	_table_name := 'u_' || _table_name;
	Execute 'Drop table ' || _table_name || ';';
	
    return query (select 200, 'Success'::varchar(50));
end;
$BODY$;
