create table item_group
(
    code        varchar(50) not null primary key,
    name        varchar(254),
    group_id    smallint,
    remarks     varchar(254),
    status      varchar(1) default 'A',
    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    vis_order   integer,
    data_source varchar(1) default 'N'
);

create table item_group_log
(
    log_instance_id int not null ,
    code        varchar(50) not null,
    primary key(log_instance_id, code),
    name        varchar(254),
    group_id    smallint,
    remarks     varchar(254),
    status      varchar(1) default 'A',
    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    vis_order   integer,
    data_source varchar(1) default 'N'
);

