CREATE TABLE item_warehouse(
    item_code VARCHAR(50) NOT NULL,
    whs_code VARCHAR(50) NOT NULL,
    on_hand NUMERIC(19,6),
    is_commited NUMERIC(19, 6),
    on_order NUMERIC (19,6),
    counted NUMERIC(19,6),
    user_sign SMALLINT,
    user_sign2 smallint,
    min_stock NUMERIC(19,6),
    max_stock NUMERIC(19,6),
    avg_price NUMERIC(19,6),
    locked CHAR(1) DEFAULT 'N',
    location VARCHAR(50),
    CONSTRAINT item_warehouse_pk PRIMARY KEY (item_code, whs_code)
)
