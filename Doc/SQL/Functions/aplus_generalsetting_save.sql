create or replace function aplus_generalsetting_save(_user_sign integer)
	returns table (msg_code integer, message character varying(254))
	language 'plpgsql'
AS $BODY$
declare 
	_neg_whs character varying(1);
	_system_currency character varying(10);
	_main_currency character varying(10);
	_sum_dec smallint;
	_qty_dec smallint;
	_price_dec smallint;
	_rate_dec smallint;
	_percent_dec smallint;
	_measure_dec smallint;
BEGIN
	-----------kiểm tra dữ liệu đồng tiền trên hệ thống
	if not exists(select 1 from _general_setting a inner join currency b on a.main_currency = b.code)
				or not exists(select 1 from _general_setting a inner join currency b on a.sys_currency = b.code) then
		return query select 10103 :: integer, N'Đồng tiền thiết lập không tồn tại trên hệ thống' :: character varying;
		return;
	END IF;
	
	-------------- kiểm tra dữ liệu thiết lập số chữ số thập phân
	if exists(select 1 from _general_setting where sum_dec not between 0 and 6 or qty_dec not between 0 and 6 or
			 								price_dec not between 0 and 6 or rate_dec not between 0 and 6 or
			 								percent_dec not between 0 and 6 or measure_dec not between 0 and 6) then
		return query select 10104 :: integer, N'Thiết lập số chữ số thập phân không chính xác' :: character varying;
		return;
	end if;
	
	----------- kiểm tra dữ liệu format time
	if exists(select 1 from _general_setting where coalesce(time_format, 'HHmm') not in ('HHmm','hhmm')) then
		return query select 10106 :: integer, N'Dữ liệu thiết lập định dạng giờ không chính xác' :: character varying;
		return;
	END IF;
	
	----------- kiểm tra dữ liệu date time
	if exists(select 1 from _general_setting where coalesce(date_format, 'yyyyMMdd') not in ('yyyyMMdd', 'yyyyddMM', 'ddMMyyyy', 'MMddyyyy'
																						,'yyyyMMMMdd', 'yyyyddMMMM', 'ddMMMMyyyy', 'MMMMddyyyy'
																						, 'yyMMdd', 'yyddMM', 'ddMMyy', 'MMddyy'
																						, 'yyMMMMdd', 'yyddMMMM', 'ddMMMMyy', 'MMMMddyy')) then
		return query select 10105 :: integer, N'Dữ liệu thiết lập định dạng ngày tháng không chính xác' :: character varying;
		return;
	END IF;
	
	----------- kiểm tra dữ liệu thiết lập các dấu phân cách
	if exists(select 1 from _general_setting where coalesce(date_sep, '') = '' or coalesce(dec_sep, '') = '' or coalesce(thous_sep, '') = '') then
		return query select 10200 :: integer, N'Dữ liệu thiết lập dấu phân cách đơn vị hoặc ngày tháng không chính xác' :: character varying;
		return;
	END IF;
	
	----------- kiểm tra dữ liệu thiết lập khách hàng mặc định
	if not exists(select 1 from _general_setting a inner join partner b on coalesce(a.def_cust,'') = '' or a.def_cust = b.card_code) then
		return query select 10107 :: integer, N'Khách hàng mặc định không tồn tại trên hệ thống' :: character varying;
		return;
	END IF;
	
	---update các giá trị mặc định của hệ thống khi giá trị trường = null
	update _general_setting set is_shift = 'N' where coalesce(is_shift, '') = '';
	update _general_setting set is_aut_prt_bill = 'N' where coalesce(is_aut_prt_bill, '') = '';
	update _general_setting set dis_cod = 'N' where coalesce(dis_cod, '') = '';
	update _general_setting set dis_card = 'N' where coalesce(dis_card, '') = '';
	update _general_setting set dis_tranf = 'N' where coalesce(dis_tranf, '') = '';
	update _general_setting set chg_sal_prlist = 'N' where coalesce(chg_sal_prlist, '') = '';
	
	----------- kiểm tra dữ liệu thiết lập yes/no
	if not exists(select 1 from _general_setting where coalesce(neg_whs, '') not in ('N','Y') or is_shift not in ('N','Y') or is_aut_prt_bill not in ('N','Y')
				 							or dis_cod not in ('N','Y') or dis_card not in ('N','Y') or dis_tranf not in ('N','Y')
				 							or chg_sal_prlist not in ('N','Y')) then
		return query select 10108 :: integer, N'Giá trị thiết lập không đúng. Các giá trị là Y/N' :: character varying;
		return;
	END IF;
		
	--lấy dữ liệu cũ để check update
	select main_currency, sys_currency, sum_dec, qty_dec, price_dec, rate_dec, percent_dec, measure_dec, neg_whs from general_setting 
				into _main_currency, _system_currency, _sum_dec, _qty_dec, _price_dec, _rate_dec, _percent_dec, _measure_dec, _neg_whs;
				
	------------------- kiểm tra setting âm kho. Nếu đang cho phép âm kho mà sửa thành không được phép âm thì tồn kho của các item trên tất cả các kho phải >= 0
	if exists(select 1 from _general_setting where neg_whs = 'Y' and _neg_whs = 'N') and exists(select 1 from item_warehouse where on_hand < 0) THEN
		return query select 10356 :: integer, N'Kho đang âm không thể thay đổi thiết lập cho phép âm kho' :: character varying;
		return;
	END IF;

	if exists(select 1 from partner) or exists(select 1 from item)
	THEN
		------------------ kiểm tra thay đổi thiết lập đồng tiền khi đã tồn
		if exists(select 1 from _general_setting where main_currency != _main_currency or sys_currency != _system_currency) then
			return query select 10201 :: integer, N'Chỉ có thể thiết lập đồng tiền khi hệ thống chưa có phát sinh nghiệp vụ' :: character varying;
			return;
		end if;
		------------------- kiểm tra dữ liệu setting các chữ số thập phân không được ít hơn các thiết lập đã có
		if exists(select 1 from general_setting where sum_dec < _sum_dec or qty_dec < _qty_dec or price_dec < _price_dec or rate_dec < _rate_dec 
						or percent_dec < _percent_dec or measure_dec < _measure_dec) THEN
			return query select 10202 :: integer, N'thiết lập chữ số thập phân không được ít hơn dữ liệu đã thiết lập' :: character varying;
			return;
		END if;
	END if;
	
	------- update lại các dữ liệu không được thay đổi
	update _general_setting a set "domain" = b."domain", "version" = b."version", create_date = b.create_date, create_time = b.create_time
						, company_type = b.company_type, user_sign = _user_sign, update_date = current_date, update_time = cast(to_char(now(), 'HHMI') as integer)
					from general_setting b;
				
	----- xóa dữ liệu cũ và update dữ liệu mới vào hệ thống
	delete from general_setting;
	insert into general_setting select * from _general_setting;

	return query select 200 :: integer,'' :: character varying;

END; $BODY$