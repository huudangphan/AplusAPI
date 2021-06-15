create table trademark
(
    code         varchar(20) not null
        primary key,
    name         varchar(200),
    status       varchar(1) default 'A',
    create_date  date,
    create_time  smallint,
    user_sign    smallint,
    user_sign2   smallint,
    log_instance integer,
    update_date  date,
    update_time  smallint
);

create table trademark_log
(
    log_instance_id int not null,
    code         varchar(20) not null,
        primary key (log_instance_id, code),
    name         varchar(200),
    status       varchar(1) default 'A',
    create_date  date,
    create_time  smallint,
    user_sign    smallint,
    user_sign2   smallint,
    log_instance integer,
    update_date  date,
    update_time  smallint
);

