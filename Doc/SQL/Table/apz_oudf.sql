create table apz_oudf
(
    table_name    varchar(50)  not null,
    column_id     varchar(50)  not null,
    column_name   varchar(254) not null,
    data_type     varchar(20)  not null default 'string'::varchar(20),
    primary key (table_name, column_id, column_name),
    sub_type      varchar(20),
    size          int,
    default_value varchar(254),
    is_required   varchar(1)            default 'N'::varchar(1),
    linked_type   varchar(10)           default '00'::varchar(10),
    linked_object varchar(50),
    object_type   varchar(20),
    layout_id     int,
    create_date   date,
    create_time   smallint,
    update_date   date,
    update_time   smallint,
    user_sign     smallint,
    user_sign2    smallint
)



