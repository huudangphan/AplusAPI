CREATE TABLE financial_period
(
    code          character varying(50) NOT NULL primary key,
    name          character varying(254),
    status        character varying(1) DEFAULT 'A',
    from_ref_date date,
    to_ref_date   date,
    from_due_date date,
    to_due_date   date,
    remarks       character varying(254),
    data_source   character varying(1) DEFAULT 'N',
    user_sign     smallint,
    user_sign2    smallint,
    create_date   date,
    create_time   smallint,
    update_date   date,
    update_time   smallint
);

CREATE TABLE financial_period_log
(
    log_instance_id int                   not null,
    code            character varying(50) NOT NULL,
    primary key (log_instance_id, code),
    name            character varying(254),
    status          character varying(1) DEFAULT 'A',
    from_ref_date   date,
    to_ref_date     date,
    from_due_date   date,
    to_due_date     date,
    remarks         character varying(254),
    data_source     character varying(1) DEFAULT 'N',
    user_sign       smallint,
    user_sign2      smallint,
    create_date     date,
    create_time     smallint,
    update_date     date,
    update_time     smallint
);