create or replace function aplus_udf_save(
    _tran_type varchar(1)
)
    returns int
    language plpgsql
as
$$
declare

    -- link variables
    _key_code           varchar(50) default '';

    -- udf variables
    _table_name         varchar(50) default '';
    _column_id          varchar(50) default '';
    _column_name        varchar(254) default '';
    _data_type          varchar(20) default 'string';
    _sub_type           varchar(20) default '';
    _size               int default 1;
    _default_value      varchar(254) default '';
    _is_required        varchar(3) default 'N';
    _linked_type        varchar(10) default '00';
    _linked_object      varchar(20) default '';


    -- update data variables 
    _date_now           date default current_date;
    _time_now           int default to_char(current_timestamp, 'hhmi');
    _create_date        date default coalesce((select create_date
                                               from apz_oudf
                                               where column_id = _column_id), _date_now);
    _create_time        smallint default coalesce((select create_time
                                                   from apz_oudf
                                                   where column_id = _column_id), _time_now);
    _udf_prefix         varchar(20) default 'u';
    _udf_column_id      varchar(50) default '';
    _alter_size         varchar(10) default '(%1$s)';
    _alter_table_prefix varchar(254) default '';
    _alter_data_type    varchar(20) default 'varchar';
    _alter_is_required  varchar(50) default '';
    _test_sql           varchar(600) default '';

