create table apz_oggp (
    code varchar(50) not null ,
    name varchar(50),
    f_code varchar(50),
    type varchar(10),
    path varchar(254),
    name_with_type varchar(254),
    path_with_type varchar(254),
    status varchar(1) default 'A'::varchar(1),
    user_sign smallint,
    user_sign2 smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    primary key (code)
)

