# 00 OBJECT INFO
**Attachment Table**: `apz_atc1`  

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                                  |
|-------------|------------|------------|-------------|--------------------------|---------------|------------------------------------------|
| doc_entry   | int        |            | Y           | link apz_oatc            |               | Id danh sách attachment                  |
| line_num    | int        |            | Y           |                          |               | Line Id                                  |
| src_path    | string     | max        | N           |                          |               | Đường dẫn folder lấy file                |
| trgt_path   | string     | max        | N           |                          |               | Đường dẫn folder đích                    |
| file_type   | string     | 200        | N           |                          |               | Loại file                                |
| file_name   | string     | 254        | N           |                          |               | Tên file                                 |
| file_ext    | string     | 8          | N           |                          |               | file extension                           |
| user_sign   | smallint   |            | N           | link apz_ousr            |               | người tạo                                |
| create_date | date       |            | N           |                          |               | Ngày tạo                                 |
| copied      | string     | 1          | N           | * Y: Yes<br>* N: No      | N             | đã copy                                  |
| override    | string     | 1          | N           | * Y: Yes<br>* N: No      | N             | Được ghi đè                              |
| sub_path    | string     | max        | N           |                          |               | Đường dẫn phụ                            |
| free_text   | string     | 254        | N           |                          |               | Ghi chú                                  |
| uri         | string     | max        | N           |                          |               | Link trong trường hợp lưu cloud          |
| dwl_uri     | string     | max        | N           |                          |               | Đường dẫn tải file trong trường hợp lưu cloud |
| file_id     | string     | 200        | N           |                          |               | Id file khi lưu cloud                    |
