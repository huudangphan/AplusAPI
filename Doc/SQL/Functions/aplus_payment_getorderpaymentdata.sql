CREATE OR REPLACE FUNCTION Aplus_Payment_GetOrderPaymentData
(
	_obj_Type character varying(20),
	_doc_entry character varying(12),
	_doc_table character varying(10),
	_payment_type character varying(20)
)
RETURNS SETOF refcursor
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE _ref1 refcursor;
		_ref2 refcursor;
		_currency character varying(3) = (select main_curncy from apz_cinf);
		_sql_query character varying = '
	insert into _temp_doc_payment (group_type, doc_type, canceled, doc_date, doc_due_date, card_code, card_name, address, doc_cur, doc_rate, doc_total, doc_time, obj_type, create_date, series, user_sign, project, doc_status, branch, create_time)
		select ''' || case _obj_type when '13' then '1001' else '1002' end || ''',''1000'', ''N'', doc_date, doc_due_date, card_code, card_name, address, doc_cur, doc_rate, doc_total, doc_time, '''|| _payment_type || ''', create_date, -1, user_sign, project, ''C'', branch, doc_time from ' || _doc_table || ' where doc_entry = ' || _doc_entry || ';
	
	insert into _temp_base_payment (line_num, base_entry, base_type, inv_total, currency, total_payment, user_sign)
		select 0, ' || _doc_entry || ', obj_type, doc_total, doc_cur, doc_total, user_sign from ' || _doc_table || ' where doc_entry = ' || _doc_entry || ';

	update ' || _docTable || ' set update_date = current_date where doc_entry = ' || _doc_entry;
BEGIN

	------------- lấy dữ liệu ship code
	create temp table _temp_base as 
	( select a.doc_entry, a.line_num from apz_rct1 a inner join apz_rct2 b ON a.doc_entry = b.doc_entry
													where b.base_type = _obj_type and b.base_entry = _doc_entry and a.pay_mth <> 'COD' and coalesce(a.line_status, 'O') = 'O' group by a.doc_entry, a.line_num );

	----------------------------- Trong truường hợp xác nhận thanh toán thì close các phiếu shipcode tương ứng của invoice ----------------------------------------
	if exists(select 1 from _temp_base) then

		update a set line_status = 'C' from apz_rct1 a inner join _temp_base b on a.doc_entry = b.doc_entry and a.line_num = b.line_num;
		update a set doc_status = 'C' from apz_orct a inner join (select doc_entry from _temp_base group by doc_entry) b ON a.doc_entry = b.doc_entry
																	 where not exists(select 1 from apz_rct1 x where x.doc_entry = a.doc_entry and x.line_status = 'O');

	end if;
	
	create temp table _temp_doc_payment as (select * from apz_orct limit 0);
	create temp table _temp_base_payment as (select * from apz_rct2 limit 0);

	execute(_sql_query);

	open _ref1 for select * from _temp_doc_payment;
	return next _ref1;
	
	open _ref2 for select * from _temp_base_payment;
	return next _ref2;

END $BODY$