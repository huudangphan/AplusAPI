create or replace function aplus_item_component_save(_tran_type character varying, _user_id integer)
    returns table
            (
                msg_code int,
                obj      varchar(50)
            )
    language plpgsql
as
$$
DECLARE
    _current_date        date default current_date;
    _current_time        smallint default (select to_char(now(), 'HHMI'));
    _item_code           varchar(50) default '';
    _item_type           varchar(3) default '';
    _new_id              int default 0;
    _old_value_item_type varchar(1) default '';
    _ugp_entry           int default -1;
    _default_whs         varchar(8) default '';
    _main_currency       varchar(3) default '';
    _property_count      smallint default 0;
    _sale_unit_entry     varchar(50) default '';
    _sale_unit_code      varchar(50) default '';
BEGIN

    -- Validate
    if (select count(1) from _apz_ogit) > 1 then
        return query (select 1000, ''::varchar(50)); --DataNotCorrect
        return;
    end if;

    if (not exists(select 1 from _apz_ogit)) then
        return query (select 10004, ''::varchar(50)); --DataNotProvide
        return;
    end if;


    if not exists(select 1 from apz_oitb where code = (select itms_grp_cod from _apz_ogit)) then
        return query (select 10025, ''::varchar(50)); --ItemGroupNotExists
        return;
    end if;

    if not exists(select 1 from apz_otrm where code = (select tramrk from _apz_ogit)) then
        return query (select 10026, ''::varchar(50)); --ItemTradeMarkNotExists
        return;
    end if;

    -- Update variable
    _item_code = coalesce((select f_itm_code from _apz_ogit), '');
    _default_whs = coalesce((select whs_code from apz_owhs limit 1), '');
    _main_currency = coalesce((select main_currency from apz_cinf), '');

    if (_tran_type = ' A
                          ') then
        update _apz_ogit set user_sign = _user_id where coalesce(user_sign, 0) = 0;
    else
        update _apz_ogit
        set user_sign = coalesce((select user_sign from apz_ogit where f_itm_code = _item_code), _user_id);
        update _apz_ogit set user_sign2 = _user_id where coalesce(user_sign2, 0) = 0;
    end if;
    update _apz_ogit set item_type = 'N' where coalesce(item_type, '') = '';
    update _apz_ogit
    set property1_name = null,
        property2_name = null,
        property3_name = null,
        property_count = null
    where property_item = 'N';
    update _apz_ogit set property1 = null where coalesce(property1_name, '') = '';
    update _apz_ogit set property2 = null where coalesce(property2_name, '') = '';
    update _apz_ogit set property3 = null where coalesce(property3_name, '') = '';

    if (select 1 from _apz_ogit where item_type != 'N' and property1 = '' and property2 = '' and property3 = '')
    then
        _property_count = coalesce((select property_count from _apz_ogit), 0);
        if (_property_count = 0) then
            return query (select 10027, ''::varchar(50)); --ItemPropertyNotProvide
            return;
        elseif (_property_count = 1) then
            if (select 1 from _apz_ogit where coalesce(property1, '') = '') then
                return query (select 10027, ''::varchar(50)); --ItemPropertyNotProvide
                return;
            end if;
        elseif (_property_count = 2) then
            if (select 1 from _apz_ogit where coalesce(property2, '') = '') then
                return query (select 10027, ''::varchar(50)); --ItemPropertyNotProvide
                return;
            end if;
        elseif (_property_count = 3) then
            if (select 1 from _apz_ogit where coalesce(property3, '') = '') then
                return query (select 10027, ''::varchar(50)); --ItemPropertyNotProvide
                return;
            end if;
        else
            return query (select 10028, ''::varchar(50)); --ItemPropertyNotCorrect
            return;
        end if;
    end if;

    -- unit of measure processing
    select coalesce(s_uom_entry, ''), sal_unit_msr, coalesce(ugp_entry, -1), coalesce(item_type, 'N')
    into _sale_unit_code, _sale_unit_code, _ugp_entry, _item_type
    from _apz_ogit;

    if exists(select 1
              from apz_oivl a
                       inner join apz_oitm itm on a.item_code = itm.item_code
              where a.ugp_entry != _ugp_entry
                and itm.f_itm_code = _item_code) then
        return query (select 10023, ''::varchar(50)); --DataOnTransaction
        return;
    end if;

    -- Process Data
    if (_tran_type = 'A') then
        -- TODO: Find way to select exact prefix of item code (if user input same as generate)
        if coalesce(_item_code, '') = '' then
            _new_id = coalesce((select max(cast(substring(f_itm_code from length(trim(_item_type)) + 1 for
                                                          length(trim(f_itm_code)) - length(trim(_item_type))) as int))
                                from apz_ogit
                                where f_itm_code like _item_type::varchar(3) || '%'), 0) + 1;
            if _new_id < 10000 then
                _new_id = 10000;
            end if;
            _item_code = _item_type || cast(_new_id as varchar);
            update _apz_ogit set f_itm_code = _item_code, create_date = _current_date, create_time = _current_time;
        end if;
    else
        if not exists(select 1 from apz_ogit where f_itm_code = _item_code) then
            return query (select 10001, ''::varchar(50)); --DataNotFound
            return;
        end if;
        if exists (select 1
            from apz_ogit
            where f_itm_code = _item_code
              and coalesce(item_type, 'N') = 'P'
              and _item_type != 'P') then
            return query (select 10032, ''::varchar(50)); --PropertyNotUpdate
            return;
        end if;

        create local temp table _temp_item_create as
        select * from apz_oitm where f_itm_code = _item_code;

        if ((select 1 from apz_ogit where f_itm_code = _item_code and coalesce(item_type, 'N') != 'P') is not null) then
            if ((select 1
                 from apz_oivl a
                          inner join _temp_item_create tic on a.item_code = tic.item_code) is not null) then
                return query (select 10023, ''::varchar(50)); --DataOnTransaction
                return;
            end if;
        end if;
        if (_item_type = 'P' and (select 1
                                  from apz_oitm a,
                                       _apz_ogit b
                                  where a.f_itm_code = _item_code
                                      and (coalesce(a.property1, '') != '' and (select 1
                                                                                from unnest(string_to_array(b.property1, ','))
                                                                                where unnest = a.property1) is null)
                                     or (coalesce(a.property2, '') != '' and (select 1
                                                                              from unnest(string_to_array(b.property2, '
                           , '))
                                                                              where unnest = a.property2) is null)
                                     or (coalesce(a.property3, '') != '' and (select 1
                                                                              from unnest(string_to_array(b.property3, '
                           , '))
                                                                              where unnest = a.property3) is null)
        ) is not null) then
            return query (select 10034, ''::varchar(50)); --PropertiesUsedByComponents
            return;
        end if;


        --         _old_value_item_type = coalesce((select item_type from apz_ogit where f_itm_code = _item_code), 'N');
--         if ((select 1
--              from apz_oivl a
--                       inner join _temp_item_create c on a.item_code = c.item_code and
--                                                         ((_old_value_item_type in ('B', 'M') and _old_value_item_type != _item_type))) is not null) then
--             return query select 400220, 'Item in transaction, could not update item combo property!':: varchar;
--         end if;

        if (_tran_type = ' A ') then
            update _apz_ogit set create_date = _current_date, create_time = _current_time;
        end if;

        if (_tran_type = ' U ') then
            update _apz_ogit set update_date = _current_date, update_time = _current_time;
            update _apz_ogit
            set create_date = (select create_date from apz_ogit where f_itm_code = _item_code),
                create_time = (select create_time from apz_ogit where f_itm_code = _item_code);
        end if;
    end if;

    delete from apz_ogit where f_itm_code = _item_code;
    insert into apz_ogit (select * from _apz_ogit);

    if (_tran_type = ' U ') then
        update apz_oitm as a
        set itms_grp_cod   = b.itms_grp_cod,
            tramrk         = b.tramrk,
            item_tags      = b.tags,
            valid_for      = b.status,
            ugp_entry      = b.ugp_entry,
            property1_name = b.property1_name,
            property2_name = b.property2_name,
            property3_name = b.property3_name,
            property_item  = b.property_item,
            property_count = b.property_count
        from _apz_ogit as b
        where a.f_itm_code = b.f_itm_code;
    end if;
    return query (select 200, _item_code::varchar(50));
    return;
END ;

$$;

alter function aplus_item_component_save(varchar, integer) owner to "POSMAN";

