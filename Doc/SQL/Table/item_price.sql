create table item_price
(
    item_code  varchar(50) not null,
    price_list integer     not null,
    price      numeric(19, 6),
    currency   varchar(3),
    add_price1 numeric(19, 6),
    currency1  varchar(3),
    add_price2 numeric(19, 6),
    currency2  varchar(3),
    uom_code   varchar(50),
    constraint item_price_pk
        primary key (item_code, price_list)
);

