create or replace function aplus_document_objecttpe(_doc_type character varying(1) default '', _obj_type character varying(20) default '')
	returns table (object_type character varying(20), doc_type character varying(1), onhand_type character varying(2))
	language 'plpgsql'
as $$
BEGIN
	create local temporary table _temp_objecttype (object_type character varying(20), doc_type character varying(1), onhand_type character varying(2));
	insert into _temp_objecttype values
			('13', 'S', 'O'),
			('13000', 'S', 'N'),
			('15', 'S', 'O'),
			('16', 'S', 'I'),
			('17', 'S', 'ON'),
			('23', 'S', 'N'),
			('234000031', 'S', 'IN'),
			('18', 'P', 'I'),
			('25', 'P', 'N'),
			('20', 'P', 'I'),
			('21', 'P', 'O'),
			('22', 'P', 'IN'),
			('234000032', 'P', 'ON'),
			('1470000113', 'P', 'N'),
			('59', 'I', 'I'),
			('60', 'I', 'O'),
			('67', 'I', 'O'),
			('1250000001', 'I', 'N');

	return query select * from _temp_objecttype a where (coalesce(_doc_type, '') = '' or a.doc_type = _doc_type) and (coalesce(_obj_type, '') = '' or a.object_type = _obj_type);
	drop table _temp_objecttype;
END; $$