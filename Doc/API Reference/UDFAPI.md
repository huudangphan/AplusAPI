# 00 OBJECT INFO
**Object Type**: `152`

**Setting field Table**: `apz_oudf`

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table                 | Default Value | Remarks                                  |
|---------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| table_name      | string     | 50         | Y           |                                          |               | id table tạo udf                         |
| column_id     | string     | 50         | Y           |                                          |               | Tên cột udf                              |
| column_name   | string     | 254        | Y           |                                          |               | Tên hiển thị                             |
| data_type     | string     | 20         | Y           | 1. string<br>2. int<br>3. numeric<br>4. datetime | string        | kiểu dữ liệu của trường                  |
| sub_type      | string     | 20         | N           | 1. Phone<br>2. Email<br>3. Address<br>4. text<br>5. Amount<br>6. Quantity<br>7. Price<br>8. Rate<br>9. Percent |               | kiểu dữ liệu phụ. với kiểu string có 4 kiểu phụ: 1=> 4<br>- còn lại là kiểu phụ của numeric |
| size          | int        |            | N           |                                          |               | Kích thước tối đa                        |
| default_value | string     | 254        | N           |                                          |               | giá trị mặc định                         |
| is_required   | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | trường bắt buộc                          |
| linked_type    | string     | 10         | N           | * 00: không kink<br>* 01 : link system object<br>* 02: link udo <br>* 03: link udt<br>* 04: valid lookup data(Code - Name)<br>* 05 - valid list data | 00            | Kiểu link dữ liệu                        |
| linked_object | string     | 50         | N           |                                          |               | object type của object được link         |
| object_type   | string     | 20         | N           |                                          |               | Object type của object tạo udf           |
| layout_id     | int        |            | N           | link apz_udf1                            |               | Layoud id của udf                        |
| create_date   | date       |            | N           |                                          |               | Ngày tạo                                 |
| create_time   | smallint   |            | N           |                                          |               | thời gian tạo                            |
| update_date   | date       |            | N           |                                          |               | ngày tạo                                 |
| update_time   | smallint   |            | N           |                                          |               | Thời gian tạo                            |
| user_sign     | smallint   |            | N           | link apz_ousr                            |               | người tạo                                |
| user_sign2    | smallint   |            | N           | link apz_ousr                            |               | người update cuối cùng                   |

**Valid Field Table**: `apz_udf2`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks            |
|-------------|------------|------------|-------------|--------------------------|---------------|--------------------|
| table_name    | string     | 50         | Y           |                          |               | table id           |
| column_id   | string     | 50         | Y           |                          |               | Column Id          |
| key_code    | string     | 50         | Y           |                          |               | Giá trị của trường |
| key_name    | string     | 254        | N           |                          |               | tên hiển thị       |

**UDF layout**: `apz_udf1`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks            |
|-------------|------------|------------|-------------|--------------------------|---------------|--------------------|
| line_num    | int        |            | Y           |                          |               | số thứ tự          |
| layout_name | string     | 50         | Y           |                          |               | tên bản link       |
| table_name  | string     | 50         | N           |                          |               | tên bản link       |


valid lookup : use key (code name)
valid list : not use key

# 01 API GET UDF BY CONDITION

**Remarks**: Lấy tất cả User Define Field theo điều kiện

**URL**: `/Udf/Get`

**Method**: `POST`

**Body**:

(get all)

```json
{
  
}
```

(by condition)

```json
{
  "table_name": "apz_test",
  "column_id": "udf_test",
  "column_name": "test",
  "data_type": "string",
  "search_name": "some text",
  "page_index": 0,
  "page_size": 0
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`


## Response

```json
{
    "msg_code": 200,
    "content": [...udf list],
    "message": null
}
```

# 02 API CREATE UDF

**Remarks**: Tạo mới User Define Field 

**URL**: `/Udf/Create`

**Method**: `POST`

**Body**:

```json
{
  "udf": [
    {
      "table_name": "apz_test",
      "column_id": "LetMesEE",
      "column_name": "Console",
      "data_type": "string",
      "default_value": "sdfsdfa",
      "size": 0,
      "is_required": "Y",
      "linked_type": "00",
      "linked_object": "",
    }
  ],
  "udf2": [
    (feature comming soon - now just leave it)
  ]
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": null
}
```

# 03 API UPDATE UDF

**Remarks**: Tạo mới User Define Field

**URL**: `/Udf/Update`

**Method**: `POST`

**Body**:

```json
{
  "udf": [
    {
      "table_name": "apz_test",
      "column_id": "LetMesEE",
      "column_name": "Console",
      "data_type": "string",
      "default_value": "sdfsdfa",
      "size": 0,
      "is_required": "Y",
      "linked_type": "00",
      "linked_object": "",
    }
  ],
  "udf2": [
    (feature comming soon - now just leave it)
  ]
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": null
}
```

# 04 API DELTE UDF

**Remarks**: Tạo mới User Define Field

**URL**: `/Udf/Create`

**Method**: `POST`

**Body**:

```json
{
  "table_name": "apz_test",
  "column_id": "LetMesEE"
}
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "LetMesEE"
}
```



