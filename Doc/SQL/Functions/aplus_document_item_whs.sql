CREATE or replace function aplus_document_item_whs(_obj_type int, _trans_type character varying(1), _obj_id character varying(254))
	returns table (msg_code integer, message character varying(254))
	language 'plpgsql'
as $BODY$
declare
	_main_currency character varying(3);
	_neg_whs character varying(1);
	_card_code character varying(50);
	_canceled character varying(1);
	_new_tranq_id int;
BEGIN

	_neg_whs = coalesce((select neg_whs from APZ_CINF), 'N');
	_main_currency = coalesce((SELECT main_currency FROM APZ_CINF),'');
	_card_code = coalesce((SELECT card_code FROM _temp_doc),'');
	select canceled from _temp_doc into _canceled;
	
	------------------------- lấy table action của object hiện tại --------------------------------
	create local temporary table _object_action as select * from aplus_document_objectaction(_obj_type, 0, '');
	
	------------------------------ validate số lượng copy không được lớn hơn số lượng base với các object in/out kho
	if exists(select 1 from _temp_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
			  								inner join _temp_current_lines c on a.line_num = b.line_num
			  								inner join _object_action d on a.base_type = d.base_object
			 					where d.onhand_type = 6 and a.inv_qty - c.inv_qty > b.open_qty) then
		return query select 10335 :: integer, 'Copy quantity cannot greater than base quantity' :: character varying;
		return;
	end if;
	
	--------------------------------- validate quantity ----------------------------------------------------
	if _neg_whs = 'N' then
		create local temporary table _temp_object_qty as 
			select a.item_code, a.whs_code, sum(a.inv_qty - coalesce(b.inv_qty, 0)) inv_qty 
				from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
									inner join _object_action c on a.base_type = c.base_object where c.onhand_type = 1 group by a.item_code, a.whs_code;

		if exists(select 1 from _temp_object_qty a inner join apz_oitw b on a.item_code = b.item_code and a.whs_code = b.whs_code where a.inv_qty > b.on_hand) then
			return query select 10336 :: integer, 'Quantity of item is not enough' :: character varying;
			return;
		end if;
	end if;
	------------------------------- end validate quantity -------------------------------------------
	
	------------------------------------ lấy dữ liệu các line update quantity data -----------------------------------------
	create local temporary table _update_lines as 
			select a.doc_entry, a.line_num, a.create_date, a.create_time, a.item_code, a.price, a.currency, a.rate, a.whs_code, a.user_sign, a.open_qty, a.quantity, a.unit_msr
				, a.ugp_entry, a.uom_code, a.num_per_msr, a.inv_qty, a.open_inv_qty, a.base_type, a.base_entry, a.base_line, b.item_cost 
			from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
						where b.item_code is null or a.quantity <> b.quantity or a.item_code <> b.item_code or a.price <> b.price or a.rate <> b.rate or a.currency <> b.currency 
							or a.whs_code <> b.whs_code or a.unit_msr <> b.unit_msr or a.ugp_entry <> b.ugp_entry or a.uom_code <> b.uom_code or a.num_per_msr <> b.num_per_msr 
							or a.line_status <> b.line_status;
	
	-------------------------- log cân bằng lại log kho của dữ liệu base data----------------------------------------
	_new_tranq_id = coalesce((select max(trans_seq) from apz_oivl), 0);
	
	insert into apz_oivl (trans_seq,trans_type,create_by,obj_type,doc_entry,line_num,doc_date,item_code,in_qty,out_qty,price,currency,rate,create_date,create_time,whs_code
						  ,user_sign,date_source,post_status,sum_stock,open_qty,tree_id,parent_id,version_num,qty,unit_msr,ugp_entry,uom_code,num_per_msr,price_list,item_cost
						  ,table_id,card_code, inv_qty, open_inv_qty, base_type, base_entry, base_line, target_type, target_entry, target_line, action_type)
		select _new_tranq_id + row_number() over(order by a.line_num), d.base_onhand_type, a.user_sign, b.obj_type, b.doc_entry, b.line_num, c.doc_date, b.item_code, 
				0, 0, b.price, b.currency, b.rate, current_date, cast(to_char(current_timestamp, 'HHmi') as smallint), b.whs_code
				, a.user_sign, 'N', null, null, null, null, null, null, null, b.unit_msr, b.ugp_entry, b.uom_code, b.num_per_msr, c.group_num, b.item_cost
				, null, c.card_code, -1 * (case when a.inv_qty - coalesce(e.inv_qty, 0) > b.open_qty then b.open_qty else a.inv_qty - coalesce(e.inv_qty, 0) end)
				, null, b.base_type, b.base_entry, b.base_line, _obj_type, a.doc_entry, a.line_num, _trans_type
				from _update_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
											inner join _temp_current_doc c on a.doc_entry = c.doc_entry
											left join _temp_current_lines e on a.line_num = e.line_num
											inner join _object_action d on a.base_type = d.base_object 
							where d.base_onhand_type in ('1','2', '3', '4') and b.open_qty > 0;
							
	---------------------------------------- cân bằng lại connit và order inventory data của các phiếu base ---------------------------------------------------------
	update apz_oitw a set is_commited = coalesce(a.is_commited, 0) - coalesce(b.inv_qty,0) from (select a.item_code, a.whs_code
										   			, sum(case when coalesce(a.inv_qty,0) - coalesce(c.inv_qty, 0) > coalesce(b.open_qty,0) 
														  			then coalesce(b.open_qty,0) 
														  			else coalesce(a.inv_qty,0) - coalesce(c.inv_qty, 0) end) inv_qty 
										   		from _update_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
																		left join _temp_current_lines c on a.line_num = c.line_num
																		inner join _object_action d on a.base_type = d.base_object 
													where d.base_onhand_type = '3' and b.open_qty > 0 group by a.item_code, a.whs_code) b 
							where a.item_code = b.item_code and a.whs_code = b.whs_code;
							
	update apz_oitw a set on_order = coalesce(a.on_order, 0) - coalesce(b.inv_qty,0) from (select a.item_code, a.whs_code
										   			, sum(case when coalesce(a.inv_qty,0) - coalesce(c.inv_qty, 0) > coalesce(b.open_qty,0) 
														  			then coalesce(b.open_qty,0) 
														  			else coalesce(a.inv_qty,0) - coalesce(c.inv_qty, 0) end) inv_qty 
										   		from _update_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
																		left join _temp_current_lines c on a.line_num = c.line_num
																		inner join _object_action d on a.base_type = d.base_object 
													where d.base_onhand_type = '4' and b.open_qty > 0 group by a.item_code, a.whs_code) b 
							where a.item_code = b.item_code and a.whs_code = b.whs_code;
							
	update apz_oitm a set is_commited = coalesce(a.is_commited, 0) - coalesce(b.inv_qty,0) from (select a.item_code
										   			, sum(case when coalesce(a.inv_qty,0) - coalesce(c.inv_qty, 0) > coalesce(b.open_qty,0) 
														  			then coalesce(b.open_qty,0) 
														  			else coalesce(a.inv_qty,0) - coalesce(c.inv_qty, 0) end) inv_qty 
										   		from _update_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
																		left join _temp_current_lines c on a.line_num = c.line_num
																		inner join _object_action d on a.base_type = d.base_object 
													where d.base_onhand_type = '3' and b.open_qty > 0 group by a.item_code) b 
							where a.item_code = b.item_code;
							
	update apz_oitm a set on_order = coalesce(a.on_order, 0) - coalesce(b.inv_qty,0) from (select a.item_code
										   			, sum(case when coalesce(a.inv_qty,0) - coalesce(c.inv_qty, 0) > coalesce(b.open_qty,0) 
														  			then coalesce(b.open_qty,0) 
														  			else coalesce(a.inv_qty,0) - coalesce(c.inv_qty, 0) end) inv_qty 
										   		from _update_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
																		left join _temp_current_lines c on a.line_num = c.line_num
																		inner join _object_action d on a.base_type = d.base_object 
													where d.base_onhand_type = '4' and b.open_qty > 0 group by a.item_code) b 
							where a.item_code = b.item_code;
					
	--------------------------------- log nhập xuất lại dữ liệu của phiếu trong trường hợp update -----------------------------------
	_new_tranq_id = coalesce((select max(trans_seq) from apz_oivl), 0);
	
	insert into apz_oivl (trans_seq,trans_type,create_by,obj_type,doc_entry,line_num,doc_date,item_code,in_qty,out_qty,price,currency,rate,create_date,create_time,whs_code
						  ,user_sign,date_source,post_status,sum_stock,open_qty,tree_id,parent_id,version_num,qty,unit_msr,ugp_entry,uom_code,num_per_msr,price_list,item_cost
						  ,table_id,card_code, inv_qty, open_inv_qty, base_type, base_entry, base_line, target_type, target_entry, target_line, action_type)
		select _new_tranq_id + row_number() over(order by a.line_num), d.revert_update_type, a.user_sign, _obj_type, a.doc_entry, a.line_num, c.doc_date, a.item_code, 
				0, 0, a.price, a.currency, a.rate, current_date, cast(to_char(current_timestamp, 'HHmi') as smallint), a.whs_code
				, a.user_sign, 'N', null, null, null, null, null, null, null, a.unit_msr, a.ugp_entry, a.uom_code, a.num_per_msr, c.group_num, a.item_cost
				, null, c.card_code, -1 * a.inv_qty, null, a.base_type, a.base_entry, a.base_line, null, null, null, _trans_type
				from _temp_current_lines a inner join _update_lines b on a.doc_entry = b.doc_entry and a.line_num = b.line_num
											inner join _temp_current_doc c on a.doc_entry = c.doc_entry
											inner join _object_action d on a.base_type = d.base_object 
							where d.revert_update_type in ('3', '4');
							
	---------------------------------------- log nhập xuất dữ liệu của phiếu --------------------------------------------------
	_new_tranq_id = coalesce((select max(trans_seq) from apz_oivl), 0);
	
	insert into apz_oivl (trans_seq,trans_type,create_by,obj_type,doc_entry,line_num,doc_date,item_code,in_qty,out_qty,price,currency,rate,create_date,create_time,whs_code
						  ,user_sign,date_source,post_status,sum_stock,open_qty,tree_id,parent_id,version_num,qty,unit_msr,ugp_entry,uom_code,num_per_msr,price_list,item_cost
						  ,table_id,card_code, inv_qty, open_inv_qty, base_type, base_entry, base_line, target_type, target_entry, target_line, action_type)
		select _new_tranq_id + row_number() over(order by a.line_num), c.onhand_type, a.user_sign, _obj_type, a.doc_entry, a.line_num, b.doc_date, a.item_code, 
				0, 0, a.price, a.currency, a.rate, current_date, cast(to_char(current_timestamp, 'HHmi') as smallint), a.whs_code
				, a.user_sign, 'N', null, null, null, null, null, null, null, a.unit_msr, a.ugp_entry, a.uom_code, a.num_per_msr, b.group_num, a.item_cost
				, null, b.card_code, (case when _trans_type = 'C' then -1 else 1 end) * a.inv_qty, null, a.base_type, a.base_entry, a.base_line, null, null, null, _trans_type
				from _update_lines a inner join _temp_doc b on a.doc_entry = b.doc_entry
											inner join _object_action c on a.base_type = c.base_object 
							where c.onhand_type in ('1', '2','3', '4');
	
	---------------------------------------- update lại dữ liệu inventory data -------------------------------------------------------
	update apz_oitw a set on_hand = coalesce(a.on_hand,0) - (case when _trans_type = 'C' then -1 else 1 end) * coalesce(b.inv_qty, 0) 
						from (select a.item_code, a.whs_code, sum(a.inv_qty) inv_qty from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
										   											inner join _object_action c on a.base_type = c.base_object 
														where b.item_code is null and c.onhand_type = '1' group by a.item_code, a.whs_code) b 
					where a.item_code = b.item_code and a.whs_code = b.whs_code;
												
	update apz_oitw a set on_hand = coalesce(a.on_hand,0) + (case when _trans_type = 'C' then -1 else 1 end) * coalesce(b.inv_qty, 0) 
						from (select a.item_code, a.whs_code, sum(a.inv_qty) inv_qty from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
										   										inner join _object_action c on a.base_type = c.base_object 
														where b.item_code is null and c.onhand_type = '2' group by a.item_code, a.whs_code) b 
					where a.item_code = b.item_code and a.whs_code = b.whs_code;
												
	update apz_oitw a set is_commited = coalesce(a.is_commited,0) + (case when _trans_type = 'C' then -1 else 1 end) * coalesce(b.inv_qty, 0) 
						from (select a.item_code, a.whs_code
							  		, sum(case when a.line_status <> 'O' then -1 * coalesce(b.open_inv_qty, 0) else coalesce(a.inv_qty,0) - coalesce(b.inv_qty,0) end) inv_qty 
							  from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
										   		inner join _object_action c on a.base_type = c.base_object 
										where c.onhand_type = '3' and coalesce(b.line_status, 'O') = 'O' group by a.item_code, a.whs_code) b 
					where a.item_code = b.item_code and a.whs_code = b.whs_code;
					
	update apz_oitw a set on_order = coalesce(a.on_order,0) + (case when _trans_type = 'C' then -1 else 1 end) * coalesce(b.inv_qty, 0) 
						from (select a.item_code, a.whs_code
							  		, sum(case when a.line_status <> 'O' then -1 * coalesce(b.open_inv_qty, 0) else coalesce(a.inv_qty,0) - coalesce(b.inv_qty,0) end) inv_qty 
							  from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
										   		inner join _object_action c on a.base_type = c.base_object 
										where c.onhand_type = '4' and coalesce(b.line_status, 'O') = 'O' group by a.item_code, a.whs_code) b 
					where a.item_code = b.item_code and a.whs_code = b.whs_code;
	
	update apz_oitm a set on_hand = coalesce(a.on_hand,0) - (case when _trans_type = 'C' then -1 else 1 end) * coalesce(b.inv_qty, 0) 
						from (select a.item_code, sum(a.inv_qty) inv_qty from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
										   											inner join _object_action c on a.base_type = c.base_object 
														where b.item_code is null and c.onhand_type = '1' group by a.item_code) b 
					where a.item_code = b.item_code;
												
	update apz_oitm a set on_hand = coalesce(a.on_hand,0) + (case when _trans_type = 'C' then -1 else 1 end) * coalesce(b.inv_qty, 0) 
						from (select a.item_code, sum(a.inv_qty) inv_qty from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
										   										inner join _object_action c on a.base_type = c.base_object 
														where b.item_code is null and c.onhand_type = '2' group by a.item_code) b 
					where a.item_code = b.item_code;
												
	update apz_oitm a set is_commited = coalesce(a.is_commited,0) + (case when _trans_type = 'C' then -1 else 1 end) * coalesce(b.inv_qty, 0) 
						from (select a.item_code
							  		, sum(case when a.line_status <> 'O' then -1 * coalesce(b.open_inv_qty, 0) else coalesce(a.inv_qty,0) - coalesce(b.inv_qty,0) end) inv_qty 
							  from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
										   		inner join _object_action c on a.base_type = c.base_object 
										where c.onhand_type = '3' and coalesce(b.line_status, 'O') = 'O' group by a.item_code) b 
					where a.item_code = b.item_code;
					
	update apz_oitm a set on_order = coalesce(a.on_order,0) + (case when _trans_type = 'C' then -1 else 1 end) * coalesce(b.inv_qty, 0) 
						from (select a.item_code
							  		, sum(case when a.line_status <> 'O' then -1 * coalesce(b.open_inv_qty, 0) else coalesce(a.inv_qty,0) - coalesce(b.inv_qty,0) end) inv_qty 
							  from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num
										   		inner join _object_action c on a.base_type = c.base_object 
										where c.onhand_type = '4' and coalesce(b.line_status, 'O') = 'O' group by a.item_code) b 
					where a.item_code = b.item_code;
	--------------------------- END UPDATE inventory data -------------------------------------------------------------------
	
	if _trans_type = 'C' then
		update _temp_lines set open_qty = 0, open_inv_qty = 0;
	end if;
	
	return query SELECT 200 :: integer, '' :: character varying;
END; $BODY$