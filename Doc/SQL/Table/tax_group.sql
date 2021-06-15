create table tax_group
(
    code         varchar(50) not null primary key,
    name         varchar(254),
    status       varchar(1) default 'A',
    rate         numeric(19, 6),
    user_sign    smallint,
    user_sign2   smallint,
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    datasource   varchar(1) default 'N'
);

create table tax_group_log
(
    log_instance_id int not null,
    code         varchar(50) not null,
     primary key(log_instance_id, code),
    name         varchar(254),
    status       varchar(1) default 'A',
    rate         numeric(19, 6),
    user_sign    smallint,
    user_sign2   smallint,
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    datasource   varchar(1) default 'N'
);




