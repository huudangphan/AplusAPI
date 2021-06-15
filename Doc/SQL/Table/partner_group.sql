create table partner_group
(
    code         varchar(50) not null primary key,
    name         varchar(254),
    type         varchar(1)  not null,
    locked       varchar(1),
    price_list   smallint,
    user_sign    smallint,
    user_sign2   smallint,
    status       varchar(1) default '',
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    log_instance integer,
    data_source  varchar(1) default 'N'
);

create table partner_group_log
(
    log_instance_id int not null,
    code         varchar(50) not null,
    primary key(log_instance_id, code),
    name         varchar(254),
    type         varchar(1)  not null,
    locked       varchar(1),
    price_list   smallint,
    user_sign    smallint,
    user_sign2   smallint,
    status       varchar(1) default '',
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    log_instance integer,
    data_source  varchar(1) default 'N'
);
