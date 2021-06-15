-- Table: user_config_2_log

-- DROP TABLE user_config_2_log;

CREATE TABLE user_config_2_log
(
	log_instance_id integer NOT NULL,
    user_id integer NOT NULL,
    code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    name character varying(1000) COLLATE pg_catalog."default"
);