create or replace function aplus_basemasterdata_validate_trademark_constraints(_tran_type varchar(1), _code varchar(50)) returns int
    language plpgsql
as
$$
begin

--     if _tran_type = 'D' then
--         if exists(select 1 from apz_ogit where tramrk = _code) then
--             return 10044;
--         end if;
--     end if;
    
    return 200;
end
$$