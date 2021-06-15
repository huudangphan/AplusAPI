-- FUNCTION: POSMAN_TEST.posmanerp_user_getalldata(character varying, character varying, character varying, integer, integer, integer, integer, timestamp without time zone, timestamp without time zone)

-- DROP FUNCTION "POSMAN_TEST".posmanerp_user_getalldata(character varying, character varying, character varying, integer, integer, integer, integer, timestamp without time zone, timestamp without time zone);

CREATE OR REPLACE FUNCTION "POSMAN_TEST".posmanerp_user_getalldata(
	table_name character varying DEFAULT ''::character varying,
	code character varying DEFAULT ''::character varying,
	name character varying DEFAULT ''::character varying,
	branch integer DEFAULT 0,
	user_sign integer DEFAULT 0,
	page_size integer DEFAULT 0,
	page_index integer DEFAULT 0,
	from_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
	to_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP)
    RETURNS refcursor
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE 
    ref1 refcursor;
    sum_record INT;
	page_count INT;
BEGIN
	-- query to get data
	EXECUTE 'CREATE LOCAL TEMP TABLE "#temp_doc" as 
        SELECT *, row_number() over (order by "create_date" desc) row_num FROM '|| table_name ||' WHERE 1 = 1 '
        || (CASE WHEN code = '' THEN '' ELSE ' AND user_code = '''|| code || ''' ' END)
        || (CASE WHEN name = '' THEN '' ELSE ' AND user_name LIKE ''%'|| name ||'%'' ' END)
		|| (CASE WHEN branch = 0 THEN '' ELSE ' AND branch_code = '''|| branch || '''' END)
		|| (CASE WHEN user_sign = 0 THEN '' ELSE ' AND user_sign = '''|| user_sign ||'''' END)
		|| ' AND create_date between ''' || from_date || ''' and ''' || to_date || ''' '||';';
		
	--Count record
	Select Max(row_num) into sum_record from "#temp_doc" where 1=1;	
	--Kiểm tra page size
	IF(page_size <> 0 AND page_index <> 0)
	Then
		--Compare total record
		IF (sum_record > page_size)
		Then
			--Tính số trang theo page size
			IF Mod(sum_record,page_size) = 0
			THEN
				page_count := sum_record/page_size;
			ELSE
				page_count := (sum_record/page_size) + 1;
			END IF;
			If (page_index <= page_count)
			Then
				OPEN ref1 FOR SELECT * FROM "#temp_doc" x WHERE 1 = 1 And x.row_num Between page_size*(page_index-1)+1 And page_size*(page_index);
				RETURN ref1;
			End If;
			--Trả về table trống khi không thỏa điều kiện page_id
			OPEN ref1 FOR SELECT * FROM "#temp_doc" x WHERE 1 = 1 And x.row_num = 0;
			RETURN ref1;
		End If;
	End if;
	--Trả dữ liệu
	OPEN ref1 FOR SELECT * FROM "#temp_doc" WHERE 1 = 1;
	RETURN ref1;
END;
$BODY$;

ALTER FUNCTION "POSMAN_TEST".posmanerp_user_getalldata(character varying, character varying, character varying, integer, integer, integer, integer, timestamp without time zone, timestamp without time zone)
    OWNER TO "POSMAN";
