# 00 OBJECT INFO
**Object Type**: `400002` **ItemComponents**

**Object Table**: `apz_ogit`

| Column Name    | Data Type | Max Length | Is Required | List Value/ Linked Table    | Default Value | Remarks                          |
|----------------|-----------|------------|-------------|-----------------------------|---------------|----------------------------------|
| f_itm_code     | string    | 50         | Y           |                             |               | Id item gốc                      |
| f_itm_name     | string    | 254        | N           |                             |               | Tên item gốc                     |
| status         | string    | 1          | N           | A: Active <br/> I: Inactive | A             | Trạng thái item                  |
| tramrk         | string    | 254        | N           |                             |               | Thương hiệu của sản phẩm         |
| create_date    | date      |            | N           |                             |               | Ngày tạo                         |
| create_time    | smallint  |            | N           |                             |               | Giờ tạo                          |
| user_sign      | smallint  |            | N           |                             |               | Người tạo                        |
| user_sign2     | smallint  |            | N           |                             |               | Người cập nhật                   |
| update_date    | date      |            | N           |                             |               | Ngày cập nhật                    |
| update_time    | smallint  |            | N           |                             |               | Giờ cập nhật                     |
| tags           | string    | 254        | N           |                             |               | item tags                        |
| itms_grp_cod   | string    | 50         | N           |                             |               | item groups                      |
| ugp_entry      | int       |            | N           |                             |               | mã nhóm đơn vị tính              |
| sal_unit_msr   | string    | 50         | N           |                             |               |                                  |
| s_uom_entry    | string    | 50         | N           |                             |               |                                  |
| remarks        | string    | 254        | N           |                             |               | mô tả                            |
| property1_name | string    | 254        | N           |                             |               | tên thuộc tính 1                 |
| property1      | string    | 50         | N           |                             |               | giá trị thuộc tính 1             |
| property2_name | string    | 254        | N           |                             |               | tên thuộc tính 2                 |
| property2      | string    | 50         | N           |                             |               | giá trị thuộc tính 2             |
| property3_name | string    | 254        | N           |                             |               | tên thuộc tính 3                 |
| property3      | string    | 50         | N           |                             |               | giá trị thuộc tính 3             |
| property_item  | string    |            | N           | Y: yes <br/> N: no          | N             | item có property hay không       |
| property_count | smallint  |            | N           |                             |               | số lượng thuộc tính item sử dụng |
| item_type      | string    | 3          | N           |                             |               | loại sản phẩm                    |
| data_source    | string    | 1          | N           |                             |               | nguồn dữ liệu                    |

**Object Type**: `4` **Items**

**Object Table**: `apz_oitm`

