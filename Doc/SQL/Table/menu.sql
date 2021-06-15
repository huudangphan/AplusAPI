CREATE TABLE menu
(
    menu_code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    menu_name character varying(254) COLLATE pg_catalog."default",
    father_code character varying(50) COLLATE pg_catalog."default",
    menu_type character varying(1) COLLATE pg_catalog."default",
    status character varying(1) COLLATE pg_catalog."default" DEFAULT 'Y'::character varying,
    language_code character varying(10) COLLATE pg_catalog."default",
    CONSTRAINT menu_pkey PRIMARY KEY (menu_code)
);