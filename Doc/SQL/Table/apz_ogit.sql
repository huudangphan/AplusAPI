create table apz_ogit
(
    f_itm_code     varchar(50) not null
        constraint apz_ogit_pkey
            primary key,
    f_itm_name     varchar(254),
    status         varchar(1),
    tramrk         varchar(50),
    create_date    date,
    create_time    smallint,
    user_sign      smallint,
    user_sign2     smallint,
    update_date    date,
    update_time    smallint,
    tags           varchar(254),
    itms_grp_cod   varchar(50),
    ugp_entry      integer,
    sal_unit_msr   varchar(50),
    s_uom_entry    varchar(50),
    remarks        varchar,
    property1_name varchar(254),
    property1      varchar(50),
    property2_name varchar(254),
    property2      varchar(50),
    property3_name varchar(254),
    property3      varchar(50),
    property_item  varchar,
    property_count smallint,
    item_type      varchar(3),
    data_source    varchar(1) default 'N'::character varying
);

alter table apz_ogit
    owner to "POSMAN";