| Column Name    | Data Type | Max Length | Is Required | List Value/ Linked Table         | Default Value | Remarks                                        |
|----------------|-----------|------------|-------------|----------------------------------|---------------|------------------------------------------------|
| item_code      | string    | 50         | Y           |                                  |               | mã item con                                    |
| item_name      | string    | 254        | N           |                                  |               | tên item con                                   |
| itms_grp_cod   | string    | 50         | N           | link apz_oitb                    |               | mã item group                                  |
| item_type      | string    | 3          | N           |                                  |               | loại item                                      |
| prchse_item    | string    | 1          | N           | * Y: Yes<br>* N: No              | N             | (?)                                            |
| sell_item      | string    | 1          | N           | * Y: Yes<br>* N: No              | N             | (?)                                            |
| invn_item      | string    | 1          | N           | * Y: Yes<br>* N: No              | N             | (?)                                            |
| on_hand        | numeric   |            | N           |                                  |               | số lượng tồn kho                               |
| is_commited    | numeric   |            | N           |                                  |               | (?)                                            |
| on_order       | numeric   |            | N           |                                  |               | số lượng đang giao dịch                        |
| dfltwh         | string    | 8          | N           | link apz_obpl                    |               | kho mặc định                                   |
| buy_unit_msr   | string    | 100        | N           | (?) link apz_ouom                |               | đơn vị tính khi bán                            |
| num_in_buy     | numeric   |            | N           |                                  |               | giá trị đơn vị tính khi bán                    |
| sal_unit_msr   | string    | 100        | N           | (?) link apz_ouom                |               | đơn vị tính khi mua                            |
| num_in_sale    | numeric   |            | N           |                                  |               | giá trị đơn vị tính khi mua                    |
| invn_unit_msr  | string    | 100        | N           | (?) link apz_ouom                |               | đơn vị tính trong kho                          |
| user_sign      | smallint  |            | N           | link apz_ousr                    |               | người tạo                                      |
| user_sign2     | smallint  |            | N           | link apz_ousr                    |               | người cập nhật                                 |
| asset_item     | string    | 1          | N           |                                  |               | (?)                                            |
| height         | numeric   |            | N           |                                  |               | (?)                                            |
| height_unit    | string    | 100        | N           |                                  |               | (?)                                            |
| width          | numeric   |            | N           |                                  |               | (?)                                            |
| width_unit     | string    | 100        | N           |                                  |               | (?)                                            |
| length         | numeric   |            | N           |                                  |               | (?)                                            |
| length_unit    | string    | 100        | N           |                                  |               | (?)                                            |
| weight         | numeric   |            | N           |                                  |               | (?)                                            |
| weight_unit    | string    | 100        | N           |                                  |               | (?)                                            |
| avg_price      | numeric   |            | N           |                                  |               | giá trung bình                                 |
| status         | string    | 1          | N           | * A: Active<br>* I: Inactive     | N             | có sử dụng giới hạn khoảng thời gian hay không |
| valid_from     | date      |            | N           |                                  |               | có giá trị từ (ngày)                           |
| valid_to       | date      |            | N           |                                  |               | có giá trị đến (ngày)                          |
| frozen_for     | string    | 1          | N           | * Y: Yes<br>* N: No              | N             | có sử dụng ngày vô hiệu khóa                   |
| frozen_from    | date      |            | N           |                                  |               | vô hiệu hóa từ (ngày)                          |
| frozen_to      | date      |            | N           |                                  |               | vô hiệu hóa đến (ngày)                         |
| ship_type      | smallint  |            | N           |                                  |               | kiểu giao hàng                                 |
| lead_time      | integer   |            | N           |                                  |               | (?)                                            |
| series         | integer   |            | N           |                                  |               | (?)                                            |
| atc_entry      | integer   |            | N           | link apz_oatc  (?) link apz_atc1 |               | Id file đính kèm                               |
| attactment     | string    |            | N           |                                  |               | ? giá trị đính kèm                             |
| create_date    | date      |            | N           |                                  |               | ngày tạo                                       |
| update_date    | date      |            | N           |                                  |               | ngày cập nhật                                  |
| create_time    | smallint  |            | N           |                                  |               | giờ tạo                                        |
| update_time    | smallint  |            | N           |                                  |               | giờ cập nhật                                   |
| tramrk         | string    | 50         | N           | link apz_otrm                    |               | nhãn hiệu                                      |
| barcode        | string    | 254        | N           |                                  |               | giá trị barcode                                |
| remarks        | string    |            | N           |                                  |               | mô tả                                          |
| item_tags      | string    | 500        | N           | link apz_otag                    |               | item tags                                      |
| vat_group      | string    | 50         | N           | link apz_ovtg                    |               | nhóm thuế giá trị gia tăng                     |
| last_pur_prc   | numeric   |            | N           |                                  |               | (?)                                            |
| ugp_entry      | integer   |            | N           | (?) link apz_ougp                |               | nhóm đơn vị tính                               |
| location       | string    | 254        | N           |                                  |               | vị trí                                         |
| f_itm_code     | string    | 50         | N           | link apz_ogit                    |               | mã item cha                                    |
| s_uom_entry    | string    | 50         | N           | (?) link apz_ugp1                |               | đơn vị tính cho SO                             |
| p_uom_entry    | string    | 50         | N           | (?) link apz_ugp1                |               | đơn vị tính cho PO                             |
| i_uom_entry    | string    | 50         | N           | (?) link apz_ugp1                |               | đơn vị tính tồn kho                            |
| line_id        | smallint  |            | N           |                                  |               | đánh số thứ tự item                            |
| qrcode         | string    | 50         | N           |                                  |               | mã qr                                          |
| property1_name | string    | 254        | N           |                                  |               | tên thuộc tính 1                               |
| property1      | string    | 50         | N           |                                  |               | giá trị thuộc tính 1                           |
| property2_name | string    | 254        | N           |                                  |               | tên thuộc tính 2                               |
| property2      | string    | 50         | N           |                                  |               | giá trị thuộc tính 2                           |
| property3_name | string    | 254        | N           |                                  |               | tên thuộc tính 3                               |
| property3      | string    | 50         | N           |                                  |               | giá trị thuộc tính 3                           |
| property_item  | string    | 1          | N           |                                  |               | có sử dụng thuộc tính hay không                |
| property_count | smallint  |            | N           |                                  |               | số lượng thuộc tính được sử dụng               |
| data_source    | string    | 1          | N           |                                  |               | nguồn dữ liệu                                  |



