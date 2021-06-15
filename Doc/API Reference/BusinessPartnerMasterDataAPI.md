# 00 OBJECT INFO
**Object Type**: `2`
**BusinessPartner Table**: `apz_ocrd`  

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table| Default Value | Remarks       |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| card_code         | string     | 50         | Y           |                   |           | Mã BP                         |
| card_name         | string     | 254        | N           |                   |           | Tên                           |
| card_type         | string     | 254        | N           | * C: Customer<br>* S: Vendor <br> * L: Leads  |           | Loại BP   |
| group_code        | string     | 20         | N           | linked partner_group   |      | Nhóm BP                       |
| address           | string     | 100        | N           |                   |           | Địa chỉ BP                    |
| city              | string     | 100        | N           |                   |           | Thành phố                     |
| country           | string     | 100        | N           |                   |           | Quốc gia                      |
| zip_code          | string     | 20         | N           |                   |           | Mã zip                        |
| e_mail            | string     | 100        | N           |                   |           | Email                         |
| phone1            | string     | 20         | N           |                   |           | Số điện thoại 1               |
| phone2            | string     | 20         | N           |                   |           | Số điện thoại 2               |
| fax               | string     | 20         | N           |                   |           | Số fax                        |
| cntct_prsn        | string     | 100        | N           |                   |           | Người liên lạc                |
| remarks           | string     | 254        | N           |                   |           | Ghi chú                       |
| list_num          | smallint   |            | N           | linked apz_opln   |           | Bảng giá                      |
| currency          | string     | 3          | N           |                   |           | Đơn vị tiền tệ                |
| create_date       | date       |            | N           |                   |           | Ngày tạo                      |
| create_time       | smallint   |            | N           |                   |           | Thời gian tạo                 |
| update_date       | date       |            | N           |                   |           | Ngày cập nhật                 |
| update_time       | smallint   |            | N           |                   |           | Thời gian cập nhật            |
| user_sign         | smallint   |            | N           | linked user_info  |           | Người tạo                     |
| user_sign2        | smallint   |            | N           | linked user_info  |           | Người cập nhật                |
| vat_group         | string     | 8          | N           | linked apz_ovtg   |           | Thông tin loại VAT            |
| obj_type          | string     | 20         | N           |                   |           | Loại object                   |
| ship_type         | smallint   |            | N           |                   |           | Thông tin loại vận chuyển     |
| ship_to_def       | string     | 254        | N           |                   |           | Địa chỉ giao hàng mặc định    |
| bill_to_def       | string     | 254        | N           |                   |           | Địa chỉ thanh toán mặc định   |
| status            | string     | 1          | N           |* A: Active<br>* I: Inactive|  | Trạng thái                    |

**BusinessPartner Address Table**: `apz_crd1`  

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table| Default Value | Remarks       |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| line_num       | integer      |            | Y           | 1,2,3,...                    |           | Mã địa chỉ theo mã BP  |
| card_code      | string       | 254        | Y           | * C: KH10000<br>* S: NCC10000|           | Mã BusinessPartner     |
| adres_type     | string       |            | Y           | * S: Ship<br>* B: Bill       |           | Loại địa chỉ           |
| address        | string       | 254        | Y           |                              |           | Địa chỉ                |
| street         | string       | 100        | N           |                              |           | Đường                  |
| block          | string       | 100        | N           |                              |           | Khu phố                |
| zip_code       | string       | 25         | N           |                              |           | Mã zip                 |
| city           | string       | 100        | N           |                              |           | Thành phố/Tỉnh         |
| country        | string       | 50         | N           |                              |           | Quốc gia               |
| ward           | string       | 50         | N           |                              |           | Phường/xã              |
| user_sign      | smallint     | 50         | N           | linked apz_ousr              |           | Người tạo              |
| create_date    | date         |            | N           |                              |           | Ngày tạo               |
| create_time    | smallint     |            | N           |                              |           | Thời gian tạo          |
| update_date    | date         |            | N           |                              |           | Ngày cập nhật          |
| update_time    | smallint     |            | N           |                              |           | Thời gian cập nhật     |
| user_sign2     | smallint     | 50         | N           | linked apz_ousr              |           | Người cập nhật         |
| cntc_per       | string       | 254        | N           |                              |           | Người liên lạc         |
| phone          | string       | 20         | N           |                              |           | Số điện thoại          |
| district       | string       | 50         | N           |                              |           | Quận/huyện             |
| is_def         | string       |            | N           | * Y: Yes<br>* N: No          |           | Địa chỉ default, chỉ được chọn một địa chỉ làm default |

# 01 API GET BPMASTERDATA BY ID

**URL** : `/BusinessPartnerMasterData/GetById?id=NCC10000`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "bussiness_partner": [
            {
                "card_code": "NCC10000",
                "card_name": "BPSExameple2",
                "card_type": "S",
                "group_code": "1001",
                "address": null,
                "city": null,
                "country": null,
                "zip_code": null,
                "e_mail": "BPSExameple2@apzon.com",
                "phone1": "0926322111",
                "phone2": "0546925453",
                "fax": "05563621489",
                "cntct_prsn": "aasd",
                "remarks": null,
                "list_num": 1,
                "atc_entry": 3,
                "create_date": "2021-04-07T00:00:00",
                "create_time": null,
                "update_date": null,
                "update_time": null,
                "user_sign": null,
                "vat_group": "VAT101",
                "obj_type": null
            }
        ],
        "address": [
            {
                "line_num": 1,
                "card_code": "NCC10000",
                "adres_type": "S",
                "address": "1088 Cách mạng tháng tám, Tân Bình",
                "street": "Cách mạng tháng tám",
                "block": "",
                "zip_code": "",
                "city": "Hồ Chí Minh",
                "country": "Việt Nam",
                "ward": null,
                "user_sign": null,
                "obj_type": null,
                "create_date": "2021-04-07T00:00:00",
                "cntc_per": null,
                "phone": "0926458332",
                "is_def": "Y",
                "district": null,
                "update_date": null,
                "update_time": null,
                "create_time": null
            }
        ]
    },
    "message": "Success"
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 02 API GET LIABILITIES

