-- Table: user_object_search

-- DROP TABLE user_object_search;

CREATE TABLE user_object_search
(
    code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    col_code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    col_name character varying(254) COLLATE pg_catalog."default",
    col_num integer,
    create_date date,
    create_time smallint,
    user_sign smallint,
    update_date date,
    update_time smallint,
    user_sign2 smallint,
    CONSTRAINT apz_udo2_pkey PRIMARY KEY (code, col_code)
);