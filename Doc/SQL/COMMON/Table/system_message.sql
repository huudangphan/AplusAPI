create table system_message (
    msg_code int not null,
    lang_code varchar(20) not null default 'en-US',
    msg varchar(254),
    msg_identifier varchar(50) not null ,
    primary key (lang_code, msg_code),
    unique (msg_code, msg_identifier)
);
