create or replace procedure aplus_document_itemcost()
	language 'plpgsql'
AS $BODY$
BEGIN

	------------- Định nghĩa bảng kết quả tính toán
	create local temporary table _tbl_cost (item_code character varying(50), whs_code character varying(50)
										   , operation char(1), quantity numeric(19,6), price numeric(19,6), inv_qty numeric(19,6));

	-------- lấy dữ liệu item tính toán lại cost
	create local temporary table _temp_item_cost as select a.item_code, b.obj_type, a.inv_qty, a.quantity, a.price, a.base_type, a.base_entry, a.base_line, a.whs_code, c.operation, c.base_cost
			 from _temp_lines a inner join _temp_doc b ON a.doc_entry = b.doc_entry
							   inner join aplus_document_updatecostobject() c ON c.obj_type = b.obj_type and c.base_type = a.base_type;

	insert into _tbl_cost (item_code, whs_code, operation, quantity, price, inv_qty)
		select a.item_code, a.whs_code, a.operation, a.quantity, case when a.base_cost = 'N' then a.price else coalesce(c.item_cost, 0) end, a.inv_qty
					from _temp_item_cost a
						left join _temp_base_data c On a.base_type = c.obj_type and c.doc_entry = a.base_entry and c.line_num = a.base_line;
	
	update apz_oitw a set avg_price = (coalesce(a.on_hand,0) * coalesce(a.avg_price, 0) + coalesce(b.total, 0)) / (case coalesce((a.on_hand + b.quantity),0) when 0 then 1 else a.on_hand + b.quantity end)
				from (select item_code, whs_code, sum((case operation when '+' then 1 else -1 end) * quantity * price) total, sum(quantity) quantity from _tbl_cost x group by item_code, whs_code) b 
					where a.item_code = b.item_code and a.whs_code = b.whs_code;

END; $BODY$