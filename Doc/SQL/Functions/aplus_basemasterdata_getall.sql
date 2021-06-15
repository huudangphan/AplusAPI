CREATE OR REPLACE FUNCTION aplus_basemasterdata_getall(
    _table_name character varying DEFAULT ''::character varying,
    _code character varying DEFAULT ''::character varying,
    _name character varying DEFAULT ''::character varying,
    _status character varying DEFAULT 'A'::character varying,
    _page_index integer DEFAULT 0,
    _page_size integer DEFAULT 0,
    _order_by VARCHAR(10) DEFAULT ''::varchar(10),
    _is_ascending BOOLEAN DEFAULT TRUE,
    _from_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    _to_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    _type varchar(1) default ''::varchar(1))
    RETURNS SETOF refcursor
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000
AS
$BODY$
DECLARE
    pagination  refcursor default 'pagination';
    object_data refcursor default 'object_data';
    _sum_record INT;
BEGIN
    if (_page_index is null) then
        _page_index = 0;
    end if;
    if (_page_size is null) then
        _page_size = 0;
    end if;
    EXECUTE '
        CREATE LOCAL TEMP TABLE _temp_filter as 
        SELECT *, row_number() over (order by create_date, create_time desc) row_num FROM ' || _table_name ||
            ' WHERE 1 = 1 '
                || (CASE WHEN _code = '' OR _code IS NULL THEN ' ' ELSE ' AND code = ' || quote_literal(_code) END)
                || (CASE
                        WHEN _name = '' OR _name IS NULL THEN ''
                        ELSE ' AND name LIKE ' || quote_literal('%' || _name || '%') END)
                ||
            (CASE WHEN _status = '' or _status is NULL THEN '' ELSE ' AND status = ' || quote_literal(_status) END)
                || (CASE WHEN _from_date IS NULL THEN '' ELSE ' AND create_date >= ''' || _from_date || '''' END)
                || (CASE WHEN _to_date IS NULL THEN '' ELSE ' AND create_date <= ''' || _to_date || '''' END)
                || (Case when coalesce(_type, '') = '' then '' else ' and type = ' || quote_literal(_type) end)
                || (CASE
                        WHEN _order_by is null or _order_by = '' THEN ' Order By create_date, create_time '
                        ELSE ' Order By ' || _order_by || '' END)
        || (CASE WHEN _is_ascending = TRUE THEN ' ASC ' ELSE ' DESC ' END);
    _sum_record := COALESCE((SELECT COUNT(1) FROM _temp_filter), 0);
    OPEN pagination FOR SELECT _page_index                           page_index,
                               CASE
                                   WHEN _page_size <= 0 THEN null
                                   WHEN (_sum_record / _page_size) * _page_size < _sum_record
                                       THEN _sum_record / _page_size + 1
                                   ELSE _sum_record / _page_size END page_total,
                               _page_size                            page_size,
                               _sum_record                           sum_record;
    CREATE LOCAL TEMP TABLE _doc_result as
    select * from _temp_filter limit 0;
    insert into _doc_result
    SELECT *
    FROM _temp_filter
    WHERE _page_size <= 0
       OR _page_index <= 0
       OR (row_num - 1 >= _page_index * _page_size and row_num - 1 < (_page_index + 1) * _page_size);
    --     (row_num + _page_size >= _page_index * _page_size and row_num + _page_size < (_page_index + 1) * _page_size);
--         (row_num + (_page_size) between _page_index*_page_size and (_page_index + 1)*_page_size);
    open object_data for SELECT *
                         FROM _doc_result;

    RETURN NEXT pagination;
    RETURN NEXT object_data;
END
$BODY$;

select 