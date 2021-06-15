-- Table: user_config_1

-- DROP TABLE user_config_1;

CREATE TABLE user_config_1
(
    user_id integer,
    default_company character varying(50) COLLATE pg_catalog."default",
    default_warehouse character varying(50) COLLATE pg_catalog."default",
    create_date date,
    remarks character varying(254) COLLATE pg_catalog."default",
    create_time smallint,
    update_date date,
    update_time smallint
);