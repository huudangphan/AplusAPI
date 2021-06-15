create or replace function aplus_pricelist_save(_tran_type varchar(1))
    returns table
            (
                msg_code int,
                msg      varchar(50)
            )
    language plpgsql
as
$$
declare
    _doc_entry    int default coalesce((select doc_entry
                                        from _apz_opln), -1);
    _current_date date default current_date;
    _current_time int default to_char(CURRENT_TIMESTAMP, 'hhmi');
begin
    if (_tran_type = 'A') then
        _doc_entry = (select max(doc_entry) + 1 from apz_opln);
        update _apz_opln set doc_entry = _doc_entry, create_date = _current_date, create_time = _current_time;

    else

        if (_doc_entry = -1) then
            return query (select 12, 'doc_entry not provided!'::varchar(50));
            return;
        end if;
        if (not exists(select 1 from apz_opln where doc_entry = _doc_entry)) then
            return query (select 400010, 'doc_entry not found. Please try again!'::varchar(50));
            return;
        end if;

        update _apz_opln
        set create_date = coalesce((select create_date from apz_opln where doc_entry = _doc_entry), _current_date),
            create_time = coalesce((select create_time from apz_opln where doc_entry = _doc_entry), _current_time);
    end if;

--     if (exists(select 1
--                from apz_itm1 ip
--                where ip.item_code in (select _itm.item_code from _apz_itm1 _itm)
--                  and ip.price_list = _doc_entry)) then
--         return query (select 500010, 'Item price has been existed on price list'::varchar(50));
--         return;
--     end if;
    
    update _apz_itm1 set price_list = _doc_entry;

    delete from apz_opln where doc_entry = _doc_entry;
    delete from apz_itm1 where price_list = _doc_entry;

    insert into apz_opln (select * from _apz_opln);
    insert into apz_itm1 (select * from _apz_itm1);
    
    return query (select 200, 'Success'::varchar(50)); 
end;
$$;