**Item Warehouse**:  `apz_oitw`

| Column Name | Data Type | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                          |
|-------------|-----------|------------|-------------|--------------------------|---------------|----------------------------------|
| item_code   | string    | 50         | Y           | link apz_oitm            |               | Mã item                          |
| whs_code    | string    | 50         | Y           | link apz_owhs            |               | Mã kho                           |
| on_hand     | numeric   |            | N           |                          |               | tồn kho                          |
| is_commited | numeric   |            | N           |                          |               | (?) số lượng item được xác nhận  |
| on_order    | numeric   |            | N           |                          |               | (?) số lượng item đang giao dịch |
| counted     | numeric   |            | N           |                          |               | (?) số lượng đã đếm              |
| user_sign   | smallint  |            | N           |                          |               | người tạo                        |
| user_sign2  | smallint  |            | N           |                          |               | người cập nhật                   |
| min_stock   | numeric   |            | N           |                          |               | Số lượng tối thiểu trong kho     |
| max_stock   | numeric   |            | N           |                          |               | Số lượng tối đa trong kho        |
| avg_price   | numeric   |            | N           |                          |               | Giá thành trung bình             |
| locked      | string    | 1          | N           | * Y: Yes<br>* N: No      | N             | bị khóa hay chưa                 |
| location    | string    | 50         | N           |                          |               | vị trí                           |

# 01 API GET ITEM and IT'S COMPONENTS

**Remarks**: Hiển thị danh sách item cha theo điều kiện:

**URL**: `/ItemMasterData/Get`

**Method**: `POST`

**Body**:

> Get All

```json
{
}
```

> Get By Condition

```json
{
  "code": "item code",
  "name": "item name",
  "groups": ["some groups"],
  "tags": ["some tags (id)"],
  "trademarks": "macedonian",
  "types": "S",
  "status": "A",
  "search_name": "search any text",
  "from_date": "",
  "to_date": "",
  "page_index": 0,
  "page_size": 0
}
```

**Auth Required**: `Bearer token`

##Response

```json
{
    "msg_code": 200,
    "content": {
        "pagination": [
            {
                "page_index": 0,
                "page_total": null,
                "page_size": 0,
                "sum_record": 1
            }
        ],
        "object_data": [
            {
                "row_num": 1,
                "f_itm_code": "S10000",
                "f_itm_name": "Lan ra ha noi",
                "status": null,
                "tramrk": "Assy",
                "create_date": "2021-05-05T00:00:00",
                "create_time": 759,
                "user_sign": 1,
                "user_sign2": 1,
                "update_date": null,
                "update_time": null,
                "tags": null,
                "itms_grp_cod": "ITM2",
                "ugp_entry": null,
                "sal_unit_msr": null,
                "s_uom_entry": null,
                "remarks": null,
                "property1_name": null,
                "property1": null,
                "property2_name": null,
                "property2": null,
                "property3_name": null,
                "property3": null,
                "property_item": null,
                "property_count": null,
                "item_type": "S",
                "data_source": "N",
                "on_hand": 0.000000
            }
        ]
    },
    "message": null
}
```

