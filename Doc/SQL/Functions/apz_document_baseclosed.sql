CREATE or replace FUNCTION apz_document_baseclosed()
	RETURNS TABLE(base_type character varying(20),obj_type character varying(20))
	language 'plpgsql'
AS $body$
BEGIN

	create local temporary table _tbl_base_close(base_type character varying(20),obj_type character varying(20))
	INSERT INTO _tbl_base_close
	        ( base_type, obj_type)
	VALUES  ('13','15','N','Y'),
			('15','16','Y','Y'),
			('18','20','N','Y'),
			('20','21','Y','Y')
	RETURN
END; $body$
