CREATE OR REPLACE FUNCTION aplus_document_beforeaction
(
	_object_type integer,
	_transaction_type character,
	_doc_entry integer,
	_user_sign integer
)
	RETURNS TABLE(msg_code integer, message character varying(254)) 
	LANGUAGE 'plpgsql'
AS $BODY$
declare
		_card_code character varying(50);
		_item_code character varying(50);
		_whs_code character varying(50);
		_vat_group character varying(50);
		_current_date date = current_date;
		_current_time smallint = cast(to_char(current_timestamp, 'HHmi') as smallint);
		_max_line_id int;
		_table_id int;
		_total_bef_v numeric(19, 6);
		_discnt_per numeric(19, 6);
		_multi_branch character varying(1);
		_main_currency character varying(3);
		_is_valid_data character varying(1) = 'N';
BEGIN

	----------------------------------------------------------- VALIDATE THÔNG TIN -----------------------------------------------------------------------
	delete from _temp_lines where coalesce(item_code,'') = '';
	
	------------------ Kiểm tra số lượng bản ghi header
	if coalesce((select count(1) from _temp_doc),0) <> 1 then
		return query select 10000 :: int, 'Dữ liệu phiếu không đúng' :: character varying;
		return;
	end if;
	
	----------------------- Kiểm tra tồn tại dữ liệu document để lưu
	if not exists(select 1 from _temp_doc) or not exists(select 1 from _temp_lines) then
		return query select 10003 :: int, 'Dữ liệu phiếu không được cung cấp' :: character varying;
		return;
	end if;
	--lấy thông tin setting mutil branch trên hệ thống
	select coalesce(multi_branch, 'N') from apz_cinf into _multi_branch;
	------------------------ Kiểm tra thông tin chi nhánh
	if _multi_branch = 'N' then
		update _temp_doc set company = -1;
	elseif not exists(select 1 from _temp_doc a inner join apz_obpl b On coalesce(a.company,-1) = b.code) then
		return query select 10338 :: int, 'Document company is not correct' :: character varying;
		return;
	end if;
	------------------------------------------------ Kiểm tra thông tin khách hàng -----------------------------------
	
	_card_code = coalesce((select card_code from _temp_doc),'');
	---chỉ kiểm tra thông tin đối tác khi các phiếu khác phiếu inventory
	if _card_code = '' and _object_type not in ('59','60', '67', '1250000001') then
		return query select 10140 :: int, 'Business partner code not provide' :: character varying;
		return;
	end if;
	
	if _card_code <> '' and not exists(select 1 from apz_ocrd where card_code = _card_code and status = 'A') then
		return query select 10339 :: int, 'Please choose a active business partner' :: character varying;
		return;
	end if;
	------------------------------------------------- END kiểm tra thông tin khách hàng ------------------------------------

	---------------------- Kiểm tra thông tin mặt hàng
	_item_code = coalesce((select a.item_code from _temp_lines a left join apz_oitm b ON a.item_code = b.item_code 
						   	where b.item_code is null or coalesce(b.status,'N') = 'N' limit 1),'');
	if _item_code <> '' then
		return query select 10340 :: int, ('Mặt hàng ''' || _item_code || ''' không tồn tại trên hệ thống. Vui lòng kiểm tra lại') :: character varying;		
		return;
	end if;
	
	--------------- không chuyển kho với các mặt hàng là bom sale và service
	
	if exists(select 1 from aplus_document_objectaction(_object_type, '-1', 'I')) then
		_item_code = coalesce((select a.item_code from _temp_lines a inner join apz_oitm b ON a.item_code = b.item_code 
							   			where coalesce(b.invnt_item,'N') = 'N' limit 1),'');
		if _item_code <> '' then
			return query select 10341 :: int, ('Mặt hàng ''' || _item_code || ''' là mặt hàng không lưu kho') :: character varying;
			return;
		end if;
	end if;
	
	if exists(select 1 from aplus_document_objectaction(_object_type, '-1', 'O')) then
		_item_code = coalesce((select a.item_code from _temp_lines a inner join apz_oitm b ON a.item_code = b.item_code 
							   			where coalesce(b.sell_item,'N') = 'N' limit 1),'');
		if _item_code <> '' then
			return query select 10342 :: int, ('Mặt hàng ''' || _item_code || ''' là mặt hàng không được bán') :: character varying;
			return;
		end if;
		
		if not exists(select 1 from apz_ocrd where card_code = _card_code and card_type = 'C') then
			return query select 10107 :: int, 'Customer does not exists' :: character varying;
			return;
		end if;
	
	end if;
	
	if exists(select 1 from aplus_document_objectaction(_object_type, '-1', 'P')) then
		_item_code = coalesce((select a.item_code from _temp_lines a inner join apz_oitm b ON a.item_code = b.item_code 
							   			where coalesce(b.prchse_item,'N') = 'N' limit 1),'');
		if _item_code <> '' then
			return query select 10343 :: int, ('Mặt hàng ''' || _item_code || ''' là mặt hàng không được mua') :: character varying;
			return;
		end if;
		
		if not exists(select 1 from apz_ocrd where card_code = _card_code and card_type = 'S') then
			return query select 10359 :: int, 'Suplier does not exists' :: character varying;
			return;
		end if;
	
	end if;

	-------------------------- Kiểm tra thông tin kho -------------------------------------------
	------------------ kiểm tra các case đặc thù của trasfer --------------------------------------
	if _object_type in ('67', '1250000001') then
		if not exists(select 1 from _temp_doc a left join apz_owhs b on coalesce(a.from_whs_cod, '') = '' or (b.whs_code = a.from_whs_cod and b.status = 'A') where b.whs_code is not null) then
			return query select 10344 :: int, 'Kho chuyển hàng không tồn tại trên hệ thống. Vui lòng kiểm tra lại' :: character varying;
			return;
		end if;
		
		if not exists(select 1 from _temp_doc a left join apz_owhs b on coalesce(a.whs_code, '') = '' or (b.whs_code = a.whs_code  and b.status = 'A') 
					  				where b.whs_code is not null) then
			return query select 10344 :: int, 'Kho nhận hàng không tồn tại trên hệ thống. Vui lòng kiểm tra lại' :: character varying;
			return;
		end if;
		
		--------- update lại các line chưa có kho bằng dữ liệu default trên header
		update _temp_lines a set whs_code = b.whs_code from _temp_doc b where coalesce(a.whs_code,'') = '';
		update _temp_lines a set from_whs_cod = b.from_whs_cod from _temp_doc b where coalesce(a.from_whs_cod,'') = '';
		
		_whs_code = coalesce((select a.from_whs_cod from _temp_lines a left join apz_owhs b ON a.from_whs_cod = b.whs_code and b.status = 'A' where b.whs_code is null limit 1),'');
		if _whs_code <> '' then
			return query select 10344 :: int, ('Kho ' || _whs_code || ' không tồn tại trên hệ thống. Vui lòng kiểm tra lại') :: character varying;
			return;
		end if;
		
		if exists(select 1 from _temp_lines a inner join apz_oitw b on a.item_code = b.item_code and a.from_whs_cod = b.whs_cod where b.status = 'I') then
			return query select 10345 :: int, 'Mặt hàng không được sử dụng trên kho' :: character varying;
			return;
		end if;
		
		_item_code = coalesce((select item_code from _temp_lines where whs_code = from_whs_cod limit 1),'');
		if _item_code <> '' then
			return query select 10346 :: int, ('Mặt hàng ''' || _item_code || ''' không thể chuyển cùng kho') :: character varying;
			return;
		end if;
	end if;
	-------------------- END validate transfer ----------------------------------------------

	_whs_code = coalesce((select a.whs_code from _temp_lines a left join apz_owhs b ON a.whs_code = b.whs_code and b.status = 'A' where b.whs_code is null limit 1),'');
	if _whs_code <> '' then
		return query select 10344 :: int, ('Kho ' || _whs_code || ' không tồn tại trên hệ thống. Vui lòng kiểm tra lại') :: character varying;
		return;
	end if;
	---------------------------------- END kiểm tra thông tin kho --------------------------------

	---------------------------------- Kiểm tra thông tin số lượng mặt hàng trong kho
	if exists(select 1 from _temp_lines where coalesce(quantity,0) = 0) then
		return query select 10406 :: int, 'Số lượng mặt hàng phải khác 0' :: character varying;
		return;
	end if;
	
	-------------- thông tin vat doc ----------------------------------------------------------------
	_vat_group = coalesce((select a.vat_group from _temp_doc a left join apz_ovtg b ON a.vat_group = b.code where coalesce(b.status, 'I') = 'I' limit 1),'');
	if _vat_group <> '' then
		return query select 10145 :: int, ('Mã thuế ' || _vat_group || ' không tồn tại trên hệ thống. Vui lòng kiểm tra lại') :: character varying;
		return;
	end if;
	-- vat group line
	_vat_group = coalesce((select a.vat_group from _temp_lines a left join apz_ovtg b ON a.vat_group = b.code where coalesce(b.status, 'I') = 'I' limit 1),'');
	if _vat_group <> '' then
		return query select 10145 :: int, ('Mã thuế ' || _vat_group || ' không tồn tại trên hệ thống. Vui lòng kiểm tra lại') :: character varying;
		return;
	end if;
	-------------------------------------- Nợ đoạn validate base type ở đây -----------------------------------------------------
	-------------------------------------- End  validate base type --------------------------------------------------------------

	------kiểm tra thông tin price list
	if not exists(select 1 from _temp_doc a inner join apz_opln b on coalesce(a.group_num, 0) = b.doc_entry) then
		return query select 10144::int, N'bảng giá không tồn tại trên hệ thống' :: character varying;
		return;
	end if;

	----------------------------------------------- update các thông tin mặc định ------------------------------------------------
	_main_currency = (select main_currency from apz_cinf);
	
	update _temp_doc set doc_cur = _main_currency where coalesce(doc_cur, '') = '';
	
	----validate doc currency
	if not exists(select 1 from _temp_doc a inner join apz_ocrn b on a.doc_cur = b.code where b.status = 'A') then
		return query select 10103 :: int, 'Đồng tiền không tồn tại trên hệ thống' :: character varying;
		return;
	end if;
	
	Update _temp_lines set currency = _main_currency where coalesce(currency, '') = '';
	----validate line currency
	if exists(select 1 from _temp_lines a left join apz_ocrn b on a.currency = b.code where coalesce(b.status, 'I') = 'I') then
		return query select 10103 :: int, 'Đồng tiền không tồn tại trên hệ thống' :: character varying;
		return;
	end if;

	update _temp_doc set doc_date = _current_date where doc_date is null;
	
	--- validate docDueDate
	if exists(select 1 from _temp_doc where doc_due_date is null) then
		return query select 10347 :: int, 'DueDate không được để trống' :: character varying;
		return;
	end if;
	
	update _temp_doc set doc_rate = 1 where coalesce(doc_rate, 0) <= 0;
	update _temp_doc set obj_type = _object_type;
	update _temp_doc set doc_status = 'O' where coalesce(doc_status,'') = '';
	update _temp_doc set canceled = 'N' where coalesce(canceled,'') = '';
	update _temp_lines set rate = 1 where coalesce(rate, 0) <= 0;
	update _temp_lines a set item_cost = b.avg_price from apz_oitw b where a.item_code = b.item_code and a.whs_code = b.whs_code;
	------------- cập nhật ngày tạo và người tạo
	if _transaction_type = 'A' or _transaction_type = 'C' then
		update _temp_doc set create_date = _current_date, create_time = _current_time, user_sign = _user_sign;
		update _temp_lines set create_date = _current_date, create_time = _current_time, user_sign = _user_sign;
	else
		if not exists(select 1 from _temp_current_doc) then
			return query select 10358 :: int, 'Document does not exists' :: character varying;
			return;
		end if;
		
		update _temp_doc a set create_date = b.create_date, create_time = b.create_time, user_sign = b.user_sign,
							user_sign2 = _user_sign, update_date = _current_date, update_time = _current_time from _temp_current_doc b;
		update _temp_lines a set create_date = b.create_date, create_time = b.create_time, user_sign = b.user_sign,
						user_sign2 = _user_sign, update_date = _current_date, update_time = _current_time
						, item_cost = b.item_cost, open_qty = case when b.quantity = b.open_qty then a.quantity else b.open_qty end
					from _temp_current_lines b where a.line_num = b.line_num;
		update _temp_lines set create_date = _current_date, create_time = _current_time, user_sign = _user_sign where user_sign is null;
	end if;

	----------- validate thông tin đơn vị tính của mặt hàng
	update _temp_lines set ugp_entry = -1 where coalesce(ugp_entry, 0) < 1;
	
	if exists(select 1 from _temp_lines a inner join apz_oitm b on a.item_code = b.item_code where a.ugp_entry != b.ugp_entry) then
		return query select 10348 :: int, 'Đơn vị tính của mặt hàng không chính xác' :: character varying;
		return;
	end if;

	if exists(select 1 from _temp_lines a left join APZ_UGP1 b ON a.ugp_entry = b.ugp_entry and a.uom_code = b.uom_code where a.ugp_entry <> -1 and b.ugp_entry is null) then
		return query select 10348 :: int, 'Thông tin đơn vị tính của mặt hàng không chính xác' :: character varying;
		return;
	end if;

	update _temp_lines set tree_type = 'N' where coalesce(tree_type, '') = '';
	if exists(select 1 from _temp_lines where tree_type not in ('N', 'A', 'S', 'I', 'P', 'T')) then
		return query select 10349 :: int, 'Dữ liệu tree_type không chính xác' :: character varying;
		return;
	END IF;
	
	------------------ update thông tin lineNum của các line không có thông tin 
	if exists(select 1 from _temp_lines where coalesce(line_num, -1) < 0) or exists(select 1 from _temp_lines group by line_num having count(1) > 1) then
		return query select 10300::int, 'Line_Num data is not correct' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_lines where coalesce(vis_order, -1) < 0) or exists(select 1 from _temp_lines group by vis_order having count(1) > 1) then
		return query select 10361::int, 'Visible order is not correct' :: character varying;
		return;
	end if;
	
	update _temp_lines set father_id = -1 where coalesce(father_id,-1) < 0;
	
	if exists(select 1 from _temp_lines where father_id = line_num) 
		or exists(select 1 from _temp_lines x where father_id <> -1 and not exists(select 1 from _temp_lines y where y.father_id = x.line_num)) then
		return query select 10350::int, 'Dữ liệu fathe_id không chính xác' :: character varying;
		return;
	end if;

	------------- validate Father id của mặt hàng con
	_item_code = coalesce((select item_code from _temp_lines where tree_type = 'I' and father_id is null limit 1),'');
	if _item_code <> '' then
		return query select 10351::integer, ('Thông tin không chính xác, mặt hàng ' || _item_code || ' không thuộc combo') :: character varying;
		return;
	end if;

	------------------- Validate thông tin mặt hàng con của mặt hàng combo
	_item_code = coalesce((select item_code from _temp_lines a where tree_type = 'S' and not exists(select 1 from _temp_lines x where coalesce(x.father_id,-1) = a.line_num) limit 1),'');
	if _item_code <> '' then
		return query select 46::integer, ('Thông tin không chính xác, mặt hàng combo ' || _item_code || ' không có mặt hàng con') :: character varying;
		return;
	end if;
	
	--------------------- trong trường hợp document đã close thì không được update
	if _transaction_type = 'U' and exists(select 1 from _temp_current_doc where doc_status = 'C') then
		return query select 10506 :: integer, 'Closed Document cannot be updated' :: character varying;
		return;
	end if;

	update _temp_doc set table_id = null where coalesce(table_id, 0) < 0;
	_table_id = coalesce((select table_id From _temp_doc), 0 );
	if _table_id > 0 and not exists(select 1 from APZ_OTMD where code = cast(_table_id as character varying)) then
		return query select 10302::integer, ('Bàn ' || cast(_table_id as character varying) || ' không tồn tại trên hệ thống') :: character varying;
		return;
	end if;
	
	update _temp_lines set open_qty = quantity where _transaction_type = 'A';
	update _temp_lines set line_status = 'O' where coalesce(line_status, '') = '';
	update _temp_lines a set ship_date = b.doc_due_date from _temp_doc b where a.ship_date is null;
	update _temp_lines set ugp_entry = -1 where coalesce(ugp_entry,0) = 0;
	--update a NumPerMsr = b.AltQty from "#tempLines" a inner join APZ_UGP1 b ON a.UgpEntry = b.UgpEntry and a.UomEntry = b.UomEntry
	update _temp_lines set num_per_msr = 1 where coalesce(num_per_msr, 0) = 0;
	update _temp_lines set inv_qty = quantity * num_per_msr, open_inv_qty = open_qty * num_per_msr;
	update _temp_doc set doc_entry = _doc_entry, doc_num = _doc_entry;
	update _temp_lines set doc_entry = _doc_entry;
	update _temp_lines a set line_status = 'C' from _temp_doc b where b.doc_status <> 'O';
	update _temp_lines set line_status = 'C' where open_inv_qty = 0;
	update _temp_doc set doc_status = 'C' where not exists(select 1 from _temp_lines where line_status = 'O');

	update _temp_lines set base_type = -1 where base_type is null;

	if _object_type = '67' and _transaction_type = 'A' then
		update a set price_bef_di = b.avg_price from _temp_lines a inner join apz_oitw b On a.item_code = b.item_code and a.from_whs_cod = b.whs_code;
	end if;

	-------------tính toán lại giá trên hệ thống --------------------------------------------------
	---- nếu chưa có cả 3 loại giá thì tính giá theo total discount và vat
	update _temp_lines set price_af_vat = line_total / quantity where coalesce(price_bef_di,0) = 0 and coalesce(price,0) = 0 and coalesce(price_af_vat,0) = 0;
	----- ưu tiên tính toán lại giá theo giá trước discount ------------------------------------
	update _temp_lines set price = price_bef_di * (1 - coalesce(disc_prcnt, 0)/100)
						 , price_af_vat = price_bef_di * (1 - coalesce(disc_prcnt, 0)/100) * (1 + coalesce(vat_prcnt, 0)/100) where coalesce(price_bef_di,0) <> 0;
						 
	---- nếu không có giá trước dis_count thì tính theo giá sau dis_count
	update _temp_lines set price_bef_di = case when coalesce(disc_prcnt, 0) = 100 then 0 else price / (1 - coalesce(disc_prcnt, 0)/100) end
						 , price_af_vat = price * (1 + coalesce(vat_prcnt, 0)/100) where coalesce(price_bef_di,0) = 0 and coalesce(price,0) <> 0;
						 
	---- nếu không có giá trước dis count và sau dis count thì tính theo giá sau thuế
	update _temp_lines set price_bef_di = case when coalesce(disc_prcnt, 0) = 100 or coalesce(vat_prcnt, 0) = -100 then 0 else  price_af_vat / ((1 - coalesce(disc_prcnt, 0)/100) * (1 + coalesce(vat_prcnt, 0)/100)) end
						 , price = case when coalesce(vat_prcnt, 0) = -100 then price_af_vat / (1 + coalesce(vat_prcnt, 0)/100) end
			where coalesce(price_bef_di,0) = 0 and coalesce(price,0) = 0 and coalesce(price_af_vat,0) <> 0;
	--------------------- end tính toán giá -----------------------------------------------------
	update _temp_lines set total_bef_v = quantity * price;
	_total_bef_v = (select sum(coalesce(total_bef_v,0)) from _temp_lines);
	update _temp_doc set disc_prcnt = case when _total_bef_v <> 0 then (coalesce(disc_sum,0) * 100) / _total_bef_v else 0 end;
	_discnt_per = coalesce((select disc_prcnt from _temp_doc), 0);
	update _temp_lines set line_total = total_bef_v * (1 - _discnt_per / 100) * (1 + coalesce(vat_prcnt,0) / 100)
							, line_total_fc = total_bef_v * (1 - _discnt_per / 100) * (1 + coalesce(vat_prcnt,0) / 100) * rate;
	update _temp_lines set line_vat = total_bef_v * coalesce(vat_prcnt, 0)/100, line_vatl_f = total_bef_v * (coalesce(vat_prcnt, 0)/100) * coalesce(rate, 0)/100;
	update _temp_doc set vat_sum = (select Sum(coalesce(line_vat,0)) from _temp_lines)
						, vat_sum_fc = (select Sum(coalesce(line_vatl_f,0)) from _temp_lines)
						, doc_total = (select Sum(coalesce(line_total,0)) from _temp_lines) * (1 - coalesce(disc_prcnt,0)/100)
						, doc_total_fc = (select Sum(coalesce(line_total_fc,0)) from _temp_lines) * (1 - coalesce(disc_prcnt,0)/100);

	----------------------------------------------- end update thông tin mặc định ------------------------------------------------
	
	------------------------------------------ validate các trường không được update -----------------------------------
	if _transaction_type = 'U' then
		if exists(select 1 from _temp_doc a, _temp_current_doc b where a.card_code <> b.card_code) then
			return query select 10308 :: integer, 'Cannot update Business Partner' :: character varying;
			return;
		end if;
		
		if exists(select 1 from _temp_doc a, _temp_current_doc b where a.company <> b.company) then
			return query select 10309 :: integer, 'Cannot change company of document' :: character varying;
			return;
		end if;
		
		if exists(select 1 from _temp_doc a, _temp_current_doc b where a.doc_date <> b.doc_date) then
			return query select 10316 :: integer, 'Cannot change Posting Date of document' :: character varying;
			return;
		end if;
		
		if exists(select 1 from _temp_doc a, _temp_current_doc b where a.doc_cur <> b.doc_cur) then
			return query select 10319 :: integer, 'Cannot change currency of document lines' :: character varying;
			return;
		end if;
		
		if exists(select 1 from aplus_document_objectaction(_object_type, 0, '') where onhand_type in (2, 1, 6)) then
			if exists(select 1 from _temp_doc a, _temp_current_doc b where a.vat_group <> b.vat_group) then
				return query select 10310 :: integer, 'Cannot change VAT of document' :: character varying;
				return;
			end if;
			
			if exists(select 1 from _temp_doc a, _temp_current_doc b where a.vat_group <> b.vat_group) then
				return query select 10310 :: integer, 'Cannot change VAT of document' :: character varying;
				return;
			end if;
			
			if exists(select 1 from _temp_doc a, _temp_current_doc b where a.vat_percent <> b.vat_percent) then
				return query select 10311 :: integer, 'Cannot change VAT Percent of document' :: character varying;
				return;
			end if;
			
			if exists(select 1 from _temp_doc a, _temp_current_doc b where a.disc_prcnt <> b.disc_prcnt) then
				return query select 10312 :: integer, 'Cannot change Discount Percent of document' :: character varying;
				return;
			end if;
			
			if exists(select 1 from _temp_doc a, _temp_current_doc b where a.doc_due_date <> b.doc_due_date) then
				return query select 10317 :: integer, 'Cannot change Due Date of document' :: character varying;
				return;
			end if;
			
			if exists(select 1 from _temp_doc a, _temp_current_doc b where a.doc_rate <> b.doc_rate) then
				return query select 10320 :: integer, 'Cannot change document rate' :: character varying;
				return;
			end if;
			
			if exists(select 1 from _temp_lines a left join _temp_current_lines b on a.line_num = b.line_num where b.line_num is null)
					or exists(select 1 from _temp_current_lines a left join _temp_lines b on a.line_num = b.line_num where b.line_num is null) then
				return query select 10360 :: integer, 'Cannot add or delete line of inventory effect document' :: character varying;
				return;
			END IF;
			_is_valid_data = 'Y';
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  			where a.item_code <> b.item_code and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10313 :: integer, 'Cannot change items of document' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
					  where (a.base_type <> b.base_type or a.base_entry <> b.base_entry or a.base_line = b.base_line) and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10314 :: integer, 'Cannot change linked data of document' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.quantity <> b.quantity and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10315 :: integer, 'Cannot change item quantity of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.ship_date <> b.ship_date and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10318 :: integer, 'Cannot change shipping Date of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.price <> b.price and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10321 :: integer, 'Cannot change price of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.currency <> b.currency and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10322 :: integer, 'Cannot change currency of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  	where a.rate <> b.rate and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10323 :: integer, 'Cannot change rate of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.disc_prcnt <> b.disc_prcnt and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10324 :: integer, 'Cannot change discount percent of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.whs_code <> b.whs_code and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10325 :: integer, 'Cannot change warehouse of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.tree_type <> b.tree_type and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10326 :: integer, 'Cannot change Tree Type of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.vat_group <> b.vat_group and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10327 :: integer, 'Cannot change Vat Group of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.vat_prcnt <> b.vat_prcnt and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10328 :: integer, 'Cannot change Vat Percent of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where (a.unit_msr <> b.unit_msr or a.ugp_entry <> b.ugp_entry or a.uom_code <> b.uom_code) and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10329 :: integer, 'Cannot change item unit of document lines' :: character varying;
			return;
		end if;
			
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.num_per_msr <> b.num_per_msr and (_is_valid_data = 'Y' or b.line_status <> 'O' or b.inv_qty > b.open_inv_qty)) then
			return query select 10330 :: integer, 'Cannot change unit convert rate of document lines' :: character varying;
			return;
		end if;
		
		if exists(select 1 from _temp_lines a inner join _temp_current_lines b on a.line_num = b.line_num
				  		where a.line_status <> b.line_status and b.line_status <> 'O') then
			return query select 103337 :: integer, 'Cannot change status of closed line' :: character varying;
			return;
		end if;
	end if;
	------------------------------------------------ end validate update data --------------------------------------------------------
	
	--------------------------- kiểm tra dữ liệu copy từ base data ----------------------------------------
	if exists(select 1 from _temp_lines a left join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
			 					where a.base_type <> -1 and b.obj_type is null) then
		return query select 10331 :: integer, 'Linked document does not exists' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
			 					where a.item_code <> b.item_code) then
		return query select 10332 :: integer, 'Linked item cannot be changed' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
			 					where a.whs_code <> b.whs_code) then
		return query select 10333 :: integer, 'Linked warehouse cannot be changed' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_lines a inner join _temp_base_data b on a.base_type = b.obj_type and a.base_entry = b.doc_entry and a.base_line = b.line_num
			 					where (a.unit_msr <> b.unit_msr or a.ugp_entry <> b.ugp_entry or a.uom_code <> b.uom_code)) then
		return query select 10334 :: integer, 'Linked unit cannot be changed' :: character varying;
		return;
	end if;
	-------------------------------------- end validate copy data ---------------------------------------
	
	return query select 200 :: integer, cast(_doc_entry as character varying);
	
END$BODY$;