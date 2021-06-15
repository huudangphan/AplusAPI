# 00 OBJECT INFO
**Object Type**: `206`

**UDT Table**: `user_object`

| Column Name    | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                                  |
|----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| code           | string     | 50         | Y           |                          |               | Mã udo                |
| name           | string     | 254        | N           |                          |               | Tên Udo               |
| header_table   | string     | 50         | Y           | link apz_oudt            |               | table header          |
| object_type    | string     | 50         | Y           |                          |               | object type           |
| type           | string     | 50         | Y           | * Document<br>* MasterData |             | udo type              |
| mng_series     | string     | 1          | N           | * Y: Yes<br>* N: No      | N             | quản lý theo series. hiện tại không dùng => để giá trị default |
| can_delete     | string     | 1          | N           | * Y: Yes<br>* N: No      | Y             | có thể xóa bản ghi    |
| can_close      | string     | 1          | N           | * Y: Yes<br>* N: No      | Y             | có thể colose         |
| can_cancel     | string     | 1          | N           | * Y: Yes<br>* N: No      | Y             | có thể cancel         |
| can_find       | string     | 1          | N           | * Y: Yes<br>* N: No      | Y             | có thể tìm kiếm       |
| menu_item      | string     | 1          | N           | * Y: Yes<br>* N: No      | Y             | Có hiển thị menu chức năng    |
| menu_caption   | string     | 254        | N           |                          |               | Tên chức năng trên menu  |
| menu_type      | string     | 1          | N           | * H : header - line stype<br>* M : matrix tab | H             | loại form                                |
| father_menu    | string     | 254        | N           |                          |               | menu cha chứa udo     |
| menu_postition | int        |            | N           |                          |               | vị trí trên menu cha. Mặc định ở cuối cùng |
| create_date    | date       |            | N           |                          |               | Ngày tạo udo          |
| create_time    | smallint   |            | N           |                          |               | Thời gian tạo         |
| user_sign      | smallint   |            | N           | link apz_ousr            |               | người tạo             |
| update_date    | date       |            | N           |                          |               | ngày cập nhật         |
| update_time    | smallint   |            | N           |                          |               | thời gian cập nhật    |
| user_sign2     | smallint   |            | N           | link apz_ousr            |               | Người cập nhật cuối cùng |

**UDT Child Table**: `user_object_child`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                  |
|-------------|------------|------------|-------------|--------------------------|---------------|--------------------------|
| code        | string     | 50         | Y           | link apz_oudt            |               | Mã udo                   |
| child_table | string     | 50         | Y           | link apz_oudt            |               | table con                |
| child_name  | string     | 254        | N           |                          |               | Tên của table con        |
| child_num   | int        |            | N           |                          |               | Số thứ tự của bảng con   |
| create_date | date       |            | N           |                          |               | Ngày tạo                 |
| create_time | smallint   |            | N           |                          |               | thời gian tạo            |
| user_sign   | smallint   |            | N           | link apz_ousr            |               | người tạo                |
| update_date | date       |            | N           |                          |               | ngày cập nhật            |
| update_time | smallint   |            | N           |                          |               | thời gian cập nhật       |
| user_sign2  | smallint   |            | N           |                          |               | Người cập nhật cuối cùng |

**UDT find column Table**: `user_object_search`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                  |
|-------------|------------|------------|-------------|--------------------------|---------------|--------------------------|
| code        | string     | 50         | Y           | link apz_oudt            |               | Mã udo                   |
| col_code    | string     | 50         | Y           |                          |               | column Name              |
| col_name    | string     | 254        | N           |                          |               | Tên hiển thị trên màn hình search |
| col_num     | int        |            | N           |                          |               | số thứ tự                |
| create_date | date       |            | N           |                          |               | ngày tạo                 |
| create_time | smallint   |            | N           |                          |               | thời gian tạo            |
| user_sign   | smallint   |            | N           | link apz_ousr            |               | người tạo                |
| update_date | date       |            | N           |                          |               | Ngày cập nhật            |
| update_time | smallint   |            | N           |                          |               | thời gian cập nhật       |
| user_sign2  | smallint   |            | N           | link apz_ousr            |               | người cập nhật cuối cùng |

