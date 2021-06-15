create table apz_olgt
(
    code        varchar(50) primary key not null,
    name        varchar(254),
    status      varchar(1) default 'A',
    remarks     varchar(254),
    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    data_source varchar(1) default 'N'
)


