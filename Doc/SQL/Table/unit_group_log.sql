-- Table: POSMAN_TEST.unit_group_log

-- DROP TABLE unit_group_log;

CREATE TABLE unit_group_log
(
	log_instance_id integer NOT NULL,
    ugp_entry integer NOT NULL,
    ugp_name character varying(100) COLLATE pg_catalog."default",
    base_uom character varying(50) COLLATE pg_catalog."default",
    data_source character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    user_sign integer,
    log_instance integer DEFAULT 0,
    update_date date,
    create_date date,
    update_time integer,
    create_time integer,
    status character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    user_sign2 smallint,
    PRIMARY KEY (log_instance_id,ugp_entry)
);