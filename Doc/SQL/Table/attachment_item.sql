CREATE TABLE attachment_item
(
    doc_entry integer NOT NULL,
    line_num integer NOT NULL,
    src_path text COLLATE pg_catalog."default",
    trgt_path text COLLATE pg_catalog."default",
    file_type character varying(200) COLLATE pg_catalog."default",
    file_name character varying(254) COLLATE pg_catalog."default",
    file_ext character varying(8) COLLATE pg_catalog."default",
    user_sign smallint,
    create_date timestamp without time zone,
    copied character(1) COLLATE pg_catalog."default",
    override character(1) COLLATE pg_catalog."default",
    sub_path text COLLATE pg_catalog."default",
    free_text character varying(254) COLLATE pg_catalog."default",
    uri text COLLATE pg_catalog."default",
    dwl_uri text COLLATE pg_catalog."default",
    file_id character varying(200) COLLATE pg_catalog."default",
    CONSTRAINT attachment_item_pkey PRIMARY KEY (doc_entry, line_num)
);