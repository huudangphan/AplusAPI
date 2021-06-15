create or replace function aplus_pricelist_get(
    _doc_entry int default -1,
    _list_name varchar(254) default '',
    _from_date date default null,
    _to_date date default null,
    _page_index int default 0,
    _page_size int default 0,
    _search_name varchar(254) default ''
)
    returns setof refcursor
    language plpgsql
as
$$
declare
    pagination  refcursor default 'pagination';
    object_data refcursor default 'object_data';
    _sum_record int default 0;
begin
    create local temp table _temp_filter as
    select *, row_number() over (ORDER BY create_date, create_time desc) row_num
    from apz_opln
    where true
        and case when coalesce(_doc_entry, -1) = -1 then true else doc_entry::varchar like '%' || _doc_entry || '%' end
        and case when coalesce(_list_name, '') = '' then true else list_name like '%' || _list_name || '%' end
        and case when _from_date isnull then true else _from_date >= create_date end
        and case when _to_date isnull then true else _to_date <= create_date end
        and _search_name = ''
       or _search_name isnull
       or doc_entry::varchar like '%' || _search_name || '%'
       or list_name like '%' || _search_name || '%'
       or remarks like '%' || _search_name || '%'
    order by create_date, create_time desc;

    _sum_record = coalesce((select count(1) from _temp_filter), 0);

    open pagination for select _page_index                           page_index,
                               _page_size                            page_size,
                               _sum_record                           sum_record,
                               case
                                   when _page_index <= 0 then 0
                                   when (_sum_record / _page_size) * _page_size < _sum_record
                                       then _sum_record / _page_size + 1
                                   else _sum_record / _page_size end page_total;
    create local temp table _temp_result as
    select * from _temp_filter limit 0;

    insert into _temp_result
    select *
    from _temp_filter
    where _page_size <= 0
       or _page_index <= 0
       or (row_num - 1 >= _page_index * _page_size and row_num < (_page_index + 1) * _page_size);

    open object_data for select *
                         from _temp_result
                         where _page_size <= 0
                            or _page_index <= 0
                            or (row_num - 1 >= _page_index * _page_size and row_num < (_page_index + 1) * _page_size);

    return next pagination;
    return next object_data;
end;
$$




