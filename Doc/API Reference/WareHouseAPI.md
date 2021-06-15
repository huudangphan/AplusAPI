# 00 OBJECT INFO

**Object Type**: `64`

**Form Table**: `apz_owhs`

| Column Name  | Data Typpe | Max Length | Is Required | List Value/ Linked Table   | Default Value | Remarks                                  |
|--------------|------------|------------|-------------|----------------------------|---------------|------------------------------------------|
| whs_code     | string     | 50         | Y           |                            |               | Mã kho                                   |
| whs_name     | string     | 254        | N           |                            |               | Tên kho                                  |
| status       | string     | 1          | N           | * A: Active<br>* I: Inactive | A             | trạng thái kho                           |
| user_sign    | smallint   |            | N           | link apz_ousr              |               | người tạo                                |
| user_sign2   | smallint   |            | N           | link apz_ousr              |               | người cập nhật                                |
| street       | string     | 100        | N           |                            |               | tên đường                                |
| block        | string     | 100        | N           |                            |               | tên phường/xã                            |
| zip_code     | string     | 20         | N           |                            |               | Mã bưu chính                             |
| ward         | string     | 50         | N           |                            |               | Phường xã                                |
| district     | string     | 50         | N           |                            |               | Quận Huyện                               |
| city         | string     | 100        | N           |                            |               | Tỉnh thành                               |
| country      | string     | 50         | N           |                            |               | Quốc gia                                 |
| drop_ship    | string     | 1          | N           | * Y : Yes<br>* N : No      | N             |  kho ảo                                  |
| create_date  | date       |            | N           |                            |               | Ngày tạo                                 |
| create_time  | smallint   |            | N           |                            |               | Thời gian tạo                            |
| update_date  | date       |            | N           |                            |               | Ngày cập nhật                            |
| update_time  | smallint   |            | N           |                            |               | Thời gian cập nhật                       |
| cpn_code     | string     |            | Y           | link apz_ocpn              |               | Mã công ty (hoặc công ty con)                              |
| shipper      | string     | 50         | N           |                            |               | Người giao hàng                          |
| dft_bin_abs  | int        |            | N           |                            |               | Id Bin mặc định trong trường hợp kho quản lý theo bin(hiện chưa dùng) |
| dft_bin_enfd | string     | 1          | N           | * Y : Yes<br>* N : No      | N             | Chưa dùng                                |
| bin_activat  | string     | 1          | N           | * Y : Yes<br>* N : No      | N             | Kho quản lý theo bin                     |
| bin_septor   | string     | 5          | N           |                            |               | chuỗi phân cách các thành phần trong bin |
| atc_entry    | int        |            | N           | Link apz_oatc              |               | id danh sách attachment                  |


# 01 API GET WAREHOUSE BY CONDITION

**Remarks**: API Lấy dữ liệu tất cả warehouse hoặc có thể lọc theo điều kiện

**URL**: `/Warehouse/Get`

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
  "code": "whs3000",
  "name": "Macedonian Storage",
  "status": "A",
  "search_name": "some text",
  "from_date": "2001-01-16",
  "to_date": "2021-05-10"
}
```

**Auth Required**: `Bearer token`

**Permissions required**: `None`

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
                "sum_record": 2
            }
        ],
        "object_data": [
            {
                "whs_code": "whs3000",
                "whs_name": "Macedonian Storage",
                "user_sign": -1,
                "street": null,
                "block": null,
                "zip_code": null,
                "city": null,
                "country": null,
                "drop_ship": "N",
                "create_date": "2021-04-09T00:00:00",
                "create_time": 501,
                "update_date": null,
                "update_time": null,
                "cpn_code": -1,
                "shipper": null,
                "dft_bin_abs": null,
                "dft_bin_enfd": "N",
                "bin_activat": "N",
                "bin_septor": null,
                "atc_entry": null,
                "status": "A",
                "row_num": 1
            },
            {
                "whs_code": "whs100",
                "whs_name": " Kho Hà Nội",
                "user_sign": 1000,
                "street": " Trần Duy Hưng",
                "block": " ",
                "zip_code": " 10000",
                "city": " Hà Nội",
                "country": " Việt Nam",
                "drop_ship": " ",
                "create_date": null,
                "create_time": null,
                "update_date": null,
                "update_time": null,
                "cpn_code": null,
                "shipper": " ",
                "dft_bin_abs": null,
                "dft_bin_enfd": " ",
                "bin_activat": " ",
                "bin_septor": " ",
                "atc_entry": null,
                "status": "A",
                "row_num": 2
            }
        ]
    },
    "message": null
}
```

