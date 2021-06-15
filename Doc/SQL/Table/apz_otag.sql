create table apz_otag
(
    code        varchar(50) not null
        constraint apz_otag_pkey
            primary key,
    name        varchar(200),
    status      varchar(1) default 'A'::character varying(1),
    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    data_source varchar(1) default 'N'::character varying,
);

alter table apz_otag
    owner to "POSMAN";

