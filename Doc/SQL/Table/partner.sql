-- Table: partner

-- DROP TABLE partner;

CREATE TABLE partner
(
    card_code character varying(50) COLLATE pg_catalog."default",
    card_name character varying(254) COLLATE pg_catalog."default",
    card_type character varying COLLATE pg_catalog."default",
    group_code character varying COLLATE pg_catalog."default",
    address character varying(100) COLLATE pg_catalog."default",
    city character varying(100) COLLATE pg_catalog."default",
    country character varying(100) COLLATE pg_catalog."default",
    zip_code character varying(20) COLLATE pg_catalog."default",
    e_mail character varying(100) COLLATE pg_catalog."default",
    phone1 character varying(20) COLLATE pg_catalog."default",
    phone2 character varying(20) COLLATE pg_catalog."default",
    fax character varying(20) COLLATE pg_catalog."default",
    cntct_prsn character varying(100) COLLATE pg_catalog."default",
    remarks character varying(254) COLLATE pg_catalog."default",
    balance numeric(19,6),
    list_num smallint,
    slp_code integer,
    currency character varying(3) COLLATE pg_catalog."default",
    atc_entry integer,
    attachment text COLLATE pg_catalog."default",
    dfl_account character varying(50) COLLATE pg_catalog."default",
    dfl_branch integer,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    user_sign smallint,
    status character varying(1) COLLATE pg_catalog."default",
    valid_from date,
    valid_to date,
    frozen_for character varying COLLATE pg_catalog."default",
    frozen_from date,
    frozen_to date,
    vat_group character varying(8) COLLATE pg_catalog."default",
    obj_type character varying(20) COLLATE pg_catalog."default",
    ship_type smallint,
    series integer,
    ship_to_def character varying(254) COLLATE pg_catalog."default",
    bill_to_def character varying(254) COLLATE pg_catalog."default",
    gender character varying COLLATE pg_catalog."default",
    date_of_bir date,
    per_tax_num character varying(50) COLLATE pg_catalog."default",
    datasource character varying COLLATE pg_catalog."default",
    user_sign2 integer
);