# 02 API GET ITEM Documents by condition

**Remarks**: Hiển thị danh sách item cha theo điều kiện:

**URL**: `/ItemMasterData/GetDocumentItems`

**Method**: `POST`

**Body**:

```json
{
  "page_index": 0,
  "page_size": 0,
  "tran_type": "S",
  "price_list": 0
}
```

##Response

```json
{
    "msg_code": 200,
    "content": {
        "pagination": [
            {
                "page_index": 0,
                "page_total": 0,
                "page_size": 0,
                "total": 0
            }
        ],
        "object_data": [...]
    },
    "message": null
}
```

# 03 GET GRAND ITEM WITH ALL IT'S COMPONENTS BY ID

**Remarks**: Hiển thị chi tiết item cha và tất cả thành phần của nó bao gồm danh sách item con 

**URL** `/ItemMasterData/GetById?id=S10000`

**URI Parameter**:

1. id: item_code của item cha

**Method**: `GET`

**Body**: `NONE`

**Auth Required**: `Bearer token`

**Permission Required**: `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "item": [
            {
                "f_itm_code": "S10000",
                "f_itm_name": "Meo Con",
                "status": null,
                "tramrk": "Assy",
                "create_date": "2021-05-06T00:00:00",
                "create_time": 732,
                "user_sign": 1,
                "user_sign2": 1,
                "update_date": null,
                "update_time": null,
                "tags": null,
                "itms_grp_cod": "ITM2",
                "ugp_entry": null,
                "sal_unit_msr": null,
                "s_uom_entry": null,
                "remarks": null,
                "property1_name": null,
                "property1": null,
                "property2_name": null,
                "property2": null,
                "property3_name": null,
                "property3": null,
                "property_item": null,
                "property_count": null,
                "item_type": "S",
                "data_source": "N"
            }
        ],
        "item_components": [
            {
                "item_code": "S10000",
                "item_name": "Meo Muop",
                "itms_grp_cod": "ITM2",
                "item_type": "S",
                "prchse_item": "Y",
                "sell_item": "Y",
                "invn_item": "Y",
                "on_hand": null,
                "is_commited": null,
                "on_order": null,
                "dfltwh": "whs100",
                "buy_unit_msr": null,
                "num_in_buy": 1.000000,
                "sal_unit_msr": null,
                "num_in_sale": 1.000000,
                "invn_unit_msr": null,
                "user_sign": 1,
                "user_sign2": null,
                "asset_item": null,
                "height": null,
                "height_unit": null,
                "width": null,
                "width_unit": null,
                "length": null,
                "length_unit": null,
                "weight": null,
                "weight_unit": null,
                "avg_price": null,
                "valid_for": null,
                "valid_from": null,
                "valid_to": null,
                "frozen_for": null,
                "frozen_from": null,
                "frozen_to": null,
                "ship_type": null,
                "lead_time": null,
                "series": -1,
                "atc_entry": 1,
                "IsCommited": null,
                "create_date": "2021-05-06T00:00:00",
                "update_date": null,
                "create_time": 732,
                "update_time": null,
                "tramrk": "Assy",
                "barcode": null,
                "remarks": null,
                "item_tags": null,
                "vat_group": "VAT101",
                "last_pur_prc": null,
                "ugp_entry": null,
                "location": null,
                "f_itm_code": "S10000",
                "s_uom_entry": null,
                "p_uom_entry": null,
                "i_uom_entry": null,
                "line_id": 0,
                "qrcode": null,
                "property1_name": null,
                "property1": null,
                "property2_name": null,
                "property2": null,
                "property3_name": null,
                "property3": null,
                "property_item": null,
                "property_count": null,
                "data_source": "N"
            },
            {
                "item_code": "S10001",
                "item_name": "Meo Len Bang",
                "itms_grp_cod": "ITM2",
                "item_type": "S",
                "prchse_item": "Y",
                "sell_item": "Y",
                "invn_item": "Y",
                "on_hand": null,
                "is_commited": null,
                "on_order": null,
                "dfltwh": "whs100",
                "buy_unit_msr": null,
                "num_in_buy": 1.000000,
                "sal_unit_msr": null,
                "num_in_sale": 1.000000,
                "invn_unit_msr": null,
                "user_sign": 1,
                "user_sign2": null,
                "asset_item": null,
                "height": null,
                "height_unit": null,
                "width": null,
                "width_unit": null,
                "length": null,
                "length_unit": null,
                "weight": null,
                "weight_unit": null,
                "avg_price": null,
                "valid_for": null,
                "valid_from": null,
                "valid_to": null,
                "frozen_for": null,
                "frozen_from": null,
                "frozen_to": null,
                "ship_type": null,
                "lead_time": null,
                "series": -1,
                "atc_entry": 2,
                "attachment": null,
                "create_date": "2021-05-06T00:00:00",
                "update_date": null,
                "create_time": 732,
                "update_time": null,
                "tramrk": "Assy",
                "barcode": null,
                "remarks": null,
                "item_tags": null,
                "vat_group": "VAT101",
                "last_pur_prc": null,
                "ugp_entry": null,
                "location": null,
                "f_itm_code": "S10000",
                "s_uom_entry": null,
                "p_uom_entry": null,
                "i_uom_entry": null,
                "line_id": 1,
                "qrcode": null,
                "property1_name": null,
                "property1": null,
                "property2_name": null,
                "property2": null,
                "property3_name": null,
                "property3": null,
                "property_item": null,
                "property_count": null,
                "data_source": "N"
            }
        ],
        "attachment": [
            {
                "doc_entry": 1,
                "line_num": 0,
                "src_path": null,
                "trgt_path": null,
                "file_type": "W",
                "file_name": "Meo1",
                "file_ext": ".exe",
                "user_sign": null,
                "create_date": null,
                "copied": null,
                "override": null,
                "sub_path": null,
                "free_text": null,
                "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "file_id": "Meo thu 1"
            },
            {
                "doc_entry": 2,
                "line_num": 0,
                "src_path": null,
                "trgt_path": null,
                "file_type": "W",
                "file_name": "meo2",
                "file_ext": ".vegas",
                "user_sign": null,
                "create_date": null,
                "copied": null,
                "override": null,
                "sub_path": null,
                "free_text": null,
                "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "file_id": "Meo thu 2"
            }
        ]
    },
    "message": null
}
```

