# 00 OBJECT INFO
**Object Type**: `10011`

**Auth Table**: `auth`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table             | Default Value | Remarks                    |
|-------------|------------|------------|-------------|--------------------------------------|---------------|----------------------------|
| menu_code   | string     | 50         | Y           | Linked apz_omni                      |               | Mã chức năng               |
| menu_name   | string     | 254        | N           |                                      |               | Tên chức năng              |
| father_code | string     | 50         | N           |                                      |               | Folder trên menu           |
| menu_type   | string     | 1          | Y           | * F: function<br>* P: Path           | F             | loại menu                  |
| auth        | string     | 1          | N           | * F: full<br>* R: Read only<br>* N: None | N             | Giá trị phân quyền         |
| user_id     | smallint   |            | Y           | Linked apz_ousr                      |               | người dùng được phân quyền |

**Menu Table**: `menu`

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table   | Default Value | Remarks                          |
|---------------|------------|------------|-------------|----------------------------|---------------|----------------------------------|
| menu_code     | string     | 50         | Y           | Linked apz_omni            |               | Mã chức năng                     |
| menu_name     | string     | 254        | N           |                            |               | Tên chức năng                    |
| father_code   | string     | 50         | N           |                            |               | Folder trên menu                 |
| menu_type     | string     | 1          | Y           | * F: function<br>* P: Path | F             | loại menu                        |
| status        | string     | 1          | N           | * A: active<br>* I: inactive | A             | trạng thái sử dụng trên hệ thống |


**User Table**: `user_info`

* link tới api user

# 01 API GET LIST MENU USER AUTH

**Remarks: lấy danh sách phân quyên chức năng của người dùng**:

**URL** : `/Auth/Get?user_id=1`

**URI Paramater**:

1. user_id                         id người dùng lấy danh sách phân quyền

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
            "menu_code": "1001",
            "menu_name": "Sales Order",
            "father_code": "101",
            "menu_type": "F",
            "auth": "F",
            "user_id": 1
        }
    ],
    "message": null
}
```

**MsgCode**: 
```text
200     - Success
3       - Exception
Other   - error
```

# 02 API UPDATE AUTH USER

**Remarks: Cập nhật dữ liệu phân quyền chức năng của người dùng**

**URL** : `/Auth/Update`

**Method** : `POST`

**Body** : 
```json
[
    {
        "menu_code": "1001",                        function id
        "menu_name": "Sales Order",                 Tên chức năng
        "father_code": "101",                       folder chứa chức năng
        "menu_type": "F",                           Loain menu. F: function - P: Path(folder)
        "auth": "F",                                giá trị phân quyền: F: full - R: Readonly - N: none không có quyền gì
        "user_id": 1                                id user được phân quyền
    }
]
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
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 03 API UPDATE THÔNG TIN MENU LIST

**Remarks: Update lại dữ liệu cây menu**

**URL** : `/Auth/UpdateMenuItem`

**Method** : `POST`

**Body** : 
```json
[
    {
        "menu_code": "1001",                        function id
        "menu_name": "Sales Order",                 Tên chức năng
        "father_code": "101",                       folder chứa chức năng
        "menu_type": "F",                           Loain menu. F: function - P: Path(folder)
        "status": "A",                              trạng thái chức năng: A: active - N: Not active
    }
]
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
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```