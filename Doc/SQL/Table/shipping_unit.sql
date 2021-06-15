create table shipping_unit
(
    code        varchar(50) primary key not null,
    name        varchar(254),
    status      varchar(1) default 'A',
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

create table shipping_unit_log
(
    log_instance_id int not null,
    code        varchar(50)  not null,
    primary key(log_instance_id, code),
    name        varchar(254),
    status      varchar(1) default 'A',
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



