CREATE TABLE user_token
(
    user_code character varying(254) COLLATE pg_catalog."default" NOT NULL,
    database_name character(254) COLLATE pg_catalog."default" NOT NULL,
    last_login_date date,
    last_login_time smallint,
    expiry_date date,
    domain_name character varying(254) COLLATE pg_catalog."default",
    user_sign integer,
    whs_code character varying(50) COLLATE pg_catalog."default",
    branch_code character varying(50) COLLATE pg_catalog."default",
    access_token character varying(500) COLLATE pg_catalog."default" NOT NULL,
    validated character varying(1) COLLATE pg_catalog."default",
    CONSTRAINT "APZ_OJAT_pkey" PRIMARY KEY (user_name)
)

TABLESPACE pg_default;