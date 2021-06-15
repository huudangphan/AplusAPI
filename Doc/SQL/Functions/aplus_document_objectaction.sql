create or replace function aplus_document_objectaction(_obj_type integer default 0, _base_obj integer default 0
								, _item_type character varying(1) default '')
	returns table (object_type int, base_object int, item_type character varying(1), onhand_type int
				   , base_onhand_type int, revert_update_type int)
	language 'plpgsql'
as $$
BEGIN
	create local temporary table _temp_baseobjectaction (object_type int, base_object int, item_type character varying(1)
														 , onhand_type int, base_onhand_type int, revert_update_type int);
	insert into _temp_baseobjectaction values
			(13, -1, 'O', 1, 5, 5),
			(13, 13, 'O', 1, 5, 5),
			(13, 23, 'O', 1, 5, 5),
			(13, 17, 'O', 1, 4, 5),
			(13, 15, 'O', 6, 5, 5),
			(1300, -1, 'O', 5, 5, 5),
			(15, -1, 'O', 1, 5, 5),
			(15, 15, 'O', 1, 5, 5),
			(15, 23, 'O', 1, 5, 5),
			(15, 17, 'O', 1, 4, 5),
			(15, 16, 'O', 1, 5, 5),
			(16, -1, 'O', 2, 5, 5),
			(16, 16, 'O', 2, 5, 5),
			(16, 15, 'O', 2, 5, 5),
			(16, 234000031, 'N', 2, 4, 5),
			(17, -1, 'O', 3, 5, 3),
			(17, 23, 'O', 3, 5, 3),
			(23, -1, 'O', 5, 5, 5),
			(234000031, -1, 'O', 4, 5, 4),
			(18, -1, 'P', 2, 5, 5),
			(18, 18, 'P', 2, 5, 5),
			(18, 20, 'P', 6, 5, 5),
			(18, 22, 'P', 2, 4, 5),
			(18, 540000006, 'P', 2, 5, 5),
			(25, -1, 'P', 5, 5, 5),
			(20, -1, 'P', 2, 5, 5),
			(20, 20, 'P', 2, 5, 5),
			(20, 21, 'P', 2, 5, 5),
			(20, 22, 'P', 2, 4, 5),
			(20, 540000006, 'P', 2, 5, 5),
			(21, -1, 'P', 1, 5, 5),
			(21, 21, 'P', 1, 5, 5),
			(21, 20, 'P', 1, 5, 5),
			(21, 540000006, 'P', 1, 5, 5),
			(22, -1, 'P', 4, 5, 4),
			(22, 540000006, 'P', 4, 5, 4),
			(234000032, -1, 'P', 3, 5, 3),
			(540000006, -1, 'P', 5, 5, 5),
			(59, -1, 'I', 2, 5, 5),
			(59, 59, 'I', 2, 5, 5),
			(59, 60, 'I', 2, 5, 5),
			(60, -1, 'I', 1, 5, 5),
			(60, 60, 'I', 1, 5, 5),
			(60, 59, 'I', 1, 5, 5),
			(67, -1, 'I', 1, 5, 5),
			(67, 67, 'I', 1, 5, 5),
			(67, 1250000001, 'I', 1, 3, 5),
			(1250000001, -1, 'I', 3, 5, 3);

	return query select * from _temp_baseobjectaction a where (coalesce(_obj_type, 0) = 0 or a.object_type = _obj_type) 
															and (coalesce(_base_obj, 0) = 0 or a.base_object = _base_obj)
															and (coalesce(_item_type, '') = '' or a.item_type = _item_type);
	drop table _temp_baseobjectaction;
END; $$