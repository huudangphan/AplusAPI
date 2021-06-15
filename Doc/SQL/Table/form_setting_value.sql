CREATE TABLE form_setting_value
(
    column_id character varying(50) COLLATE pg_catalog."default" NOT NULL,
    key_code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    key_name character varying(254) COLLATE pg_catalog."default",
    setting_entry integer NOT NULL,
    CONSTRAINT form_setting_value_pkey PRIMARY KEY (setting_entry, column_id, key_code)
);