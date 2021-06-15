CREATE TABLE user_license
(
    database_name character varying(50) COLLATE pg_catalog."default",
    user_code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    install_number character varying(50) COLLATE pg_catalog."default",
    license_type character varying(50) COLLATE pg_catalog."default"
)

TABLESPACE pg_default;