CREATE TABLE partner_address
(
    line_num integer NOT NULL,
    card_code character varying(15) COLLATE pg_catalog."default" NOT NULL,
    adres_type character varying COLLATE pg_catalog."default" NOT NULL,
    address character varying(254) COLLATE pg_catalog."default" NOT NULL,
    street character varying(100) COLLATE pg_catalog."default",
    block character varying(100) COLLATE pg_catalog."default",
    zip_code character varying(20) COLLATE pg_catalog."default",
    city character varying(100) COLLATE pg_catalog."default",
    country character varying(50) COLLATE pg_catalog."default",
    ward character varying(50) COLLATE pg_catalog."default",
    user_sign smallint,
    obj_type character varying(20) COLLATE pg_catalog."default",
    create_date date,
    cntc_per character varying(254) COLLATE pg_catalog."default",
    phone character varying(20) COLLATE pg_catalog."default",
    is_def character varying COLLATE pg_catalog."default",
    district character varying(50) COLLATE pg_catalog."default",
    update_date date,
    update_time smallint,
    create_time smallint,
    user_sign2 integer,
    CONSTRAINT partner_address_pkey PRIMARY KEY (line_num, card_code, adres_type)
)

TABLESPACE pg_default;
