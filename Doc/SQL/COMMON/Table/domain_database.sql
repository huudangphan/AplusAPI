CREATE TABLE domain_database
(
    domain character varying(50) COLLATE pg_catalog."default" NOT NULL,
    database_name character varying(254) COLLATE pg_catalog."default" NOT NULL,
    hardware_key character varying(254) COLLATE pg_catalog."default",
    CONSTRAINT "APZ_OLDD_pkey" PRIMARY KEY (domain, database_name)
)

TABLESPACE pg_default;