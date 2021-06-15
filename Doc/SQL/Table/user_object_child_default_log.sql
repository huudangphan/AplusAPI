-- Table: user_object_child_default_log

-- DROP TABLE user_object_child_default_log;

CREATE TABLE user_object_child_default_log
(
	log_instance_id integer NOT NULL,
    code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    child_table character varying(50) COLLATE pg_catalog."default" NOT NULL,
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
    PRIMARY KEY (log_instance_id, code, child_table, col_code)
);