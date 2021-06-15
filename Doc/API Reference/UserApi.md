# 00 OBJECT INFO
**Object Type**: `12`

**User Table**: `user_info`  

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table| Default Value | Remarks       |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| user_id           | integer    |            | Y           | 1000,...              |           | Mã Người dùng             |
| user_code         | string     | 254        | Y           |                       |           | Tên đăng nhập             |
| user_name         | string     | 254        | N           |                       |           | Tên người dùng            |
| password          | string     | 254        | N           |                       |           | Mật khẩu                  |
| email             | string     | 254        | N           |                       |           | Email                     |
| phone1            | string     | 20         | N           |                       |           | Số điện thoại 1           |
| phone2            | string     | 20         | N           |                       |           | Số điện thoại 2           |
| address           | string     | 254        | N           |                       |           | Địa chỉ                   |
| status            | string     | 1          | N           | * A: Active<br>* I: InActive|A    | Trạng thái người dùng     |
| user_type         | string     | 1          | N           | * S: Super<br>* N: Normal| N      | Loại người dùng           |
| atc_entry         | integer    |            | N           |                       |           |  |
| user_sign         | smallint   |            | N           |                       |           | Người tạo                 |
| create_date       | date       |            | N           |                       |           | Ngày tạo                  |
| create_time       | smallint   |            | N           |                       |           | Thời gian tạo             |
| update_date       | date       |            | N           |                       |           | Ngày cập nhật             |
| update_time       | smallint   |            | N           |                       |           | Thời gian cập nhật        |
| user_sign2        | smallint   |            | N           |                       |           | Người cập nhật            |

**User Table**: `user_company`  

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table| Default Value | Remarks       |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| user_id           | integer    |            | Y           | linked apz_ousr       |           | Mã người dùng             |
| company_code      | string     | 50         | Y           | linked apz_obpl       |           | Mã chi nhánh              |
| company_name      | string     | 254        | N           |                       |           | Tên chi nhánh             |
| create_date       | date       |            | N           |                       |           | Ngày tạo                  |
| create_time       | smallint   |            | N           |                       |           | Thời gian tạo             |
| update_date       | date       |            | N           |                       |           | Ngày cập nhật             |
| update_time       | smallint   |            | N           |                       |           | Thời gian cập nhật        |
| remarks           | string     | 20         | N           |                       |           | Ghi chú                   |
| status            | string     | 20         | N           |                       |           | Trạng thái                |

**User Table**: `user_config_1`  

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table| Default Value | Remarks       |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| user_id           | integer    |            | Y           |                       |           | Mã người dùng             |
| default_company   | smallint   |            | N           | linked apz_obpl       |           | Mã chi nhánh              |
| default_warehouse | smallint   |            | N           | linked apz_owhs       |           | Mã kho                    |
| create_date       | date       |            | N           |                       |           | Ngày tạo                  |
| create_time       | smallint   |            | N           |                       |           | Thời gian tạo             |
| update_date       | date       |            | N           |                       |           | Ngày cập nhật             |
| update_time       | smallint   |            | N           |                       |           | Thời gian cập nhật        |
| remarks           | string     |            | N           |                       |           | Ghi chú                   |

**User interface config Table**: `user_config_2`  

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table| Default Value | Remarks       |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| user_id           | integer    |            | Y           | linked apz_ousr   |           | Mã người dùng             |
| name              | string     | 50         | Y           |                   |           | Mã Config                 |
| code              | string     | 254        | N           |                   |           | Tên Config                |

#  01 API Create
**URL** : `/User/Create`

**Method** : `POST`

**Body** : 
```json
{
   "info":[
      {
         "user_code":"Thienthn1",
         "user_name":"Tran Thien 1",
         "password":"Thienthn1",
         "email":"Thienthn1@apzon.com",
         "phone1":"0574635475",
         "phone2":"",
         "address":"",
         "status":"A",
         "user_type":"N"
      }
   ],
   "company":[
      {
         "company_code":"B2"
      }
   ],
   "system_default":[
        {
            "default_company":"V",
            "default_warehouse":"whs100"
        }
   ]
}

```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": {
        "user_code": "Thienthn1",
        "password": "**************"
    },
    "message": "Created Successfully."
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  02 API Get By Id
**URL** : `/User/GetById?user_id=4`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": {
        "info": [
            {
                "user_id": 4,
                "user_code": "sonnv1",
                "user_name": "Ngo van son 1",
                "password": "",
                "email": "",
                "phone1": "",
                "phone2": "",
                "address": "",
                "create_date": "2021-05-20T00:00:00",
                "create_time": 1138,
                "update_date": "2021-05-21T00:00:00",
                "update_time": 1615,
                "status": "A",
                "user_type": "N",
                "user_sign": 21,
                "user_sign2": 1
            }
        ],
        "company": [
            {
                "company_code": "HN1",
                "company_name": "Hà Nội 2"
            },
            {
                "company_code": "B2",
                "company_name": "Italia 190"
            }
        ],
        "system_default": [
            {
                "default_company": "HN1",
                "default_warehouse": "whs3000"
            }
        ]
    },
    "message": "Success."
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  03 API Get By Code
**URL** : `/User/GetByCode?user_code=sonnv1`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": {
        "info": [
            {
                "user_id": 4,
                "user_code": "sonnv1",
                "user_name": "Ngo van son 1",
                "password": "",
                "email": "",
                "phone1": "",
                "phone2": "",
                "address": "",
                "create_date": "2021-05-20T00:00:00",
                "create_time": 1138,
                "update_date": "2021-05-21T00:00:00",
                "update_time": 1615,
                "status": "A",
                "user_type": "N",
                "user_sign": 21,
                "user_sign2": 1
            }
        ],
        "company": [
            {
                "company_code": "HN1",
                "company_name": "Hà Nội 2"
            },
            {
                "company_code": "B2",
                "company_name": "Italia 190"
            }
        ],
        "system_default": [
            {
                "default_company": "HN1",
                "default_warehouse": "whs3000"
            }
        ]
    },
    "message": "Success."
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  04 API Get List User
**URL** : `/User/Get`

