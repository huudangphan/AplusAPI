# 00 OBJECT INFO
**Object Type**:

| Object Type | Object Name                              |
|-------------|------------------------------------------|
| 13          | Sale Invoice                             |
| 13000       | Sale Reserve Invoice                     |
| 15          | Delivery                                 |
| 16          | Sale Return                              |
| 17          | Sale Order                               |
| 23          | Sale Quotation                           |
| 234000031   | Sale Return Request                      |
| 18          | Purchase Invoice                         |
| 25          | Purchase reserve Invoice                 |
| 20          | Good Receipt Po(nhập hàng từ nhà cung cấp) |
| 21          | Purchase Return                          |
| 22          | Purchase order                           |
| 234000032   | Purchase return request                  |
| 540000006   | Purchase quotation                       |
| 59          | Good receipt (nhập nội bộ)               |
| 60          | Good Issue (xuât nội bộ)                 |
| 67          | Inventory transfer                       |
| 1250000001  | Inventory transfer request               |

**Doc Table**:

| Column Name  | Data Typpe | Max Length | Is Required | List Value/ Linked Table                 | Default Value | Remarks                                  |
|--------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| doc_entry    | int        |            | Y           |                                          |               | Id phiếu                                 |
| doc_num      | int        |            |             |                                          |               | Số phiếu                                 |
| doc_type     | string     | 1          |             | * I : Item<br>* S : Service              | I             | loại phiếu                               |
| canceled     | string     | 1          |             | * N: No<br>* Y: Yes                      | N             | phiếu cancel                             |
| doc_status   | string     | 1          |             | * O: Open<br>* C: Close                  | O             | trạng thái của phiếu                     |
| obj_type     | string     | 20         |             | * valid theo danh sách object type       |               | object type của phiếu                    |
| doc_date     | date       |            | Y           |                                          |               | Ngày tạo phiếu                           |
| doc_time     | smallint   |            |             |                                          |               | Thời gian tạo phiếu                      |
| doc_due_date | date       |            | Y           |                                          |               | Ngày đến hạn                             |
| card_code    | string     | 50         |             | link apz_ocrd                            |               | mã đối tác kinh doanh                    |
| card_name    | string     | 254        |             |                                          |               | Tên đối tác kinh doanh                   |
| address      | string     | 254        |             |                                          |               | địa chỉ                                  |
| num_at_card  | string     | 254        |             |                                          |               | BP reference                             |
| vat_percent  | numeric    | 19,6       |             |                                          |               | phần trăm thuế                           |
| vat_sum      | numeric    | 19,6       |             |                                          |               | tổng thuế                                |
| vat_sum_fc   | numeric    | 19,6       |             |                                          |               | tổng thuế theo ngoại tệ                  |
| disc_prcnt   | numeric    | 19,6       |             |                                          |               | phần trăm giảm giá                       |
| disc_sum     | numeric    | 19,6       |             |                                          |               | tổng tiền giảm giá                       |
| disc_sum_fc  | numeric    | 19,6       |             |                                          |               | tổng tiền giảm giá theo ngoại tệ         |
| doc_cur      | string     | 3          |             | link apz_ocrn                            |               | đồng tiền của phiếu                      |
| doc_rate     | numeric    | 19,6       |             |                                          |               | tỉ lệ quy đổi từ ngoại tệ ra nội tệ      |
| doc_total    | numeric    | 19,6       |             |                                          |               | tổng tiền của phiếu                      |
| doc_total_fc | numeric    | 19,6       |             |                                          |               | tổng tiền của phiếu theo ngoại tệ        |
| ref1         | string     | 50         |             |                                          |               | reference info                           |
| comments     | string     | 254        |             |                                          |               | Ghi chú                                  |
| group_num    | int        |            |             | link apz_opln                            |               | bảng giá sử dụng                         |
| update_date  | date       |            |             |                                          |               | ngày cập nhật                            |
| create_date  | date       |            |             |                                          |               | ngày tạo phiếu trên hệ thống             |
| series       | int        |            |             |                                          | -1            | mã chuỗi số tự động(chưa dùng)           |
| tax_date     | date       |            |             |                                          |               | Ngày tính thuế                           |
| user_sign    | smallint   |            |             | link apz_ousr                            |               | id user tạo phiếu                        |
| wdd_status   | string     | 1          |             | * - : without<br>* W: Pending<br>* Y: Approved<br>* N: Reject<br>* C: Cancel | -             | trạng thái phê duyệt của phiếu           |
| draft_key    | int        |            |             | link apz_odrf                            |               | id phiếu draft trong trường hợp phiếu được tạo từ phiếu draft |
| total_expns  | numeric    | 19,6       |             |                                          |               | phí vận chuyển                           |
| total_exp_fc | numeric    | 19,6       |             |                                          |               | phí vận chuyển tính theo ngoại tệ        |
| ship_to_code | string     | 50         |             |                                          |               | Mã địa chỉ giao hàng                     |
| rounding     | string     | 1          |             | * Y: Yes<br>* N: No                      |               | trạng thái làm tròn                      |
| req_date     | date       |            |             |                                          |               | request date                             |
| cancel_date  | date       |            |             |                                          |               | Ngày hủy phiếu                           |
| project      | string     | 50         |             |                                          |               | dự án(chưa sử dụng)                      |
| from_date    | date       |            |             |                                          |               | Từ ngày(chưa dùng)                       |
| to_date      | date       |            |             |                                          |               | đến ngày(chưa dùng)                      |
| vat_date     | date       |            |             |                                          |               | ngày tính thuế                           |
| header       | string     | max        |             |                                          |               | header khi in phiếu                      |
| footer       | string     | max        |             |                                          |               | footer khi in phiếu                      |
| email        | string     | 100        |             |                                          |               | địa chỉ thư điện tử                      |
| notify       | string     | 1          |             | * N: No<br>* Y: Yes                      |               | hiện thông báo                           |
| atc_entry    | int        |            |             | link apz_oatc                            |               | attachment info                          |
| vat_group    | string     | 50         |             | link apz_ovtg                            |               | mã vat                                   |
| phone        | string     | 20         |             |                                          |               | số điện thoại                            |
| cus_cash     | numeric    | 19,6       |             |                                          |               | số tiền khách trả                        |
| cash_return  | numeric    | 19,6       |             |                                          |               | số tiền trả lại khách                    |
| ship_unit    | string     | 50         |             | link apz_ospu                            |               | đơn vị vận chuyển                        |
| ship_ref     | string     | 200        |             |                                          |               | shipping reference                       |
| exc_pee_per  | numeric    | 19,6       |             |                                          |               | phần trăm phí đổi hàng                   |
| exc_pee_sum  | numeric    | 19,6       |             |                                          |               | tổng tiền đổi hàng                       |
| whs_code     | string     | 50         |             | link apz_owhs                            |               | kho nhận hàng transfer hoặc transfer request |
| from_whs_cod | string     | 50         |             | link apz_owhs                            |               | kho lấy hàng transfer hoặc transfer request |
| table_id     | int        |            |             | link apz_otmd                            |               | table id                                 |
| disc_type    | string     | 1          |             | * P: Phần trăm<br>* C: tiền mặt          | P             | hình thức discount                       |
| company      | string     | 50         |             | link apz_obpl                            |               | công ty                                  |

