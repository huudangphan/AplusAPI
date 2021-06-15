
## Navigation:
object_type: [ObjectType.md](../ObjectType.md)

MasterData request body for Modify Data: [request_body.md](request_body.md)

msg_code: [MsgCode.md](../MsgCode.md) 

# 01 API GET LIST BaseMasterData by object_type

**URL** : `/BaseMasterData/Get`

**Method** : `POST`

**Body** : 
```json
{
    "object_type": 52,
    "code": "",
    "name": "",
    "status": "A",
    "page_index": 0,
    "page_size": 0,
    "order_by": "",
    "is_ascending": false,
    "from_date": "14/2/2021",
    "to_date": "22/3/2021"
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
        "list": [
            {
                "code": "ITM2000",
                "name": null,
                "remarks": null,
                "status": "A",
                "user_sign": -1,
                "branch": 0,
                "create_date": "2021-03-19T00:00:00",
                "create_time": 1002,
                "update_date": null,
                "update_time": null,
                "data_source": null,
                "vis_order": null,
                "row_num": 1
            }
        ]
    },
    "message": null
}
```
**msg_code** : 
```text
200     - Success
3       - Exception
Other   - error
```



# 02 Get master data by code with object type

**URL**: `/BaseMasterData/GetById`

**METHOD**: `POST`

**Body**: 
```json
{
    "object_type": 52,
    "code": "ITM2000",
}
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response: 
```json
{
    "msg_code": 200,
    "content": [
        {
            "code": "ITM2000",
            "name": null,
            "remarks": null,
            "status": "A",
            "user_sign": -1,
            "branch": 0,
            "create_date": "2021-03-19T00:00:00",
            "create_time": 1002,
            "update_date": null,
            "update_time": null,
            "data_source": null,
            "vis_order": null
        }
    ],
    "message": null
}
```

# 03 Create base master data with object_type

**URL**: `/BaseMasterData/Create`

**METHOD**: `POST`

**Body**: 
```json
{
    "object_type": 52,
    "data": [
        {
            "code": "ITM2000",
            "name": "Đồ Tàu",
            "remarks": "Sản phẩm từ bên chung hoa đại lục chuyển về",
            "status": "A"
        }
    ]
}
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response: 
```json
{
    "msg_code": 200,
    "content": "",
    "message": null
}
```

**Message code**:
```
200: SUCCESS,
2: ERROR
10004: DATA NOT PROVIDED
10005: CODE NOT PROVIDED
```

# 04 Update base master data with object_type

**URL**: `/BaseMasterData/Update`

**METHOD**: `POST`

**Body**: 
```json
{
    "object_type": 52,
    "data": [
        {
            "code": "ITM2000",
            "name": "Đồ Hàn Quốc",
            "remarks": "Sản phẩm Sịn Sò với các mẫu sành điệu",
            "status": "A"
        }
    ]
}
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response: 
```json
{
    "msg_code": 200,
    "content": "",
    "message": null
}
```

**Message code**:
```
200: SUCCESS,
2: ERROR,
10004: DATA NOT PROVIDED
10005: CODE NOT PROVIDED
```


# 05 Delete base master data with object_type

**URL**: `/BaseMasterData/Delete`

**METHOD**: `POST`

**Body**: 
```json
{
    "object_type": 52,
    "code": "ITM1000"
}
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response: 
```json
{
    "msg_code": 200,
    "content": "ITM1000",
    "message": null
}
```

**Message code**:
```
200: SUCCESS,
2: ERROR
10005: CODE NOT PROVIDED
```

## General Message Code
```
13: FUNCTION NOT SUPPORTED
```