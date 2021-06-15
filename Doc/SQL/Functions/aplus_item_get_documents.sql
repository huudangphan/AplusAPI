create or replace function aplus_item_get_documents(
    _page_index integer DEFAULT 0,
    _page_size integer DEFAULT 0,
    _tran_type character varying DEFAULT 'O'::character varying,
    _price_list int default 0,
    _whscode character varying DEFAULT ''::character varying)
    returns SETOF refcursor
    language plpgsql
as
$$
DECLARE
    pagination  refcursor;
    object_data refcursor;
    _sum_record INT DEFAULT 0;
BEGIN
    IF _page_size IS NULL THEN
        _page_size = 0;
    END IF;
    IF _page_index IS NULL THEN
        _page_index = 0;
    END IF;

    CREATE LOCAL TEMP TABLE _temp_doc AS
    SELECT row_number() OVER (ORDER BY a.create_date, a.create_time DESC) row_num,
           a.item_code,
           a.item_name,
           a.remarks,
           a.item_type,
           a.vat_group,
           --(missing) price list info
           e.rate                                          vat_prcnt,
           CAST(NULL AS NUMERIC)                           discnt,
           c.name                                          trade_mark,
           a.barcode
    FROM apz_oitm a
             LEFT JOIN (select item_code, price, currency from apz_itm1 where price_list = _price_list) b
                       ON a.item_code = b.item_code
             LEFT JOIN apz_otrm c ON a.tramrk = c.code
             left join (select item_code, coalesce(on_hand, 0) on_hand from apz_oitw where whs_code = _whscode) w
                       on a.item_code = w.item_code
             LEFT JOIN apz_oitb d ON a.itms_grp_cod = d.code
             LEFT JOIN apz_ovtg e ON COALESCE(a.vat_group, '') = e.code
             LEFT JOIN (SELECT item_code, COALESCE(on_hand, 0) on_hand FROM apz_oitw WHERE whs_code = _whscode) x
                       ON a.item_code = x.item_code
    WHERE 1 = 1
        and  COALESCE(a.valid_for, 'Y') = 'Y'
      and (_tran_type = 'O' and a.sell_item = 'Y' or _tran_type = 'I' and a.invn_item = 'Y')
    order by a.create_date, a.create_time desc;
    SELECT COALESCE(COUNT(1), 0) INTO _sum_record FROM _temp_doc;

    OPEN pagination FOR SELECT _page_index                           page_index,
                               CASE
                                   WHEN _page_size <= 0 THEN 0
                                   WHEN (_sum_record / _page_size) * _page_size < _sum_record
                                       THEN _sum_record / _page_size + 1
                                   else _sum_record / _page_size END page_total,
                               _page_size                            page_size,
                               _sum_record                           total;
    CREATE LOCAL TEMP TABLE _temp_result AS
    SELECT * FROM _temp_doc LIMIT 0;

    INSERT INTO _temp_result
    SELECT *
    FROM _temp_doc
    WHERE _page_size <= 0
       OR _page_index < 0
       OR row_num BETWEEN _page_index * _page_size AND (_page_index + 1) * _page_size;
    OPEN object_data FOR SELECT *
                         FROM _temp_result x
                         WHERE _page_size <= 0
                            or row_num - 1 >= _page_index * _page_size and row_num < _page_index + 1 * _page_size;

    RETURN NEXT pagination;
    RETURN NEXT object_data;
END
$$;