**Line Table**:

| Column Name  | Data Typpe | Max Length | Is Required | List Value/ Linked Table                 | Default Value | Remarks                                  |
|--------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| doc_entry    | int        |            | Y           |                                          |               | Id phiếu                                 |
| line_num     | int        |            | Y           |                                          |               | Line id                                  |
| target_type  | int        |            |             | link list object                         | -1            | object của phiếu copy to                 |
| trget_entry  | int        |            |             |                                          |               | id của phiếu copy to                     |
| base_ref     | string     | 50         |             |                                          |               | doc_num của phiếu base                   |
| base_type    | int        |            |             |                                          | -1            | object type của phiếu base               |
| base_entry   | int        |            |             |                                          |               | Id của phiếu base                        |
| base_line    | int        |            |             |                                          |               | Line id của base line                    |
| line_status  | string     | 1          |             | * O : Open<br>* C : Close                | O             | trạng thái của line                      |
| item_code    | string     | 50         | Y           | link apz_oitm                            |               | mã mặt hàng                              |
| item_name    | string     | 254        |             |                                          |               | Tên mặt hàng                             |
| quantity     | numeric    | 19,6       | Y           |                                          |               | số lượng                                 |
| ship_date    | date       |            | Y           |                                          |               | ngày giao hàng                           |
| open_qty     | numeric    | 19,6       |             |                                          |               | số lượng còn open                        |
| price        | numeric    | 19,6       |             |                                          |               | giá trước thuế và sau discount           |
| currency     | string     | 3          |             | link apz_ocrn                            |               | đồng tiền                                |
| rate         | numeric    | 19,6       |             |                                          |               | tỷ lệ quy đổi ngoại tệ                   |
| disc_prcnt   | numeric    | 19,6       |             |                                          |               | Phần trăm giảm giá                       |
| line_total   | numeric    | 19,6       |             |                                          |               | tổng tiền của line. sau thuế và discount |
| whs_code     | string     | 50         |             | link apz_owhs                            |               | mã kho nhập/xuât hàng                    |
| tree_type    | string     | 1          |             | * N : not bom<br>* A : assembly<br>* S : Sales<br>* I : Bom sales component<br>* P : Production<br>* T : template | N             | loại bom                                 |
| tax_status   | string     | 1          |             | * Y : Yes<br>* N : No                    | Y             | trạng thái tính thuế                     |
| price_bef_di | numeric    | 19,6       |             |                                          |               | giá trước thuế, trước discount           |
| doc_date     | date       |            |             |                                          |               | Ngày tạo phiếu                           |
| use_base_un  | string     | 1          |             | * N : No<br>* Y : Yes                    | Y             | sử dụng đơn vị lưu kho để nhập/xuất      |
| project      | string     | 50         |             |                                          |               | Dự án                                    |
| vat_prcnt    | numeric    | 19,6       |             |                                          |               | phần trăm tính thuế                      |
| vat_group    | string     | 50         |             | link apz_ovtg                            |               | mã thuế                                  |
| price_af_vat | numeric    | 19,6       |             |                                          |               | giá sau thuế sau discount                |
| height       | numeric    | 19,6       |             |                                          |               | chiều cao hàng                           |
| width        | numeric    | 19,6       |             |                                          |               | chiều rộng hàng                          |
| length       | numeric    | 19,6       |             |                                          |               | chiều dài hàng                           |
| volume       | numeric    | 19,6       |             |                                          |               | thể tích hàng                            |
| height_unit  | string     | 50         |             |                                          |               | đơn vị chiều cao                         |
| width_unit   | string     | 50         |             |                                          |               | đơn vị chiều rộng                        |
| length_unit  | string     | 50         |             |                                          |               | đơn vị chiều dài                         |
| volume_unit  | string     | 50         |             |                                          |               | đơn vị thể tích                          |
| vis_order    | int        |            |             |                                          |               | thứ tự hiển thị trên giao diện           |
| address      | string     | 254        |             |                                          |               | địa chỉ giao hàng                        |
| remarks      | string     | 254        |             |                                          |               | ghi chú                                  |
| line_vat     | numeric    | 19,6       |             |                                          |               | tổng tiền thuế                           |
| line_vatl_f  | numeric    | 19,6       |             |                                          |               | tổng tiền thuế theo ngoại tệ             |
| unit_msr     | string     | 100        |             |                                          |               | tên đơn vị tính                          |
| num_per_msr  | numeric    | 19,6       |             |                                          |               | tỉ lệ quy đổi ra đơn vị lưu kho          |
| from_whs_cod | string     | 50         |             | link apz_owhs                            |               | kho lấy hàng trên phiếu transfer và transfer request |
| inv_qty      | numeric    | 19,6       |             |                                          |               | số lượng theo đơn vị lưu kho             |
| open_inv_qty | numeric    | 19,6       |             |                                          |               | số lượng đang open theo đơn vị lưu kho   |
| line_vendor  | string     | 50         |             | link apz_ocrd                            |               | vendor code trên các phiếu quotation     |
| create_date  | date       |            |             |                                          |               | ngày tạo trên hệ thống                   |
| update_date  | date       |            |             |                                          |               | ngày cập nhật cuối cùng trên hệ thống    |
| user_sign    | smallint   |            |             | link apz_ousr                            |               | id user tạo phiếu                        |
| total_bef_v  | numeric    | 19,6       |             |                                          |               | tổng tiền trường thuế sau discount       |
| disc_sum     | numeric    | 19,6       |             |                                          |               | tổng tiền giảm giá                       |
| uom_code     | string     | 50         |             | link apz_ugp1                            |               | mã đơn vị tính trong trường hợp mặt hàng là mutil đơn vị tính |
| ugp_entry    | int        |            |             | link apz_ougp                            |               | nhóm đơn vị tính của mặt hàng            |
| father_id    | int        |            |             |                                          |               | father id của mặt hàng trong trường hợp là bom |
| item_cost    | numeric    | 19,6       |             |                                          |               | giá vốn trên kho tại thời điểm tạo phiếu |
| disc_type    | string     | 1          |             | * P : Phần trăm<br>* C : tiền mặt        | P             | hình thức discount                       |

