create or replace function apz_document_DecOnhandObject()
returns table (obj_type character varying(20), is_ins char(1))
language 'plpgsql'
as $body$
begin
	create local temp table "#tbl" (obj_type character varying(20), is_ins char(1));
	insert into "#tbl"
	values ('13','Y'),('60','Y'), ('15', 'Y'), ('21', 'Y');
	return query select * from "#tbl";
end
$body$