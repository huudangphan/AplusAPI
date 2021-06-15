create table table_info
(
    code        varchar(50) primary key not null,
    name        varchar(254),
    remarks     varchar(254),
    status      varchar(1) default 'A',
    reserve     varchar(1),
    type        varchar(1),
    location    varchar(254),
    company     varchar(50),
    vis_order   int,
    tab_number  int,

    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint

);

create table table_info_log
(
    log_instance_id int         not null,
    code            varchar(50) not null,
    primary key (log_instance_id, code),
    name            varchar(254),
    remarks         varchar(254),
    status          varchar(1) default 'A',
    reserve         varchar(1),
    type            varchar(1),
    location        varchar(254),
    company         varchar(50),
    vis_order       int,
    tab_number      int,

    user_sign       smallint,
    user_sign2      smallint,
    create_date     date,
    create_time     smallint,
    update_date     date,
    update_time     smallint
)