CREATE TABLE auth
(
    user_id smallint NOT NULL,
    menu_code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    auth character varying(1) COLLATE pg_catalog."default",
    user_sign smallint,
    update_date date,
    update_time smallint,
	create_date date,
    create_time smallint,
    CONSTRAINT auth_pkey PRIMARY KEY (user_id, menu_code)
);