**Copy Process**:

Sales:

1. Sales Quotation => Sales Order => Delivery => Invoice

2. Sales Quotation => Delivery => Invoice

3. Sales Quotation => Reserve Invoice / Invoice

4. Sales Order => Reserve Invoice / Invoice

5. Delivery => Sales Return Request / Sales Return

6. Reserve Invoice => Delivery

7. Sales Return/ Sales Return Request => Delivery

Purchase:

1. Purchase Quotation => Purchase Order => Good Receipt PO => Purchase Invoice

2. Purchase Quotation => Good Receipt PO => Purchase Invoice

3. Purchase Quotation => Purchase Reserve Invoice / Purchase Invoice

4. Purchase Order => Purchase Reserve Invoice / Purchase Invoice

5. Good Receipt PO => Purchase Return Request / Purchase Return

6. Purchase Reserve Invoice => Good Receipt PO

7. Purchase Return/ Purchase Return Request => Good Receipt PO

Inventory:

1. Good Receipt => Good Issue

2. Good Issue => Good Receipt

3. Inventory Transfer Request => Inventory Transfer

# 01 API GET LIST Documents

**Remarks: lấy danh phiếu document theo object type**:

**URL** : `/Documents/Get`

**URI Paramater**: `NONE`

**Method** : `POST`

