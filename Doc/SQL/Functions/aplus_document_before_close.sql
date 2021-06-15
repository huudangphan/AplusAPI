create or replace function aplus_document_before_close(_object_type character varying(20), _doc_entry integer, _user_sign integer
													   , _company character varying(50))
	returns table (msg_code integer, message character varying(50))
	language 'plpgsql'
as $BODY$
BEGIN


END; $BODY$