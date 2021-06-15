CREATE or REPLACE FUNCTION aplus_document_updatecostobject()
RETURNS  TABLE
(
	obj_type int,
	base_type int,
	operation char(1),
	base_cost char(1)
)
language 'plpgsql'
AS $body$
BEGIN
	create local temp table _tbl_object
	(
		obj_type int,
		base_type int,
		operation char(1),
		base_cost char(1)
	);
	INSERT into _tbl_object( obj_type, base_type, operation, base_cost)
	VALUES	(13, -1, '-', 'N'),
			(13, 17, '-', 'N'),
			(13, 23, '-', 'N'),
			(15, -1, '-', 'N'),
			(15, 17, '-', 'N'),
			(15, 23, '_', 'N'),
			(16, -1, '+', 'N'),
			(16, 15, '+', 'Y'),
			(16, 13, '+', 'Y'),
			(18, -1, '+', 'N'),
			(18, 22, '+', 'N'),
			(18, 234000032, '+', 'N'),
			(20, -1, '+', 'N'),
			(20, 22, '+', 'N'),
			(20, 234000032, '+', 'N'),
			(21, -1, '-', 'N'),
			(21, 20, '-', 'Y'),
			(21, 18, '-', 'Y'),
			(67, -1, '+', 'N'),
			(67, 1250000001, '+', 'N');
					
	RETURN query select * from _tbl_object;
	drop table _tbl_object;
END $body$;