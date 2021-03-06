CREATE TABLE payment
(
    doc_entry integer NOT NULL,
    doc_num integer,
    canceled character varying(1) COLLATE pg_catalog."default",
    doc_date date,
    doc_due_date date,
    group_type smallint,
    card_code character varying(50) COLLATE pg_catalog."default",
    card_name character varying(254) COLLATE pg_catalog."default",
    address character varying(254) COLLATE pg_catalog."default",
    doc_cur character varying(3) COLLATE pg_catalog."default",
    doc_rate numeric(19,6),
    doc_total numeric(19,6),
    ref1 character varying(254) COLLATE pg_catalog."default",
    comments character varying(254) COLLATE pg_catalog."default",
    doc_time smallint,
    obj_type character varying(20) COLLATE pg_catalog."default",
    updatedate date,
    create_date date,
    series integer,
    user_sign smallint,
    log_instance integer,
    vat_group character varying(8) COLLATE pg_catalog."default",
    vat_sum numeric(19,6),
    vat_sumfc numeric(19,6),
    vat_prcnt numeric(19,6),
    project character varying(20) COLLATE pg_catalog."default",
    doc_status character varying(1) COLLATE pg_catalog."default",
    cancel_date date,
    draft_key integer,
    owner_code integer,
    create_time smallint,
    update_time smallint,
    atc_entry integer,
    doc_type smallint,
    tag character varying(254) COLLATE pg_catalog."default",
    company character varying(50) COLLATE pg_catalog."default",
    CONSTRAINT payment_pkey PRIMARY KEY (doc_entry)
);