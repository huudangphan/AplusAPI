create or replace function aplus_item_getbyid(_id varchar(50))
    returns setof refcursor
    language plpgsql
as
$$
declare
    item      refcursor default 'object_data';
    component refcursor default 'document_data';
    attachment refcursor default 'attachment';
begin
    open item for select * from apz_ogit where f_itm_code = _id;
    open component for select * from apz_oitm it where f_itm_code = _id;
    open attachment for select atc.* from apz_atc1 atc inner join apz_oitm itm on itm.atc_entry = atc.doc_entry where f_itm_code = _id;  

    return next item;
    return next component;
    return next attachment;
end;
$$;

