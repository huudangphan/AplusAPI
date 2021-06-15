-- Table: unit_group

-- DROP TABLE unit_group;

CREATE TABLE unit_group
(
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
    CONSTRAINT apz_ougp_primary PRIMARY KEY (ugp_entry)
);