CREATE TABLE form_setting
(
    setting_entry integer NOT NULL,
    form_id character varying(50) COLLATE pg_catalog."default" NOT NULL,
    item_id character varying(50) COLLATE pg_catalog."default" NOT NULL,
    language_code character varying(5) COLLATE pg_catalog."default",
    datasource character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    form_name character varying(254) COLLATE pg_catalog."default",
    CONSTRAINT form_setting_pkey1 PRIMARY KEY (setting_entry)
);