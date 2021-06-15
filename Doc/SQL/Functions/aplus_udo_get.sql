-- FUNCTION: aplus_udo_get(character varying, character varying, integer, integer, integer, timestamp without time zone, timestamp without time zone)

-- DROP FUNCTION aplus_udo_get(character varying, character varying, integer, integer, integer, timestamp without time zone, timestamp without time zone);

CREATE OR REPLACE FUNCTION aplus_udo_get(
	_code character varying DEFAULT ''::character varying,
	_name character varying DEFAULT ''::character varying,
	_user_sign integer DEFAULT 0,
	_page_size integer DEFAULT 0,
	_page_index integer DEFAULT 0,
	_from_date timestamp without time zone DEFAULT CURRENT_DATE,
	_to_date timestamp without time zone DEFAULT CURRENT_DATE)
    RETURNS SETOF refcursor 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	-- Bảng phân trang
    ref1 refcursor;
	-- Bảng kết quả
	ref2 refcursor;
    _sum_record INT;
BEGIN
	-- query to get data
	Create Local Temp Table "#temp_doc" as 
	Select *, row_number() over (order by create_date,create_time desc) row_num 
	FROM user_object x
	WHERE true
        AND (case when _code isnull or _code = '' then true else code::varchar LIKE '%' || _code || '%' end)
        AND (case when _name isnull or _name = '' then true else name::varchar LIKE '%' || _name || '%' end)
		AND (case when _user_sign = 0 or _user_sign isnull then true else user_sign = _user_sign end)
        AND (case when _from_date isnull then true else _from_date >= create_date end)
        AND (case when _to_date isnull then true else _to_date <= create_date end)
	order by create_date desc, create_time desc;
	
	--Count record
	Select Count(1) into _sum_record from "#temp_doc";
	
	--Bảng phân trang
	OPEN ref1 FOR SELECT _page_index page_index, CASE WHEN _page_size <= 0 THEN 0 
	WHEN (_sum_record/_page_size)*_page_size < _sum_record THEN _sum_record/_page_size + 1 else _sum_record/_page_size END page_total, _page_size page_size, _sum_record sum_record;
	
	if(_page_size = 0 or _page_index = 0) Then
		OPEN ref2 FOR SELECT x.* 
		FROM "#temp_doc" x;
	else
		OPEN ref2 FOR SELECT x.* 
		FROM "#temp_doc" x WHERE x.row_num Between _page_size*(_page_index-1)+1 And _page_size*(_page_index);
	End if;
	
	RETURN Next ref1;
	RETURN Next ref2;
	
END;
$BODY$;