# 04 GET DETAIL ITEM DOCUMENT BY ID

**Remarks**: Hiển thị chi tiết item con theo id

**URL** `/ItemMasterData/GetDocumentById?id=S10000`

**URI Parameter**:

1. id: item_code của item con

**Method**: `GET`

**Body**: `NONE`

**Auth Required**: `Bearer token`

**Permission Required**: `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "object_data": [
            {
                "item_code": "S10000",
                "item_name": "Meo Muop",
                "itms_grp_cod": "ITM2",
                "item_type": "S",
                "prchse_item": "Y",
                "sell_item": "Y",
                "invn_item": "Y",
                "on_hand": null,
                "is_commited": null,
                "on_order": null,
                "dfltwh": "whs100",
                "buy_unit_msr": null,
                "num_in_buy": 1.000000,
                "sal_unit_msr": null,
                "num_in_sale": 1.000000,
                "invn_unit_msr": null,
                "user_sign": 1,
                "user_sign2": null,
                "asset_item": null,
                "height": null,
                "height_unit": null,
                "width": null,
                "width_unit": null,
                "length": null,
                "length_unit": null,
                "weight": null,
                "weight_unit": null,
                "avg_price": null,
                "valid_for": null,
                "valid_from": null,
                "valid_to": null,
                "frozen_for": null,
                "frozen_from": null,
                "frozen_to": null,
                "ship_type": null,
                "lead_time": null,
                "series": -1,
                "atc_entry": 1,
                "attachment": null,
                "create_date": "2021-05-06T00:00:00",
                "update_date": null,
                "create_time": 732,
                "update_time": null,
                "tramrk": "Assy",
                "barcode": null,
                "remarks": null,
                "item_tags": null,
                "vat_group": "VAT101",
                "last_pur_prc": null,
                "ugp_entry": null,
                "location": null,
                "f_itm_code": "S10000",
                "s_uom_entry": null,
                "p_uom_entry": null,
                "i_uom_entry": null,
                "line_id": 0,
                "qrcode": null,
                "property1_name": null,
                "property1": null,
                "property2_name": null,
                "property2": null,
                "property3_name": null,
                "property3": null,
                "property_item": null,
                "property_count": null,
                "data_source": "N"
            }
        ],
        "price_list": [],
        "attachment": [
            {
                "doc_entry": 1,
                "line_num": 0,
                "src_path": null,
                "trgt_path": null,
                "file_type": "W",
                "file_name": "Meo1",
                "file_ext": ".exe",
                "user_sign": null,
                "create_date": null,
                "copied": null,
                "override": null,
                "sub_path": null,
                "free_text": null,
                "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
                "file_id": "Meo thu 1"
            }
        ],
        "uoms": []
    },
    "message": null
}
```

