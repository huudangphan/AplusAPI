CREATE TABLE s_order
(
    doc_entry integer NOT NULL,
    doc_num integer,
    doc_type character varying(1) COLLATE pg_catalog."default",
    canceled character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    doc_status character varying(1) COLLATE pg_catalog."default",
    obj_type character varying(20) COLLATE pg_catalog."default",
    doc_date date,
    doc_time smallint,
    doc_due_date date,
    card_code character varying(50) COLLATE pg_catalog."default",
    card_name character varying(254) COLLATE pg_catalog."default",
    address character varying(254) COLLATE pg_catalog."default",
    num_at_card character varying(100) COLLATE pg_catalog."default",
    vat_percent numeric(19,6),
    vat_sum numeric(19,6),
    vat_sum_fc numeric(19,6),
    disc_prcnt numeric(19,6),
    disc_sum numeric(19,6),
    disc_sum_fc numeric(19,6),
    doc_cur character varying(3) COLLATE pg_catalog."default",
    doc_rate numeric(19,6),
    doc_total numeric(19,6),
    doc_total_fc numeric(19,6),
    ref1 character varying(50) COLLATE pg_catalog."default",
    comments character varying(254) COLLATE pg_catalog."default",
    group_num smallint,
    slp_code integer,
    confirmed character varying(1) COLLATE pg_catalog."default",
    sys_rate numeric(19,6),
    update_date date,
    create_date date,
    series integer,
    tax_date date,
    user_sign smallint,
    wdd_status character varying(1) COLLATE pg_catalog."default",
    draft_key integer,
    total_expns numeric(19,6),
    total_exp_fc numeric(19,6),
    ship_to_code character varying(50) COLLATE pg_catalog."default",
    rounding character varying(1) COLLATE pg_catalog."default",
    req_date date,
    cancel_date date,
    project character varying(20) COLLATE pg_catalog."default",
    from_date date,
    to_date date,
    vat_date date,
    bpl_name character varying(100) COLLATE pg_catalog."default",
    header text COLLATE pg_catalog."default",
    footer text COLLATE pg_catalog."default",
    department smallint,
    email character varying(100) COLLATE pg_catalog."default",
    notify character varying(1) COLLATE pg_catalog."default",
    atc_entry integer,
    vat_group character varying(8) COLLATE pg_catalog."default",
    phone character varying(20) COLLATE pg_catalog."default",
    cus_cash numeric(19,6),
    cash_return numeric(19,6),
    ship_unit character varying(50) COLLATE pg_catalog."default",
    ship_ref character varying(200) COLLATE pg_catalog."default",
    exc_pee_per numeric(19,6),
    exc_pee_sum numeric(19,6),
    del_status character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    ret_status character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    pmt_status character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    whs_code character varying(50) COLLATE pg_catalog."default",
    from_whs_cod character varying(50) COLLATE pg_catalog."default",
    table_id integer,
    disc_type character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    company character varying(50) COLLATE pg_catalog."default",
    CONSTRAINT s_order_pkey PRIMARY KEY (doc_entry)
);