**Body** :
```json
{
    "object_type": 17,                              Object Type của object lấy data
    "doc_entry": 1,
    "from_date": "2020-01-01",                      từ ngày format yyyy-MM-dd
    "to_date": null,
    "company": null,                                Công ty lấy data
    "status": "O",                                  Trạng thái phiếu
    "search_name": null,                            Search theo doc_entry hoặc card_code
    "page_size": 20,
    "page_index": 1,
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "pagination": [
            {
                "page_index": 0,
                "page_size": 0,
                "sum_record": 2,
                "page_total": 0
            }
        ],
        "object_data": [
            {
                "doc_entry": 1,
                "canceled": "N",
                "doc_status": "O",
                "obj_type": "17",
                "doc_date": "2021-03-03",
                "doc_due_date": "2021-04-04",
                "card_code": "KH001",
                "card_name": "Irs Viet Nam",
                "doc_cur": "VND",
                "doc_total": 1000000.0,
                "doc_total_fc": 230000000.0,
                "comments": "no comment",
                "group_num": 1,
                "company": "C01"
            },
        ]
    }
    "message": null
}
```

**MsgCode**: 
```text
200     - Success
3       - Exception
Other   - error
```

# 02 API GET DATA LINE BY ENTRY

**Remarks: lấy chi tiết line của document theo entry**:

