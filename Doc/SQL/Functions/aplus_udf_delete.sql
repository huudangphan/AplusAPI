create or replace function aplus_udf_delete(_table_name varchar(50), _column_id varchar(50)) returns int
    language plpgsql
as
$$
declare
    _alter_column_id varchar(50) default lower(concat_ws('_', 'u', _column_id));
begin

    if not exists(select 1 from apz_oudf where table_name = _table_name and column_id = _column_id) then
        return 10013; --UdfNotExists;
    end if;

    -- UNCHECKED: check is udf existed in form setting or not
    if exists(select 1
              from apz_ofst fs
                       inner join apz_fst1 fs1 on fs.setting_entry = fs1.setting_entry
              where fs1.column_id = _column_id
                and fs.item_id = _table_name
                and fs1.is_udf = 'Y') then
        return 10017; --UdfInUseByFormSetting
    end if;

    if not exists(select 1
              from information_schema.columns
              where column_name = _alter_column_id
                and table_name = _table_name
                and table_schema = current_schema) then
        return 10019; --FieldNotExistsInTable 
    end if;

    delete from apz_oudf where column_id = _column_id and table_name = _table_name;
    delete from apz_udf2 where column_id = _column_id and table_name = _table_name;

--     raise exception '%', 'alter table ' || _table_name || ' drop column ' || _alter_column_id;
    execute 'alter table ' || _table_name || ' drop column ' || _alter_column_id;
    
    return 200;
end ;
$$;

