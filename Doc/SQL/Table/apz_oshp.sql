create table apz_oshp
(
    code        varchar(50) not null
        constraint apz_oshp_pkey
            primary key,
    name        varchar(200),
    store_code  varchar(50),
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
    data_source varchar(1) default 'N'::character varying
);

alter table apz_oshp
    owner to "POSMAN";