**URL** : `/Documents/GetLinesByEntry?obj_type=17&doc_entry=1`

**URI Paramater**:
```TEXT
obj_type               object type của chức năng lấy dữ liệu
doc_entry              Entry của phiếu lấy dữ liệu
```

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": [
        {
            "doc_entry": 1,
            "line_num": 0,
            "base_type": -1,
            "base_entry": null,
            "base_line": null,
            "line_status": "O",
            "item_code": "SP001",
            "item_name": "Note 20 utra",
            "quantity": 10.0,
            "ship_date": "2020-04-04",
            "open_qty": 10.0,
            "price": 10000000.0,
            "currency": "VND",
            "disc_prcnt": 0.0,
            "line_total": 100000000.0,
            "whs_code": "WH01",
            "price_bef_di": 10000000.0,
            "use_base_un": "Y",
            "vat_prcnt": 10.0,
            "vat_group": "S10",
            "price_af_vat": 1100000.0,
            "vis_order": 1,
            "line_vat": 10000000.0,
            "unit_msr": "CAI",
            "num_per_msr": 1,
            "inv_qty": 10.0,
            "open_inv_qty": 10.0,
        },
    ]
    "message": null
}
```

**MsgCode**: 
```text
200     - Success
3       - Exception
Other   - error
```

# 03 API CREATE DOCUMENT

**Remarks: Tạo mới một document**:

**URL** : `/Documents/Create`

**URI Paramater**: `NONE`

**Method** : `POST`

**Body** :
```JSON
{
    "object_type": 17,
    "document": {
        "document": [
                {
                    "doc_entry": 1,
                    "canceled": "N",
                    "doc_status": "O",
                    "obj_type": "17",
                    "doc_date": "2021-03-03",
                    "doc_due_date": "2021-04-04",
                    "card_code": "KH001",
                    "card_name": "Irs Viet Nam",
                    "doc_cur": "VND",
                    "doc_total": 1000000.0,
                    "doc_total_fc": 230000000.0,
                    "comments": "no comment",
                    "group_num": 1,
                    "company": "C01"
                },
            ],
        "document_lines": [
            {
                "doc_entry": 1,
                "line_num": 0,
                "base_type": -1,
                "base_entry": null,
                "base_line": null,
                "line_status": "O",
                "item_code": "SP001",
                "item_name": "Note 20 utra",
                "quantity": 10.0,
                "ship_date": "2020-04-04",
                "open_qty": 10.0,
                "price": 10000000.0,
                "currency": "VND",
                "disc_prcnt": 0.0,
                "line_total": 100000000.0,
                "whs_code": "WH01",
                "price_bef_di": 10000000.0,
                "use_base_un": "Y",
                "vat_prcnt": 10.0,
                "vat_group": "S10",
                "price_af_vat": 1100000.0,
                "vis_order": 1,
                "line_vat": 10000000.0,
                "unit_msr": "CAI",
                "num_per_msr": 1,
                "inv_qty": 10.0,
                "open_inv_qty": 10.0,
            },
        ],
        "Attachment":[
            {
                "doc_entry": 0,
                "line_num": 0,
                "file_type": "W",
                "file_name": "image01",
                "file_ext": ".icon",
                "file_id": "abcxyz",
                "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
            }
        ]
    }
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": 1,                       id phiếu mới tạo
    "message": null
}
```

**MsgCode**: 
```text
200     - Success
3       - Exception
Other   - error
```

# 04 API UPDATE DOCUMENT

**Remarks: Cập nhật một document**:

**URL** : `/Documents/Update`

**URI Paramater**: `NONE`

**Method** : `POST`

**Body** :
```JSON
{
    "object_type": 17,
    "document": {
        "document": [
                {
                    "doc_entry": 1,
                    "canceled": "N",
                    "doc_status": "O",
                    "obj_type": "17",
                    "doc_date": "2021-03-03",
                    "doc_due_date": "2021-04-04",
                    "card_code": "KH001",
                    "card_name": "Irs Viet Nam",
                    "doc_cur": "VND",
                    "doc_total": 1000000.0,
                    "doc_total_fc": 230000000.0,
                    "comments": "no comment",
                    "group_num": 1,
                    "company": "C01"
                },
            ],
        "document_lines": [
            {
                "doc_entry": 1,
                "line_num": 0,
                "base_type": -1,
                "base_entry": null,
                "base_line": null,
                "line_status": "O",
                "item_code": "SP001",
                "item_name": "Note 20 utra",
                "quantity": 10.0,
                "ship_date": "2020-04-04",
                "open_qty": 10.0,
                "price": 10000000.0,
                "currency": "VND",
                "disc_prcnt": 0.0,
                "line_total": 100000000.0,
                "whs_code": "WH01",
                "price_bef_di": 10000000.0,
                "use_base_un": "Y",
                "vat_prcnt": 10.0,
                "vat_group": "S10",
                "price_af_vat": 1100000.0,
                "vis_order": 1,
                "line_vat": 10000000.0,
                "unit_msr": "CAI",
                "num_per_msr": 1,
                "inv_qty": 10.0,
                "open_inv_qty": 10.0,
            },
        ],
        "Attachment":[
            {
                "doc_entry": 0,
                "line_num": 0,
                "file_type": "W",
                "file_name": "image01",
                "file_ext": ".icon",
                "file_id": "abcxyz",
                "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
            }
        ]
    }
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": 1,                       id phiếu update
    "message": null
}
```

**MsgCode**: 
```text
200     - Success
3       - Exception
Other   - error
```

# 05 API CANCEL DOCUMENT

**Remarks: Hủy một document**:

**URL** : `/Documents/Cancel`

**URI Paramater**: `NONE`

**Method** : `POST`

**Body** :
```JSON
{
    "object_type": 17,
    "document": {
        "document": [
                {
                    "doc_entry": 1,
                    "canceled": "Y",
                    "doc_status": "C",
                    "obj_type": "17",
                    "doc_date": "2021-03-03",
                    "doc_due_date": "2021-04-04",
                    "card_code": "KH001",
                    "card_name": "Irs Viet Nam",
                    "doc_cur": "VND",
                    "doc_total": 1000000.0,
                    "doc_total_fc": 230000000.0,
                    "comments": "no comment",
                    "group_num": 1,
                    "company": "C01"
                },
            ],
        "document_lines": [
            {
                "doc_entry": 1,
                "line_num": 0,
                "base_type": 17,
                "base_entry": 1,
                "base_line": 1,
                "line_status": "C",
                "item_code": "SP001",
                "item_name": "Note 20 utra",
                "quantity": 10.0,
                "ship_date": "2020-04-04",
                "open_qty": 10.0,
                "price": 10000000.0,
                "currency": "VND",
                "disc_prcnt": 0.0,
                "line_total": 100000000.0,
                "whs_code": "WH01",
                "price_bef_di": 10000000.0,
                "use_base_un": "Y",
                "vat_prcnt": 10.0,
                "vat_group": "S10",
                "price_af_vat": 1100000.0,
                "vis_order": 1,
                "line_vat": 10000000.0,
                "unit_msr": "CAI",
                "num_per_msr": 1,
                "inv_qty": 10.0,
                "open_inv_qty": 10.0,
            },
        ],
        "Attachment":[
            {
                "doc_entry": 0,
                "line_num": 0,
                "file_type": "W",
                "file_name": "image01",
                "file_ext": ".icon",
                "file_id": "abcxyz",
                "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
            }
        ]
    }
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": 1,                       id phiếu cancel
    "message": null
}
```

**MsgCode**: 
```text
200     - Success
3       - Exception
Other   - error
```

# 06 API CLOSE DOCUMENT

**Remarks: Đóng một document**:

**URL** : `/Documents/Close`

**URI Paramater**: `NONE`

**Method** : `POST`

**Body** :
```json
{
    "object_type": 17,                              Object Type của object lấy data
    "doc_entry": 1
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": 1,                       id phiếu close
    "message": null
}
```

**MsgCode**: 
```text
200     - Success
3       - Exception
Other   - error
```