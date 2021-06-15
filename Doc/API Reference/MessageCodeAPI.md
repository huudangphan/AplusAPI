**MessageCode Table**: `apz_omsc`

| Column Name           | Data Typpe | Max Length | Is Required | List Value/ Linked Table             | Default Value | Remarks                    |
|-------------          |------------|------------|-------------|--------------------------------------|---------------|----------------------------|
| msg_code              | int        |            | Y           |                                      |               | Mã lỗi                     |
| lang_code             | string     | 254        | Y           |                                      |               | Mã ngôn ngữ                |
| msg                   | string     | 50         | N           |                                      |               | Mô tả lỗi                  |
| msg_identifier        | string     | 50         | Y           |                                      |               | Tên lỗi mess                  |

# 01 API Get all Message 

**Remarks: lấy danh sách mã lỗi và tên lỗi theo ngôn ngữ**:

**URL** : `/MessageCode/Get`

**Method** : `POST`

**Body** : 

```json
{
  "code": "en-US"
}
```

code: language code

**Auth required** : `NO AUTH`

**Permissions required** : `None`

## Response

```json
{
  "msg_code": 200,
  "content": [
    {
      "msg_code": ...,
      "lang_code": "en-US",
      "msg": ...
    },
    {
      "msg_code": ...,
      "lang_code": "en-US",
      "msg": ...
    },
    ...
  ],
  "message": null
}
```

# 02 Restore system message


**Remarks: Khôi phục lại các message mặc định của hệ thống (nhớ phải cảnh báo người dùng)**:

**URL** : `/MessageCode/RestoreSystemMessage`

**Method** : `POST`

**Body** : `none`

or

```json
[{
}]
```


**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": null,
    "message": "Message Restored!"
}
```

# 03 Update

**Remarks: Cập nhật các message từ client và từ hệ thống**:

**URL** : `/MessageCode/RestoreSystemMessage`

**Method** : `POST`

**Body** : `none`

or

```json
[
  {
  "msg_code": 0,
  "lang_code": "en-US",
  "msg": "NONSDDFSDF",
  "msg_identifier": "None"
  },
  {
    "msg_code": 1,
    "lang_code": "en-US",
    "msg": "TestERFFFFF",
    "msg_identifier": "Test"
  },
  {
    ... danh sách message đã được chỉnh sửa
  },
]
```

**Auth required** : `Bearer Token`

**Permissions required** : `None`

## Response

