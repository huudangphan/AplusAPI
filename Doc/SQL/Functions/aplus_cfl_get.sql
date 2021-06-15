-- create or replace function aplus_cfl_get(
--     _table_name varchar(50),
--     _form_id varchar(20),
--     _item_id varchar(20),
--     _sub_query varchar(254),
--     _user_id int,
--     _language_code varchar(10)
-- ) returns refcursor
--     language plpgsql
-- as
-- $$
-- declare
--     object_data    refcursor default 'object_data';
--     _setting_entry int default 0;
-- begin
--     if not exists(select 1 from user_info where user_id = _user_id) then
--         _user_id = -1;
--     end if;
--     
--     -- get setting entry by form_id and item_id
--     _setting_entry = coalesce((select setting_entry
--                                from apz_ofst
--                                where form_id = _form_id
--                                  and item_id = _item_id
--                                  and language_code = _language_code), 0);
-- 
-- 
--     -- Get all field from form setting and visible = 'Y';
--     create local temp table _get_form_setting_field as
--     select fs1.column_id
--     from apz_ofst fs
--              inner join apz_fst1 fs1 on fs.setting_entry = fs1.setting_entry
--     where fs.setting_entry = _setting_entry
--       and fs.language_code = _language_code
--       and fs1.is_visible = 'Y'
--       and user_id = _user_id;
-- 
--     -- Reorder column (code, name always in position 1 and 2);
--     create local temp table _ordered_field as
--     SELECT column_name
--     FROM information_schema.columns
--     WHERE table_name = _item_id
--       and table_schema = (select current_schema)
--       and column_name in (select column_id from _get_form_setting_field)
--     order by ordinal_position;
-- 
--     open object_data for
--         execute 'select ' || (
--         select coalesce(string_agg(column_name, ','), '*')
--         from _ordered_field
--     ) || ' from ' || _table_name || ' where true ' || _sub_query;
-- 
-- 
-- --     open object_data for select coalesce(string_agg(column_name, ','), '*') from _ordered_field;
--     return object_data;
-- end
-- $$;


create or replace function aplus_cfl_get_test(
    _table_name varchar(50),
    _form_id varchar(20),
    _item_id varchar(20),
    _sub_query varchar(254),
    _user_id int,
    _language_code varchar(10)
) returns refcursor
    language plpgsql
as
$$
declare
    object_data    refcursor default 'object_data';
    _setting_entry int default 0;
begin
    if not exists(select 1 from user_info where user_id = _user_id) then
        _user_id = -1;
    end if;

    -- get setting entry by form_id and item_id
    _setting_entry = coalesce((select setting_entry
                               from apz_ofst
                               where form_id = _form_id
                                 and item_id = _item_id
                                 and language_code = _language_code), 0);


    -- Get all field from form setting and visible = 'Y';
    create local temp table _get_form_setting_field as
    select fs1.column_id
    from apz_ofst fs
             inner join apz_fst1 fs1 on fs.setting_entry = fs1.setting_entry
    where fs.setting_entry = _setting_entry
      and fs.language_code = _language_code
      and fs1.is_visible = 'Y'
      and user_id = _user_id;

    -- Reorder column (code, name always in position 1 and 2);
    create local temp table _ordered_field as
    SELECT column_name
    FROM information_schema.columns
    WHERE table_name = _item_id
      and table_schema = (select current_schema)
      and column_name in (select column_id from _get_form_setting_field)
    order by ordinal_position;

    open object_data for
        execute 'select ' || (
        select coalesce(string_agg(column_name, ','), '*')
        from _ordered_field
    ) || ' from ' || _table_name || ' where true ' || _sub_query;


--     open object_data for select coalesce(string_agg(column_name, ','), '*') from _ordered_field;
    return object_data;
end
$$;

select 

create table apz_test4 as select * from apz_oitb;



select * from apz_test4



select * from apz_test2