begin

    select table_name,
           column_id,
           column_name,
           data_type,
           sub_type,
           size,
           default_value,
           is_required,
           coalesce(linked_type, '00'),
           coalesce(linked_object, '')
    into
        _table_name,
        _column_id,
        _column_name,
        _data_type,
        _sub_type,
        _size,
        _default_value,
        _is_required,
        _linked_type,
        _linked_object
    from _apz_oudf;

    --- validate data
    if (_linked_type != '00') then
        if (coalesce((select object_type from _apz_oudf), '') = '') then
            return 10006; -- ObjectTypeNotProvide
        end if;
    end if;

    if (_size < 0) then
        return 10021; --ColumnSizeMustGreaterThanZero
    end if;

    if (_linked_type = '04' or _linked_type = '05') then
        if not exists(select 1 from _apz_udf2) then
            return 10008; --LinkedDataNotProvide
        end if;
        
        _key_code = coalesce((select key_code from _apz_udf2), '');
        if (_key_code = '') then
            return 10008; --LinkedDataNotProvide
        end if;

        if coalesce((select table_name from _apz_udf2), '') = '' then
            return 10008; --LinkedTableNotProvide
        end if;
    end if;


    if (_tran_type = 'A') then
        if exists(select 1 from apz_oudf where column_id = _column_id and table_name = _table_name) then
            return 10011; --ColumnIdExisted
        end if;
        if (_linked_type = '02') then
            if exists(select 1 from apz_oudo where object_type = _linked_object) then
                return 10012; --UdfExisted
            end if;
        end if;
        if (_linked_type = '03') then
            if not exists(select 1 from apz_oudt where table_name = _linked_object) then
                return 10012; --UdfExisted
            end if;
        end if;
        if (_linked_type = '05' or _linked_type = '04') then
            if exists(select 1 from apz_udf2 where table_name = _linked_object) then
                return 10012; --UdfExisted
            end if;
        end if;
    else
        if not exists(select 1 from apz_oudf where column_id = _column_id and table_name = _table_name) then
            return 10010; --ColumnIdNotExists
        end if;
        if (_linked_type = '02') then
            if not exists(select 1 from apz_oudo where object_type = _linked_object) then
                return 10013; --UdfNotExists
            end if;
        end if;
        if (_linked_type = '03') then
            if not exists(select 1 from apz_oudt where table_name = _linked_object) then
                return 10013; --UdfNotExists
            end if;
        end if;
        if (_linked_type = '05' or _linked_type = '04') then
            if not exists(select 1 from apz_udf2 where table_name = _table_name) then
                return 10013; --UdfNotExists
            end if;
        end if;

        if exists(select 1 from apz_fst1 where column_id = _column_id and is_udf = 'Y') then
            return 10017; -- UdfInUseByFormSetting
        end if;
        if (select data_type from apz_oudf where column_id = _column_id and table_name = _table_name) != _data_type then
            return 10020; --CouldNotModifyColumnDataType 
        end if;
    end if;
    -- check valid lookup
    if _linked_type = '04' and exists(select 1
                                      from apz_udf2
                                      where column_id = _column_id
                                        and table_name = _table_name
                                        and key_code = _key_code) then
        if _tran_type = 'A' then
            return 10015; --LookupKeyExisted
        else
            return 10016; --LookupKeyNotExists
        end if;
    end if;


    -- update data
    if (_tran_type = 'A') then
        update _apz_oudf
        set create_date = _create_date,
            create_time = _create_time;
    else
        update _apz_oudf
        set create_date = _create_date,
            create_time = _create_time,
            update_date = _date_now,
            update_time = _time_now,
            user_sign   = coalesce((select user_sign from apz_oudf where column_id = _column_id),
                                   (select user_sign2 from _apz_oudf));
    end if;


    delete from apz_oudf where column_id = _column_id;
    insert into apz_oudf (select * from _apz_oudf);

    if (_linked_type = '04' or _linked_type = '05') then
        delete from apz_udf2 where column_id = _column_id;
        insert into apz_udf2 (select * from _apz_udf2);
    end if;


    -- ReConcat column_id for alter table
    _udf_column_id = concat_ws('_', _udf_prefix, _column_id);

    -- update alter param by _data_type for alter table;
    if (_data_type = 'string') then
        _alter_data_type = 'varchar';
        --default size;
        if (coalesce(_size, 0) = 0) then
            _alter_size = '';
        else
            _alter_size = format(_alter_size, _size);
        end if;
        -- default value;
        _default_value = quote_literal(_default_value);
    end if;

    -- TODO: type int and numeric must check value is number or not;
    if (_data_type = 'int') then
        _alter_data_type = 'int';
        _alter_size = '';
        if (_default_value = '') then
            _default_value = -1;
        end if;

        if (_is_required = 'Y' and coalesce(_default_value, '') = '') then
            _default_value = -1;
        end if;
    end if;

    if (_data_type = 'numeric') then
        _alter_data_type = 'numeric';
        _alter_size = format(_alter_size, '16,9');

        if (_is_required = 'Y' and coalesce(_default_value, '') = '') then
            _default_value = -1;
        end if;
    end if;
    if (_data_type = 'datetime') then
        _alter_data_type = 'date';
        _alter_size = '';
        if (_is_required = 'Y') then
            if (coalesce(_default_value, '') = '') then
                _default_value = 'current_date';
            else
                _default_value = quote_literal(_default_value);
            end if;
        end if;
    end if;

    -- set required `Y = (not) null, N = () null`;

    if _tran_type = 'A' then
        if (_is_required = 'Y') then
            _alter_is_required = 'not';
        end if;
    else
        if (_is_required = 'Y') then
            _alter_is_required = ' set not null ';
        else
            _alter_is_required = ' drop not null ';
        end if;
    end if;


    -- set prefix alter;
    if (_tran_type = 'A') then
        _alter_table_prefix = 'alter table ' || _table_name || ' add column ' || _udf_column_id;
    else
        _alter_table_prefix = 'alter table ' || _table_name || ' alter column ' || _udf_column_id;
    end if;
    -- check exists column in table and schema and do alter;
    if (_tran_type = 'A') then
        if exists(select 1
                  from information_schema.columns
                  where column_name = lower(_udf_column_id)
                    and table_name = _table_name
                    and table_schema = current_schema) then
            return 10018; --FieldExistsInTable
        end if;
        _test_sql = _alter_table_prefix || ' ' || _alter_data_type || ' ' || _alter_size ||
                    ' default ' || _default_value || ' ' || _alter_is_required || ' null;';
        execute _test_sql;
    else
        if not exists(select 1
                      from information_schema.columns
                      where column_name = lower(_udf_column_id)
                        and table_name = _table_name
                        and table_schema = current_schema) then
            return 10019; --FieldNotExistsInTable
        end if;
        -- set default
        execute _alter_table_prefix || ' set default ' || _default_value || ';';
        -- set size
        execute _alter_table_prefix || ' type ' || _alter_data_type || _alter_size || ';';
        -- set required
        execute _alter_table_prefix || ' ' || _alter_is_required || ';';
    end if;
    return 200;
end ;
$$;
