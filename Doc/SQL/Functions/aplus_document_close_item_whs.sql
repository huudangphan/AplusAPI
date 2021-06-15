create or replace function aplus_document_close_item_whs(_object_type int, _doc_entry integer, _user_sign integer, _trans_type character varying(1))
	returns table (msg_code integer, message character varying(50))
	language 'plpgsql'
as $BODY$
declare _new_tranq_id integer = 0;
BEGIN

	create local temporary table _temp_close_objectaction as select * from aplus_document_objectaction(_object_type, 0, '');
	
	---------------------------------------- log nhập xuất dữ liệu của phiếu order và commit --------------------------------------------------
	_new_tranq_id = coalesce((select max(trans_seq) from apz_oivl), 0);
	
	insert into apz_oivl (trans_seq,trans_type,create_by,obj_type,doc_entry,line_num,doc_date,item_code,in_qty,out_qty,price,currency,rate,create_date,create_time,whs_code
						  ,user_sign,date_source,post_status,sum_stock,open_qty,tree_id,parent_id,version_num,qty,unit_msr,ugp_entry,uom_code,num_per_msr,price_list,item_cost
						  ,table_id,card_code, inv_qty, open_inv_qty, base_type, base_entry, base_line, target_type, target_entry, target_line, action_type)
		select _new_tranq_id + row_number() over(order by a.line_num), c.revert_update_type, a.user_sign, _object_type, a.doc_entry, a.line_num, b.doc_date, a.item_code, 
				0, 0, a.price, a.currency, a.rate, current_date, cast(to_char(current_timestamp, 'HHmi') as smallint), a.whs_code
				, a.user_sign, 'N', null, null, null, null, null, null, null, a.unit_msr, a.ugp_entry, a.uom_code, a.num_per_msr, b.group_num, a.item_cost
				, null, b.card_code, -1 * a.open_inv_qty, null, -1, null, null, -1, null, null, _trans_type
				from _temp_lines a inner join _temp_doc b on a.doc_entry = b.doc_entry
											inner join _temp_close_objectaction c on a.base_type = c.base_object 
							where c.revert_update_type <> '5' and a.open_inv_qty <> 0;
							
	--------------------------------------------- update inventory data --------------------------------------------------------------------
	update apz_oitw a set is_commited = coalesce(a.is_commited,0) - coalesce(b.inv_qty, 0) 
						from (select a.item_code, a.whs_code, sum(a.open_inv_qty) inv_qty from _temp_lines a
										   											inner join _temp_close_objectaction c on a.base_type = c.base_object 
														where c.revert_update_type = '3' and a.line_status = 'O' group by a.item_code, a.whs_code) b 
					where a.item_code = b.item_code and a.whs_code = b.whs_code;
					
	update apz_oitw a set on_order = coalesce(a.on_order,0) - coalesce(b.inv_qty, 0) 
						from (select a.item_code, a.whs_code, sum(a.open_inv_qty) inv_qty from _temp_lines a
										   											inner join _temp_close_objectaction c on a.base_type = c.base_object 
														where c.revert_update_type = '4' and a.line_status = 'O' group by a.item_code, a.whs_code) b 
					where a.item_code = b.item_code and a.whs_code = b.whs_code;
					
	update apz_oitm a set is_commited = coalesce(a.is_commited,0) - coalesce(b.inv_qty, 0) 
						from (select a.item_code, sum(a.open_inv_qty) inv_qty from _temp_lines a
										   											inner join _temp_close_objectaction c on a.base_type = c.base_object 
														where c.revert_update_type = '3' and a.line_status = 'O' group by a.item_code) b 
					where a.item_code = b.item_code;
					
	update apz_oitm a set on_order = coalesce(a.on_order,0) - coalesce(b.inv_qty, 0) 
						from (select a.item_code, sum(a.open_inv_qty) inv_qty from _temp_lines a
										   											inner join _temp_close_objectaction c on a.base_type = c.base_object 
														where c.revert_update_type = '4' and a.line_status = 'O' group by a.item_code) b 
					where a.item_code = b.item_code;
					
	return query select 200 :: integer, '' :: character varying;

END; $BODY$