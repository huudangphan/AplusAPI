create or replace function aplus_basemasterdata_validate_uoms_constraints(_tran_type varchar(1), _code varchar(50)) returns int
    language plpgsql as
$$
begin

--     if (_tran_type = 'D') then
--         if exists(select 1 from apz_oitm where s_uom_entry = _code or p_uom_entry = _code or i_uom_entry = _code) or
--            exists(select 1 from apz_ogit where s_uom_entry = _code) then
--             return 10045;
--         end if;
-- 
--         if(select 1 from apz_oivl ivl where ivl.uom_code = _code) then
--             return 10023;
--         end if;
--     end if;
--     
--     if(_tran_type = 'U') then
--         if(select 1 from apz_oivl ivl where ivl.uom_code = _code) then
--             return 10023;
--         end if;
--     end if;
    
    return 200;
end
$$

