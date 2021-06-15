-- Table: user_info_log

-- DROP TABLE user_info_log;

CREATE TABLE user_info_log
(
	log_instance_id integer NOT NULL,
    user_id integer NOT NULL,
    user_code character varying(254) COLLATE pg_catalog."default",
    user_name character varying(254) COLLATE pg_catalog."default",
    password character varying(254) COLLATE pg_catalog."default",
    email character varying(254) COLLATE pg_catalog."default",
    phone1 character varying(20) COLLATE pg_catalog."default",
    phone2 character varying(20) COLLATE pg_catalog."default",
    address character varying(254) COLLATE pg_catalog."default",
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint,
    status character varying(1) COLLATE pg_catalog."default" DEFAULT 'A'::character varying,
    user_type character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    atc_entry integer,
    user_sign smallint,
    company_code character varying(50) COLLATE pg_catalog."default",
    user_sign2 smallint,
    PRIMARY KEY (log_instance_id,user_id)
)

TABLESPACE pg_default;

ALTER TABLE "POSMAN_TEST".user_info
    OWNER to "POSMAN";