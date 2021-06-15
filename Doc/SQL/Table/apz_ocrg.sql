create table apz_ocrg
(
    code         varchar(50) not null
        constraint apz_ocrg_pkey
            primary key,
    name         varchar(254),
    type         varchar(1)  not null,
    locked       varchar(1),
    price_list   smallint,
    user_sign    smallint,
    user_sign2   smallint,
    status       varchar(1),
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    log_instance integer,
    data_source  varchar(1) default 'N'::character varying
);

alter table apz_ocrg
    owner to "POSMAN";

 