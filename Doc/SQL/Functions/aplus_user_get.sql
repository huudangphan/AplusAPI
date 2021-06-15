-- FUNCTION: aplus_user_get(character varying, character varying[], character varying, character varying, integer, integer, integer, timestamp without time zone, timestamp without time zone)

-- DROP FUNCTION aplus_user_get(character varying, character varying[], character varying, character varying, integer, integer, integer, timestamp without time zone, timestamp without time zone);

CREATE OR REPLACE FUNCTION aplus_user_get(
	_search_name character varying DEFAULT ''::character varying,
	_company character varying[] DEFAULT ARRAY[''::text],
	_status character varying DEFAULT ''::character varying,
	_user_type character varying DEFAULT ''::character varying,
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
	Create temp table "#temp_doc" as
	Select *, row_number() over() as row_num
	From (
		SELECT Distinct(x.user_id),x.user_code,x.user_name,x.email,x.phone1,x.phone2,x.address,
			x.create_date,x.create_time,x.user_sign,x.update_date,x.update_time,x.user_sign2,x.status
		FROM user_info x join user_company y on x.user_id = y.user_id
		WHERE (case when _user_sign is null or _user_sign = 0 then true else x.user_sign = _user_sign end)
        	AND (case when _from_date is null then true else _from_date >= x.create_date end)
        	AND (case when _to_date is null then true else _to_date <= x.create_date end)
			AND (case when (((Select Count(_company)) = 0) or 
							(((Select Count(_company)) = 1) AND (_company = ARRAY[''::character varying] or _company::character varying = '{}') )) 
				 then true 
				 else y.company_code = ANY(_company) end)
			AND	(case when ( (_search_name isnull) or (_search_name LIKE '') )then true else 
				 (x.user_code::varchar LIKE '%' || _search_name || '%' or
		   		 x.user_name::varchar LIKE '%' || _search_name || '%' or
		   		 x.email::varchar LIKE '%' || _search_name || '%' or
		    	 x.phone1::varchar LIKE '%' || _search_name || '%' or
		    	 x.phone2::varchar LIKE '%' || _search_name || '%')end)
			AND (case when _status is null or _status = '' then true else x.status LIKE '%' || _status || '%' end)
			AND (case when _user_type is null or _user_type = '' then true else x.user_type LIKE '%' || _user_type || '%' end)
		order by x.create_date desc,x.create_time desc) Base;
	
	--Count record
	Select Count(row_num) into _sum_record from "#temp_doc";	
	
	--Bảng phân trang
	OPEN ref1 FOR SELECT _page_index page_index, CASE WHEN _page_size <= 0 THEN 0 
	WHEN (_sum_record/_page_size) * _page_size < _sum_record THEN _sum_record/_page_size + 1 else _sum_record/_page_size END page_total, _page_size page_size, _sum_record sum_record;
	
	If _page_size != 0 OR _page_index != 0 Then
		OPEN ref2 FOR SELECT x.*
		FROM "#temp_doc" x WHERE x.row_num Between _page_size*(_page_index-1)+1 And _page_size*(_page_index);
	else
		OPEN ref2 FOR SELECT x.*
		FROM "#temp_doc" x;
	End if;
	
	RETURN Next ref1;
	RETURN Next ref2;
	
END;
$BODY$;
