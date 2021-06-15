create table apz_ocrn
(
    code         varchar(3) not null
        constraint apz_ocrn_pkey
            primary key,
    name         varchar(100),
    locked       varchar(1) default 'N'::character varying(1),
    round_sys    varchar,
    decimals     smallint,
    status       varchar(1) default 'A'::character varying(1),
    user_sign    smallint,
    user_sign2   smallint,
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    log_instance smallint,
    data_source  varchar(1) default 'N'::character varying
);

alter table apz_ocrn
    owner to "POSMAN";

