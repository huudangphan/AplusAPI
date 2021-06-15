create table shift
(
    code        varchar(50) not null primary key,
    name        varchar(254),
    store_code  varchar(50),
    status      varchar(1) default 'A',
    pos_id      integer,
    cash_id     integer,
    clock_date  date,
    clock_time  integer,
    clock_type  varchar,
    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    data_source varchar(1) default 'N'
);

create table shift_log
(
    log_instance_id int not null,
    code        varchar(50) not null, 
    primary key(log_instance_id, code),
    name        varchar(254),
    store_code  varchar(50),
    status      varchar(1) default 'A',
    pos_id      integer,
    cash_id     integer,
    clock_date  date,
    clock_time  integer,
    clock_type  varchar,
    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    data_source varchar(1) default 'N'
);
