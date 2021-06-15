create or replace function aplus_basemasterdata_validate_bpgroup_constraints(_tran_type varchar(1), _code varchar(50) default '') returns int
    language plpgsql as
$$
declare
    _group_code varchar(50) default '';
begin

--     if (_tran_type = 'D') then
--         _group_code = _code;
--         
--         if exists(select 1 from apz_ocrd where group_code = _group_code) then
--             return 10037; --BpGrpUsedInBp
--         end if;
--     end if;
--     
--     if(_tran_type = 'U') then
-- --         _group_code = coalesce((select group_code from _apz_ocrg limit 1), '');
--         --- example
--     end if;
--     
    return 200;
end;
$$