**UDT default view column Table**: `user_object_default`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                  |
|-------------|------------|------------|-------------|--------------------------|---------------|--------------------------|
| code        | string     | 50         | Y           | link apz_oudt            |               | Mã udo                   |
| col_code    | string     | 50         | Y           |                          |               | column Name              |
| col_name    | string     | 254        | N           |                          |               | tên hiển thị trên view   |
| col_num     | int        |            | N           |                          |               | số thứ tự                |
| col_edit    | string     | 1          | N           | * Y : Yes<br>* N : No    | Y             | cho phép sửa             |
| create_date | date       |            | N           |                          |               | ngày tạo                 |
| create_time | smallint   |            | N           |                          |               | thời gian tạo            |
| user_sign   | smallint   |            | N           |                          |               | người tạo                |
| update_date | date       |            | N           |                          |               | ngày cập nhật            |
| update_time | smallint   |            | N           |                          |               | thời gian cập nhật       |
| user_sign2  | smallint   |            | N           |                          |               | người cập nhật cuối cùng |

**UDT default view child column Table**: `user_object_child_default`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                  |
|-------------|------------|------------|-------------|--------------------------|---------------|--------------------------|
| code        | string     | 50         | Y           | link apz_oudt            |               | Mã udo                   |
| child_table | string     | 50         | Y           | link apz_oudt            |               | table con                |
| col_code    | string     | 50         | Y           |                          |               | column Name              |
| col_name    | string     | 254        | N           |                          |               | tên hiển thị trên view   |
| col_num     | int        |            | N           |                          |               | số thứ tự                |
| col_edit    | string     | 1          | N           | * Y : Yes<br>* N : No    | Y             | cho phép sửa             |
| create_date | date       |            | N           |                          |               | ngày tạo                 |
| create_time | smallint   |            | N           |                          |               | thời gian tạo            |
| user_sign   | smallint   |            | N           |                          |               | người tạo                |
| update_date | date       |            | N           |                          |               | ngày cập nhật            |
| update_time | smallint   |            | N           |                          |               | thời gian cập nhật       |
| user_sign2  | smallint   |            | N           |                          |               | người cập nhật cuối cùng |


# 01 API GET UDO BY ID

