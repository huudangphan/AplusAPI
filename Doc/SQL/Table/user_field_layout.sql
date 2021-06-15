create table user_field_layout
(
    line_num int not null,
    layout_name varchar(50) not null,
    table_id varchar(50),
    primary key (line_num, layout_name)
)
