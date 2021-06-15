CREATE OR REPLACE FUNCTION Aplus_Payment_Cancel
(
	_obj_type character varying(20),
	_doc_entry int
)
RETURNS TABLE(msg_code int, message character varying(254))
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
	if not exists(select 1 from apz_orct where obj_type = _obj_type and doc_entry = _doc_entry) then
		return query select 10000 :: int, 'Dữ liệu phiếu không tồn tại trên hệ thống' :: character varying;
		return;
	end if;

	if exists(select 1 from apz_orct where obj_type = _obj_type and doc_entry = _doc_entry and coalesce(canceled, 'N') <> 'N') then
		return query select 10358 :: int, 'Phiếu đã bị hủy. Không thể tiếp tục hủy' :: character varying;
		return;
	end if;

	if exists(select 1 from apz_orct where obj_type = _obj_type and doc_entry = _doc_entry and doc_status <> 'O') then
		return query select 10306 :: int, 'Phiếu đã đóng. Không thể hủy phiếu' :: character varying;
		return;
	end if;

	update apz_orct set canceled = 'Y', doc_status = 'C' where doc_entry = _doc_entry;
	update apz_rct1 set line_status = 'C' where doc_entry = _doc_entry;

	------------lấy thông tin base document
	create temp table _temp_base as (select base_type, base_entry from apz_rct2 where doc_entry = _doc_entry group by base_entry, base_type);

	------------------- Reopen các phiếu shipcode đã bị close
	update a set line_status = 'O' from apz_rct1 a inner join apz_rct2 b ON a.doc_entry = b.doc_entry inner join _temp_base c ON b.base_type = c.base_type and b.base_entry = c.base_entry where a.pay_mth = 'COD';

	update a set doc_status = 'O' from apz_orct a where exists(select 1 from apz_rct1 a inner join apz_rct2 b ON a.doc_entry = b.doc_entry 
																						inner join _temp_base c ON b.base_type = c.base_type and b.base_entry = c.base_entry 
															   							where a.doc_entry = b.doc_entry and a.pay_mth = 'COD');

	---- update status invoice payment
	update a set pmt_status = 'N' from apz_oinv a inner join _temp_base b on a.doc_entry = b.base_entry and b.base_type = '13';

	---- update status Return payment
	update a set pmt_status = 'N' from apz_ordn a inner join _temp_base b on a.doc_entry = b.base_entry and b.base_type = '16';

	---- update status purchase payment
	update a set pmt_status = 'N' from apz_opch a inner join _temp_base b on a.doc_entry = b.base_entry and b.base_type = '18';

	---- update status purchase Return payment
	update a set pmt_status = 'N' from apz_orpd a inner join _temp_base b on a.doc_entry = b.base_entry and b.base_type = '21';

	return query select 0 :: int, _doc_entry :: character varying;

END $BODY$