create or replace function apz_document_IncOnhandObject()
returns table (obj_type character varying(20), is_ins char(1))
language 'plpgsql'
as $body$
begin
	create local temp table "#tbl" (obj_type character varying(20), is_ins char(1));
	insert into "#tbl"
	values ('16','Y'),('59','Y'), ('67', 'Y'), ('18', 'Y'), ('20', 'Y');
	return query select * from "#tbl";
end
$body$