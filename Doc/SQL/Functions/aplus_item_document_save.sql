create or replace function aplus_item_document_save(_tran_type character varying, _user_id integer)
    returns TABLE
            (
                msg_code integer,
                msg      character varying
            )
    language plpgsql
as
$$
-- noinspection SqlInsertValues

declare
    _item_code        varchar(50) default '';
    _f_itm_code       varchar(50) default '';
    _sale_unit_entry  varchar(50) default '';
    _sale_unit_code   varchar(50) default '';
    _ugp_entry        int default -1;
    _trade_mark       varchar(50) default '';
    _itms_grp_code    varchar(50) default '';
    _tags             varchar(254) default '';
    _status           varchar(1) default 'A';
    _barcode          varchar(50) default '';
    _current_date     date default current_date;
    _current_time     smallint default to_char(current_timestamp, 'hhmi');
    _new_id           int default -1;
    _main_currency    varchar(3) default (select main_currency
                                          from apz_cinf
                                          limit 1);
    _default_whs      varchar(8) default (select whs_code
                                          from apz_oitw
                                          limit 1);
    -- grand item property; 
    _f_property1_name varchar(254) default '';
    _f_property2_name varchar(254) default '';
    _f_property3_name varchar(254) default '';
    _f_property1      varchar default '';
    _f_property2      varchar default '';
    _f_property3      varchar default '';
    _f_item_type      varchar(3) default '';
    _f_property_count smallint;
    -- inherit item property
    _property1        varchar(50);
    _property2        varchar(50);
    _property3        varchar(50);
    _n_property1      varchar(50);
    _n_property2      varchar(50);
    _n_property3      varchar(50);


