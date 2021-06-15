-- Table: user_object_child_log

-- DROP TABLE user_object_child_log;

CREATE TABLE user_object_child_log
(
	log_instance_id integer NOT NULL,
    code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    child_table character varying(50) COLLATE pg_catalog."default" NOT NULL,
    child_name character varying(254) COLLATE pg_catalog."default",
    child_num integer,
    create_date date,
    create_time smallint,
    user_sign smallint,
    update_date date,
    update_time smallint,
    user_sign2 smallint,
    PRIMARY KEY (log_instance_id,code,child_table)
);