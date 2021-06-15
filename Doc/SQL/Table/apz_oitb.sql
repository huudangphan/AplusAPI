-- auto-generated definition
create table apz_oitb
(
    code        varchar(50) not null
        constraint apz_oitb_pkey
            primary key,
    name        varchar(254),
    group_id    smallint,
    remarks     varchar(254),
    status      varchar(1) default 'A'::character varying(1),
    user_sign   smallint,
    user_sign2  smallint,
    create_date date,
    create_time integer,
    update_date date,
    update_time integer,
    vis_order   integer,
    data_source varchar(1) default 'N'::character varying
);

alter table apz_oitb
    owner to "POSMAN";

