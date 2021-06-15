-- FUNCTION: aplus_businesspartnermasterdata_getliabilities(character varying)

-- DROP FUNCTION aplus_businesspartnermasterdata_getliabilities(character varying);

CREATE OR REPLACE FUNCTION aplus_businesspartnermasterdata_getliabilities(
	code character varying)
    RETURNS SETOF refcursor 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    ref1 refcursor;
	currency varchar(3);
BEGIN
	EXECUTE 'CREATE LOCAL TEMP TABLE "#temp_data" as 
        select obj_type, doc_total from APZ_OINV 
								where card_code ='''|| code ||''' and canceled not in (''Y'', ''C'')
		union ALL
		select obj_type, doc_total from APZ_ORDN 
								where card_code = '''|| code ||''' and canceled not in (''Y'', ''C'')
		union all
		select x.obj_type, sum(y.line_total) doc_total
						from APZ_ORCT x inner join APZ_RCT1 y ON x.doc_entry = y.doc_entry 
					where group_type = ''1001'' and card_code = '''||code ||''' and x.canceled not in (''Y'', ''C'') and y.pay_mth <> ''COD''
					group by x.doc_entry, x.obj_type, x.doc_cur, x.create_date, x.create_time, x.user_sign, x.comments;';
					
	update "#temp_data" set doc_total = -1 * doc_total where obj_type in ('16', '810018');
	
	Select main_currency into currency from APZ_CINF;
	
	OPEN ref1 FOR 
	SELECT sum(doc_total) liab_total, currency FROM "#temp_data" WHERE 1 = 1; 

	Return Next ref1;
END;
$BODY$;
