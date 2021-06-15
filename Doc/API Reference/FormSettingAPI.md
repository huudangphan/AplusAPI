# 00 OBJECT INFO
**Object Type**: `810034`
**Form Table**: `form_setting`

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks          |
|---------------|------------|------------|-------------|--------------------------|---------------|------------------|
| setting_entry | int        |            | Y           |                          |               | id setting       |
| form_id       | string     | 50        | Y           |                          |               | id Form          |
| item_id       | string     | 50         | N           |                          |               | item Id          |
| language_code | string     | 1          | N           | linked apz_oslg          |               | Ngôn ngữ setting |

**Setting Table**: `form_setting_user`

| Column Name    | Data Typpe | Max Length | Is Required | List Value/ Linked Table                 | Default Value | Remarks                          |
|----------------|------------|------------|-------------|------------------------------------------|---------------|----------------------------------|
| setting_entry  | int        |            | Y           | linked apz_ofst                          |               | id setting                       |
| user_id        | smallint   |            | Y           | linked apz_ousr                          |               | user Id                          |
| column_id      | string     | 50         | Y           |                                          |               | id trường được setting           |
| column_name    | string     | 254        | N           |                                          |               | Lable hiển thị của trường        |
| data_type      | string     | 50         | N           | 1. string<br>2. int<br>3. numeric<br>4. datetime<br>5. Tab |               | kiểu dữ liệu của trường          |
| sub_type       | string     | 50         | N           | 1. Phone<br>2. Email<br>3. Address<br><br>4. text <br>5. Amount<br>6. Quantity<br>7. Price<br>8. Rate<br>9. Percent |               | kiểu dữ liệu phụ                 |
| maximum_length | int        |            | N           |                                          |               | Độ dài tối đa của trường         |
| is_visible     | string     | 1          | N           | * Y: Yes<br>* N: No                      | Y             | có hiển thị hay không            |
| is_editable    | string     | 1          | N           | * Y: Yes<br>* N: No                      | Y             | Có được edit không               |
| is_required    | string     | 1          | N           | * Y: Yes<br>* N: No                      | Y             | Có bắt buộc không                |
| default_value  | string     | 254        | N           |                                          |               | Giá trị mặc định                 |
| is_udf         | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | Có phải udf không                |
| view_group     | int        |            | N           |                                          |               | nhóm hiển thị trên giao diện     |
| view_order     | int        |            | N           |                                          |               | thứ tự hiển thị trên nhóm        |
| linked_type    | string     | 10         | N           | * 00: không kink<br>* 01 : link system object<br>* 02: link udo <br>* 03: link udt<br>* 04: valid lookup data(Code - Name)<br>* 05 - valid list data | 00            | Kiểu link dữ liệu                |
| linked_object  | string     | 50         | N           |                                          |               | object type của object được link |
| col_fixed      | string     | 1          | N           | * N: không fix<br>* L: neo trái<br>* R: nep phải | N             | neo vị trí cho trường trên lưới  |
| col_width      | int        |            | N           |                                          |               | Độ rộng của trường               |
| col_height     | int        |            | N           |                                          |               | chiều cao của trường             |
| is_ch_edit     | string     | 1          | N           | * Y : Yes<br>* N : No                    | Y             | cho phép chỉnh sửa is_editable   |
| is_ch_visible  | string     | 1          | N           | * Y : Yes<br>* N : No                    | Y             | cho phép chỉnh sửa is_visible    |
| lay_order      | int        |            | N           |                                          |               | số thứ tự layout của udf         |
| lay_name       | string     | 254        | N           |                                          |               | Tên hiển thị của layout          |

**Valid Table**: `form_setting_value`

| Column Name | Data Typpe | Max Length | Is Required | List Value/ Linked Table | Default Value | Remarks            |
|-------------|------------|------------|-------------|--------------------------|---------------|--------------------|
| table_name    | string     | 50         | Y           |                          |               | table id           |
| column_id   | string     | 50         | Y           |                          |               | Column Id          |
| key_code    | string     | 50         | Y           |                          |               | Giá trị của trường |
| key_name    | string     | 254        | N           |                          |               | tên hiển thị       |

# 01 API GET FORM SETTING INFO

**Remarks: API lấy thông tin setting của form theo formId, itemId, user id và language code**:

**URL** : `/FormSetting/Get?form_id=1001&item_id=oitm&lng_codevi-VN&user_id=1`

**URI Paramater**:

