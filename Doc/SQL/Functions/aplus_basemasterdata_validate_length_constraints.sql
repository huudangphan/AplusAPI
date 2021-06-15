create or replace function aplus_basemasterdata_validate_length_constraints(_code varchar(50), _tran_type varchar(1)) returns int
    language plpgsql
as
$$
begin
--     if (_tran_type = 'D') then
--         if exists(select 1 from apz_oitm where length_unit = _code) then
--             return 10049;
--         end if;
--     end if;

    return 200;
end
$$