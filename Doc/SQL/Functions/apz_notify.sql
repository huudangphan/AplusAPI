
CREATE OR REPLACE FUNCTION apz_notify(
	objtype character varying DEFAULT ''::character varying,
	transtype character varying DEFAULT ''::character varying,
	num_of_key integer DEFAULT 0,
	key_columns character varying DEFAULT ''::character varying,
	obj_id character varying DEFAULT ''::character varying)
    RETURNS TABLE(error integer, error_message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
declare "error" int = 0;
	declare "error_message" varchar(254) = '';
	begin
		return query select "error", "error_message";
	end;
$BODY$;

ALTER FUNCTION apz_notify(character varying, character varying, integer, character varying, character varying)
    OWNER TO "POSMAN";
