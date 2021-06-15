create table apz_ovtg
(
    code         varchar(50) not null
        constraint apz_ovtg_pkey
            primary key,
    name         varchar(254),
    status       varchar(1) default 'A'::character varying,
    rate         numeric(19, 6),
    user_sign    smallint,
    user_sign2   smallint,
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    log_instance integer,
    datasource   varchar(1) default 'N'::character varying
);

alter table apz_ovtg
    owner to "POSMAN";

