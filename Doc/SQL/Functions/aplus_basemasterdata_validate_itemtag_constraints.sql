create or replace function aplus_basemasterdata_validate_itemtag_constraints(_tran_type varchar(1), _code varchar(50)) returns int
    language plpgsql as
$$
declare
begin

--     if _tran_type = 'D' then
--         if (exists(select unnest
--                    from unnest(string_to_array((select string_agg((tags), ',') from apz_ogit), ','))
--                    where unnest = _code)) then
--             return 10043;
--         end if;
-- 
--         if exists(select unnest
--                   from unnest(string_to_array((select string_agg((item_tags), ',') from apz_oitm), ','))
--                   where unnest = _code) then
--             return 10043;
--         end if;
--     end if;
    
    return 200;
end
$$

