create or replace function aplus_basemasterdata_validate_currency_constraints(_tran_type varchar(1), _code varchar(50)) returns int
    language plpgsql as
$$
declare
begin

--     if _tran_type = 'D' then
--         if (
--                 exists(select 1 from apz_ocrd where currency = _code)
--                 or exists(select 1 from apz_oivl where currency = _code)
--                 or exists(select 1 from apz_ostr where currency = _code)
--                 or exists(select 1 from apz_itm1 where currency = _code or currency1 = _code or currency2 = _code)
--                 or exists(select 1 from apz_ospu where currency = _code)
--             ) then
--             return 10040;
--         end if;
--         if (exists(select 1 from apz_oivl where currency = _code)) then
--             return 10023;
--         end if;
-- 
--     else
--         if _tran_type = 'U' then
--             if (exists(select 1 from apz_oivl where currency = _code)) then
--                 return 10023;
--             end if;
--         end if;
--     end if;

    return 200;
end
$$;
