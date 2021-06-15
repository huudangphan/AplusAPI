CREATE OR REPLACE FUNCTION Aplus_Return_GetPayment
(
	_return_Entry character varying(20),
	_user_sign character varying(20),
	_obj_type character varying(20),
	_payment_type character varying(20),
	_doc_table character varying(10)
)
RETURNS SETOF refcursor
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE _ref1 refcursor;
		_ref2 refcursor;
		_ref3 refcursor;
		_sqlQuery character varying = 'insert into _apz_rct1(pay_mth, line_total, currency, line_num, line_status)
		select ''C'' pay_mth, doc_total line_total, doc_cur currency, 0 line_num, ''C'' line_status from ' || _docTable || ' where doc_entry = ' || _returnEntry;
		_currency character varying(3) = (select main_curncy from apz_cinf);
BEGIN
	
	create temp table _apz_rct1 as (select * from apz_rct1 limit 0);
	create temp table _temp_doc_payment as (select * from apz_orct limit 0);
	create temp table _temp_base_payment as (select * from apz_rct2 limit 0);

	execute(_sql_query);

	update _apz_rct1 set currency = _currency where coalesce(currency, '') = '';

	_sqlQuery = 'insert into _temp_doc_payment (group_type, doc_type, canceled, doc_date, doc_due_date, card_dode, card_name, dddress, doc_cur, doc_rate, doc_total, doc_time, obj_type, create_date, series, user_sign, project, doc_status, branch, create_time)
		select ''' || case _payment_type when '810018' then '1002' else '1001' end || ''',''1001'', ''N'', doc_date, doc_due_date, card_code, card_name, address, doc_cur, doc_rate, doc_total, doc_time
								, '''|| _payment_type || ''', create_date, -1, user_sign, project, ''C'', branch, doc_time from '|| _doc_table || ' where doc_entry = ' || _return_entry || ';

	insert into _temp_base_payment(line_num, base_entry, base_type, inv_total, curency, total_payment, user_sign)
		select 0, ' ||  _return_entry || ', obj_type, doc_total, doc_cur, doc_total, user_sign from ' || _docTable || ' where doc_entry = ' || _returnEntry;

	execute(_sqlQuery);

	open _ref1 for select * from _temp_doc_payment;
	open _ref2 for select * from _apz_rct1;
	open _ref3 for select * from _temp_base_payment;
	
	return next _ref1;
	return next _ref2;
	return next _ref3;

END $BODY$
