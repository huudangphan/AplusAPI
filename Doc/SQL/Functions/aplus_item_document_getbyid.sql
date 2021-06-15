create or replace function aplus_item_document_getbyid(_id varchar(50), _price_list smallint default -1)
    returns setof refcursor
    language plpgsql
as
$$
declare
    _ugp_entry  int default -1;
    _atc_entry  int default -1;
    attachment  refcursor default 'attachments';
    object_data refcursor default 'object_data';
    price_lists refcursor default 'price_lists';
    uoms        refcursor default 'uoms';

begin
    select coalesce(ugp_entry, -1), coalesce(atc_entry, -1)
    into _ugp_entry, _atc_entry
    from apz_oitm
    where item_code = _id;
    open object_data for select it.* from apz_oitm it where it.item_code = _id;
    open price_lists for select * from apz_itm1 where item_code = _id;
    open attachment for select * from apz_atc1 where doc_entry = _atc_entry;
    open uoms for select * from apz_ugp1 where ugp_entry = _ugp_entry;

    return next object_data;
    return next price_lists;
    return next attachment;
    return next uoms;
end;
$$;