# 05 API CREATE ITEM WITH COMPONENTS

**Remarks**: API tạo mới Item cha và các thành phần của nó

**URL**: `/ItemMasterData/Create`

**Method**: `POST`

**Body**:

```json
{
    "item": [
        {
            "f_itm_code": "",
            "f_itm_name": "Meo Con",
            "itms_grp_cod": "ITM2",
            "tramrk": "Assy",
            "item_type": "S",
            "vat_group": "VAT101"
        }
    ],
    "item_components": [
        {
            "item_code": "",
            "f_itm_code": "",
            "line_id": 0,
            "item_name": "Meo Muop",
            "item_type": "S",
            "itms_grp_cod": "ITM2",
            "tramrk": "Assy",
            "vat_group": "VAT101"
        },
        {
            "item_code": "",
            "f_itm_code": "",
            "item_name": "Meo Len Bang",
            "line_id": 1,
            "item_type": "P",
            "itms_grp_cod": "ITM2",
            "tramrk": "Assy",
            "vat_group": "VAT101"
        }
    ],
    "attachment": [
        {
            "line_id": 0,
            "line_num": 0,
            "file_type": "W",
            "file_name": "Meo1",
            "file_ext": ".exe",
            "file_id": "Meo thu 1",
            "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
            "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
        },
        {
            "line_id": 1,
            "line_num": 0,
            "file_type": "W",
            "file_name": "meo2",
            "file_ext": ".vegas",
            "file_id": "Meo thu 2",
            "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
            "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
        }
    ]
}
```

```text
- line_id bắt buộc phải có và được đánh số từ 0. line_id đánh số thứ tự cho các components của item con
- vat_group trong item con phải có
```

**Auth Required**: `Bearer Token`

**Permissions Required**: `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "S10000"
}
```

# 06 API UPDATE ITEM WITH COMPONENTS

**Remarks**: API tạo mới Item cha và các thành phần của nó

**URL**: `/ItemMasterData/Update`

**Method**: `POST`

**Body**:

