create or replace function apz_document_searchdoc
(
	_doc_table character varying(10),
	_line_table character varying(10),
	_from_date date DEFAULT NULL::date,
	_to_date date DEFAULT NULL::date,
	_whs_code character varying(50) DEFAULT ''::character varying,
	_status character varying(1) DEFAULT ''::character varying,
	_search_text character varying(254) DEFAULT ''::character varying,
	_page_size integer DEFAULT 0,
	_page_index integer DEFAULT 0
)
returns setof refcursor
language 'plpgsql'
AS $BODY$
declare ref1 refcursor;
		ref2 refcursor;
		ref3 refcursor;
		sqlText character varying(1000);
		sumRecord int;
begin
	if coalesce(status, '') = '' then
		_status := 'A';
	end if;
	
	if _page_size is null then
		_page_size := 0;
	end if;

	if _page_index is null then
		_page_index := 0;
	end if;
	
	sqlText := 'create local temp table "#tempDoc" as 
	select *, row_number() over(order by doc_entry desc) row_num from ' || _doc_table ||
	' where 1 = 1' || (case when _from_date is null then '' else ' and doc_date >= ''' || cast(_from_date as character varying(15)) || '''' end)
	|| (case when _to_date is null then '' else ' and doc_date <= ''' || cast(_to_date as character varying(15)) || '''' end)
	|| (case when _status = 'A' then '' else ' and status = ''' || _status || '''' end)
	|| (case when coalesce(_whs_code, '') = '' then '' else ' and whs_code = ''' || _whs_code || '''' end);
	
	execute (sqlText);
	
	sumRecord := coalesce((select count(1) from "#tempDoc"), 0);
	
	open ref1 for select _page_index "_page_index", case when _page_size <= 0 then null when (sumRecord/_page_size)*_page_size < sumRecord then sumRecord/_page_size + 1 else sumRecord/_page_size end "page_total",
						_page_size "_page_size", sumRecord "total";
	
	create local temp table "#docResult" as (select * from "#tempDoc" limit 0);
	insert into "#docResult" select * from "#tempDoc" where _page_size <= 0 or _page_index < 0 or row_num between _page_index*_page_size and (_page_index + 1)*_page_size;
	sqlText := 'select a.*, b.user_name, coalesce((select sum(total_bef_v) from ' || _line_table || ' xx where xx.doc_entry = a.doc_entry),0) temp_total 
				from "#docResult" a left join apz_ousr b ON a.user_sign = b.user_id order by a.doc_entry DESC';
	open ref2 for execute(sqlText);
	raise notice '%', sqlText;
	
	sqlText := 'select a.doc_entry, a.line_num, a.item_code, a.item_name, a.quantity, a.price, a.currency, a.line_total, a.vat_group, a.vat_prcnt, a.unit_msr, a.total_bef_v, a.free_text
				from ' || _line_table || ' a inner join "#docResult" b ON a.doc_entry = b.doc_entry order by a.doc_entry DESC';
	open ref3 for execute(sqlText);
	
	return next ref1;
	return next ref2;
	return next ref3;
end
$BODY$;