create table unit
(
    code         varchar(50) not null primary key,
    name         varchar(254),
    status       varchar(1) default 'A',
    data_source  varchar(1) default 'N',
    user_sign    smallint,
    user_sign2   smallint,
    log_instance smallint,
    update_date  date,
    update_time  smallint,
    create_date  date,
    create_time  smallint,
    int_symbol   varchar(20),
    ewb_unit     integer,
    remarks      varchar(254)
);

create table unit_log
(
    log_instance_id int not null,
    code         varchar(50) not null, primary key(log_instance_id, code),
    name         varchar(254),
    status       varchar(1) default 'A',
    data_source  varchar(1) default 'N',
    user_sign    smallint,
    user_sign2   smallint,
    log_instance smallint,
    update_date  date,
    update_time  smallint,
    create_date  date,
    create_time  smallint,
    int_symbol   varchar(20),
    ewb_unit     integer,
    remarks      varchar(254)
);

