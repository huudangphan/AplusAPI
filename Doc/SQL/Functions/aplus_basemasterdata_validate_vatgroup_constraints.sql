create or replace function aplus_basemasterdata_validate_vatgroup_constraints(_tran_type varchar(1), _code varchar(50)) returns int
    language plpgsql as
$$
begin
--     if (_tran_type = 'D') then
--         if exists(select 1 from apz_ocrd where vat_group = _code) then
--             return 10046;
--         end if;
--         
--         if exists(select 1 from apz_oitm where vat_group = _code ) then 
--             return 10047;
--         end if;
--         
--     end if;
    
    return 200;
end
$$