create or replace function aplus_attachment_save(_docentry integer DEFAULT 0)
    returns  integer
    language plpgsql
as
$$
BEGIN

    ----kiểm tra dữ liệu docentry nếu chưa tồn tại thì khởi tạo id mới
    if coalesce(_docEntry,0) = 0 THEN
        _docEntry = coalesce((select Max(doc_entry) from attachment) , 0) + 1 ;
    END IF;

    --- cập nhật dữ liệu entry vào table con
    update _apz_atc1 set doc_entry = _docEntry;

    --xóa dữ liệu cũ
    delete from attachment where doc_entry = _docEntry;
    delete from attachment_item where doc_entry = _docEntry;

    ---insert dữu liệu vào table
    insert into attachment values (_docEntry);
    insert into attachment_item (select * from _apz_atc1);

    return _docEntry;

END;
$$;

