CREATE OR REPLACE FUNCTION aplus_document_get(_doc_table character varying(10), _line_table character varying(10), _f_date date default null, _t_date date default null,
												_company character varying(50) default '', _status char(1) default '', _search_text character varying(254) default '',
												_page_size int default 0, _page_index int default 0)
	returns setof refcursor
	language 'plpgsql'
AS $BODY$
declare _ref_page_info refcursor;
		_ref_data refcursor;
		_sql_text character varying(50000) = '';
		_sum_record int = 0;
BEGIN

	if coalesce(_status, '') = '' then
		_status = 'A';
	end if;

	if _page_size isnull then
		_page_size = 0;
	end if;

	if _page_index isnull then
		_page_index = 0;
	end if;
	
	if coalesce(_company, '') = '' then
		_company = '';
	end if;
	
	if coalesce(_search_text, '') = '' then
		_search_text = '';
	end if;

	_sql_text = 'create local temporary table _temp_doc as select a.*, ROW_NUMBER() OVER ( ORDER BY a.doc_entry DESC) AS row_num from ' || _doc_table || ' a 
							  where 1 = 1 ' 
									-------------------------------------- FromDate -------------------------------------
									|| coalesce(('and doc_date >= ''' || TO_CHAR(_f_date, 'yyyy-MM-dd') || ''''), '')
									-------------------------------------- ToDate ---------------------------------------
									|| coalesce(('and doc_date <= ''' || TO_CHAR(_t_date, 'yyyy-MM-dd') || ''''), '')
									-------------------------------------- STATUS ---------------------------------------
									|| (case when _status = 'A' then '' else 'and doc_status = ''' || _status || '''' end)
									-------------------------------------- SEARCH TEXT ----------------------------------
									|| (case when _search_text = '' then '' else 'and cast(a.doc_entry as character varying) like ''%' || _search_text || '%''' end)
									--------------------------------------- KHO -----------------------------------------
									|| (case when _company = '' then '' else 'and company = ''' || _company || '''' end) || ';';

	execute(_sql_text);
	_sum_record = coalesce((select count(1) from _temp_doc),0);
	open _ref_page_info for
		select _page_index page_index, case when _page_size <= 0 then null when (_sum_record / _page_size) * _page_size < _sum_record 
											then _sum_record / _page_size + 1 else _sum_record / _page_size end page_total,
			_page_size page_size, _sum_record total;
	return next _ref_page_info;
	
	open _ref_data for
		select * from _temp_doc where _page_size <= 0 or _page_index < 0 or row_num -1 >= _page_index * _page_size and row_num -1 < (_page_index + 1) * _page_size
				order by doc_entry;
	return next _ref_data;
	
END; $BODY$