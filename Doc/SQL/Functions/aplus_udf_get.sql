create or replace function aplus_udf_get(
    _table_name varchar(50),
    _column_id varchar(50),
    _column_name varchar(254),
    _data_type varchar(20),
    _search_name varchar(50),
    _page_index int default 0,
    _page_size int default 0)
    returns setof refcursor
    language plpgsql
as
$$
declare
    pagination  refcursor default 'pagination';
    object_data refcursor default 'object_data';
    _sum_record int default 0;
begin
    if _page_size isnull then
        _page_size = 0;
    end if;
    if _page_index isnull then
        _page_index = 0;
    end if;

    create local temp table _temp_filter as
    select *, row_number() over (order by create_date, create_time desc) row_num
    from apz_oudf
    where (coalesce(_table_name, '') = '' or table_name like '%' || _table_name || '%')
      and (coalesce(_column_id, '') = '' or column_id like '%' || _column_id || '%')
      and (coalesce(_column_name, '') = '' or column_name like '%' || _column_name || '%')
      and (coalesce(_data_type, '') = '' or data_type like '%' || _data_type || '%')
      and (coalesce(_search_name, '') = '' or table_name like '%' || _search_name || '%' or
           column_id like '%' || _search_name || '%' or column_name like '%' || _search_name || '%' or
           data_type like '%' || _search_name || '%')
    order by create_date, create_time desc;

    _sum_record = coalesce((select count(1) from _temp_filter), 0);

    open pagination for select _page_index                           page_index,
                               case
                                   when _page_size <= 0 then 0
                                   when (_sum_record / _page_size) * _page_size < _sum_record
                                       then _sum_record / _page_size + 1
                                   else _sum_record / _page_size end page_total,
                               _page_size                            page_size,
                               _sum_record                           sum_record;

    open object_data for select *
                         from _temp_filter
                         where _page_size <= 0
                            or _page_index <= 0
                            or (row_num - 1 >= _page_index * _page_size and
                                row_num - 1 < (_page_index + 1) * _page_size);

    return next pagination;
    return next object_data;
end;
$$