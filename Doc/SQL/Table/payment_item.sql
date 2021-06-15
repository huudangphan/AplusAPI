CREATE TABLE payment_item
(
    doc_entry integer NOT NULL,
    line_num integer NOT NULL,
    due_date date,
    bank_code character varying(50) COLLATE pg_catalog."default",
    comments character varying(254) COLLATE pg_catalog."default",
    line_status character(1) COLLATE pg_catalog."default",
    line_total numeric(19,6),
    currency character varying(50) COLLATE pg_catalog."default",
    pay_mth character varying(10) COLLATE pg_catalog."default",
    payment_ref character varying(50) COLLATE pg_catalog."default",
    project character varying(20) COLLATE pg_catalog."default",
    user_sign smallint,
    log_instance integer,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    CONSTRAINT payment_item_pkey PRIMARY KEY (doc_entry, line_num)
);