1. form_id,                        form id của chức năng lấy dữ liệu setting
2. item_id,                        Item id của tab lấy dữ liệu setting. Nếu không có item id thì là lấy setting trên cả form
3. lng_code,                       Ngôn ngữ hệ thống người dùng đang sử dụng
4. user_id,                        id người dùng trên hệ thống

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "form_data": [
            {
                "setting_entry": 1,                         id setting
                "form_id": "4",                             id Form
                "item_id": "apz_oitm",                      item Id
                "language_code": "vi_VN",                   language code
            }
        ],
        "setting_data": [
            {
                "setting_entry": 1,                         id setting
                "user_id": 1,                               Id user được setting
                "column_id": "item_code",                   id trường được setting
                "column_name": "Mã hàng",                   Lable hiển thị của trường
                "data_type": "nvarchar",                    kiểu dữ liệu của trường
                "sub_type": "email",                        sub type của trường
                "maximum_length": 254,                      Số ký tự tối đa của trường
                "is_visible": "Y",                          có hiển thị hay không Y: yes - N: No
                "is_editable": "Y",                         có cho phép sửa không Y: yes - N - No
                "is_required": "Y",                         có phải trường bắt buộc không Y: yes - N - No
                "default_value": "I",                       giá trị mặc định
                "is_udf": "N",                              có phải udf không Y: yes - N - No
                "view_group": 1,                            nhóm hiển thị trên giao diện
                "view_order": 10,                           thứ tự hiển thị trên nhóm
                "linked_type": "01",                        Loại link dữ liệu. 00: không kink - 01 : link system object - 02: link udo - 03: link udt - 04: valid lookup data(Code - Name) - 05 - valid list data
                "linked_object": "4",                       object type của system object được link
                "col_fixed": "N",                           neo vị trí cho trường trên lưới. N: không fix - L: neo trái - R: nep phải
                "col_width": 20,                            độ rộng của trường
                "col_height": 100,                          chiều cao của trường
                "lay_order": 1,                             udf layout order
                "lay_name": "layout 1"                      udf layout name
            }
        ],
        "valid_data": [
            {
                "setting_entry": 1,                         id setting
                "column_id": "item_code",                   id trường được setting
                "key_code": "A",                            Giá trị của trường
                "key_name": "Active"                        tên hiển thị
            }
        ]
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

# 02 API UPDATE VALID VALUE

**Remarks**

**URL** : `/FormSetting/UpdateValidValue`

**Method** : `POST`

**Body** : 
```json
{
    "form_data": [
        {
            "setting_entry": 1,                         id setting
            "form_id": "4",                             id Form
            "item_id": "apz_oitm",                      item Id
            "language_code": "vi_VN",                   language code
        }
    ],
    "valid_data": [
        {
            "setting_entry": 1,                         id setting
            "column_id": "item_code",                   id trường được setting
            "key_code": "A",                            Giá trị của trường
            "key_name": "Active"                        tên hiển thị
        },
        {
            "setting_entry": 1,                         id setting
            "column_id": "item_code",                   id trường được setting
            "key_code": "I",                            Giá trị của trường
            "key_name": "InActive"                      tên hiển thị
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

# 03 API UPDATE DATA FROM SETTING

**Remarks: api cập nhật dữ liệu valid của các trường trên form (sử dụng để setting mặc định các trường có valid trên hệ thống không ăn theo user)**

**URL** : `/FormSetting/Update`

**Method** : `POST`

**Body** : 
```json
{
    "form_data": [
        {
            "setting_entry": 1,                         id setting
            "form_id": "4",                             id Form
            "item_id": "apz_oitm",                      item Id
            "language_code": "vi_VN",                   language code
        }
    ],
    "setting_data": [
        {
            "setting_entry": 1,                         id setting (setting của form nào)
            "user_id": 1,                               Id user được setting. -1 là setting mặc định của hệ thống
            "column_id": "item_code",                   id trường được setting
            "column_name": "Mã hàng",                   Lable hiển thị của trường
            "data_type": "nvarchar",                    kiểu dữ liệu của trường
            "sub_type": "email",                        sub type của trường
            "maximum_length": 254,                      Số ký tự tối đa của trường
            "is_visible": "Y",                          có hiển thị hay không Y: yes - N: No
            "is_editable": "Y",                         có cho phép sửa không Y: yes - N - No
            "is_required": "Y",                         có phải trường bắt buộc không Y: yes - N - No
            "default_value": "I",                       giá trị mặc định
            "is_udf": "N",                              có phải udf không Y: yes - N - No
            "view_group": 1,                            nhóm hiển thị trên giao diện
            "view_order": 10,                           thứ tự hiển thị trên nhóm
            "linked_type": "01",                        Loại link dữ liệu. 00: không kink - 01 : link system object - 02: link udo - 03: link udt - 04: valid lookup data(Code - Name) - 05 - valid list data
            "linked_object": "4",                       object type của system object được link
            "linked_udt": "@UTF1",                      UDT được link
            "linked_udo": "@OPRM",                      object type của udo được link
            "col_fixed": "N",                           neo vị trí cho trường trên lưới. N: không fix - L: neo trái - R: nep phải
            "col_width": 20,                            độ rộng của trường
            "col_height": 100,                          chiều cao của trường
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