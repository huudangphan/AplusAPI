create table apz_otmd (
    code varchar(50) primary key not null,
    name varchar(254),
    remarks varchar,
    locked varchar(1) default 'N',
    reserve varchar(1),
    type varchar(1),
    location varchar(254),
    branch varchar(50),
    vis_order int,
    tab_number int,
    
    user_sign smallint,
    user_sign2 smallint,
    create_date date,
    create_time smallint,
    update_date date,
    update_time smallint
    
)