create table currency
(
    code         varchar(3) not null primary key,
    name         varchar(100),
    round_sys    varchar,
    decimals     smallint,
    status       varchar(1) default 'A',
    user_sign    smallint,
    user_sign2   smallint,
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    data_source  varchar(1) default 'N'
);


create table currency_log
(
    log_instance_id int        not null,
    code            varchar(3) not null,
    primary key (log_instance_id, code),
    name            varchar(100),
    round_sys       varchar,
    decimals        smallint,
    status          varchar(1) default 'A',
    user_sign       smallint,
    user_sign2      smallint,
    create_date     date,
    create_time     smallint,
    update_date     date,
    update_time     smallint,
    data_source     varchar(1) default 'N'
);
