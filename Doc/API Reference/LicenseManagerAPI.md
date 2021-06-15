# 00 OBJECT INFO
**Object Type**: `10005`

**User License Table**: `apz_lou`

| Column Name    | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks                       |
|----------------|------------|------------|-------------|--------------------------|---------------|-------------------------------|
| license_type   | string     | 10         | Y           |                          |               | Loại giấy phép                |
| type_name      | string     | 254        | N           |                          |               | Tên loại giấy phép            |
| amount         | int        |            | N           |                          |               | Số lượng license              |
| use_amount     | int        |            | N           |                          |               | Số lượng license đã sử dụng   |
| rem_amount     | int        |            | N           |                          |               | Số lượng license còn lại      |
| exp_date       | date       |            | N           |                          |               | Ngày hệt hạn của license type |
| install_number | string     | 50         | Y           |                          |               | ID giấy phép                  |

**User Table**: `apz_ousr`

Tham chiếu đến User reference

# 01 API IMPORT LICENSE

**Remarks: License import vào sẽ được sử dụng cho toàn hệ thống không theo database**:

**URL** : `/LicenseManager/Import`

**Method** : `POST`

**Body** : 
```json
{
    "license_info": "Nội dung file license"
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
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 02 API UPDATE MAPPING USER LICENSE

**Remarks: <br>- Mmapping license cho các user trên database. các user không có trong danh sách users là không update dữ liệu. <br>
- Trong trường hợp xóa license của user thì sẽ không có thông tin license của user ở users_license**

**URL** : `/LicenseManager/UpdateUsersLicense`

**Method** : `POST`

**Body** : 
```json
{
    "users": [
        {
            "user_code": "thanhnv"                          Mã người dùng trên hệ thống
        }
    ],
    "users_license":[
        {
            "user_code": "thanhnv",
            "license_type": "P"
        }
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
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 03 API GET THÔNG TIN LICENSE

**Remarks: Lấy thông tin giấy phép trên hệ thống**

**URL** : `/LicenseManager/Get`

**Method** : `POST`

**Body** : 
```json
{
    "license_info": [
        {
            "license_type": "P"                             loại license
            , "type_name": "License Pro"                    tên license
            , "amount": 10                                  số lượng user của license
            , "use_amount": 5                               số lương license đã sử dụng
            , "rem_amount": 5                               số lượng license chưa sử dụng
            , "exp_date": "2021-12-12"                      ngày hết hạng của license
            , "install_number": "xyz"                       ID license file
        }
    ],
    "users_license":[
        {
            "user_code": "thanhnv",
            "license_type": "P"
        }
    ],
    "database_users":[
        {
            "user_code": "thanhnv",
            "user_name": "Nguyễn Văn Thành",
            "email": null,
            "phone1": null,
            "status": null
        }
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
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 04 API lấy thông tin hwkey trên hệ thống

**Remarks: Lấy thông tin HardwareKey của hệ thống**

**URL** : `/LicenseManager/GetHwKey`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `NONE`

**Permissions required** : `NONE`

## Response

```json
{
    "msg_code": 200,
    "content": "hwktext",
    "message": null
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```