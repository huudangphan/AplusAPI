CREATE TABLE license_info
(
    install_number character varying(50) COLLATE pg_catalog."default" NOT NULL,
    bp_code character varying(50) COLLATE pg_catalog."default",
    hardware_key character varying(254) COLLATE pg_catalog."default" NOT NULL,
    file_id character varying(50) COLLATE pg_catalog."default",
    license_type character varying(50) COLLATE pg_catalog."default",
    license_info text COLLATE pg_catalog."default" NOT NULL,
    date_request character varying(400) COLLATE pg_catalog."default",
    CONSTRAINT "APZ_OALC_pkey" PRIMARY KEY (install_number, hardware_key)
)

TABLESPACE pg_default;