# 02 API GET WAREHOUSE BY WAREHOUSE ID

**Remarks**: API Lấy detail warehouse theo whs_code

**URL**: `/Warehouse/GetById?id=whs3000`

**URL Parameter**: 

1. id: warehouse id

**Method**: `GET`

**Body**: `NONE`

**Auth Required**: `Bearer token`

**Permissions required**: `None`

## Response

```json
{
    "msg_code": 200,
    "content": [
        {
            "whs_code": "whs3000",
            "whs_name": "Macedonian Storage",
            "user_sign": -1,
            "street": null,
            "block": null,
            "zip_code": null,
            "city": null,
            "country": null,
            "drop_ship": "N",
            "create_date": "2021-04-09T00:00:00",
            "create_time": 501,
            "update_date": null,
            "update_time": null,
            "cpn_code": "-1",
            "shipper": null,
            "dft_bin_abs": null,
            "dft_bin_enfd": "N",
            "bin_activat": "N",
            "bin_septor": null,
            "atc_entry": null,
            "status": "A"
        }
    ],
    "message": null
}
```


# 03 API CREATE WAREHOUSE

**Remarks**: Thêm mới kho

**URL**: `/Warehouse/Create`

**URL**: `/PriceList/Create`

**Method**: `POST`

**Body**:

```json
[
  {
    "whs_code": "whs3000",
    "whs_name": "Macedonian Storage",
    "street": "Trần Duy Hưng",
    "block": "Block1",
    "zip_code": "10000",
    "city": "Hà nội",
    "country": "Việt Nam",
    "drop_ship": "N",
    "shipper": "Luân",
    "dft_bin_abs": 1,
    "dft_bin_enfd": "N",
    "bin_activat": "N",
    "bin_septor": ",",
    "atc_entry": 1,
    "status": "A"
  }
]
```

**Auth Required**: `Bearer Token`

**Permissions Required**: `NONE`


## Response

```json
{
  "msg_code": 200,
  "content": null,
  "message": ""
}
```

# 03 API UPDATE WAREHOUSE

**Remarks**: Cập nhật kho hiện tại

**URL**: `/Warehouse/Update`

**Method**: `POST`

**Body**:

```json
[
  {
    "whs_code": "whs3000",
    "whs_name": "Assyrian Storage",
    "street": "Võ Hoàng Yên",
    "block": "Block1",
    "zip_code": "10000",
    "city": "TP HCM",
    "country": "Việt Nam",
    "drop_ship": "N",
    "shipper": "Dũng",
    "dft_bin_abs": 1,
    "dft_bin_enfd": "N",
    "bin_activat": "N",
    "bin_septor": ",",
    "atc_entry": 1,
    "status": "A"
  }
]
```

**Auth Required**: `Bearer Token`

**Permissions Required**: `NONE`


## Response

```json
{
  "msg_code": 200,
  "content": null,
  "message": ""
}
```

# 05 API DELETE WAREHOUSE

**Remarks**: Xóa kho

**URL** `/Warehouse/Delete`

**Method**: `POST`

**Body**:

```json
{
    "code": "whs3000"
}
```

**Auth Required**: `Bearer token`

**Permission Required**: `NONE`

## Response

```json
{
  "msg_code": 200,
  "content": null,
  "message": "whs3000"
}
```

