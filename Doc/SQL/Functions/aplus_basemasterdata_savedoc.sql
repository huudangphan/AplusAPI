CREATE OR REPLACE FUNCTION aplus_basemasterdata_savedoc  (
    _table_name VARCHAR,
    _tran_type VARCHAR,
    _code VARCHAR DEFAULT 0)
    RETURNS table
            (
                msg_code   int,
                error_list varchar(1000)
            )
    LANGUAGE plpgsql
AS
$$
DECLARE
    _time_now        int = to_char(CURRENT_TIMESTAMP, 'HH24MI');
    _not_exists      bool default true;
    -- list the code has error when operation process data
    _code_error_list varchar(1000) default '';

BEGIN

    -- validate data
    if (_tran_type = 'D') then
        if coalesce(_code, '') = '' then
            return query (select 10005, _code_error_list);
            return;
        end if;
        execute 'select not exists(select 1 from '|| _table_name ||' where code='|| quote_literal(_code)|| ');' into _not_exists;
        if _not_exists = true then
            return query (select 10003, _code_error_list);
            return;
        end if;

    else
        _code = coalesce((select string_agg((code), ',') from _tmp_master_data), '');
        
        -- check duplicate code
        if exists(select 1 from _tmp_master_data group by (code) having count(code) > 1) then
            _code_error_list =
                    (select string_agg(distinct (code), ',')
                     from _tmp_master_data
                     group by code
                     having count(code) > 1);
            return query (select 10052, _code_error_list);
            return;
        end if;
        
        execute 'select exists(select 1 from unnest(string_to_array(' || quote_literal(_code) ||
                ', '','')) left outer join ' || _table_name || ' on unnest = code where code isnull);' into _not_exists;
        

        if (_tran_type = 'A') then
            -- check list code is not exists in specific tab
            if (_not_exists = false) then
                -- Join list error code to string with comma (use for returning)
                execute 'select string_agg(unnest, '','')
                                    from unnest(string_to_array(' || quote_literal(_code) || ', '',''))
                                             left outer join ' || _table_name || ' on code = unnest
                                    where code notnull;' into _code_error_list;
                return query (select 10002, _code_error_list);
            end if;
        else
            
            -- check list code is existed in specific table
            if (_not_exists = true) then
                -- Join list error code to string with comma (use for returning)
                execute 'select string_agg(unnest, '','')
                                    from unnest(string_to_array(' || quote_literal(_code) || ', '',''))
                                             left outer join ' || _table_name || ' on code = unnest
                                    where code isnull;' into _code_error_list;
                return query (select 10003, _code_error_list);
                return;
            end if;
        end if;
    end if;

    --update data
    if _tran_type = 'A' THEN
        EXECUTE 'update _tmp_master_data set create_date = ' || quote_literal(CURRENT_DATE) ||
                ', create_time = ' ||
                _time_now || ';';
    ELSE
        if _tran_type = 'U' then
            execute 'update _tmp_master_data a set (create_date, create_time, user_sign, user_sign2, update_date, update_time) = (coalesce(b.create_date, current_date), coalesce(b.create_time, to_char(current_timestamp, ''hh24mi'')::int), coalesce(b.user_sign, a.user_sign), a.user_sign, current_date, ' ||
                    _time_now || ') from ' || _table_name || ' b where a.code = b.code;';
        ELSE
            EXECUTE 'delete from ' || _table_name || ' where code = ' || quote_literal(_code) || ';';
            return query (select 200, _code_error_list);
            return;
        end if;
        
    end if;
    execute 'delete from ' || _table_name || ' where Code in (' ||
            (select string_agg(quote_literal(unnest), ',') from unnest(string_to_array(_code, ','))) || ');';
    EXECUTE 'insert into ' || _table_name || ' select * from _tmp_master_data;';

    return query (select 200, _code);
    return;
END
$$;


CREATE OR REPLACE FUNCTION aplus_basemasterdata_savedoc(
    _table_name VARCHAR,
    _tran_type VARCHAR,
    _code VARCHAR DEFAULT 0)
    RETURNS int
    LANGUAGE 'plpgsql'
AS
$$
DECLARE
    _check_exists varchar;
    _time_now     int = to_char(CURRENT_TIMESTAMP, 'HH24MI');
    _cr_time      int DEFAULT 0;
    _cr_date      date DEFAULT CURRENT_DATE;
BEGIN
    EXECUTE 'select code from ' || _table_name || ' where code = ''' || _code || '''' into _check_exists;
    if _tran_type = 'A' THEN
        if _check_exists is not null THEN
            return 10002; --DataExisted;
        end if;
        EXECUTE 'update "_' || _table_name || '" set create_date = ''' || CURRENT_DATE || ''', create_time = ' ||
                _time_now || ';';
    ELSE
        if _check_exists is null then
            return 10003; --RecordNotFound
        end if;
        if _tran_type = 'U' then
            EXECUTE 'select create_date from ' || _table_name || ' where code=' || quote_literal(_code) ||
                    ';' INTO _cr_date;
            EXECUTE 'select create_time from ' || _table_name || ' where code=' || quote_literal(_code) ||
                    ';' INTO _cr_time;
            EXECUTE 'update _' || _table_name || '
                set create_date = ' || quote_literal(CASE WHEN _cr_date IS NULL THEN CURRENT_DATE ELSE _cr_date END) || ',
                create_time =' || CASE WHEN _cr_time IS NULL THEN _time_now ELSE _cr_time END || ',
                update_date =' || quote_literal(current_date) || ',
                update_time =' || _time_now || ',
                user_sign = (select user_sign from ' || _table_name || ' where code = ' || quote_literal(_code) ||
                    ') ;';
        ELSE
            EXECUTE 'delete from ' || _table_name || ' where code = ' || quote_literal(_code) || '';
            return 200;
        end if;
    end if;
    EXECUTE 'delete from ' || _table_name || ' where Code = ' || quote_literal(_code) || ';';
    EXECUTE 'insert into ' || _table_name || ' select * from _' || _table_name || ';';
    return 200; 
END
$$;