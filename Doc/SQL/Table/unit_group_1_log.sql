-- Table: unit_group_1_log

-- DROP TABLE unit_group_1_log;

CREATE TABLE unit_group_1_log
(
	log_instance_id integer NOT NULL,
    ugp_entry integer NOT NULL,
    uom_code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    alt_qty numeric(19,6),
    base_qty numeric(19,6),
    log_instance integer DEFAULT 0,
    line_num integer,
    wght_factor smallint DEFAULT 0,
    udf_factor integer DEFAULT '-1'::integer,
    is_base_uom character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    PRIMARY KEY (log_instance_id, ugp_entry, uom_code)
);