```json
{
    "item": [
        {
            "f_itm_code": "S10000",
            "f_itm_name": "Thit Heo",
            "status": null,
            "tramrk": "MACE",
            "itms_grp_cod": "ITM3",
        }
    ],
    "item_components": [
        {
            "o_item_code": "S10000",
            "item_code": "S10000",
            "item_name": "Thit Bo Nha trong",
            "f_itm_code": "S10000",
            "itms_grp_cod": "ITM3",
            "item_type": "S",
            "tramrk": "MACE",
            "item_tags": null,
            "vat_group": "VAT101",
            "line_id": 0,
        },
        {
            "o_item_code": "S10001",
            "item_code": "S10001",
            "item_name": "Thit bo ben kia song",
            "itms_grp_cod": "ITM3",
            "item_type": "S",
            "tramrk": "MACE",
            "vat_group": "VAT101",
            "line_id": 1,
            "f_itm_code": "S10000",
        }
    ],
    "attachment": [
        {
            "line_id": 0,
            "line_num": 0,
            "file_type": "W",
            "file_name": "home",
            "file_ext": ".exe",
            "file_id": "abcxyz",
            "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
            "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
        },
        {
            "line_id": 1,
            "line_num": 0,
            "file_type": "W",
            "file_name": "america",
            "file_ext": ".vegas",
            "file_id": "asfsfsg",
            "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
            "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
        }
    ]
},
```

```text
- o_item_code là item code cũ 
- item_code là code mới nếu có sửa
```


**Auth Required**: `Bearer Token`

**Permissions Required**: `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "S10000"
}
```

# 06 API DELETE ITEM WITH COMPONENTS

**Remarks**: API tạo mới Item cha và các thành phần của nó

**URL**: `/ItemMasterData/Delete`

**Method**: `POST`

**Body**:

```json
{
    "code":"S10000"
}
```

**Auth Required**: `Bearer Token`

**Permissions Required**: `NONE`


## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "S10000"
}
```

# 07 API CREATE ITEM 

**Remarks**: API tạo mới Item Con

**URL**: `/ItemMasterData/CreateDocumentItem`

**Method**: `POST`

**Body**:

```json
{
  "item": [
    {
      "item_code": "S10006",
      "item_name": "Bo nha trong duoc",
      "f_itm_code": "S10000",
      "itms_grp_cod": "ITM2",
      "item_type": "S",
      "tramrk": "Assy",
      "item_tags": null,
      "vat_group": "VAT101",
      "line_id": 0,
    }
  ],
  "attachment": [
    {
      "line_id": 0,
      "line_num": 0,
      "file_type": "W",
      "file_name": "home",
      "file_ext": ".exe",
      "file_id": "abcxyz",
      "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
      "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
    },
  ]
}
```

**Auth Required**: `Bearer Token`

**Permissions Required**: `NONE`

# Response 

```json
{
    "msg_code": 200,
    "content": null,
    "message": "S10006"
}
```

# 08 API UPDATE ITEM

**Remarks**: API cập nhật Item Con

**URL**: `/ItemMasterData/UpdateDocumentItem`

**Method**: `POST`

**Body**:

```json
{
  "item": [
    {
      "o_item_code": "S10006",
      "item_code": "S10006",
      "item_name": "Bo nha trong duoc",
      "f_itm_code": "S10000",
      "itms_grp_cod": "ITM2",
      "item_type": "S",
      "tramrk": "Assy",
      "item_tags": null,
      "vat_group": "VAT101",
      "line_id": 0,
    }
  ],
  "attachment": [
    {
      "line_id": 0,
      "line_num": 0,
      "file_type": "W",
      "file_name": "home",
      "file_ext": ".exe",
      "file_id": "abcxyz",
      "uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat",
      "dwl_uri": "https://teams.microsoft.com/_?tenantId=cfd68027-c0af-4d75-ba6f-b77bf1a71a99#/conversations/19:14f42df42f284ad6ba0fbf5b7d17fbae@thread.v2?ctx=chat"
    },
  ]
}
```

```text
- o_item_code là item code cũ 
- item_code là code mới nếu có sửa
```

**Auth Required**: `Bearer Token`

**Permissions Required**: `NONE`

# Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "S10006"
}
```

# 08 API DELETE ITEM

**Remarks**: API cập nhật Item Con

**URL**: `/ItemMasterData/DeleteDocumentItem`

**Method**: `POST`

**Body**:
```json
{
    "code": "S10006"
}
```

**Auth Required**: `Bearer Token`

**Permissions Required**: `NONE`

# Response 

```json
{
    "msg_code": 200,
    "content": null,
    "message": "S10006"
}
```