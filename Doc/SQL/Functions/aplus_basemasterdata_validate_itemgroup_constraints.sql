create or replace function aplus_basemasterdata_validate_itemgroup_constraints(_tran_type varchar(1), _code varchar(50)) returns int
    language plpgsql as
$$
declare
begin

--     if _tran_type = 'D' then
--         if exists(select 1 from apz_ogit where itms_grp_cod = _code) or
--            exists(select 1 from apz_oitm where itms_grp_cod = _code) then
--             return 10042;
--         end if;
--     end if;
    
    return 200;
end
$$