begin
    if (select count(1) from _apz_oitm) > 1 then
        return query (select 10000, ''::varchar); --DataNotCorrect
        return;
    end if;

    if (select 1 from _apz_oitm) is null then
        return query (select 10004, ''::varchar); --DataNotProvide
        return;
    end if;

    -- assign data to declared parameter
    _item_code = coalesce((select item_code from _apz_oitm), ''::varchar);
    _f_itm_code = coalesce((select f_itm_code from _apz_oitm), ''::varchar);

    select s_uom_entry,
           sal_unit_msr,
           ugp_entry,
           tramrk,
           itms_grp_cod,
           tags,
           status,
           property1_name,
           property1,
           property2_name,
           property2,
           property3_name,
           property3,
           item_type,
           property_count
    into
        _sale_unit_entry,
        _sale_unit_code,
        _ugp_entry,
        _trade_mark,
        _itms_grp_code,
        _tags,
        _status,
        _f_property1_name,
        _f_property1,
        _f_property2_name,
        _f_property2,
        _f_property3_name,
        _f_property3,
        _f_item_type,
        _f_property_count
    from apz_ogit
    where f_itm_code = _f_itm_code;

    update _apz_oitm
    set ugp_entry      = _ugp_entry,
        buy_unit_msr   = _sale_unit_code,
        sal_unit_msr   = _sale_unit_code,
        invn_unit_msr  = _sale_unit_code,
        s_uom_entry    = _sale_unit_entry,
        p_uom_entry    = _sale_unit_entry,
        i_uom_entry    = _sale_unit_entry,
        num_in_buy     = 1,
        num_in_sale    = 1,
        itms_grp_cod   = _itms_grp_code,
        item_tags      = _tags,
        tramrk         = _trade_mark,
        property1_name = _f_property1_name,
        property1      = _f_property1,
        property2_name = _f_property2_name,
        property2      = _f_property2,
        property3_name = _f_property3_name,
        property3      = _f_property3,
        item_type      = _f_item_type,
        property_count = _f_property_count;

    --- validate constraint data
    if not exists(select 1 from apz_oitb where code = (select itms_grp_cod from _apz_oitm)) then
        return query (select 10025, ''::varchar); --ItemGroupNotExists
        return;
    end if;
    if not exists(select 1 from apz_otrm where code = (select tramrk from _apz_oitm)) then
        return query (select 10026, ''::varchar); --ItemTradeMarkNotExists
        return;
    end if;

    if not exists(select 1 from apz_ovtg where code = (select vat_group from _apz_oitm)) then
        return query (select 10145, ''::varchar); --VATGroupNotFound
        return;
    end if;

    _barcode = coalesce((select barcode from _apz_oitm), '');

    if (_f_itm_code != 'P') then
        update _apz_oitm
        set property1      = null,
            property2      = null,
            property3      = null,
            property1_name = null,
            property2_name = null,
            property3_name = null,
            property_count = null;
    else
        if (_f_property_count not in (1, 2, 3)) then
            return query (select 10027, ''::varchar); --ItemPropertyNotProvide
            return;
        end if;


        select property1, property2, property3
        into _property1 , _property2 , _property3
        from apz_oitm
        where item_code = _item_code;

        select coalesce(property1, ''),
               coalesce(property2, ''),
               coalesce(property3, '')
        into _n_property1 ,
            _n_property2,
            _n_property3
        from _apz_oitm;

        if exists(select 1
                  from apz_oitm
                  where f_itm_code = _f_itm_code
                    and item_code != _item_code
                    and _n_property1 != coalesce(property1, '')
                    and _n_property2 != coalesce(property2, '')
                    and _n_property3 != coalesce(property3, '')) then
            return query (select 10035, ''::varchar); --PropertyExisted
            return;
        end if;

        if (_f_property_count in (1, 2, 3)) then
            create local temp table _temp_prop1 as
            select unnest prop1 from unnest(string_to_array(_f_property1, ','));
            update _temp_prop1 set prop1 = _n_property1 where prop1 = _property1;
            _f_property1 = string_agg(prop1, ',') from _temp_prop1;

            update apz_ogit set property1 = _f_property1 where f_itm_code = _f_itm_code;

            if (_f_property_count in (1)) then
                update _apz_oitm set property2_name = null, property3_name = null, property2 = null, property3 = null;
            end if;
        end if;
        if (_f_property_count in (2, 3)) then
            create local temp table _temp_prop2 as
            select unnest prop2 from unnest(string_to_array(_f_property2, ','));
            update _temp_prop2 set prop2 = _n_property2 where prop2 = _property2;
            _f_property2 = string_agg(prop2, ',') from _temp_prop2;

            update apz_ogit set property2 = _f_property2 where f_itm_code = _f_itm_code;

            if (_f_property_count in (2)) then
                update _apz_oitm set property3_name = null, property3 = null;
            end if;
        end if;

        if (_f_property_count = 3) then
            create local temp table _temp_prop3 as
            select unnest prop3 from unnest(string_to_array(_f_property3, ','));
            update _temp_prop3 set prop3 = _n_property3 where prop3 = _property3;
            _f_property3 = string_agg(prop3, ',') from _temp_prop3;

            update apz_ogit set property3 = _f_property3 where f_itm_code = _f_itm_code;
        end if;
    end if;
    if (_tran_type = 'A') then
        if coalesce(_item_code, '') = '' then
            _new_id = coalesce((select max(cast(
                    substring(item_code from length(trim(_f_item_type)) + 1 for
                              length(trim(item_code)) - length(trim(_f_item_type))) as int))
                                from apz_oitm
                                where item_code like _f_item_type :: varchar(3) || '%'
                                  and length(trim(item_code)) > length(trim(_f_item_type)) + 1), 0) + 1;

            if _new_id < 10000 then
                _new_id = 10000;
            end if;
            _item_code = _f_item_type || cast(_new_id as varchar);
            update _apz_oitm set item_code = _item_code;
        end if;
        if (select 1 from apz_oitm where item_code = _item_code) is not null then
            return query (select 10002, ''::varchar); --DataExisted
        end if;
    else
        if coalesce((select 1 from apz_oitm where item_code = _item_code), -1) = -1 then
            return query (select 10036, ''::varchar); --ItemCodeNotFound
        end if;

        if (select 1 from apz_ogit where f_itm_code = _f_itm_code) is null then
            return query (select 10036, ''::varchar); --ItemCodeNotFound
        end if;
    end if;

    update _apz_oitm set item_type = 'N' where coalesce(item_type, '') = '';
    update _apz_oitm set prchse_item = 'Y' where coalesce(prchse_item, '') = '';
    update _apz_oitm set sell_item = 'Y' where coalesce(sell_item, '') = '';
    update _apz_oitm
    set invn_item = case when coalesce(invn_item, '') = '' then 'Y' when item_type in ('B', 'S') then 'N' end;
    update _apz_oitm set dfltwh = _default_whs where coalesce(dfltwh, '') = '';
    update _apz_oitm set series = -1 where coalesce(series, 0) = 0;
    update _apz_oitm set valid_for = _status where coalesce(valid_for, '') = '';
    update _apz_oitm set user_sign = _user_id where coalesce(user_sign, 0) = 0;

    if (_tran_type = 'A') then
        update _apz_oitm set create_date = _current_date, create_time = _current_time;
    end if;
    if (_tran_type = 'U') then
        update _apz_oitm set update_date = _current_date, update_time = _current_time;
        update _apz_oitm
        set create_date = coalesce((select create_date from apz_oitm where item_code = _item_code), _current_date);
        update _apz_oitm
        set create_time = coalesce((select create_time from apz_oitm where item_code = _item_code), _current_time);
    end if;

    if (_tran_type = 'A' and exists(select 1 from _apz_oitm where invn_item = 'Y')) then
        delete from apz_oitw where item_code = _item_code;
        insert into apz_oitw (item_code, whs_code, user_sign, locked, on_hand, on_order, is_commited)
        select _item_code, whs_code, _user_id, 'N', 0, 0, 0
        from apz_owhs;
    end if;

    delete from apz_oitm where item_code = _item_code;
    insert into apz_oitm select * from _apz_oitm;
    
    
    return query (select 200, _item_code::varchar);
end
$$;

alter function aplus_item_document_save(varchar, integer) owner to "POSMAN";
