create table apz_ospu
(
    code        varchar(50) primary key not null,
    name        varchar,
    status      varchar(1) default 'A'::varchar(1),
    price_unit  numeric(19, 6),
    currency    varchar(3),
    remarks     varchar(254),
    is_def      varchar(1) default 'N',
    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint
)