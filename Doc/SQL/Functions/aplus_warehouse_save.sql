create or replace function aplus_warehouse_save(_tran_type varchar(1))
    returns int
    language plpgsql
as
$$
declare
    _current_date date default current_date;
    _current_time smallint default to_char(current_timestamp, 'hh24mi');
    _whs_code     varchar(50) default coalesce((select whs_code
                                                from _warehouse), '');
    _cpn_code     varchar(50) default coalesce((select cpn_code
                                                from _warehouse), '');
begin


    if (_whs_code = '') then
        return 10005;
    end if;

    if exists((select 1 from company where code = _cpn_code)) then
        return 10124;
    end if;

    if (exists(select 1
               from _warehouse a
                        inner join geography b on a.country = b.code
               where b.type = 'C')) then
        return 10058;
    end if;
    if (exists(select 1
               from _warehouse a
                        inner join geography b on a.city = b.code
               where b.type = 'P')) then
        return 10059;
    end if;
    if (exists(select 1
               from _warehouse a
                        inner join geography b on a.district = b.code
               where b.type = 'D')) then
        return 10060;
    end if;
    if (exists(select 1
               from _warehouse a
                        inner join geography b on a.ward = b.code
               where b.type = 'W')) then
        return 10061;
    end if;

    if (_tran_type = 'A') then
        if exists((select 1 from warehouse where whs_code = _whs_code)) then
            return 10057;
        end if;
        update _warehouse set create_date = _current_date, create_time = _current_time;
    else
        
        if (_tran_type = 'U') then
            if(not exists(select 1 from warehouse where whs_code = _whs_code)) then
                return 10125;
            end if;
            
            if(exists(select 1 from warehouse_journal where whs_code = _whs_code)) then
                return 10023;
            end if;
            if( (select status from _warehouse) != (select status from warehouse)) then
                if (exists(select 1
                           from item_warehouse
                           where whs_code = _whs_code
                             and (on_hand > 0
                               or is_commited > 0
                               or on_order > 0))) then
                    return 10062;
                end if;
            end if;

            update _warehouse a
            set (create_date, create_time, update_date, update_time, user_sign) = (coalesce(b.create_date, _current_date),
                                                                                   coalesce(b.create_time, _current_time),
                                                                                   _current_date, _current_time,
                                                                                   coalesce(b.user_sign, a.user_sign))
            from warehouse b
            where a.whs_code = b.whs_code;
        end if;

    end if;

    delete from warehouse where whs_code = _whs_code;
    insert into warehouse (select * from _warehouse);

    return 200;
end;
$$