**Method** : `POST`

**Body** :  
```json
{
    "search_name":"",
    "user_sign":"",
    "type":"N",
    "status":"I",
    "company":["ROM1"],
    "page_index":0,
    "page_size":0,
    "from_date":"",
    "to_date":""
}
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

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
                "user_id": 13,
                "user_code": "Testing11",
                "user_name": "Testing11",
                "email": "Testing11@apzon.com",
                "phone1": "0255411199",
                "phone2": "0223401210",
                "address": "999 Cách mạng tháng tám, Tân Bình",
                "create_date": "2021-04-08T00:00:00",
                "create_time": null,
                "user_sign": 1,
                "update_date": "2021-04-17T00:00:00",
                "update_time": 843,
                "user_sign2": 1,
                "status": "I"
            }
        ]
    },
    "message": "Success."
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  05 API Get List Config User
**URL** : `/User/GetListConfig?user_code=sonnv`

**Method** : `GET`

**Body** : `None`

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": [
        {
            "code": "LANGUAGE",
            "name": "en-US"
        },
        {
            "code": "USER_MENU_FAVORITES",
            "name": "SETTING_USER"
        },
        {
            "code": "FORM_INFO",
            "name": "TRUE"
        },
        {
            "code": "AUTO_MENU_HIDDEN",
            "name": "FALSE"
        },
        {
            "code": "SYSTEM_INFO",
            "name": "FALSE"
        }
    ],
    "message": "Success"
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  06 API Get List Company User
**URL** : `/User/GetListCompany?user_code=sonnv`

**Method** : `GET`

**Body** : `None`

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": [
        {
            "company_code": "B2",
            "company_name": "Italia 190",
            "create_date": "2021-05-10T00:00:00",
            "create_time": 314,
            "update_date": "2021-05-20T00:00:00",
            "update_time": 130
        },
        {
            "company_code": "V",
            "company_name": "Vietnam",
            "create_date": "2021-05-10T00:00:00",
            "create_time": 314,
            "update_date": "2021-05-20T00:00:00",
            "update_time": 130
        },
        {
            "company_code": "HN1",
            "company_name": "Hà Nội 2",
            "create_date": "2021-05-10T00:00:00",
            "create_time": 314,
            "update_date": "2021-05-20T00:00:00",
            "update_time": 130
        }
    ],
    "message": "Success"
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  07 API Update Interface Config User
**URL** : `/User/UpdateConfig`

**Method** : `POST`

**Body** : 
```json
[
    {
        "user_code":"sonnv",
        "code":"CODE1",
        "name":"Code1"
    },
    {
        "user_code":"sonnv",
        "code":"CODE2",
        "name":"Code2"
    }
]

```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": null,
    "message": "Success."
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  08 API Update User
**URL** : `/User/Update`

**Method** : `POST`

**Body** : 
```json
{
   "info":[
      {
         "user_code":"Thienthn1",
         "user_name":"Tran Thien 1",
         "password":"Thienthn1",
         "email":"Thienthn1@apzon.com",
         "phone1":"0574635123",
         "phone2":"",
         "address":"",
         "status":"A",
         "user_type":"N"
      }
   ],
   "company":[
      {
         "company_code":"HN1"
      }
   ],
   "system_default":[
        {
            "default_company":"HN1",
            "default_warehouse":"whs100"
        }
   ]
}

```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": {
        "user_code": "Thienthn1",
        "user_name": "Tran Thien 1",
        "email": "Thienthn1@apzon.com",
        "phone1": "0574635123",
        "phone2": "",
        "address": ""
    },
    "message": "Update Successfully."
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  09 API Update Password
**URL** : `/User/UpdatePassword`

**Method** : `POST`

**Body** : 
 ```json
    [{
        "user_code":"Testing2",
        "old_password":"123456apzon",
        "new_password":"1234567apzon"
    }]
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
    {
        "msg_code": 200,
        "content": null,
        "message": "Updated Successfully."
    }
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  10 API Update Status User
**URL** : `/User/UpdateStatus`

**Method** : `POST`

**Body** : 
```json
[{
    "user_code": "thienthn@apzon.com",
    "status":"A"
}]
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
    {
        "msg_code": 200,
        "content": null,
        "message": "Updated Successfully."
    }
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  11 API Delete Interface Config User
**URL** : `/User/DeleteConfig`

**Method** : `POST`

**Body** : 
```json
[{
    "user_code":"sonnv",
    "code":"CODE1"
}]

```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": null,
    "message": "Success."
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```

#  12 API Set Password User
**URL** : `/User/SetPassword`

**Method** : `POST`

**Body** : 
```json
{
    "user_code":"testing1",
    "password":"1234567apzon"
}
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response
```json
{
    "msg_code": 200,
    "content": null,
    "message": "Success."
}
```

***MsgCode*** : 
```text
200     - Success
3       - Exception
Other   - error
```