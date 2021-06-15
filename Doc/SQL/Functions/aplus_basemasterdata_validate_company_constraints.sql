create or replace function aplus_basemasterdata_validate_company_constraints(_tran_type varchar(1), _code varchar(50) default '') returns int
    language plpgsql as
$$
declare
    
begin
    
    -- write validate here
    return 200;
end
$$;