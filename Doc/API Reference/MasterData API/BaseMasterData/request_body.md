## Navigation:

[Back to main](base_master_data_api.md)

# Request body for specific object type

**Body Example**:

```json
{
  "object_type": "(type)",
  "data": [
    {
      ...
    }
  ]
}
```

## Type definition:

## 01  Business Partner Group (oBpGroup: 10007)

partner_group:

```json
{
  "code": "(not null)",
  "type": "(not null)",
  "name": "",
  "locked": "",
  "price_list": 1,
  "status": ""
}
```

## 02 Currency (oCurrency: 37)

currency:

```json
{
  "code": "", (Currency code) (not null)
  "name": "", (Currency name)
  "locked": "N", (value: Y or N,  default N)
  "round_sys": "Y",  (value Y or N, default Y)
  "decimals": 6,  (how many digits to rounding)
  "status": "A",  (value A or I,  default A)
}
```

## 03 Item Group (oItemGroups: 52)

item_group:

```json
{
  "code": "(not null)",
  "name": "",
  "remaks": "",
  "status": ""
}
```

## 04 Shift (oShift: 810033)

shift:

```json
{
  "code": "(not null)",
  "name": "",
  "store_code": "",
  "pos_id": 1,
  "cash_id": 1,
  "clock_date": "10/5/2020",
  "clock_time": 840,
  "clock_type": "A"
}
```

## 05 Item Tag (oItemTag: 10001)

item_tag:

```json
{
  "code": "(not null)",
  "name": "",
  "status": ""
}
```

## 06 Trademarks (oTrademark: 10002)

trademark:

```json
{
  "code": "",  (Trademark code) (not null)
  "name": "",  (Trademark name)
  "status": "",  (value A or I,  default A)
}
```

## 07 Unit of measure (oUoM: 10199)

unit:

```json
{
  "code": "UOM1",  (uom code)
  "name": "",  (uom name)
  "status": "",  (value A or I,  default A)
  "int_symbol": "",
  "ewb_unit": "",
  "remark": "Some text description"
}
```

## 08 Vat Group (oVatGroup: 10012)

tax_group:

```json
{
  "code": "VAT2",  (Vat Group code)
  "name": "GRP 1",  (Vat Group name)
  "status": "A",  (value A or I,  default A)
  "rate": 7.52,  (vat group rating)
}
```

## 09 Company

**Object Type**: `700007`

**Object Table**:

company:

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table                 | Default Value | Remarks                               |
|---------------|------------|------------|-------------|------------------------------------------|---------------|---------------------------------------|
| code          | string     | 50         | Y           |                                          |               | mã công ty                            |
| name          | string     | 254        | N           |                                          |               | Tên công ty                           |
| fr_name       | string     | 254        | N           |                                          |               | Tên nước ngoài                        |
| vat_reg_num   | string     | 11         | N           |                                          |               | Mã số thuế                            |
| industry      | string     | 20         | N           |                                          |               | Ngành hàng                            |
| business      | string     | 20         | N           |                                          |               | Loại hình kinh doanh                  |
| address       | string     | 254        | N           |                                          |               | Địa chỉ công ty                       |
| status        | string     | 1          | N           | * A : Active<br>* I : Inactive           | A             | trạng thái công ty                    |
| user_sign     | smallint   |            | N           | link apz_ousr                            |               | người tạo                             |
| user_sign2    | smallint   |            | N           | link apz_ousr                            |               | người cập nhật cuối cùng              |
| create_date   | date       |            | N           |                                          |               | Ngày tạo                              |
| create_time   | smallint   |            | N           |                                          |               | Thời gian tạo                         |
| update_date   | date       |            | N           |                                          |               | Ngày cập nhật cuối cùng               |
| update_time   | smallint   |            | N           |                                          |               | Thời gian cập nhật cuối cùng          |
| dfl_cust      | string     | 50         | N           | link apz_ocrd                            |               | khách hàng mặc định                   |
| dfl_vendor    | string     | 50         | N           | link apz_ocrd                            |               | nhà cung cấp mặc định                 |
| dfl_whs       | string     | 50         | N           | link apz_owhs                            |               | kho mặc định                          |
| dfl_tax_code  | string     | 50         | N           | link apz_ovtg                            |               | mã thuế mặc định                      |
| street        | string     | 254        | N           |                                          |               | Tên đường                             |
| building      | string     | 254        | N           |                                          |               | Tòa nhà                               |
| zip_code      | string     | 20         | N           |                                          |               | Mã bưu chính                          |
| block         | string     | 254        | N           |                                          |               | phường/xã                             |
| city          | string     | 254        | N           |                                          |               | Thành phố                             |
| state         | string     | 254        | N           |                                          |               | Quận huyện                            |
| country       | string     | 254        | N           |                                          |               | Quốc gia                              |
| email         | string     | 254        | N           |                                          |               | Email                                 |
| phone1        | string     | 20         | N           |                                          |               | sdt 1                                 |
| phone2        | string     | 20         | N           |                                          |               | sdt 2                                 |
| fax           | string     | 20         | N           |                                          |               | số fax                                |
| main_currency | string     | 3          | N           |                                          |               | Đồng tiền quốc gia                    |
| sys_currency  | string     | 3          | N           |                                          |               | Đồng tiền chính trên hệ thống         |
| sum_dec       | smallint   |            | N           | * 0,1,2,3,4,5,6                          |               | số chữ số thập phân của amount        |
| qty_dec       | smallint   |            | N           | * 0,1,2,3,4,5,6                          |               | số chữ số thập phân của qty           |
| price_dec     | smallint   |            | N           | * 0,1,2,3,4,5,6                          |               | số chữ số thập phân của đơn giá       |
| rate_dec      | smallint   |            | N           | * 0,1,2,3,4,5,6                          |               | số chữ số thập phân của tỉ lệ quy đổi |
| percent_dec   | smallint   |            | N           | * 0,1,2,3,4,5,6                          |               | số chữ số thập phân của %             |
| measure_dec   | smallint   |            | N           | * 0,1,2,3,4,5,6                          |               | số chữ số thập phân của đơn vị        |
| time_format   | string     | 20         | N           | * HHmm: 24h<br>* hhmm: 12h               | HHmm          | định dạng giờ                         |
| date_format   | string     | 20         | N           | 1. yyyyMMdd<br>2. yyyyddMM<br>3. ddMMyyyy<br>4. MMddyyyy<br>5. yyyyMMMMdd<br>6. yyyyddMMMM<br>7. ddMMMMyyyy<br>8. MMMMddyyyy<br>9. yyMMdd<br>10. yyddMM<br>11. ddMMyy<br>12. MMddyy<br>13. yyMMMMdd<br>14. yyddMMMM<br>15. ddMMMMyy<br>16. MMMMddyy | yyyyyMMdd | yyyyMMdd      | định dạng ngày tháng                  |
| date_sep      | string     | 1          | N           |                                          |               | kí tự phân cách ngày tháng            |
| dec_sep       | string     | 1          | N           |                                          |               | Kí tự phân cách hàng thập phân        |
| thous_sep     | string     | 1          | N           |                                          |               | kÍ tự phân cách hàng đơn vị           |
| neg_whs       | string     | 1          | N           | * Y: Yes<br>* N: No                      | N             | cho phép âm kho                       |
| website       | string     | 254        | N           |                                          |               | Địa chỉ website                       |

