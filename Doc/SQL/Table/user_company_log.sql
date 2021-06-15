-- Table: user_company_log

-- DROP TABLE user_company_log;

CREATE TABLE user_company_log
(
	log_instance_id integer NOT NULL,
    user_id integer,
    company_code character varying(50) COLLATE pg_catalog."default",
    company_name character varying(254) COLLATE pg_catalog."default",
    create_date date,
    remarks character varying(254) COLLATE pg_catalog."default",
    status character varying(1) COLLATE pg_catalog."default",
    create_time smallint,
    update_date date,
    update_time smallint
);