CREATE TABLE system_database
(
    company_db character varying(100) COLLATE pg_catalog."default" NOT NULL,
    store_code character varying(50) COLLATE pg_catalog."default",
    company_type character varying(50) COLLATE pg_catalog."default",
    create_date date,
    company_name character varying(254) COLLATE pg_catalog."default",
    company_location character varying(254) COLLATE pg_catalog."default",
    version character varying(400) COLLATE pg_catalog."default",
    db_server character varying(400) COLLATE pg_catalog."default",
    db_user character varying(400) COLLATE pg_catalog."default",
    db_pass character varying(400) COLLATE pg_catalog."default",
    db_type character varying(50) COLLATE pg_catalog."default",
    db_language character varying(50) COLLATE pg_catalog."default",
    status character(1) COLLATE pg_catalog."default",
    db_server_port character varying(400) COLLATE pg_catalog."default",
    db_schema character varying(400) COLLATE pg_catalog."default",
    CONSTRAINT "APZ_SLSP_pkey" PRIMARY KEY (company_db)
)

TABLESPACE pg_default;