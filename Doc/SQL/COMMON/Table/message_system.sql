create table message_system
(
    msg_code       int         not null,
    lang_code      varchar(20) not null default 'en-US',
    msg            varchar(254),
    msg_identifier varchar(50) not null,
    constraint message_system_pk primary key (lang_code, msg_code),
    unique (msg_identifier)
);

