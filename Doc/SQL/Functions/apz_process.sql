CREATE OR REPLACE FUNCTION apz_process(
	objtype character varying DEFAULT ''::character varying,
	transtype character varying DEFAULT ''::character varying,
	num_of_key integer DEFAULT 0,
	key_columns character varying DEFAULT ''::character varying,
	obj_id character varying DEFAULT ''::character varying)
    RETURNS TABLE(error character varying, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
declare "error" varchar = '';
 	declare "message" varchar = '';
 	declare "tbldoc" varchar(100) = '';
 	declare "tblLine" varchar(100) = '';
 	declare "listColumns" varchar = ''; 
 	declare "sqlstr" varchar = ''; 
 	declare "mapCols" varchar = '';
 	declare "mapValue" varchar = '';
begin	
	return query select "error","message";
end;
$BODY$;

ALTER FUNCTION apz_process(character varying, character varying, integer, character varying, character varying)
    OWNER TO "POSMAN";
