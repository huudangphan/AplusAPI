CREATE OR REPLACE FUNCTION aplus_item_get(
    _page_size INT DEFAULT 0,
    _page_index INT DEFAULT 0,
    _item_code VARCHAR(50) DEFAULT '',
    _item_name VARCHAR(254) DEFAULT '',
    _from_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    _to_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    _groups VARCHAR(200) DEFAULT '',
    _tags VARCHAR(200) DEFAULT '',
    _trade_marks VARCHAR(200) DEFAULT '',
    _type VARCHAR(200) DEFAULT '',
    _status VARCHAR(1) DEFAULT 'Y',
    _search_name VARCHAR(254) DEFAULT ''
)
    RETURNS SETOF refcursor
    LANGUAGE 'plpgsql'
AS
$$
DECLARE
    _sum_record INT DEFAULT 0;
    list        refcursor;
    pagination  refcursor;
BEGIN
    IF _page_size IS NULL THEN
        _page_size = 0;
    END IF;
    IF _page_index IS NULL THEN
        _page_index = 0;
    END IF;
    CREATE LOCAL TEMP TABLE _temp_filter AS
    SELECT row_number() OVER (ORDER BY create_date, create_time DESC) row_num, *, CAST(0 AS NUMERIC(19, 6)) on_hand
    FROM apz_ogit x
    WHERE (_groups IS NULL OR _groups = '' OR
           _groups in (SELECT unnest(string_to_array(_groups, ','))))
      AND (_trade_marks IS NULL OR _trade_marks = '' OR _trade_marks
        in (SELECT unnest(string_to_array(_trade_marks, ','))))
      and (_tags is null or _tags = '' or _tags in (select unnest(string_to_array(_tags, ','))))
      AND (_type is NULL or _type = '' or item_type in (SELECT unnest(string_to_array(_type, ','))))
      AND (_status is NULL or _status = '' or x.status = _status)
      AND (_item_code is NULL OR _item_code = '' OR x.f_itm_code like '%' || _item_code || '%')
      AND (_item_name IS NULL OR _item_code = '' OR x.f_itm_name like '%' || _item_name || '%')
      AND (_search_name IS NULL OR _search_name = '' OR x.f_itm_name like '%' || _search_name || '%' OR
           x.f_itm_code like '%' || _search_name || '%')
      AND (_from_date IS NULL or create_date >= _from_date)
      AND (_to_date IS NULL or create_date <= _to_date)
      and (case when _to_date isnull then true else create_date <= _to_date end)
    order by create_date, create_time desc;

    _sum_record = COALESCE((SELECT count(1) from _temp_filter), 0);

    OPEN pagination FOR SELECT _page_index                           page_index,
                               CASE
                                   WHEN _page_size <= 0 THEN NULL
                                   WHEN (_sum_record / _page_size) * _page_size < _sum_record
                                       THEN _sum_record / _page_size + 1
                                   ELSE _sum_record / _page_size END page_total,
                               _page_size                            page_size,
                               _sum_record                           sum_record;
    CREATE LOCAL TEMP TABLE _temp_item AS
    SELECT * FROM _temp_filter LIMIT 0;

    INSERT INTO _temp_item
    SELECT *
    FROM _temp_filter
    WHERE _page_size <= 0
       OR _page_index <= 0
       OR (row_num - 1 >= _page_index * _page_size and row_num - 1 < (_page_index + 1) * _page_size);

    OPEN list FOR SELECT * FROM _temp_item x;

    RETURN NEXT pagination;
    RETURN NEXT list;
END
$$;