```json
{
  "code": "ROM1",
  "name": "Roman",
  "address": "112 StickyNote",
  "main_bpl": "N",
  "status": "A",
  "street": "InTrospecting",
  "street_no": null,
  "building": null,
  "zip_code": null,
  "block": null,
  "city": null,
  "state": null,
  "county": null,
  "country": null,
}
```

# 10 Shipping Unit (oShippingUnit 10013)

shipping_unit

```json
{
  "code": "GHTK",
  "name": "Giao hàng tiết kiệm",
  "status": "A",
  "price_unit": 1,
  "currency": "VND",
  "remarks": "GHTK Tiết kiệm 1000%",
  "is_def": "N"
}
```

# 11 Geography Location  

- oGeographyLocation: **1250000002**  (phòng trừ trường hợp có thể dùng đến)
- oCountry: **1250000003**
- oProvince: **1250000004**
- oDistrict: **1250000005**
- oWards: **1250000006**

geography

```json
{
  "code": "VN",
  "f_code": "",
  "name": "VIET NAM",
  "status": "A",
  "type": "C",
  "path": "ASIA",
  "name_with_type": "",
  "path_with_type": "",
}
```

trường type nên có giá trị là (

- C - Country
- P - Province
- D - District
- W - Wards

)

hoặc bàn bạc thống nhất 19


# 12 Posting Period

**Object Type**: `700022`

**Object Table**:

financial_period

| Column Name   | Data Typpe | Max Length | Is Required | List Value/ Linked Table   | Default Value | Remarks                  |
|---------------|------------|------------|-------------|----------------------------|---------------|--------------------------|
| code          | string     | 50         | Y           |                            |               | mã period                |
| name          | string     | 254        | N           |                            |               | tên period               |
| from_ref_date | date       |            | Y           |                            |               | posting date from        |
| to_ref_date   | date       |            | Y           |                            |               | posting date to          |
| from_due_date | date       |            | Y           |                            |               | due date from            |
| to_due_date   | date       |            | Y           |                            |               | due date to              |
| remarks       | string     | 254        | N           |                            |               | ghi chú                  |
| data_source   | string     | 1          | N           | * S: system<br>* N: client |               | nguồn lưu                |
| user_sign     | smallint   |            | N           | link apz_ousr              |               | người tạo                |
| create_date   | date       |            | N           |                            |               | ngày tạo                 |
| create_time   | smallint   |            | N           |                            |               | thời gian tạo            |
| user_sign2    | smallint   |            | N           | link apz_ousr              |               | người cập nhật cuối cùng |
| update_date   | date       |            | N           |                            |               | ngày cập nhật            |
| update_time   | smallint   |            | N           |                            |               | thời gian cập nhật       |
| status        | string     | 1          | Y           | * A: Active<br>* I: Inactive | A             | trạng thái của period    |

```json
{
  "code": "2021-11",
  "name": "thang 11",
  "status": "A",
  "from_ref_date": "2021-11-01",
  "to_ref_date": "2021-11-31",
  "from_due_date": "2021-11-01",
  "to_due_date": "2021-11-31",
}

```

# 12 Retail Table (oRetailTable: 400004)

table_info

```json
{
  "code": "wev1",
  "name": "Tablewhatever",
  "remark": "",
  "reserve": "",
  "type": "",
  "branch": "",
  "vis_order": "",
  "tab_number": 1,
}
```


# 13 Length (oLength: 1250000008)

length

```json
{
  "code": "LENG1",
  "name": "Kilometer",
  "remark": "",
  "status": "A",
}
```


# 14 Weight (oWeight: 1250000007)

weight

```json
{
  "code": "WH1",
  "name": "TAN",
  "remark": "TAN, Ta, Yen, Kylogam",
  "status": "A",
}
```