**URL** : `/BusinessPartnerMasterData/GetLiabilities?code=KH10000`

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
            "liab_total": -526000.000000,
            "currency": "VND"
        }
    ],
    "message": "Success"
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 03 API GET ALL

**URL** : `/BusinessPartnerMasterData/Get`

**Method** : `POST`

**Body** :
```json
{
    "code":"",
    "name":"",
    "type":"L",
    "user_sign":"",
    "branch":"",
    "page_index":1,
    "page_size":1,
    "from_date":"",
    "to_date":""
}
```

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "pagination": [
            {
                "page_index": 0,
                "page_total": 0,
                "page_size": 0,
                "sum_record": 1
            }
        ],
        "object_data": [
            {
                "card_code": "KHT10000",
                "card_name": "BPLExameple1",
                "card_type": "L",
                "group_code": "S1000",
                "address": null,
                "city": null,
                "country": null,
                "zip_code": null,
                "e_mail": "BPLExameple1@apzon.com",
                "phone1": "0926322111",
                "phone2": "0546925453",
                "fax": "05563621489",
                "cntct_prsn": "aasd",
                "remarks": null,
                "balance": null,
                "list_num": 1,
                "slp_code": null,
                "currency": null,
                "atc_entry": -1,
                "attachment": null,
                "dfl_account": null,
                "dfl_branch": null,
                "create_date": "2021-05-27T00:00:00",
                "create_time": null,
                "update_date": "2021-05-27T00:00:00",
                "update_time": 1424,
                "user_sign": 3,
                "status": null,
                "valid_from": null,
                "valid_to": null,
                "frozen_for": null,
                "frozen_from": null,
                "frozen_to": null,
                "vat_group": "VAT101",
                "obj_type": null,
                "ship_type": null,
                "series": null,
                "ship_to_def": null,
                "bill_to_def": null,
                "gender": null,
                "date_of_bir": null,
                "per_tax_num": null,
                "datasource": null,
                "user_sign2": 3,
                "row_num": 1
            }
        ]
    },
    "message": "Success"
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 04 API CREATE BUSINESSPARTNER

**URL** : `/BusinessPartnerMasterData/Create`

**Method** : `POST`

**Body** :
```json
{
    "info": [
        {
            "card_name":"BPLExameple1",
            "card_type":"L",
            "group_code":"S1000",
            "e_mail":"BPLExameple1@apzon.com",
            "phone1":"0926322111",
            "phone2":"0546925453",
            "fax":"05563621489",
            "cntct_prsn":"aasd",
            "list_num":1,
            "vat_group":"VAT101",
            "atc_entry":3
        }
    ],
    "address":[
        {
            "adres_type":"S",
            "address":"108 Bà Hạt, Quận 10, Tp.Hồ Chí Minh",  
            "phone":"0926458232",
            "street":"Bà Hạt",
            "block":"",
            "zip_code":"",
            "city":"Hồ Chí Minh",
            "country":"Việt Nam",
            "object_type":"",
            "is_def":"Y"
        }
    ],
    "attachment":[
    ]
}
```

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "KHT10000"
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 05 API UPDATE BUSINESSPARTNER

**URL** : `/BusinessPartnerMasterData/Update`

**Method** : `POST`

**Body** : Trường hợp address thêm mới thì điền giá trị trường line_num = 0
```json
{
    "info": [
        {
            "card_code":"KHT10000",
            "card_name":"BPLExameple1",
            "card_type":"L",
            "group_code":"S1000",
            "e_mail":"BPLExameple1@apzon.com",
            "phone1":"0926322111",
            "phone2":"0546925453",
            "fax":"05563621489",
            "cntct_prsn":"aasd",
            "list_num":1,
            "vat_group":"VAT101",
            "atc_entry":3
        }
    ],
    "address":[
        {
            "adres_type":"S",
            "address":"400 Âu Cơ, Quận 11",  
            "phone":"0926458332",
            "street":"Âu Cơ",
            "block":"",
            "zip_code":"",
            "city":"Hồ Chí Minh",
            "country":"Việt Nam",
            "object_type":"",
            "is_def":"N"
        },
        {
            "adres_type":"S",
            "address":"200 Thoại Ngọc Hầu, Bình Tân",  
            "phone":"0926458332",
            "street":"Thoại Ngọc Hầu",
            "block":"",
            "zip_code":"",
            "city":"Hồ Chí Minh",
            "country":"Việt Nam",
            "object_type":"",
            "is_def":"N"
        },
        {
            "adres_type":"B",
            "address":"55 Ngô Thì Nhậm",  
            "phone":"0926458332",
            "street":"Ngô Thì Nhậm",
            "block":"",
            "zip_code":"",
            "city":"Hồ Chí Minh",
            "country":"Việt Nam",
            "object_type":"",
            "is_def":"N"
        }
    ],
    "attachment":[
    ]
}
```

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "KHT10000"
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 06 API DELETE BUSINESSPARTNER

**URL** : `/BusinessPartnerMasterData/Delete`

**Method** : `POST`

**Body** :
```json
{
    "code":"KH10006"
}
```

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": "Success",
    "message": null
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```
