# 00 OBJECT INFO
**Object Type**: `-1`
**Object Table**: `apz_ousr`

# 01 API GET LIST DATABASE

**URL** : `/User/GetListDatabase`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `NONE`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": [
            {
                "db_code": "POSMAN",                     Database Name
                "db_name": "APZON"                     Tên công ty
            }
        ],
    "message": null
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 02 API LOGIN

**URL** : `/User/Login`

**Method** : `POST`

**Body** :
```JSON
{
	"db_code" : "posman",
	"user_name" : "thanhnv",
	"password": "123456"
}
```

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": {
            "access_token": "aKZecs/v+DTVlDkWp5PuSr6WGqoy7Q==",             Access token truy cập hệ thống
            "user_info": {
                "user_code": "thanhnv",                                     Mã người dùng
                "user_name": "Nguyễn Văn Thành",                            Tên người dùng
                "user_id": 1                                                ID người dùng trên hệ thống
            },
            "license_info": {
                "user_code": "sonnv",
                "is_user_license": true,
                "user_rule": 0,
                "rule": 0,
                "list_license_type": [
                    {
                        "type_code": "P",
                        "type_name": "Pro",
                        "license_rule": 0,
                        "license_status": true,
                        "is_mapping_user": true,
                        "amount": 10,
                        "used": 2,
                        "expiry_date": "2021-06-19T00:00:00",
                        "list_access_form": null
                    }
                ],
                "list_access_form": null,
                "expiry_date": "2021-06-19T00:00:00"
            }
    },
    "message": null
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

 

## Notes

* sone extra notes