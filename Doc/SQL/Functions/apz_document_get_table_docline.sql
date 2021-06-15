CREATE OR REPLACE FUNCTION apz_document_get_table_docline(_obj_type character varying(20))
RETURNS character varying(10)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE ret character varying(10);
BEGIN
	ret = (select CASE _obj_type
		when '13' then 'apz_inv1'
		when '14' then 'apz_rin1'
		when '15' then 'apz_dln1'
		when '16' then 'apz_rdn1'
		when '17' then 'apz_rdr1'
		when '18' then 'apz_pch1'
		when '19' then 'apz_rpc1'
		when '20' then 'apz_pdn1'
		when '21' then 'apz_rpd1'
		when '22' then 'apz_por1'
		when '23' then 'apz_qut1'
		when '1470000113' then 'apz_prq1'
		when '540000006' then 'apz_pqt1'
		when '59' then 'apz_ign1'
		when '60' then 'apz_ige1'
		when '67' then 'apz_wtr1'
		end
	);
		
	return ret;
END
$BODY$