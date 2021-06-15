create table user_field_value
(
    key_code   varchar(50)  not null,
    key_name   varchar(254),
    column_id  varchar(50)  not null,
    table_name varchar(50)  not null,
    primary key (key_code, column_id, table_name)
) -- valid value