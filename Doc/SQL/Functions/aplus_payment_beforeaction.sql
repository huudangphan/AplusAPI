CREATE OR REPLACE FUNCTION Aplus_Payment_BeforeAction
(
	_obj_type character varying(20),
	_tran_type character(1),
	_doc_entry int,
	_user_sign int,
	_branch int
)
RETURNS TABLE(msg_code int, message character varying(254))
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE _group_type character varying(20);
		_card_code character varying(50);
		_current_date date = current_date;
		_current_time smallint = to_char(current_timestamp, 'HHMI');
		_currency character varying(3) = (select main_curncy from apz_cinf);
		_sum_total numeric(19,6);
BEGIN
	----------------------------------------------------------- VALIDATE THÔNG TIN -----------------------------------------------------------------------
	------------------ Xóa các line không có item
	delete from _temp_item where coalesce(base_type,'') = '';
	delete from _temp_lines where coalesce(line_total, 0) = 0;

	update _temp_doc set obj_type = _obj_type;
	update _temp_doc set branch = _branch where coalesce(branch, 0) = 0;

	------------------ kiểm tra số lượng bản ghi header
	if (select count(1) from _temp_doc) > 1 then
		return query select 10000 :: int, 'Dữ liệu phiếu không đúng. Số bản ghi cung cấp nhiều hơn 1' :: character varying;
		return;
	end if;

	----------------------- kiểm tra tồn tại dữ liệu document để lưu
	if not exists(select 1 from _temp_doc) or not exists(select 1 from _temp_lines) then
		return query select 10003 :: int, 'Dữ liệu phiếu không được cung cấp' :: character varying;
		return;
	end if;

	------------------------------------------Kiểm tra thông tin chi nhánh
	if not exists(select 1 from _temp_doc a inner join apz_ostr b On a.branch = b.store_code) then
		return query select 10338 :: int, 'Thông tin chi nhánh không chính xác' :: character varying;
		return;
	end if;
	
	------------------------------------------------ khiểm tra thông tin đối tượng nhận -----------------------------------

	update _temp_doc set group_type = '9999' where coalesce(group_type, '') = '';

	select _group_type = group_type, _card_code = card_code from _temp_doc;
	
	if _group_type = '1001' then
		if not exists(select 1 from apz_ocrd where card_code = _card_code and card_type = 'C') then
			return query select 10140 :: int, 'Dữ liệu khách hàng chưa được cung cấp' :: character varying;
			return;
		end if;
		
		if not exists(select 1 from apz_ocrd where card_code = _card_code and card_type = 'C' and valid_for = 'Y') then
			return query select 10339 :: int, 'Khách hàng đã bị vô hiệu hóa. Vui lòng chọn khách hàng khác' :: character varying;
			return;
		end if;
	elseif _group_type = '1002' then
		if not exists(select 1 from apz_ocrd where card_code = _card_code and card_type = 'S') then
			return query select 10359 :: int, 'Dữ liệu nhà cung cấp chưa được cung cấp' :: character varying;
			return;
		end if;
		
		if not exists(select 1 from apz_ocrd where card_code = _card_code and card_type = 'S' and valid_for = 'Y') then
			return query select 10339 :: int, 'Nhà cung cấp đã bị vô hiệu hóa. Vui lòng chọn nhà cung cấp khác' :: character varying;
			return;
		end if;

	elseif _group_type = '1003' then
		if not exists(select 1 from apz_ousr where user_code = _card_code) then
			return query select 10101 :: int, 'Nhân viên chưa được cung cấp' :: character varying;
			return;
		end if;
	
		if not exists(select 1 from apz_ousr where user_code = _card_code and is_active = 'Y') then
			return query select 10100 :: int, 'Nhân viên đã bị khóa. Vui lòng chọn một nhân viên khác' :: character varying;
			return;
		end if;

	elseif _group_type = '1004' then
		if not exists(select 1 from apz_ospu where sp_code = _card_code) then
			return query select -1 :: int, 'Đối tác vận chuyển chưa được cung cấp' :: character varying;
			return;
		end if;

		if not exists(select 1 from apz_ospu where sp_code = _card_code and status = 'Y') then
			return query select -1 :: int, 'Đối tác vận chuyển đã bị vô hiệu hóa. Vui lòng chọn đối tác khác' :: character varying;
			return;
		end if;
	end if;
	------------------------------------------------- END kiểm tra thông tin khách hàng ------------------------------------

	if not exists(select 1 from _temp_doc where obj_type in ('810018', '810019')) then
		return query select 10420 :: int, 'Hình thức phiếu chưa được cung cấp' :: character varying;
		return;
	end if;
	

	if exists(select 1 from _temp_doc a left join apz_opmg b ON a.doc_type = b.code and a.obj_type = b.obj_type where b.code isnull) then
		return query select -1 :: int, 'Loại phiếu chưa được cung cấp' :: character varying;
		return;
	end if;
	

	---------------------- Kiểm tra thông tin base document
	
	if exists(select 1 from _temp_item a left join apz_oinv b ON a.base_entry = b.doc_entry where a.base_type = '13' and b.doc_entry isnull) then
		return query select -1 :: int, 'Dữ liệu phiếu bán hàng không tồn tại trên hệ thống' :: character varying;
		return;
	end if;
	

	if exists(select 1 from _temp_item a left join apz_ordn b ON a.base_entry = b.doc_entry where a.base_type = '16' and b.doc_entry isnull) then
		return query select -1 :: int, 'Dữ liệu phiếu trả hàng không tồn tại trên hệ thống' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_item a left join apz_opch b ON a.base_entry = b.doc_entry where a.base_type = '18' and b.doc_entry isnull) then
		return query select -1 :: int, 'Dữ liệu phiếu nhập hàng không tồn tại trên hệ thống' :: character varying;
		return;
	end if;
	
	if exists(select 1 from _temp_item a left join apz_orpd b ON a.base_entry = b.doc_entry where a.base_type = '21' and b.doc_entry isnull) then
		return query select -1 :: int, 'Dữ liệu phiếu trả hàng nhà cung cấp không tồn tại trên hệ thống' :: character varying;
		return;
	end if;
	

	----------------------------------- END kiểm tra base

	----------------------------------- Kiểm tra thông tin payment

	if exists(select 1 from _temp_lines a left join apz_opmt b ON a.pay_mth = b.pay_mth where b.pay_mth isnull) then
		return query select -1 :: int, 'Phương thức thanh toán không đươc sử dụng trên hệ thống' :: character varying;
		return;
	end if;

	----------------------------------- END kiêm tra thông tin payment

	----------------------------------------------- update các thông tin mặc định ------------------------------------------------

	-------------- Lấy thông tin thời gian hiện taị


	update _temp_doc set doc_cur = _currency where coalesce(doc_cur, '') = '';
	update _temp_lines set currency = _currency where coalesce(currency, '') = '';

	update _temp_doc set user_sign = _user_sign where coalesce(user_sign, 0) = 0;
	update _temp_lines set user_sign = _user_sign where coalesce(user_sign, 0) = 0;
	update _temp_item set user_sign = _user_sign where coalesce(user_sign, 0) = 0;

	update _temp_doc set doc_date = _current_date where doc_date isnull;
	update _temp_doc set doc_due_date = _current_date where doc_due_date isnull;
	update _temp_doc set doc_time = _current_time where doc_time isnull;
	update _temp_doc set doc_rate = 1;
	update _temp_doc set canceled = 'N' where coalesce(canceled, '') = '';
	update _temp_doc set doc_status = 'O' where coalesce(doc_status, '') = '';

	if _tran_type = 'A' then
			update _temp_doc set create_date = _current_date;
	else 
		update _temp_doc set update_date = _current_date, create_date = (select create_date from apz_orct where doc_entry = _doc_entry);
	end if;
	
	update _temp_lines set line_status = 'O' where coalesce(line_status, '') = '';
	if exists(select 1 from _temp_doc where doc_status = 'C') then
		update _temp_lines set line_status = 'C';
	end if;
	
	update _temp_lines set create_date = _current_date where create_date isnull;

	_sum_total = coalesce((select sum(line_total) from _temp_lines), 0);

	update _temp_doc set doc_entry = _doc_entry, doc_num = _doc_entry, doc_total = _sum_total;
	update _temp_lines set doc_entry = _doc_entry;
	update _temp_item set doc_entry = _doc_entry, total_payment = _sum_total;
	----------------------------------------------- end update thông tin mặc định ------------------------------------------------
	
	return query select 0 :: int, _doc_entry :: character varying;

END $BODY$