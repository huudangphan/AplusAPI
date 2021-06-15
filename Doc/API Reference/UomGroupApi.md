# # 00 OBJECT INFO
**Object Type**: `8102`

**UnitOfMesureGroup Table**: `unit_group` 

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table| Default Value | Remarks       |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| ugp_entry         | integer    |            | Y           |                       |           | Mã Uom Group              |
| ugp_name          | string     | 100        | N           |                       |           | Tên Uom Group             |
| base_uom          | string     | 50         | N           | link apz_ouom         |           | Đơn vị tính base          |
| data_source       | string     | 1          | N           | * S: System<br>* N: Front-end function| N | Nguồn tạo dữ liệu   |
| log_instance      | integer    |            | N           |                       |           |   |
| status            | string     | 1          | N           | * A: Active<br>* I: Inactive |    | Trạng thái của Uom Gourp  |
| create_date       | date       |            | N           |                       |           | Ngày tạo                  |
| create_time       | integer    |            | N           |                       |           | Thời gian tạo             |
| user_sign         | integer    |            | N           | link apz_ousr         |           | Người tạo                 |
| update_date       | date       |            | N           |                       |           | Ngày cập nhật             |
| update_time       | integer    |            | N           |                       |           | Thời gian cập nhật        |
| user_sign2        | integer    |            | N           | link apz_ousr         |           | Người cập nhật            |


**BusinessPartner Table**: `unit_group_1` 

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table| Default Value | Remarks       |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| ugp_entry         | integer     |           | Y           | link unit_group |           | Mã nhóm đơn vị tính               |
| uom_code          | string      | 50        | Y           | link apz_ouom |           | Mã đơn vị tính                    |
| alt_qty           | numeric     | 19,6      | N           |               |           | Tỉ lệ quy đổi so với đơn vị cơ sở |
| base_qty          | numeric     | 19,6      | N           |               |           | Số lượng đơn vị cơ sở             |
| log_instance      | integer     |           | N           |               |           | |
| line_num          | integer     |           | N           |               |           | Line Id                           |
| wght_factor       | smallint    |           | N           |               |           | Hệ số (hiện không dùng)           |
| udf_factor        | integer     |           | N           |               |           | Hệ số                             |
| is_base_uom       | string      | 1         | N           | * Y: Yes<br>* N: No  |           | Có phải đơn vị cơ sở không. một group chỉ có một đơn vị cơ sở |

# 01 API GET UnitOfMesureGroup BY ID

**URL** : `/UnitOfMesureGroup/GetById?id=1`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "group": [
            {
                "ugp_entry": 1,
                "ugp_name": "Khối Lượng",
                "base_uom": "KG",
                "data_source": "N",
                "user_sign": -1,
                "log_instance": 0,
                "update_date": "2021-04-12T00:00:00",
                "create_date": "2021-04-12T00:00:00",
                "update_time": null,
                "create_time": null,
                "status": "N",
                "user_sign2": -1
            }
        ],
        "group_details": [
            {
                "ugp_entry": 1,
                "uom_code": "KG",
                "alt_qty": 1.123000000,
                "base_qty": 2.123000000,
                "log_instance": 0,
                "line_num": 1,
                "wght_factor": 0,
                "udf_factor": -1,
                "is_base_uom": "Y",
                "name": "Kilogram"
            },
            {
                "ugp_entry": 1,
                "uom_code": "GAM",
                "alt_qty": 2.456000000,
                "base_qty": 1.457000000,
                "log_instance": 0,
                "line_num": 2,
                "wght_factor": 0,
                "udf_factor": -1,
                "is_base_uom": "N",
                "name": "gam"
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

# 02 API GET ALL

**URL** : `/UnitOfMesureGroup/Get`

**Method** : `POST`

**Body** :
```json
{
    "code":"",
    "name":"",
    "user_sign":"",
    "branch":"",
    "page_index":"1",
    "page_size":"1",
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
                "page_index": 1,
                "page_total": 1,
                "page_size": 1,
                "sum_record": 1
            }
        ],
        "data": [
            {
                "ugp_entry": 1,
                "ugp_name": "Khối Lượng",
                "base_uom": "KG",
                "data_source": "N",
                "user_sign": -1,
                "log_instance": 0,
                "update_date": "2021-04-12T00:00:00",
                "create_date": "2021-04-12T00:00:00",
                "update_time": null,
                "create_time": null,
                "status": "N",
                "user_sign2": -1,
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

# 03 API CREATE UnitOfMesureGroup

**URL** : `/UnitOfMesureGroup/Create`

**Method** : `POST`

**Body** :
```json
{
    "uom_group": [
        {
            "ugp_name":"Độ dài",
            "base_uom":"MET",
            "datasource":"N"
        }
    ],
    "uom_group_data":[
        {
            "uom_code":"MET",
            "alt_qty":0,
            "base_qty":0,
            "is_base_uom":"Y"
        }
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
    "message": "2"       Ugp_entry
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 04 API UPDATE UnitOfMesureGroup

**URL** : `/UnitOfMesureGroup/Update`

**Method** : `POST`

**Body** :
```json
{
    "uom_group": [
        {
            "ugp_entry":1,
            "ugp_name":"Khối Lượng",
            "base_uom":"KG",
            "datasource":"N"
        }
    ],
    "uom_group_data":[
        {
            "uom_code":"KG",
            "alt_qty":1.123,
            "base_qty":2.123,
            "is_base_uom":"Y"
        },
        {
            "uom_code":"GAM",
            "alt_qty":2.456,
            "base_qty":1.457,
            "is_base_uom":"N"
        }
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
    "message": "Success."
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 05 API DELETE UnitOfMesureGroup

**URL** : `/UnitOfMesureGroup/Delete`

**Method** : `POST`

**Body** :
```json
{
    "code":2            ugp_entry
}
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