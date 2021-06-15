# 00 OBJECT INFO
**Object Type**: `153`

**UDT Table**: `user_table`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table                 | Default Value | Remarks            |
|-------------|------------|------------|-------------|------------------------------------------|---------------|--------------------|
| table_name  | string     | 50         | Y           |                                 |               | table name         |
| remarks     | string     | 254        | N           |                                 |               | Mô tả              |
| table_type  | smallint   |            | Y           | * 0 : No object<br>* 1: Master data header<br>* 2: master data row<br>* 3: Document Header<br>* 4: Document row | 0             | loại table         |
| create_date | date       |            | N           |                                 |               | Ngày tạo           |
| create_time | smallint   |            | N           |                                 |               | Thời gian tạo      |
| user_sign   | smallint   |            | N           |                                 |               | Người tạo          |
| update_date | date       |            | N           |                                 |               | ngày cập nhật      |
| update_time | smallint   |            | N           |                                 |               | thời gian cập nhật |
| user_sign2  | smallint   |            | N           |                                 |               | người cập nhật     |

# 01 API GET UDT BY ID

**URL** : `/UDT/GetById?id=table_4_3`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 11,
    "content": [
        {
            "table_name": "table_4_3",
            "remarks": "Bảng test DocumentRow 3",
            "table_type": 4,
            "create_date": "2021-04-26T00:00:00",
            "create_time": 403,
            "user_sign": 12,
            "update_date": null,
            "update_time": null,
            "user_sign2": null
        }
    ],
    "message": "Success."
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 02 API GET ALL

**URL** : `/UDT/Get`

**Method** : `POST`

**Body** :
```json
{
    "name":"table_1_1",
    "user_sign":"",
    "page_index":"",
    "page_size":"",
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
                "table_name": "table_1_1",
                "remarks": "Bảng test MasterDataHeader 1",
                "table_type": 1,
                "create_date": "2021-04-26T00:00:00",
                "create_time": 402,
                "user_sign": 12,
                "update_date": null,
                "update_time": null,
                "user_sign2": null,
                "row_num": 1
            }
        ]
    },
    "message": "Success."
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 03 API CREATE UDT

**URL** : `/UDT/Create`

**Method** : `POST`

**Body** :
```json
[
    {
        "table_name":"table_0_2",
        "table_type":0,
        "remarks":"Bảng test NoObject 2"
    }
]
```

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "Success."
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 04 API UPDATE UDT

**URL** : `/UDT/Update`

**Method** : `POST`

**Body** :
```json
[
    {
        "table_name":"table_0_2",
        "remarks":"Cập nhật bảng NoObject 2"
    }
]
```

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "Success."
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 05 API DELETE UDT

**URL** : `/UDT/Delete`

**Method** : `POST`

**Body** :
```json
{
    "name":"table_0_2"
}
```

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "Success"
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```
