create table apz_ouom
(
    code         varchar(20) not null
        constraint apz_ouom_pkey
            primary key,
    name         varchar(100),
    status       varchar(1) default 'A'::character varying(1),
    data_source  varchar(1) default 'N'::character varying(1),
    user_sign    smallint,
    user_sign2   smallint,
    log_instance smallint,
    update_date  date,
    update_time  smallint,
    create_date  date,
    create_time  smallint,
    int_symbol   varchar(20),
    ewb_unit     integer,
    remarks      varchar(254)
);

alter table apz_ouom
    owner to "POSMAN";

