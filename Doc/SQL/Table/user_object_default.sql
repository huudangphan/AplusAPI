-- Table: user_object_default

-- DROP TABLE user_object_default;

CREATE TABLE user_object_default
(
    code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    col_code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    col_name character varying(254) COLLATE pg_catalog."default",
    col_num integer,
    col_edit character varying(1) COLLATE pg_catalog."default",
    create_date date,
    create_time smallint,
    user_sign smallint,
    update_date date,
    update_time smallint,
    user_sign2 smallint,
    CONSTRAINT apz_udo3_pkey PRIMARY KEY (code, col_code)
);