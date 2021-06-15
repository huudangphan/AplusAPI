CREATE OR REPLACE FUNCTION apz_document_get_table_doc(_obj_type character varying(20))
RETURNS character varying(10)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE ret character varying(10);
BEGIN
	ret = (select CASE _obj_type
		when '13' then 'apz_oinv'
		when '14' then 'apz_orin'
		when '15' then 'apz_odln'
		when '16' then 'apz_ordn'
		when '17' then 'apz_ordr'
		when '18' then 'apz_opch'
		when '19' then 'apz_orpc'
		when '20' then 'apz_opdn'
		when '21' then 'apz_orpd'
		when '22' then 'apz_opor'
		when '23' then 'apz_oqut'
		when '1470000113' then 'apz_oprq'
		when '540000006' then 'apz_opqt'
		when '59' then 'apz_oign'
		when '60' then 'apz_oige'
		when '67' then 'apz_owtr'
		end
	);
		
	return ret;
END
$BODY$