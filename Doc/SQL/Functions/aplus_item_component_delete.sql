create or replace function aplus_item_component_delete(_item_code character varying)
    returns TABLE
            (
                msg_code integer,
                msg      character varying
            )
    language plpgsql
as
$$
begin
    if not exists(select 1 from apz_ogit where f_itm_code = _item_code) then
        return query (select 10001, ''::varchar); --DataNotFound
        return;
    end if;

    if exists(select 1
              from apz_oivl a
                       inner join apz_oitm ao on a.item_code = ao.item_code
              where f_itm_code = _item_code) then
        return query (select 10023, ''::varchar); --DataOnTransaction
        return;
    end if;
    
    ------------------------------------ delete item and components------------------------
    -- delete attachment of components
    delete from apz_oatc where doc_entry in (select atc_entry from apz_oitm where f_itm_code = _item_code);
    delete from apz_atc1 where doc_entry in (select atc_entry from apz_oitm where f_itm_code = _item_code);
    
    -- delete in constraint first
    delete from apz_oitw where item_code in (select itm.item_code from apz_oitm itm where f_itm_code = _item_code);
    delete from apz_itm1 where item_code in (select itm.item_code from apz_oitm itm where f_itm_code = _item_code);

    -- do execute when all resource released
    delete from apz_ogit where f_itm_code = _item_code;
    delete from apz_oitm where f_itm_code = _item_code;
    

    return query (select 200, _item_code::varchar(50));
end
$$;

alter function aplus_item_component_delete(varchar) owner to "POSMAN";
