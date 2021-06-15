CREATE OR REPLACE FUNCTION Aplus_Payment_GetPaymentByEntry(_doc_entry int)
RETURNS SETOF refcursor
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE _ref1 refcursor;
		_ref2 refcursor;
		_ref3 refcursor;
		_ref4 refcursor;
BEGIN

	open _ref1 for select * from apz_orct where doc_entry = _doc_entry;
	open _ref2 for select * from apz_rct1 where doc_entry = _doc_entry;
	open _ref3 for select * from apz_rct2 where doc_entry = _doc_entry;
	open _ref4 for select * from apz_atc1 where abs_entry = coalesce((select atc_entry from apz_orct where doc_entry = _doc_entry limit 1), 0);
	
	return next _ref1;
	return next _ref2;
	return next _ref3;
	return next _ref4;
	
END $BODY$