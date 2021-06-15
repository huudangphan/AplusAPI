# 00 OBJECT INFO
**Object Type**: `147065`
**Form Table**: `whs_counting`

| Column Name  | Data Typpe | Max Length | Is Required | List Value/ Linked Table                 | Default Value | Remarks                                  |
|--------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| doc_entry    | int        |            | Y           |                                          |               | id                                       |
| doc_num      | int        |            | Y           |                                          |               | document number                          |
| series       | int        |            | N           |                                          |               | quy tắc đánh số. hiện chưa dùng          |
| count_date   | date       |            | N           |                                          |               | ngày kiểm kê                             |
| count_time   | smallint   |            | N           |                                          |               | thời gian kiểm kê                        |
| count_type   | string     | 1          | N           | * S: Single Counter<br>* M: Mutil Counter | S             | loại kiểm kê                             |
| taker1_type  | int        |            | N           | * 12: user                               | 12            | loại user kiểm kê                        |
| taker1_id    | int        |            | N           | type 12: link user_info                  |               | id người kiểm kê 1                       |
| taker2_type  | int        |            | N           | * 12: user                               | 12            | loại user kiểm kê                        |
| taker2_id    | int        |            | N           | type 12: link user_info                  |               | id người kiểm kê 2                       |
| doc_status   | string     | 1          | N           | * O: open<br>* C: close                  | O             | trạng thái phiếu                         |
| ref1         | string     | 254        | N           |                                          |               | reference string                         |
| remarks      | string     | 254        | N           |                                          |               | ghi chú                                  |
| user_sign    | int        |            | N           | link user_info                           |               | người tạo phiếu                          |
| create_date  | date       |            | N           |                                          |               | ngày tạo phiếu                           |
| create_time  | smallint   |            | N           |                                          |               | thời gian tạo phiếu                      |
| user_sign2   | int        |            | N           | link user_info                           |               | người update                             |
| update_date  | date       |            | N           |                                          |               | ngày cập nhật                            |
| update_time  | smallint   |            | N           |                                          |               | thời gian cập nhật                       |
| obj_type     | string     | 20         | N           | 147065                                   | 147065        | giá trị object type của chức năng: 147065 |
| wdd_status   | string     | 1          | N           | * A: approval<br>* N: not approval       | N             | trạng thái approval. Khi trạng thái này = A thì mới được thực hiện các action điều chỉnh kho |
| printed      | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | trạng thái in của phiếu                  |
| company      | string     | 50         | N           | link: company                            |               | công ty                                  |
| diff_qty     | numeric    | 19,6       | N           |                                          |               | Tổng số lượng sai khác trên tất cả line  |
| diff_percent | numeric    | 19,6       | N           |                                          |               | phần trăm sai khác                       |
| post_date    | date       |            | N           |                                          |               | Ngày thực hiện                           |
| atc_entry    | int        |            | N           | link attachment                          |               | attachment                               |

**Setting Table**: `whs_counting_item`

| Column Name  | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                                  |
|--------------|------------|------------|-------------|--------------------------|---------------|------------------------------------------|
| doc_entry    | int        |            | Y           | linked whs_counting      |               | id phiếu                                 |
| line_num     | int        |            | Y           |                          |               | line id                                  |
| item_code    | string     | 50         | Y           | link item                |               | mã hàng                                  |
| item_name    | string     | 254        | N           |                          |               | tên hàng                                 |
| freeze       | string     | 1          | N           | * Y: Yes<br>* N: No      | N             | Trạng thái đóng băng. nếu bằng Y thì không điều chỉnh kho khi có action tương ứng |
| whs_code     | string     | 50         | Y           | link warehouse           |               | kho kiểm kê                              |
| in_qty       | numeric    | 19,6       |             |                          |               | số lượng tồn trên hệ thống               |
| counted      | string     | 1          | N           | * Y: yes<br>* N: no      | N             | trạng thái kiểm kê                       |
| count_qty    | numeric    | 19,6       | N           |                          |               | số lương đếm thực tế                     |
| remarks      | string     | 254        | N           |                          |               | ghi chú                                  |
| bar_code     | string     | 254        | N           |                          |               | Barcode item                             |
| inv_uom      | string     | 1          | N           | * Y: Yes<br>* N: No      | Y             | kiểm kê bằng đơn vị lưu kho              |
| difference   | numeric    | 19,6       | N           |                          |               | Số lượng sai khác                        |
| diff_percent | numeric    | 19,6       | N           |                          |               | phần trăm sai khác                       |
| count_date   | date       |            | N           |                          |               | Ngày kiểm kê                             |
| count_time   | smallint   |            | N           |                          |               | thời gian kiểm kê                        |
| project_code | string     | 50         | N           |                          |               | dự án. chưa dùng                         |
| ocr_code     | string     | 50         | N           |                          |               | dimention 1. chưa dùng                   |
| ocr_code2    | string     | 50         | N           |                          |               | dimention 2                              |
| ocr_code3    | string     | 50         | N           |                          |               | dimention 3                              |
| ocr_code4    | string     | 50         | N           |                          |               | dimention 4                              |
| ocr_code5    | string     | 50         | N           |                          |               | dimention 5                              |
| line_status  | string     | 1          | N           | * O: Open<br>* N: Close  | O             | trạng thái của line                      |
| bin_entry    | int        |            | N           |                          |               | id của bin. Khi kho quản lý theo bin. Chưa dùng |
| vis_order    | int        |            | N           |                          |               | thứ tự hiển thị                          |
| ugp_entry    | int        |            | N           | link unit_group          |               | Mã nhóm đơn vị tính                      |
| i_uom_code   | string     | 50         | N           | link unit                |               | mã đơn vị lưu kho                        |
| uom_code     | string     | 50         | N           | link unit                |               | mã đơn vị kiểm kê                        |
| num_per_msr  | numeric    | 19,6       | N           |                          |               | tỉ lệ quy đổi đơn vị kiểm kê => đơn vị lưu kho |
| inv_qty      | numeric    | 19,6       | N           |                          |               | số lượng kiểm kê theo đơn vị lưu kho     |
| unit_msr     | string     | 254        | N           |                          |               | Tên đơn vị tính                          |
| item_cost    | numeric    | 19,6       | N           |                          |               | item cost lấy từ bảng item_warehouse tại thời điểm kiểm kê |
| user_sign    | int        |            | N           | link user_info           |               | người tạo                                |
| create_date  | date       |            | N           |                          |               | Ngày tạo                                 |
| create_time  | smallint   |            | N           |                          |               | thời gian tạo                            |
| user_sign2   | int        |            | N           | link user_info           |               | người cập nhật                           |
| update_date  | date       |            | N           |                          |               | ngày cập nhật                            |
| update_time  | smallint   |            | N           |                          |               | thời gian cập nhật                       |
