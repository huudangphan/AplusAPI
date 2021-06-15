create table apz_opln
(
    doc_entry    integer not null
        constraint apz_opln_pkey
            primary key,
    list_name    varchar(254),
    is_gross_prc varchar(1) default 'N'::character varying(1),
    valid_for    varchar(1) default 'Y'::character varying(1),
    valid_from   date,
    valid_to     date,
    prim_curr    varchar(3),
    user_sign    smallint,
    remarks      varchar(254),
    create_date  date,
    create_time  smallint,
    update_date  date,
    update_time  smallint,
    user_sign2   smallint
);

alter table apz_opln
    owner to "POSMAN";

