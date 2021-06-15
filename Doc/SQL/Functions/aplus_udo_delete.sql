-- FUNCTION: aplus_udo_delete(character varying)

-- DROP FUNCTION aplus_udo_delete(character varying);

CREATE OR REPLACE FUNCTION aplus_udo_delete(
	_code character varying)
    RETURNS TABLE(msg_code integer, msg character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
begin
	--Check udo
    If NOT EXISTS(Select 1 from user_object where code = _code)
	THEN
 		RETURN QUERY SELECT Cast(10182 as int) as msg_code, Cast('Object_code not found.' as varchar) as msg;
		Return;
 	End if;
	--Check udf
	If EXISTS(Select 1 from apz_oudf x join user_object y on x.linked_object = y.code where x.linked_type = '02' And x.linked_object = _code)
	THEN
 		RETURN QUERY SELECT Cast(10185 as int) as msg_code, Cast('Object has been used by another table.' as varchar) as msg;
		Return;
 	End if;

    delete from user_object where code = _code;
	delete from user_object_child where code = _code;
	delete from user_object_search where code = _code;
	delete from user_object_default where code = _code;
	delete from user_object_child_default where code = _code;
    return query (select 200, 'Success'::varchar(50));
end;
$BODY$;
