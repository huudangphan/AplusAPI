-- Table: user_company

-- DROP TABLE user_company;

CREATE TABLE user_company
(
    user_id integer,
    company_code character varying(50) COLLATE pg_catalog."default",
    company_name character varying(254) COLLATE pg_catalog."default",
    create_date date,
    remarks character varying(254) COLLATE pg_catalog."default",
    status character varying(1) COLLATE pg_catalog."default",
    create_time smallint,
    update_date date,
    update_time smallint
);