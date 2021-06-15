-- Table: unit_group_1

-- DROP TABLE unit_group_1;

CREATE TABLE unit_group_1
(
    ugp_entry integer NOT NULL,
    uom_code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    alt_qty numeric(19,6),
    base_qty numeric(19,6),
    log_instance integer DEFAULT 0,
    line_num integer,
    wght_factor smallint DEFAULT 0,
    udf_factor integer DEFAULT '-1'::integer,
    is_base_uom character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    CONSTRAINT apz_ugp1_pkey PRIMARY KEY (ugp_entry, uom_code)
);