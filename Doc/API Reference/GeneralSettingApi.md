# 00 OBJECT INFO
**Object Type**: `10009`

**Object Table**: `general_setting`

| Column Name     | Data Typpe | Max Length | Is Required | List Value/ Linked Table                 | Default Value | Remarks                                  |
|-----------------|------------|------------|-------------|------------------------------------------|---------------|------------------------------------------|
| version         | string     | 50         | Y           |                                          |               | Phiên bản hệ thống                       |
| company_name    | string     | 254        | Y           |                                          |               | Tên công ty                              |
| email           | string     | 254        | N           |                                          |               | Email công ty                            |
| phone1          | string     | 20         | N           |                                          |               | Số điện thoại 1                          |
| phone2          | string     | 20         | N           |                                          |               | Số điện thoại 2                          |
| address         | string     | 254        | N           |                                          |               | Địa chỉ công ty                          |
| company_type    | string     | 1          | N           |                                          |               | Loại hình công ty(chưa dùng)             |
| create_date     | date       |            | N           |                                          |               | Ngày khởi tạo hệ thống                   |
| country         | string     | 50         | N           |                                          |               | Quốc gia                                 |
| fax             | string     | 20         | N           |                                          |               | Số Fax                                   |
| main_currency   | string     | 3          | Y           | link apz_ocrn                            |               | Đồng tiền quốc gia                       |
| sys_currency    | string     | 3          | Y           | link apz_ocrn                            |               | đồng tiền mặc định hệ thống              |
| sum_dec         | smallint   |            | N           | 0,1,2,3,4,5,6                            |               | Số chữ số hàng thập phân của tổng tiền   |
| qty_dec         | smallint   |            | N           | 0.1,2,3,4,5,6                            |               | Số chữ số hàng thập phân của số lượng    |
| price_dec       | smallint   |            | N           | 0,1,2,3,4,5,6                            |               | Số chữ số hàng thập phân của giá tiền    |
| rate_dec        | smallint   |            | N           | 0,1,2,3,4,5,6                            |               | số chữ số hàng thập phân của tỷ lệ quy đổi |
| percent_dec     | smallint   |            | N           | 0,1,2,3,4,5,6                            |               | Số chữ số hàng thập phân của %           |
| measure_dec     | smallint   |            | N           | 0,1,2,3,4,5,6                            |               | Số chữ số hàng thập phân của đơn vị      |
| time_format     | string     | 10         | N           | * HHmm: 24h<br>* hhmm: 12h               |               | định dạng thời gian                      |
| date_format     | string     | 10         | N           | 1. yyyyMMdd<br>2. yyyyddMM<br>3. ddMMyyyy<br>4. MMddyyyy<br>5. yyyyMMMMdd<br>6. yyyyddMMMM<br>7. ddMMMMyyyy<br>8. MMMMddyyyy<br>9. yyMMdd<br>10. yyddMM<br>11. ddMMyy<br>12. MMddyy<br>13. yyMMMMdd<br>14. yyddMMMM<br>15. ddMMMMyy<br>16. MMMMddyy | yyyyyMMdd     | định dạng ngày tháng                     |
| date_sep        | string     | 1          | N           |                                          |               | Kí tự phân cách ngày tháng               |
| dec_sep         | string     | 1          | N           |                                          |               | Ký tự phân cách hàng thập phân           |
| thous_sep       | string     | 1          | N           |                                          |               | Ký tự phân cách hàng ngàn                |
| neg_whs         | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | Có cho phép âm kho                       |
| website         | string     | 254        | N           |                                          |               | Địa chỉ trang web                        |
| atc_entry       | int        |            | N           | link apz_oatc                            |               | Attachment entry                         |
| def_cust        | string     | 50         | N           | link apz_ocrd                            |               | khách hàng mặc định trên màn hình pos    |
| is_shift        | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | Quản lý bán hàng theo ca                 |
| is_aut_prt_bill | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | Tự động in bill khi bán hàng             |
| dis_cod         | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | không cho phép thanh toán khi giao hàng  |
| dis_card        | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | không cho phép thanh toán qua thẻ        |
| dis_tranf       | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | Không cho phép chuyển khoản              |
| max_history     | int        |            | N           |                                          |               | Số lượng bản ghi log tối đa trên các object |
| chg_sal_prlist  | string     | 1          | n           | * Y: Yes<br>* N: No                      | N             | Có cho phép thay đổi bảng giá trên màn hình pos |
| multi_branch    | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | quản lí nhiều chi nhánh                  |

