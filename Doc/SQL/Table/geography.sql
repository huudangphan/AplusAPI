create table geography
(
    code           varchar(50) not null primary key,
    name           varchar(50),
    type           varchar(10),
    f_code         varchar(50),
    path           varchar(254),
    name_with_type varchar(254),
    path_with_type varchar(254),
    status         varchar(1) default 'A'::varchar(1),
    user_sign      smallint,
    user_sign2     smallint,
    create_date    date,
    create_time    smallint,
    update_date    date,
    update_time    smallint
);

create table geography_log
(
    log_instance_id int         not null,
    code            varchar(50) not null,
    primary key (log_instance_id, code),
    name            varchar(50),
    type            varchar(10),
    f_code          varchar(50),
    path            varchar(254),
    name_with_type  varchar(254),
    path_with_type  varchar(254),
    status          varchar(1) default 'A'::varchar(1),
    user_sign       smallint,
    user_sign2      smallint,
    create_date     date,
    create_time     smallint,
    update_date     date,
    update_time     smallint
)

