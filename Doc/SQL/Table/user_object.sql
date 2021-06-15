-- Table: user_object

-- DROP TABLE user_object;

CREATE TABLE user_object
(
    code character varying(50) COLLATE pg_catalog."default" NOT NULL,
    name character varying(254) COLLATE pg_catalog."default",
    header_table character varying(50) COLLATE pg_catalog."default" NOT NULL,
    object_type character varying(50) COLLATE pg_catalog."default" NOT NULL,
    type character varying COLLATE pg_catalog."default",
    mng_series character varying(1) COLLATE pg_catalog."default" DEFAULT 'N'::character varying,
    can_delete character varying(1) COLLATE pg_catalog."default" DEFAULT 'Y'::character varying,
    can_cancel character varying(1) COLLATE pg_catalog."default" DEFAULT 'Y'::character varying,
    can_find character varying(1) COLLATE pg_catalog."default" DEFAULT 'Y'::character varying,
    menu_item character varying(1) COLLATE pg_catalog."default" DEFAULT 'Y'::character varying,
    menu_caption character varying(254) COLLATE pg_catalog."default",
    menu_type character varying(1) COLLATE pg_catalog."default" DEFAULT 'H'::character varying,
    father_menu character varying(254) COLLATE pg_catalog."default",
    menu_postition integer,
    create_date date,
    create_time smallint,
    user_sign smallint,
    update_date date,
    update_time smallint,
    user_sign2 smallint,
    can_close character varying(1) COLLATE pg_catalog."default" DEFAULT 'Y'::character varying,
    CONSTRAINT apz_oudo_pkey PRIMARY KEY (code)
);