**Attachment Table**: `apz_atc1`

tham chiếu đến attachment reference

# 01 API GET GENERAL SETTING DATA

**Remarks: lấy danh sách phân quyên chức năng của người dùng**:

**URL** : `/GeneralSetting/Get`

**URI Paramater**: `NONE`

**Method** : `GET`

**Body** : `NONE`

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
    "msg_code": 200,
    "content": {
        "general_setting":[
            {
                "version": "1.00.000",                          Phiên bản hệ thống
                "company_name": "Fpt shop",                     Tên công ty
                "email": "fpt@apzon.com",                       Email công ty
                "phone1": "036xxxxxx",                          Số điện thoại chính công ty
                "phone2": "038yyyyyy",                          Số điện thoại phụ của công ty
                "address": "Số 10 Hàng Bài",                    Địa chỉ công ty
                "company_type": "P",                            Loại công ty (chưa dùng)
                "create_date": "2021-01-01",                    Ngày sử dụng hệ thống
                "country": "Vietnam",                           Quốc gia
                "fax": "1023221",                               Số fax
                "main_currency": "VND",                         Đồng tiến quốc gia. link tới currency apz_ocrn
                "sys_currency": "USD",                          Đồng tiền mặc định trên hệ thống. link tới currency apz_ocrn
                "sum_dec": 2,                                   Số chữ số hàng thập phân của tổng tiền. từ 0 tới 6
                "qty_dec": 2,                                   Số chữ số hàng thập phân của số lượng. từ 0 tới 6
                "price_dec": 2,                                 Số chữ số hàng thập phân của giá. từ 0 tới 6
                "rate_dec": 2,                                  Số chữ số hàng thập phân của tỉ lệ quy đổi. từ 0 tới 6
                "percent_dec": 2,                               Số chữ số hàng thập phân của phần trăm. từ 0 tới 6
                "measure_dec": 2,                               Số chữ số hàng thập phân của đơn vị. từ 0 tới 6
                "time_format": "HHmm",                          Định dạng thời gian hiển thị. HHmm: 24h / hhmm: 12h
                "date_format": "yyyyMMdd",                      Định dạng ngày tháng. yyyyMMdd / yyyyddMM / ddMMyyyy / MMddyyyy / yyyyMMMMdd / yyyyddMMMM / ddMMMMyyyy / MMMMddyyyy / yyMMdd / yyddMM / ddMMyy / MMddyy / yyMMMMdd / yyddMMMM / ddMMMMyy / MMMMddyy
                "date_sep": "-",                                Kí tự phân cách các vị trí ngày tháng
                "dec_sep": ".",                                 Kí tự phân cách hàng thập phân
                "thous_sep": ",",                               Kí tự phân cách hàng ngàn
                "neg_whs": "Y",                                 Có cho phép âm kho hay không. Y: có âm kho / N: không âm kho
                "website": "fpt.vn",                            Trang web công ty
                "atc_entry": 1,                                 Thông tin danh sách attachment
                "def_cust": null,                               khách hàng mặc định
                "is_shift": "N",                                Có quản lí bán hàng theo ca không. Y: yes / N: no
                "is_aut_prt_bill": "N",                         Có tự động in bill khi bán hàng không. Y: yes / N: no
                "dis_cod": "N",                                 Không cho phép bán thu tiền sau. Y: không cho phép / N: cho phép
                "dis_card": "N",                                không cho phép thanh toán qua thẻ. Y: không cho phép / N: cho phép
                "dis_tranf": "N",                               Không cho phép thanh toán bằng chuyển khoản. Y: không cho phép / N: cho phép
                "max_history": 1000,                            Số lượng bản ghi log tối đa cho mỗi chức năng
                "chg_sal_prlist": "N"                           Có cho phép sửa bảng giá trên màn hình POS không. Y: yes / N: no
            }
        ],
        "attachments": [
                {
                    "doc_entry": 1,
                    "line_num": 0,
                    "src_path": null,
                    "trgt_path": null,
                    "file_type": null,
                    "file_name": "anh18",
                    "file_ext": "png",
                    "copied": "N",
                    "override": "Y",
                    "sub_path": null,
                    "free_text": mull,
                    "uri": null,
                    "dwl_uri": null,
                    "file_id": null
                }
            ]
    }
    "message": null
}
```
**MsgCode** : 
```text
200     - Success
3       - Exception
Other   - error
```

# 02 API UPDATE GENERAL SETTING

**Remarks: Cập nhật dữ liệu phân quyền chức năng của người dùng**

**URL** : `/GeneralSetting/Update`

**Method** : `POST`

**Body** : 
```json
{
    "general_setting":[
        {
            "version": "1.00.000",                          Phiên bản hệ thống
            "company_name": "Fpt shop",                     Tên công ty
            "email": "fpt@apzon.com",                       Email công ty
            "phone1": "036xxxxxx",                          Số điện thoại chính công ty
            "phone2": "038yyyyyy",                          Số điện thoại phụ của công ty
            "address": "Số 10 Hàng Bài",                    Địa chỉ công ty
            "company_type": "P",                            Loại công ty (chưa dùng)
            "create_date": "2021-01-01",                    Ngày sử dụng hệ thống
            "country": "Vietnam",                           Quốc gia
            "fax": "1023221",                               Số fax
            "main_currency": "VND",                         Đồng tiến quốc gia. link tới currency apz_ocrn
            "sys_currency": "USD",                          Đồng tiền mặc định trên hệ thống. link tới currency apz_ocrn
            "sum_dec": 2,                                   Số chữ số hàng thập phân của tổng tiền. từ 0 tới 6
            "qty_dec": 2,                                   Số chữ số hàng thập phân của số lượng. từ 0 tới 6
            "price_dec": 2,                                 Số chữ số hàng thập phân của giá. từ 0 tới 6
            "rate_dec": 2,                                  Số chữ số hàng thập phân của tỉ lệ quy đổi. từ 0 tới 6
            "percent_dec": 2,                               Số chữ số hàng thập phân của phần trăm. từ 0 tới 6
            "measure_dec": 2,                               Số chữ số hàng thập phân của đơn vị. từ 0 tới 6
            "time_format": "HHmm",                          Định dạng thời gian hiển thị. HHmm: 24h / hhmm: 12h
            "date_format": "yyyyMMdd",                      Định dạng ngày tháng. yyyyMMdd / yyyyddMM / ddMMyyyy / MMddyyyy / yyyyMMMMdd / yyyyddMMMM / ddMMMMyyyy / MMMMddyyyy / yyMMdd / yyddMM / ddMMyy / MMddyy / yyMMMMdd / yyddMMMM / ddMMMMyy / MMMMddyy
            "date_sep": "-",                                Kí tự phân cách các vị trí ngày tháng
            "dec_sep": ".",                                 Kí tự phân cách hàng thập phân
            "thous_sep": ",",                               Kí tự phân cách hàng ngàn
            "neg_whs": "Y",                                 Có cho phép âm kho hay không. Y: có âm kho / N: không âm kho
            "website": "fpt.vn",                            Trang web công ty
            "atc_entry": 1,                                 Thông tin danh sách attachment
            "def_cust": null,                               khách hàng mặc định
            "is_shift": "N",                                Có quản lí bán hàng theo ca không. Y: yes / N: no
            "is_aut_prt_bill": "N",                         Có tự động in bill khi bán hàng không. Y: yes / N: no
            "dis_cod": "N",                                 Không cho phép bán thu tiền sau. Y: không cho phép / N: cho phép
            "dis_card": "N",                                không cho phép thanh toán qua thẻ. Y: không cho phép / N: cho phép
            "dis_tranf": "N",                               Không cho phép thanh toán bằng chuyển khoản. Y: không cho phép / N: cho phép
            "max_history": 1000,                            Số lượng bản ghi log tối đa cho mỗi chức năng
            "chg_sal_prlist": "N"                           Có cho phép sửa bảng giá trên màn hình POS không. Y: yes / N: no
        }
    ],
    "attachments": [
        {
            "doc_entry": 1,
            "line_num": 0,
            "src_path": null,
            "trgt_path": null,
            "file_type": null,
            "file_name": "anh18",
            "file_ext": "png",
            "copied": "N",
            "override": "Y",
            "sub_path": null,
            "free_text": null,
            "uri": null,
            "dwl_uri": null,
            "file_id": null
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