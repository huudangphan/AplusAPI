CREATE OR REPLACE FUNCTION Aplus_Payment_GetListPayment
(
	_f_date date default null,
	_t_date date default null,
	_text_search character varying(254) default '',
	_filter character varying(254) default '',
	_group_type character varying(254) default '',
	_doc_type character varying(254) default '',
	_page_size int default 0,
	_page_index int default 0,
	_obj_type character varying(20) default ''
)
RETURNS SETOF refcursor
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE _ref1 refcursor;
		_sum_record int;
BEGIN
	if _page_size isnull then
		_page_size = 0;
	end if;

	if _page_index isnull then
		_page_index = 0;
	end if;

	if _group_type isnull then
		_group_type = '';
	end if;

	if _doc_type isnull then
		_doc_type = '';
	end if;

	if _filter isnull then
		_filter = '';
	end if;

	if _text_search isnull then
		_text_search = '';
	end if;

	if _obj_type isnull then
		_obj_type = '';
	end if;

	create temp table _temp_doc as (
		select *, ROW_NUMBER() OVER ( ORDER BY doc_entry DESC) AS row_num from apz_orct a where (_f_date isnull or doc_date >= _f_date)
					and (_t_date isnull or doc_date <= _t_date)
					and (_text_search = '' or cast(doc_entry as varchar) like '%' || _text_search || '%' or ref1 like '%' || _text_search || '%')
					and (_filter = '' or _filter = 'A' or _filter = 'L' and canceled = 'Y' or _filter = 'O' and doc_status = 'O' or _filter = 'C' and canceled = 'N' and doc_status = 'C')
					and (_group_type = '' or exists(select 1 from unnest(string_to_array(_groupType, ';')) where unnest = group_type))
					and (_doc_type = '' or exists(select 1 from unnest(string_to_array(_docType, ';')) where unnest = doc_type))
					and (_obj_type = '' or obj_type = _obj_type)
	);

	_sum_record = coalesce((select count(1) from _temp_doc), 0);
	select _page_index page_index, case when _page_size <= 0 then null when (_sum_record / _page_size) * _page_size < _sum_record then _sum_record / _page_size + 1 else _sum_record / _page_size end page_total,
			_page_size page_size, _sum_record total;

	create temp table _result as (select * from _temp_doc where _page_size <= 0 or _page_index < 0 or (row_num - 1 >= _page_size * _page_index and row_num - 1 < (_page_index + 1) * _page_size));

	open _ref1 for select a.*, b.user_name, c.name doc_name, d.name group_name, (select string_agg(pay_mth, ',') from apz_rct1 where doc_entry = a.doc_entry) pay_mth 
				from _result a inner join apz_ousr b on a.user_sign = b.user_id 
				inner join apz_opmg c on a.doc_type = c.code and c.ObjType = a.obj_type
				inner join apz_opmr d on a.group_type = d.code order by doc_entry desc;
				
	return next _ref1;

END $BODY$