CREATE OR REPLACE FUNCTION Aplus_Payment_GetDataCashBooks
(
	_f_date date DEFAULT NULL,
	_t_date date DEFAULT NULL,
	_text_search character varying(254) DEFAULT '',
	_doc_type int DEFAULT 0,
	_payments character varying(254) DEFAULT '',
	_user_id int DEFAULT 0,
	_card_code character varying(50) DEFAULT '',
	_branch character varying(50) DEFAULT '',
	_is_accounting character(1) DEFAULT '',
	_page_size int DEFAULT 0,
	_page_index int DEFAULT 0
)
RETURNS SETOF refcursor
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE _ref1 refcursor;
		_ref2 refcursor;
		_ref3 refcursor;
		_currency character varying(3) = (select main_curncy from apz_cinf);
		_opening_balance numeric(19,6) = 0;
		_incomming_payment numeric(19,6) = coalesce((select sum(doc_total) from _temp_doc where obj_type = 810018), 0);
		_outgoing_payment numeric(19,6) = coalesce((select sum(doc_total) from _temp_doc where obj_type = 810019), 0);
		_sum_record int = coalesce((select count(1) from _temp_doc),0);
BEGIN
	if _page_size isnull then
		_page_size = 0;
	end if;

	if _page_index isnull then
		_page_index = 0;
	end if;

	if _text_search isnull then
		_text_search = '';
	end if;

	if _doc_type isnull then
		_doc_type = 0;
	end if;

	if _payments isnull then
		_payments = '';
	end if;

	if _user_id isnull then
		_user_id = 0;
	end if;

	if _card_code isnull then
		_card_code = '';
	end if;

	if _branch isnull then
		_branch = '';
	end if;

	if _is_accounting isnull then
		_is_accounting = '';
	end if;

	create temp table _temp_payment_type as (select unnest pay_mth from unnest(string_to_array(_payments, ',')));
	create temp table _temp_data_payment as (
		select a.doc_entry, a.obj_type, a.create_date, a.doc_date, a.ref1, a.card_code, a.card_name, b.pay_mth, b.line_total doc_total, a.comments
			from apz_orct a inner join apz_rct1 b ON a.doc_entry = b.doc_entry
													left join _temp_payment_type c ON b.pay_mth = c.pay_mth
				where a.canceled = 'N' and b.pay_mth <> 'COD'
					--and (_f_date isnull or a.doc_date >= _f_date)
					and (_doc_type = 0 or Obj_Type = _docType)
					and (_t_date isnull or a.doc_date <= _t_date)
					and (_text_search = '' or cast(a.doc_entry as character varying) like '%' || _text_search || '%' or a.ref1 like '%' || _text_search || '%' or a.tag like '%' || _text_search || '%')
					and (_payments = '' or c.pay_mth is not null)
					and (_user_id = 0 or a.user_sign = _user_id)
					and (_card_code = '' or a.card_code = _card_code)
					and (_branch = '' or branch = _branch)
	);
	
	if _f_date is not null then
		_opening_balance = (select sum((case obj_type when 810018 then 1 else -1 end) * doc_total) from _temp_data_payment where doc_date < _f_date);
	end if;
	
	create temp table _temp_doc as (select *, row_number() over(order by doc_entry) as row_num from _temp_data_payment where _f_date is null or doc_date >= _f_date);

	_incomming_payment = coalesce((select sum(doc_total) from _temp_doc where obj_type = 810018), 0);

	_outgoing_payment = coalesce((select sum(doc_total) from _temp_doc where obj_type = 810019), 0);

	open _ref1 for select _opening_balance opening_balance, _incomming_payment in_total, _outgoing_payment out_total, _currency currency;

	_sum_record = coalesce((select count(1) from _temp_doc), 0);
	
	open _ref2 for select _page_index page_index, case when _page_size <= 0 then null when (_sum_record / _page_size) * _page_size < _sum_record then _sum_record / _page_size + 1 else _sum_record / _page_size end page_total,
			_page_size page_size, _sum_record to_tal;

	open _ref3 for select *, _currency currency, case obj_type when 810018 then doc_total else 0 end in_total, case obj_type when 810019 then doc_total else 0 end out_total 
	from _temp_doc x where _page_size <= 0 or _page_index < 0 or (row_num - 1 >= _pageSize * _page_index and row_num - 1 < (_page_index + 1) * _page_size);
	
	return next _ref1;
	return next _ref2;
	return next _ref3;
	
END $BODY$
