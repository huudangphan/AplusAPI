create or replace function aplus_item_document_delete(_item_code varchar(50))
    returns table
            (
                msg_code int,
                msg      varchar(50)
            )
    language plpgsql
as
$$
begin

    if exists(select 1 from apz_oitm where item_code = _item_code) then
        return query (select 10001, ''::varchar(50));
        return;
    end if;

    if exists(select 1 from apz_oivl where item_code = _item_code) then
        return query (select 10023, ''::varchar(50));
        return;
    end if;

    delete from apz_itm1 where item_code = _item_code;
    delete from apz_oitm where item_code = _item_code;
    delete from apz_oitw where item_code = _item_code;

    return query (select 200, _item_code::varchar(50));
end
$$

