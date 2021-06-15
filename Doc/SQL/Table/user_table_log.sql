-- Table: user_table_log

-- DROP TABLE user_table_log;

CREATE TABLE user_table_log
(
	log_instance_id integer NOT NULL,
    table_name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    remarks character varying(254) COLLATE pg_catalog."default",
    table_type smallint NOT NULL,
    create_date date,
    create_time smallint,
    user_sign smallint,
    update_date date,
    update_time smallint,
    user_sign2 smallint,
    PRIMARY KEY (log_instance_id ,table_name)
);