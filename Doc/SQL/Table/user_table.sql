-- Table: user_table

-- DROP TABLE user_table;

CREATE TABLE user_table
(
    table_name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    remarks character varying(254) COLLATE pg_catalog."default",
    table_type smallint NOT NULL,
    create_date date,
    create_time smallint,
    user_sign smallint,
    update_date date,
    update_time smallint,
    user_sign2 smallint,
    CONSTRAINT user_table_pkey PRIMARY KEY (table_name)
);