CREATE TABLE p_invoice_item
(
    doc_entry integer NOT NULL,
    line_num integer NOT NULL,
    target_type integer,
    trget_entry integer,
    base_ref character varying(50) COLLATE pg_catalog."default",
    base_type integer,
    base_entry integer,
    base_line integer,
    line_status character varying(1) COLLATE pg_catalog."default",
    item_code character varying(50) COLLATE pg_catalog."default",
    item_name character varying(254) COLLATE pg_catalog."default",
    quantity numeric(19,6),
    ship_date date,
    open_qty numeric(19,6),
    price numeric(19,6),
    currency character varying(3) COLLATE pg_catalog."default",
    rate numeric(19,6),
    disc_prcnt numeric(19,6),
    line_total numeric(19,6),
    whs_code character varying(8) COLLATE pg_catalog."default",
    tree_type character varying(1) COLLATE pg_catalog."default",
    acct_code character varying(15) COLLATE pg_catalog."default",
    tax_status character varying(1) COLLATE pg_catalog."default",
    price_bef_di numeric(19,6),
    doc_date date,
    use_base_un character varying(1) COLLATE pg_catalog."default",
    sub_cat_num character varying(50) COLLATE pg_catalog."default",
    ocr_code character varying(8) COLLATE pg_catalog."default",
    ocr_code2 character varying(8) COLLATE pg_catalog."default",
    ocr_code3 character varying(8) COLLATE pg_catalog."default",
    ocr_code4 character varying(8) COLLATE pg_catalog."default",
    ocr_code5 character varying(8) COLLATE pg_catalog."default",
    project character varying(20) COLLATE pg_catalog."default",
    vat_prcnt numeric(19,6),
    vat_group character varying(8) COLLATE pg_catalog."default",
    price_af_vat numeric(19,6),
    height numeric(19,6),
    width numeric(19,6),
    length numeric(19,6),
    volume numeric(19,6),
    height_unit character varying(50) COLLATE pg_catalog."default",
    width_unit character varying(50) COLLATE pg_catalog."default",
    length_unit character varying(50) COLLATE pg_catalog."default",
    volume_unit character varying(50) COLLATE pg_catalog."default",
    vis_order smallint,
    address character varying(254) COLLATE pg_catalog."default",
    remarks character varying(254) COLLATE pg_catalog."default",
    pick_status character varying(1) COLLATE pg_catalog."default",
    line_vat numeric(19,6),
    line_vatl_f numeric(19,6),
    unit_msr character varying(100) COLLATE pg_catalog."default",
    num_per_msr numeric(19,6),
    line_type character varying(1) COLLATE pg_catalog."default",
    tran_type character varying(1) COLLATE pg_catalog."default",
    from_whs_cod character varying(8) COLLATE pg_catalog."default",
    inv_qty numeric(19,6),
    open_inv_qty numeric(19,6),
    line_vendor character varying(15) COLLATE pg_catalog."default",
    item_type integer,
    create_date date,
    update_date date,
    user_sign smallint,
    total_bef_v numeric(19,6),
    disc_sum numeric(19,6),
    uom_code character varying(50) COLLATE pg_catalog."default",
    ugp_entry integer,
    father_id integer,
    item_cost numeric(19,6),
    disc_type character varying(1) COLLATE pg_catalog."default",
    CONSTRAINT p_invoice_item_pkey PRIMARY KEY (doc_entry, line_num)
);