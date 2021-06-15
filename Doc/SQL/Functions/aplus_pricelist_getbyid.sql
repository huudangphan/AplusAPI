create or replace function aplus_pricelist_getbyid(_doc_entry int, _user_id int default -1)
    returns setof refcursor
    language plpgsql
as
$$
declare
    price_detail refcursor default 'price_detail';
    item_prices  refcursor default 'item_prices';
begin

    open price_detail for select * from apz_opln where doc_entry = _doc_entry;

    open item_prices for select ip.*
                         from apz_itm1 ip
                                  inner join apz_opln pl on ip.price_list = pl.doc_entry
                         where pl.doc_entry = _doc_entry;
    return next price_detail;
    return next item_prices;
end;
$$;

