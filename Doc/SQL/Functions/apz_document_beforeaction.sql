CREATE OR REPLACE FUNCTION apz_document_beforeaction
(
	_object_type character varying(20),
	_transaction_type character,
	_doc_entry integer,
	_user_sign integer,
	_branch integer
)
RETURNS TABLE(msg_code smallint, message character varying(254)) 
LANGUAGE 'plpgsql'
AS $BODY$
declare _index_message smallint = 100;
		_card_code character varying(50);
		_item_code character varying(50);
		_from_whs character varying(50);
		_to_whs character varying(50);
		_vat_group_doc character varying(50);
		_current_date date = current_date;
		_current_time smallint = to_char(current_timestamp, 'HHmi');
		_max_line_id int;
		_table_id int;
		_total_bef_v numeric(19, 6);
		_discnt_per numeric(19, 6);
BEGIN
	----------------------------------------------------------- VALIDATE THÔNG TIN -----------------------------------------------------------------------
	delete from "#tempLines" where coalesce(item_code,'') = '';
	
	------------------ Kiểm tra số lượng bản ghi header
	if (select count(1) from "#tempDoc") > 1 then
		return query select _object_type + _index_message, N'Dữ liệu phiếu không đúng. Số bản ghi cung cấp nhiều hơn 1';
	end if;
	
	_index_message = _index_message + 1;
	
	----------------------- Kiểm tra tồn tại dữ liệu document để lưu
	if not exists(select 1 from "#tempDoc") or not exists(select 1 from "#tempLines") then
		return query select _object_type + _index_message, N'Dữ liệu phiếu không được cung cấp';
	end if;
	
	_index_message = _index_message + 1;

	------------------------ Kiểm tra thông tin chi nhánh
	update "#tempDoc" set branch = _branch where coalesce(branch, 0) = 0;
	if not exists(select 1 from "#tempDoc" a inner join apz_ostr b On a.branch = b.store_code) then
		return query select _object_type + _index_message, N'Thông tin chi nhánh không chính xác';
	end if;
	
	_index_message = _index_message + 1;

	------------------------------------------------ Kiểm tra thông tin khách hàng -----------------------------------
	_card_code = coalesce((select card_code from "#tempDoc"),'');
	if _card_code = '' and _object_type not in ('59','60', '67') then
		return query select _object_type + _index_message, N'Dữ liệu khách hàng chưa được cung cấp';
	end if;
	
	_index_message = _index_message + 1;

	if _card_code <> '' and not exists(select 1 from apz_ocrd where card_code = _card_code) then
		return query select _object_type + _index_message, N'Dữ liệu khách hàng không tồn tại trên hệ thống';
	end if;
	
	_index_message = _index_message + 1;

	if _card_code <> '' and exists(select 1 from apz_ocrd where card_code = _card_code and valid_for = 'N') then
		return query select _object_type + _index_message, N'Vui lòng chọn khách hàng đang hoạt động';
	end if;
	
	_index_message = _index_message + 1;
	------------------------------------------------- END kiểm tra thông tin khách hàng ------------------------------------

	---------------------- Kiểm tra thông tin mặt hàng
	_item_code = coalesce((select a.item_code from "#tempLines" a left join apz_oitm b ON a.item_code = b.item_code where b.item_code is null),'');
	if _item_code <> '' then
		return query select _object_type + _index_message, N'Mặt hàng ''' || _item_code || ''' không tồn tại trên hệ thống. Vui lòng kiểm tra lại';		
	end if;
	
	_index_message = _index_message + 1;

	_item_code = coalesce((select a.item_code from "#tempLines" a inner join apz_oitm b ON a.item_code = b.item_code where coalesce(b.valid_for,'N') = 'N' limit 1),'');
	if _item_code <> '' then
		return query select _object_type + _index_message, N'Mặt hàng ''' || _item_code || ''' đang không được sử dụng';
	end if;
	
	_index_message = _index_message + 1;

	--------------- không chuyển kho với các mặt hàng là bom sale và service
	if exists(select 1 from APZ_Get_List_Inventory_Object() where obj_type = _object_type) then
		_item_code = coalesce((select a.item_code from "#tempLines" a inner join apz_oitm b ON a.item_code = b.item_code where coalesce(b.invnt_item,'N') = 'N' limit 1),'');
		if item_code <> '' then
			return query select _object_type + _index_message, N'Mặt hàng ''' || _item_code || ''' là combo hoặc dịch vụ. Không thể lưu';
		end if;
	end if;
	_index_message = _index_message + 1;

	-------------------------- Kiểm tra thông tin kho -------------------------------------------

	if _object_type = '67' then

		_from_whs = coalesce((select from_whs_cod from "#tempDoc"), '');
		if _from_whs = '' then
			return query select _object_type + _index_message, N'Chi nhánh chuyển hàng chưa được cung cấp';
		end if;
		
		_index_message = _index_message + 1;
		_to_whs = coalesce((select whs_code from "#tempDoc"), '');
		if _to_whs = '' then
			return query select _object_type + _index_message, N'Chi nhánh nhận hàng chưa được cung cấp';
		end if;
		
		_index_message = _index_message + 1;

		if _to_whs = _from_whs then
			return query select _object_type + _index_message, N'Không thể chuyển hàng cùng chi nhánh';
		end if;
		
		_index_message = _index_message + 1;

		if not exists(select 1 from apz_owhs where whs_code = _from_whs) then
			return query select _object_type + _index_message, N'Kho chuyển hàng ' || _from_whs || N' không tồn tại trên hệ thống. Vui lòng kiểm tra lại';
		end if;
		
		_index_message = _index_message + 1;

		if not exists(select 1 from apz_owhs where whs_code = _to_whs) then
			return query select _object_type + _index_message, N'Kho nhận hàng ' || _to_whs || N' không tồn tại trên hệ thống. Vui lòng kiểm tra lại';
		end if;
		
		_index_message = _index_message + 1;

		update "#tempLines" set whs_code = _to_whs where coalesce(whs_code,'') = '';
		update "#tempLines" set from_whs_cod = _from_whs where coalesce(from_whs_cod,'') = '';
		
		_item_code = coalesce((select item_code from "#tempLines" where whs_code = from_whs_cod limit 1),'');
		if _item_code <> '' then
			return query select _object_type + _index_message, N'Mặt hàng ''' || _item_code || ''' không thể chuyển cùng chi nhánh';
		end if;
		
		_index_message = _index_message + 1;
	else
		_index_message = _index_message + 6;
	end if;

	if exists(select 1 from "#tempLines" where coalesce(whs_code,'') = '') then
		return query select _object_type + _index_message, N'Vui lòng chọn kho trước khi lưu';
	end if;
	_index_message = _index_message + 1;

	_from_whs = coalesce((select a.whs_code from "#tempLines" a left join apz_owhs b ON a.whs_code = b.whs_code where b.whs_code is null limit 1),'');
	if _from_whs <> '' then
		return query select _object_type + _index_message, N'Kho ' || _from_whs || N' không tồn tại trên hệ thống. Vui lòng kiểm tra lại';
	end if;
	
	_index_message = _index_message + 1;

	_from_whs = coalesce((select a.from_whs_cod from "#tempLines" a left join APZ_OWHS b ON a.from_whs_cod = b.whs_code where b.whs_code is null limit 1),'');
	if _from_whs <> '' then
		return query select _object_type + _index_message, N'Kho ' || _from_whs || N' không tồn tại trên hệ thống. Vui lòng kiểm tra lại';
	end if;
	_index_message = _index_message + 1;
	---------------------------------- END kiểm tra thông tin kho --------------------------------

	---------------------------------- Kiểm tra thông tin số lượng mặt hàng
	if exists(select 1 from "#tempLines" where coalesce(quantity,0) = 0) then
		return query select _object_type + _index_message, N'Số lượng mặt hàng phải khác 0';
	end if;
	_index_message = _index_message + 1;

	_vat_group_doc = coalesce((select a.vat_group from "#tempDoc" a left join apz_ovtg b ON a.vat_group = b.vat_code where b.vat_code is null limit 1),'');
	if _vat_group_doc <> '' then
		return query select _object_type + _index_message, N'Mã thuế ' || _vat_group_doc || N' không tồn tại trên hệ thống. Vui lòng kiểm tra lại';
	end if;
	_index_message = _index_message + 1;

	-- vat group line
	_vat_group_doc = coalesce((select a.vat_group from "#tempLines" a left join apz_ovtg b ON a.vat_group = b.vat_code where b.vat_code is null limit 1),'');
	if _vat_group_doc <> '' then
		return query select _object_type + _index_message, N'Mã thuế ' || _vat_group_doc || N' không tồn tại trên hệ thống. Vui lòng kiểm tra lại';
	end if;
	_index_message = _index_message + 1;

	-------------------------------------- Nợ đoạn validate base type ở đây -----------------------------------------------------

	-------------------------------------- End  validate base type --------------------------------------------------------------

	----------------------------------------------- update các thông tin mặc định ------------------------------------------------

	update "#tempDoc" set is_ins = 'Y' where coalesce(is_ins, '') not in ('Y', 'N');
	update "#tempDoc" set doc_cur = (select main_curncy from APZ_CINF) where coalesce(doc_cur, '') = '';
	Update "#tempLines" set currency = (select main_curncy from APZ_CINF) where coalesce(currency, '') = '';

	update "#tempDoc" set user_sign = _user_sign where coalesce(user_sign,0) = 0;
	update "#tempLines" set user_sign = _user_sign where coalesce(user_sign,0) = 0;

	update "#tempDoc" set doc_date = _current_date where coalesce(doc_date,'') = '';
	update "#tempDoc" set doc_due_date = _current_date where coalesce(doc_due_date,'') = '';
	update "#tempDoc" set doc_time = _current_time where coalesce(doc_time,'') = '';
	update "#tempDoc" set doc_rate = 1;
	update "#tempDoc" set obj_type = _object_type;
	update "#tempDoc" set doc_status = 'O' where coalesce(doc_status,'') = '';
	update "#tempDoc" set del_status = is_ins where coalesce(del_status,'') = '';
	update "#tempDoc" set ret_status = 'N' where coalesce(ret_status,'') = '';
	update "#tempDoc" set pmt_status = 'N' where coalesce(pmt_status,'') = '';
	update "#tempDoc" set whs_code = (select whs_code from "#tempLines" limit 1) where coalesce(whs_code,'') = '';
	update "#tempDoc" set canceled = 'N' where coalesce(canceled,'') = '';

	if _transaction_type = 'A' then
		update "#tempDoc" set create_date = _current_date;
		--update "#tempLines" CreateDate = @currentDate where coalesce(CreateDate,'') = ''
	else 
		update "#tempDoc" set update_date = _current_date;
	end if;

	update a set ugp_entry = b.ugp_entry, unit_msr = case when coalesce(unit_msr, '') = '' then b.sal_unit_msr else a.unit_msr end
									  , uom_entry = case when coalesce(uom_entry, '') = '' then b.s_uom_entry else a.uom_entry end from "#tempLines" a inner join APZ_OITM b ON a.item_code = b.item_code;

	if exists(select 1 from "#tempLines" a left join APZ_UGP1 b ON a.ugp_entry = b.ugp_entry and a.uom_entry = b.uom_entry where coalesce(a.ugp_entry,-1) <> -1 and b.ugp_entry is null) then
		return query select _object_type + _index_message, N'Thông tin đơn vị tính của mặt hàng không chính xác';
	end if;
	_index_message = _index_message + 1;

	update "#tempLines" set tree_type = 'N' where coalesce(tree_type, '') = '';
	update "#tempLines" set father_id = null where coalesce(father_id,-1) < 0;

	------------------ update thông tin lineNum của các line không có thông tin 
	_max_line_id = coalesce((select max(coalesce(line_num,-1)) from "#tempLines"),0) + 1;
	update "#tempLines" set line_num = _max_line_id, _max_line_id = _max_line_id + 1 where coalesce(line_num, -1) < 0;

	------------- validate Father id của mặt hàng con
	_item_code = coalesce((select item_code from "#tempLines" where tree_type = 'I' and father_id is null limit 1),'');
	if item_code <> '' then
		return query select _object_type + _index_message, N'Thông tin không chính xác, mặt hàng ' || item_code || N' không thuộc combo';
	end if;
	_index_message = _index_message + 1;

	------------------- Validate thông tin mặt hàng cha của mặt hàng trong combo
	_item_code = coalesce((select item_code from "#tempLines" a where tree_type = 'I' and not exists(select 1 from "#tempLines" x where x.line_num = a.father_id) limit 1),'');
	if item_code <> '' then
		return query select _object_type + _index_message, N'Thông tin không chính xác, không tìm thấy thông tin mặt hàng combo của mặt hàng ' || item_code || N'';
	end if;
	_index_message = _index_message + 1;

	------------------- Validate thông tin mặt hàng con của mặt hàng combo
	_item_code = coalesce((select item_code from "#tempLines" a where tree_type = 'S' and not exists(select 1 from "#tempLines" x where coalesce(x.father_id,-1) = a.line_num) limit 1),'');
	if item_code <> '' then
		return query select _object_type + _index_message, N'Thông tin không chính xác, mặt hàng combo ' || item_code || N' không có mặt hàng con';
	end if;
	_index_message = _index_message + 1;

	update "#tempDoc" set table_id = null where coalesce(table_id, 0) < 0;
	_table_id = coalesce((select table_id From "#tempDoc"), 0 );
	if _table_id > 0 and not exists(select 1 from APZ_OTMD where doc_entry = _table_id) then
		return query select _object_type + _index_message, N'Bàn ' || cast(_table_id as character varying) || N' không tồn tại trên hệ thống';
	end if;
	_index_message = _index_message + 1;

	update "#tempLines" set open_qty = quantity;
	update "#tempLines" set line_status = 'O' where coalesce(line_status, '') = '';
	update "#tempLines" set ship_date = _current_date where ship_date is null;
	update "#tempLines" set uom_entry = null where coalesce(ugp_entry,0) = 0;
	--update a NumPerMsr = b.AltQty from "#tempLines" a inner join APZ_UGP1 b ON a.UgpEntry = b.UgpEntry and a.UomEntry = b.UomEntry
	update "#tempLines" set num_per_msr = 1 where coalesce(num_per_msr, 0) = 0;
	update "#tempLines" set inv_qty = quantity * num_per_msr, open_inv_qty = open_qty * num_per_msr;
	update "#tempDoc" set doc_entry = _doc_entry, doc_num = _doc_entry;
	update "#tempLines" set doc_entry = _doc_entry;

	update "#tempLines" set base_type = -1 where base_type is null;

	if _object_type = '67' and _transaction_type = 'A' then
		update a set price = b.avg_price from "#tempLines" a inner join apz_oitw b On a.item_code = b.item_code and a.from_whs_cod = b.whs_code;
	end if;
	
	update "#tempLines" set total_bef_v = quantity * price;

	_total_bef_v = (select sum(coalesce(total_bef_v,0)) from "#tempLines");

	update "#tempDoc" set disc_prcnt = case when _total_bef_v <> 0 then (coalesce(disc_sum,0) * 100) / _total_bef_v else 0 end;

	_discnt_per = coalesce((select disc_prcnt from "#tempDoc"), 0);

	update "#tempLines" set line_total = total_bef_v * (1 - _discnt_per / 100) * (1 + coalesce(vat_prcnt,0) / 100);

	update "#tempDoc" set vat_sum = coalesce((select Sum(line_vat) from "#tempLines"), 0);

	----------------------------------------------- end update thông tin mặc định ------------------------------------------------
	
	return query select 0, _doc_entry;
	
END
$BODY$;
