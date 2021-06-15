CREATE TABLE apz_ofpr
(
    code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    name character varying(254) COLLATE pg_catalog."default",
    from_ref_date date,
    to_ref_date date,
    from_due_date date,
    to_due_date date,
    remarks character varying(254) COLLATE pg_catalog."default",
    data_source character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    user_sign smallint,
    create_date date,
    create_time smallint,
    user_sign2 smallint,
    update_date date,
    update_time smallint,
    status character varying(1) COLLATE pg_catalog."default" DEFAULT 'A'::character varying,
    CONSTRAINT apz_ofpr_pkey PRIMARY KEY (code)
);