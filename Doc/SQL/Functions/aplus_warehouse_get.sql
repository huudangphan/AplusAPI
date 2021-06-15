create or replace function aplus_warehouse_get(
    _page_index int default 0,
    _page_size int default 0,
    _whs_code varchar(50) default '',
    _whs_name varchar(254) default '',
    _status varchar(1) default '',
    _cpn_codes varchar(500) default '',
    _text_search varchar(254) default '',
    _from_date date default current_timestamp,
    _to_date date default current_timestamp
) returns setof refcursor
    language plpgsql
as
$$
declare
    _sum_record int default 0;
    pagination  refcursor default 'pagination';
    object_data refcursor default 'object_data';
begin
    if (_page_index is null) then
        _page_index = 0;
    end if;
    if (_page_size is null) then
        _page_size = 0;
    end if;

    create local temp table _temp_filter as
    select *, row_number() over (order by create_date ,create_time desc) row_num
    from warehouse
    where (coalesce(_whs_code, '') = '' or whs_code like '%' || _whs_code || '%')
      and (coalesce(_whs_name, '') = '' or whs_name like '%' || _whs_name || '%')
      and (coalesce(_status, 'A') = 'A' or status = _status)
      and (_from_date is null or create_date >= _from_date)
      and (_to_date is null or create_date <= _to_date)
      and (coalesce(_cpn_codes, '') = '' or cpn_code in (unnest(string_to_array(_cpn_codes, ','))))
      and (coalesce(_text_search, '') = '' or
           whs_code like '%' || _text_search || '%' or
           whs_name like '%' || _text_search || '%' or
           city like '%' || _text_search || '%' or
           country like '%' || _text_search || '%' or
           street like '%' || _text_search || '%')
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

    create local temp table _temp_result as
    select * from _temp_filter limit 0;
    insert into _temp_result
    select *
    from _temp_filter
    where _page_size <= 0
       or _page_index <= 0
       or (row_num - 1 >= _page_index * _page_size and row_num - 1 < (_page_index + 1) * _page_size);
    open object_data for select * from _temp_result;
--     raise exception '%', (select count(1) from _temp_filter);
    return next pagination;
    return next object_data;
end;
$$





