create or replace function aplus_pricelist_delete(_doc_entry int)
    returns table
            (
                msg_code int,
                msg      varchar(50)
            )
    language plpgsql
as
$$
begin
    if(not exists (select 1 from apz_opln where doc_entry = _doc_entry)) then
        return query (select 400010, 'Doc entry not found. Please try again!'::varchar(50));
        return;
    end if;
    
    if (exists(select 1 from apz_ocrd where list_num = _doc_entry)) then
        return query (select 500020, 'Price list in used on business partner. Could not delete!'::varchar(50));
        return;
    end if;
    if (exists(select 1 from apz_oivl where price_list = _doc_entry)) then
        return query (select 400220, 'Price list already on transaction. Could not delete!'::varchar(50));
        return;
    end if;

    delete from apz_opln where doc_entry = _doc_entry;
    delete from apz_itm1 where price_list = _doc_entry;

    return query (select 200, 'Success'::varchar(50));
end;
$$
