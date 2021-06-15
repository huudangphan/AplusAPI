create table payment_base
(
	doc_entry int not null,
	line_num int not null,
	base_entry int null,
	base_type character varying(20) null,
	inv_total numeric(19, 6) null,
	currency character varying(3) null,
	total_payment numeric(19, 6) null,
	ocr_code character varying(8) null,
	ocr_code2 character varying(8) null,
	ocr_code3 character varying(8) null,
	ocr_code4 character varying(8) null,
	ocr_code5 character varying(8) null,
	comments character varying(254) null,
	user_sign smallint null,
	primary key(doc_entry, line_num)
)