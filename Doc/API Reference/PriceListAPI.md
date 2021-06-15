# 00 OBJECT INFO

**Object Type**: `6`

**Price List Table**: `apz_opln`

| Column Name  | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                             |
|--------------|------------|------------|-------------|--------------------------|---------------|-------------------------------------|
| doc_entry    | int        |            | Y           |                          |               | id bảng giá                         |
| list_name    | string     | 254        | N           |                          |               | Tên bảng giá                        |
| is_gross_prc | string     | 1          | N           | * Y: Yes<br>* N: No      | N             | Giá sau vat                         |
| valid_for    | string     | 1          | N           | * Y: Yes<br>* N: No      | Y             | áp dụng cho khoảng thời gian        |
| valid_from   | date       |            | N           |                          |               | Thời gian bắt đầu áp dụng           |
| valid_to     | date       |            | N           |                          |               | thời gian kết thúc áp dụng bảng giá |
| prim_curr    | string     | 3          | N           | link apz_ocrn            |               | đồng tiền mặc định của bảng giá     |
| user_sign    | smallint   |            | N           | link apz_ousr            |               | người tạo                           |
| remarks      | string     | 254        | N           |                          |               | Ghi chú                             |
| create_date  | date       |            | N           |                          |               | Ngày tạo                            |
| create_time  | smallint   |            | N           |                          |               | Thời gian tạo                       |
| update_date  | date       |            | N           |                          |               | Ngày cập nhật                       |
| update_time  | smallint   |            | N           |                          |               | thời gian cập nhật                  |
| user_sign2   | smallint   |            | N           | link apz_ousr            |               | người update cuối cùng              |

**Item Price Table**: `apz_itm1`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value                 | Remarks                  |
|-------------|------------|------------|-------------|--------------------------|-------------------------------|--------------------------|
| item_code   | string     | 50         | Y           | link apz_oitm            |                               | mã hàng                  |
| price_list  | int        |            | Y           | link apz_opln            |                               | mã bảng giá              |
| price       | numeric    | 19,6       | N           |                          |                               | giá                      |
| currency    | string     | 3          | N           | link apz_ocrn            | primary currency của bảng giá | đống tiền của giá        |
| add_price1  | numeric    | 19,6       | N           |                          |                               | giá info 1               |
| currency1   | string     | 3          | N           | link apz_ocrn            | primary currency của bảng giá | đồng tiền của giá info 1 |
| add_price2  | numeric    | 19,6       | N           |                          |                               | giá info 2               |
| currency2   | string     | 3          | N           | link apz_ocrn            | primary currency của bảng giá | đồng tiền của giá info 2 |
| uom_code    | string     | 50         | N           | link apz_ouom            | đơn vị lưu kho                | đơn vị tính              |

# 01 API GET PRICE LIST

**Remarks**: Hiển thị danh sách bảng giá theo điều kiện:

**URL**: `/PriceList/Get`

**Method**: `POST`

**Body**:

**Auth Required**: `Bearer token`

> Get All

```json
{
}
```

> Get By Condition

