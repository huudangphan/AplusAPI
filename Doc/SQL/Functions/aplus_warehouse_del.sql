create or replace function aplus_warehouse_del(_whs_code varchar(50))
    returns int
    language plpgsql
as
$$
declare
begin
    if exists(select 1 from warehouse where whs_code = _whs_code) then
        return 10001;
    end if;
    if exists(select 1 from warehouse_journal where whs_code = _whs_code) then
        return 10023;
    end if;

    delete from warehouse where whs_code = _whs_code;
    return 200;
end;
$$