**URL** : `/UDO/GetById?id=object_1` &emsp;&emsp;&emsp; `id là object_code`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "udo_info": [
            {
                "code": "object_1",
                "name": "object_1_updated",
                "header_table": "table_3_1",
                "object_type": "object_1",
                "type": "Document",
                "mng_series": "N",
                "can_delete": "Y",
                "can_cancel": "Y",
                "can_find": "Y",
                "menu_item": "Y",
                "menu_caption": null,
                "menu_type": "H",
                "father_menu": null,
                "menu_postition": null,
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12,
                "can_close": "Y"
            }
        ],
        "udo_child": [
            {
                "code": "object_1",
                "child_table": "table_4_1",
                "child_name": "childname1_updated",
                "child_num": 1,
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12
            }
        ],
        "default_find": [
            {
                "code": "object_1",
                "col_code": "doc_entry",
                "col_name": "doc_entry",
                "col_num": 1,
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12
            },
            {
                "code": "object_1",
                "col_code": "doc_num",
                "col_name": "doc_num",
                "col_num": 2,
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12
            },
            {
                "code": "object_1",
                "col_code": "period",
                "col_name": "period",
                "col_num": 3,
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12
            }
        ],
        "header_view": [
            {
                "code": "object_1",
                "col_code": "doc_entry",
                "col_name": "doc_entry",
                "col_num": 1,
                "col_edit": "Y",
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12
            },
            {
                "code": "object_1",
                "col_code": "doc_num",
                "col_name": "doc_num",
                "col_num": 2,
                "col_edit": "Y",
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12
            },
            {
                "code": "object_1",
                "col_code": "period",
                "col_name": "period",
                "col_num": 3,
                "col_edit": "Y",
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12
            }
        ],
        "child_view": [
            {
                "code": "object_1",
                "child_table": "table_4_1",
                "col_code": "doc_entry",
                "col_name": "doc_entry",
                "col_num": 1,
                "col_edit": "Y",
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12
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

# 02 API GET UDO TABLE

**URL** : `/UDO/GetUdoTable?type=3` &emsp;&emsp;&emsp; `Type: 1: MasterDataHeader, 2: MasterDataRow, 3: DocumentHeader, 4: DocumentRow`

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
            "table_name": "table_3_3",
            "remarks": "Bảng test DocumentHeader 3",
            "table_type": 3,
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

# 03 API GET ALL

**URL** : `/UDO/Get`

**Method** : `POST`

**Body** :
```json
{
    "code":"",
    "name":"",
    "user_sign":"",
    "page_index":0,
    "page_size":0,
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
                "code": "object_1",
                "name": "object_1_updated",
                "header_table": "table_3_1",
                "object_type": "object_1",
                "type": "Document",
                "mng_series": "N",
                "can_delete": "Y",
                "can_cancel": "Y",
                "can_find": "Y",
                "menu_item": "Y",
                "menu_caption": null,
                "menu_type": "H",
                "father_menu": null,
                "menu_postition": null,
                "create_date": "2021-04-26T00:00:00",
                "create_time": 205,
                "user_sign": 12,
                "update_date": "2021-04-26T00:00:00",
                "update_time": 205,
                "user_sign2": 12,
                "can_close": "Y",
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

# 04 API CREATE UDO

**URL** : `/UDO/Create`

**Method** : `POST`

**Body** :
```json
{
    "udo_info":[
        {
            "code":"object_2",
            "name":"object_2",
            "header_table":"table_3_2",
            "object_type":"object_2",
            "type":"Document",
            "mng_series":null,
            "can_close":null,
            "can_delete":null,
            "can_cancel":null,
            "can_find":null,
            "menu_item":null,
            "menu_caption":null,
            "menu_type":null,
            "menu_position":null,
            "father_menu":null
        }
    ],
    "udo_child":[
        {
            "code":"object_2",
            "child_table":"table_4_2",
            "child_name":"childname1"
        }
    ],
    "default_find":[    
        {
            "code":"object_2",
            "col_code":"doc_entry",
            "col_name":"doc_entry"
        },
        {
            "code":"object_2",
            "col_code":"doc_num",
            "col_name":"doc_num"
        }
    ],
    "header_view":[
        {
            "code":"object_2",
            "col_code":"doc_entry",
            "col_name":"doc_entry"
        },
        {
            "code":"object_2",
            "col_code":"doc_num",
            "col_name":"doc_num"
        }
    ],
    "child_view":[
        {
            "code":"object_2",
            "child_table":"table_4_2",
            "col_code":"doc_entry",
            "col_name":"doc_entry",
            "col_edit":"N"
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

# 05 API UPDATE UDO

**URL** : `/UDO/Update`

**Method** : `POST`

**Body** :
```json
{
    "udo_info":[
        {
            "code":"object_2",
            "name":"update_object_2",
            "header_table":"table_3_2",
            "object_type":"object_2",
            "type":"Document",
            "mng_series":null,
            "can_close":null,
            "can_delete":null,
            "can_cancel":null,
            "can_find":null,
            "menu_item":null,
            "menu_caption":null,
            "menu_type":null,
            "menu_position":null,
            "father_menu":null
        }
    ],
    "udo_child":[
        {
            "code":"object_2",
            "child_table":"table_4_2",
            "child_name":"Update_childname1"
        }
    ],
    "default_find":[    
        {
            "code":"object_2",
            "col_code":"doc_entry",
            "col_name":"doc_entry"
        },
        {
            "code":"object_2",
            "col_code":"doc_num",
            "col_name":"doc_num"
        }
    ],
    "header_view":[
        {
            "code":"object_2",
            "col_code":"doc_entry",
            "col_name":"doc_entry"
        },
        {
            "code":"object_2",
            "col_code":"doc_num",
            "col_name":"doc_num"
        }
    ],
    "child_view":[
        {
            "code":"object_2",
            "child_table":"table_4_2",
            "col_code":"doc_entry",
            "col_name":"doc_entry",
            "col_edit":"N"
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

# 06 API DELETE UDO

**URL** : `/UDO/Delete`

**Method** : `POST`

**Body** :
```json
{
    "code":"object_2"
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
