create table apz_otrm
(
    code         varchar(20) not null
        constraint apz_otrm_pkey
            primary key,
    name         varchar(200),
    status       varchar(1) default 'A'::character varying(1),
    create_date  date,
    create_time  smallint,
    user_sign    smallint,
    user_sign2   smallint,
    log_instance integer,
    update_date  date,
    update_time  smallint
);

alter table apz_otrm
    owner to "POSMAN";