```json
{
  "doc_entry": 1,
  "name": "Bảng giá 1",
  "from_date": "2020-03-04",
  "to_date": "2021-03-04",
  "search_name": "some text"
}
```

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
        "list_name": "IN",
        "is_gross_prc": "N",
        "valid_for": "Y",
        "valid_from": null,
        "valid_to": null,
        "prim_curr": "VND",
        "user_sign": -1,
        "remarks": "Bảng giá nhập IT",
        "create_date": "2021-04-09T00:00:00",
        "create_time": 1600,
        "update_date": "2021-04-09T00:00:00",
        "update_time": 1700,
        "user_sign2": -1,
        "row_num": 1
      },
      {
        "doc_entry": 2,
        "list_name": "OUT",
        "is_gross_prc": "N",
        "valid_for": "Y",
        "valid_from": null,
        "valid_to": null,
        "prim_curr": "VND",
        "user_sign": -1,
        "remarks": "Bảng giá xuất ",
        "create_date": "2021-04-12T00:00:00",
        "create_time": 1000,
        "update_date": "2021-04-12T00:00:00",
        "update_time": 1500,
        "user_sign2": -1,
        "row_num": 2
      }
    ]
  },
  "message": null
}
```

**msg_code**:

```text
200 - SUCCESS,
3 - EXCEPTION,
other - ERROR 
```

# 02 API GET BY ENTRY AND USER_SIGN

**Remarks**: Hiển thị chi tiết bảng giá theo theo doc_entry và danh sách giá của item

**URL** `/PriceList/GetByEntry?entry=1&user_id=-1`

**URI Parameter**:

1. entry: doc_entry của price list,
2. user_id: id của user đăng nhập hệ thống (hiện tại chưa dùng đến)

**Method**: `GET`

**Body**: `NONE`

**Auth Required**: `Bearer token`

**Permission Required**: `NONE`

## Response

```json
{
  "msg_code": 200,
  "content": {
    "price_list": [
      {
        "doc_entry": 1,
        "list_name": "IN",
        "is_gross_prc": "N",
        "valid_for": "Y",
        "valid_from": null,
        "valid_to": null,
        "prim_curr": "VND",
        "user_sign": -1,
        "remarks": "Bảng giá nhập IT",
        "create_date": "2021-04-09T00:00:00",
        "create_time": 1600,
        "update_date": "2021-04-09T00:00:00",
        "update_time": 1700,
        "user_sign2": -1
      }
    ],
    "item_prices": [
      {
        "item_code": "S10001",
        "price_list": 1,
        "price": 20000.000000,
        "currency": "VND",
        "add_price1": null,
        "currency1": null,
        "add_price2": null,
        "currency2": null,
        "uom_code": "KG"
      },
      {
        "item_code": "S10002",
        "price_list": 1,
        "price": 50000.000000,
        "currency": "VND",
        "add_price1": null,
        "currency1": null,
        "add_price2": null,
        "currency2": null,
        "uom_code": "KG"
      }
    ]
  },
  "message": null
}
```

**msg_code**:

```text
200 - Success,
3 - exception,
other - error
```

# 03 API CREATE PRICE LIST

**Remarks**: API tạo mới bảng giá và danh sách giá item trong bảng giá đó

**URL**: `/PriceList/Create`

**Method**: `POST`

**Body**:

```json
{
  "price_list": [
    {
      "list_name": "Bảng giá nhập",
      "is_gross_prc": "N",
      "valid_for": "Y",
      "valid_from": "2021-02-03",
      "valid_to": "2021-04-03",
      "prim_curr": "VND",
      "remarks": "Bảng giá nhập của Các mặt hàng"
    }
  ],
  "item_prices": [
    {
      "item_code": "S10001",
      "price": 10000.44,
      "currency": "VND",
      "add_price1": 0,
      "currency1": "",
      "add_price2": 0,
      "currency2": "",
      "uom_code": "KG"
    },
    {
      "item_code": "S10003",
      "price": 6000.44,
      "currency": "VND",
      "add_price1": 0,
      "currency1": "",
      "add_price2": 0,
      "currency2": "",
      "uom_code": "KG"
    },
    {
      "item_code": "S02233",
      "price": 6000.44,
      "currency": "VND",
      "add_price1": 0,
      "currency1": "",
      "add_price2": 0,
      "currency2": "",
      "uom_code": "KG"
    }
  ]
}
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

```text
200 - Success
3 - Exception
other - Error
```

# 04 API UPDATE PRICE LIST

**Remarks**: Cập nhật bảng giá theo theo doc_entry và danh sách giá của item

**URL** `/PriceList/Update`

**Method**: `POST`

**Body**:

```json
{
  "price_list": [
    {
      "doc_entry": 1,
      "list_name": "Cập nhật bảng giá",
      "is_gross_prc": "N",
      "valid_for": "Y",
      "valid_from": "2021-02-03",
      "valid_to": "2021-04-03",
      "prim_curr": "VND",
      "remarks": "Gold"
    }
  ],
  "item_prices": [
    {
      "item_code": "S10001",
      "price": 10000.44,
      "currency": "VND",
      "add_price1": 0,
      "currency1": "",
      "add_price2": 0,
      "currency2": "",
      "uom_code": "KG"
    },
    {
      "item_code": "S10003",
      "price": 6000.44,
      "currency": "VND",
      "add_price1": 0,
      "currency1": "",
      "add_price2": 0,
      "currency2": "",
      "uom_code": "KG"
    }
  ]
}
```

**Auth Required**: `Bearer token`

**Permission Required**: `NONE`

## Response

```json
{
  "msg_code": 200,
  "content": null,
  "message": ""
}
```

**msg_code**
```text
200 - Success
12 - Doc entry not provided
400010 - Doc entry not found
3 - Exception
other - Error
```

# 05 API DELETE PRICE LIST

**Remarks**: Xóa bảng giá 

**URL** `/PriceList/Delete`

**Method**: `POST`

**Body**:

```json
{
    "doc_entry": 3
}
```

**Auth Required**: `Bearer token`

**Permission Required**: `NONE`

## Response

```json
{
  "msg_code": 200,
  "content": null,
  "message": "3"
}
```

**msg_code**
```text
200 - Success
12 - Doc entry not provided
400010 - Doc entry not found
400220 - RecordOnTransaction
500020 - Price list in used on business partner
3 - Exception
other - Error
```

