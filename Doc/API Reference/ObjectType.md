# [object type](#new-version) (new version)

# [sub_type](#list-sub-type-for-choose-from-list) (sub type for CFL) 



# Danh sách object type trên hệ thống

    Controller api là Tên obj bỏ dấu o : VD : oBusinessPartners => BusinessPartners

```Json

    oBusinessPartners =         2,              Khách hàng/nhà cung cấp
                                                    APZ_OCRD: Object Dât
                                                    APZ_CRD1: Address
    oItems =                    4,              Mặt hàng
                                                    APZ_OITM: Item Data
                                                    APZ_ITM1: Item Price
                                                    APZ_OITW: Item - ware house
    oWarehouses =               64,             Kho
                                                    APZ_OWHS: object data
    oPriceList =                6,              Bảng giá
                                                    APZ_OPLN: object data
    oInvoices =                 13,             Hóa đơn bán hàng
                                                    APZ_OINV: Header data
                                                    APZ_INV1: Item Data
    oDeliveryNotes =            15,             Phiếu giao hàng
                                                    APZ_ODLN: Header data
                                                    APZ_DLN1: Item Data
    oReturns =                  16,             Phiếu trả hàng
                                                    APZ_ORDN: Header data
                                                    APZ_RDN1: Item Data
    oOrders =                   17,             Phiếu bán hàng
                                                    APZ_ORDR: header data
                                                    APZ_RDR1: item data
    oPurchaseInvoices =         18,             Hóa đơn mua hàng
                                                    APZ_OPCH: header data
                                                    APZ_PCH1: Item Data
    oPurchaseDeliveryNotes =    20,             Phiếu nhập hàng mua từ nhà cung cấp
                                                    APZ_OPDN: header data
                                                    APZ_PDN1: item data
    oPurchaseReturns =          21,             Phiếu trả hàng mua từ nhà cung cấp
                                                    APZ_ORPD: header data
                                                    APZ_RPD1: item data
    oPurchaseOrders =           22,             Đơn hàng mua
                                                    APZ_OPOR: header data
                                                    APZ_POR1: Item data
    oQuotations =               23,             Phiếu báo giá bán hàng
                                                    APZ_OQUT: header data
                                                    APZ_QUT1: item data
    oCurrency =                 37,             Đồng tiền
                                                    APZ_OCRN: Object Data
    oItemGroups =               52,             Nhóm hàng hóa
                                                    APZ_OITB: Object Data
    oInventoryGenEntry =        59,             Phiếu nhập hàng nội bộ
                                                    APZ_OIGE: Header data
                                                    APZ_IGE1: Item Data
    oInventoryGenExit =         60,             Phiếu xuất hàng nội bộ
                                                    APZ_OIGN: Header data
                                                    APZ_IGN1: Item data
    oProductTrees =             66,             Bill of material
                                                    APZ_OITT: header data
                                                    APZ_ITT1: Component data
    oInventoryTransfer =        67,             Phiếu chuyển kho
                                                    APZ_OWTR: Header data
                                                    APZ_WTR1: Item Data
    oDraft =                    112,            Phiếu documnet nháp
                                                    APZ_ODRF: header data
                                                    APZ_DRF1: item Data
    oItemTag =                  10001,          Nhãn hàng
                                                    APZ_OTAG: Object data
    oTrademark =                10002,          Thương hiệu hàng hóa
                                                    APZ_OTRM: Object Data
    oStore =                    10004,          Cửa hàng
                                                    APZ_OSTR: Object data
    oBpGroup =                  10007,          Nhóm đối tách kinh doanh
                                                    APZ_OCRG: object data
    oIncommingPayment =         810018,         Phiếu thu
                                                    APZ_ORCT: Header data
                                                    APZ_RCT1: Item Data
    oGeneralSetting =           10009,          Cài đặt hệ thống
                                                    APZ_CINF: object Data
    oUser =                     12,             Người dùng
                                                    APZ_OUSR: object data
                                                    APZ_USR1: Branch data
    oAuth =                     10011,          Phân quyền người dùng
                                                    apz_omna: auth data
                                                    apz_omni: menu item list
    oVatGroup =                 10012,          Mã thuế
                                                    APZ_OVTG: Object Data
    oOutgoingPayment =          810019,         Phiếu chi
                                                    APZ_OVPM: header data
                                                    APZ_VPM1: item data
    oUoMDefGroups =             8102,           Nhóm đơn vị
                                                    APZ_OUGP: Header data
                                                    APZ_UGP1: Unit data
    oUoM =                      10199,          Đơn vị tính
                                                    APZ_OUOM: UOM data
    oPurchaseQuotations =       540000006,      Báo giá mua hàng
                                                    APZ_OPQT: Header data
                                                    APZ_PQT1: item data
    oInventoryTransferRequest = 1250000001,     Phiếu yêu cầu chuyển kho
                                                    APZ_OWTQ: header data
                                                    APZ_WTQ1: item data
    oExchangeRate =             400005          Exchange Rate
                                                    APZ_ORTT: object data
    oFormSetting =              810034          Form setting
                                                    apz_ofst: list setting
                                                    apz_fst1: data setting
                                                    apz_fst2: valid data setting
    oLicenseManager =           10005           License Manager
                                                    apz_oalc: license info
                                                    apz_lou: license of user
    oBranch =                   700007          Branch
                                                    apz_obpl: branch info
```

# New version

```text
navigate đến 1 file markdown khác
- [text hiển thị](đường dẫn) 

dấu cách (space) = %20 
ví dụ: item code = item%20code
```

| object_type                                                   	| value      	| remarks                                                                                                                                                                                                    	|
|---------------------------------------------------------------	|------------	|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| [oBusinessPartners](BusinessPartnerMasterDataAPI.md)           	| 2          	| Khách hàng/nhà cung cấp <br>APZ_OCRD: Object Data<br>APZ_CRD1: Address                                                                                                                                     	|
| [oItems](Inventory%20API/ItemMasterDataAPI.md)                	| 4          	| Mặt hàng<br>APZ_OITM: Item Data<br>APZ_ITM1: Item Price<br>APZ_OITW: Item - ware house                                                                                                                     	|
| [oWarehouses](WareHouseAPI.md)                                	| 64         	| Kho <br/><br>APZ_OWHS: object data                                                                                                                                                                         	|
| [oPriceList](PriceListAPI.md)                                 	| 6          	| Bảng giá<br/><br>APZ_OPLN: object data                                                                                                                                                                     	|
| oInvoices                                                     	| 13         	| Hóa đơn bán hàng<br/><br>APZ_OINV: Header data<br/><br>APZ_INV1: Item Data                                                                                                                                 	|
| oDeliveryNotes                                                	| 15         	| Phiếu giao hàng<br/><br>APZ_ODLN: Header data<br/><br>APZ_DLN1: Item Data                                                                                                                                  	|
| oReturns                                                      	| 16         	| Phiếu trả hàng<br/><br>APZ_ORDN: Header data<br/><br>APZ_RDN1: Item Data                                                                                                                                   	|
| oOrders                                                       	| 17         	| Phiếu bán hàng<br/><br>APZ_ORDR: header data<br/><br>APZ_RDR1: item data                                                                                                                                   	|
| oPurchaseInvoices                                             	| 18         	| Hóa đơn mua hàng<br/><br>APZ_OPCH: header data<br/><br>APZ_PCH1: Item Data<br/>                                                                                                                            	|
| oPurchaseDeliveryNotes                                        	| 20         	| Phiếu nhập hàng mua từ nhà cung cấp<br/><br>APZ_OPDN: header data<br/><br>APZ_PDN1: item data                                                                                                              	|
| oPurchaseReturns                                              	| 21         	| Phiếu trả hàng mua từ nhà cung cấp<br/><br>APZ_ORPD: header data<br/><br>APZ_RPD1: item data                                                                                                               	|
| oPurchaseOrders                                               	| 22         	| Đơn hàng mua<br/><br>APZ_OPOR: header data<br/><br>APZ_POR1: Item data                                                                                                                                     	|
| oQuotations                                                   	| 23         	| Phiếu báo giá bán hàng<br/><br>APZ_OQUT: header data<br/><br>APZ_QUT1: item data                                                                                                                           	|
| [oCurrency](MasterData%20API/BaseMasterData/request_body.md)  	| 37         	| Đồng tiền<br/><br>APZ_OCRN: Object Data                                                                                                                                                                    	|
| [oItemGroup](MasterData%20API/BaseMasterData/request_body.md) 	| 52         	| Nhóm hàng hóa<br/><br>APZ_OITB: Object Data                                                                                                                                                                	|
| oInventoryGenEntry                                            	| 59         	| Phiếu nhập hàng nội bộ<br/><br>APZ_OIGE: Header data<br/><br>APZ_IGE1: Item Data                                                                                                                           	|
| oInventoryGenExit                                             	| 60         	| Phiếu xuất hàng nội bộ<br/><br>APZ_OIGN: Header data<br/><br>APZ_IGN1: Item data                                                                                                                           	|
| oProductTrees                                                 	| 66         	| Bill of material<br/><br>APZ_OITT: header data<br/><br>APZ_ITT1: Component data                                                                                                                            	|
| oInventoryTransfer                                            	| 67         	| Phiếu chuyển kho<br/><br>APZ_OWTR: Header data<br/><br>APZ_WTR1: Item Data                                                                                                                                 	|
| oDraft                                                        	| 112        	| Phiếu documnet nháp<br/><br>APZ_ODRF: header data<br/><br>APZ_DRF1: item Data                                                                                                                              	|
| [oItemTag](MasterData%20API/BaseMasterData/request_body.md)   	| 10001      	| Nhãn hàng<br/><br>APZ_OTAG: Object data                                                                                                                                                                    	|
| [oTrademark](MasterData%20API/BaseMasterData/request_body.md) 	| 10002      	| Thương hiệu hàng hóa<br/><br>APZ_OTRM: Object Data                                                                                                                                                         	|
| oStore                                                        	| 10004      	| Cửa hàng <br/><br>APZ_OSTR: Object data                                                                                                                                                                    	|
| [oBpGroup](MasterData%20API/BaseMasterData/request_body.md)   	| 10007      	| Nhóm đối tách kinh doanh<br/><br>APZ_OCRG: object data                                                                                                                                                     	|
| oIncommingPayment                                             	| 810018     	| Phiếu thu<br/><br>APZ_ORCT: Header data<br/><br>APZ_RCT1: Item Data                                                                                                                                        	|
| [oGeneralSetting](GeneralSettingApi.md)                       	| 10009      	| Cài đặt hệ thống<br/><br>APZ_CINF: object Data                                                                                                                                                             	|
| [oUser](UserApi.md)                                           	| 12         	| Người dùng<br/><br>APZ_OUSR: object data<br/><br>APZ_USR1: Branch data                                                                                                                                     	|
| [oAuth](AuthAPI.md)                                           	| 10011      	| Phân quyền người dùng<br/><br>apz_omna: auth data<br/><br>apz_omni: menu item list                                                                                                                         	|
| [oVatGroup](MasterData%20API/BaseMasterData/request_body.md)  	| 10012      	| Mã thuế<br/><br>APZ_OVTG: Object Data                                                                                                                                                                      	|
| oOutgoingPayment                                              	| 810019     	| Phiếu chi<br/><br>APZ_OVPM: header data<br/><br>APZ_VPM1: item data                                                                                                                                        	|
| [oUoMDefGroups](UomGroupApi.md)                               	| 8102       	| Nhóm đơn vị<br/><br>APZ_OUGP: Header data<br/><br>APZ_UGP1: Unit data                                                                                                                                      	|
| [oUoM](MasterData%20API/BaseMasterData/request_body.md)       	| 10199      	| Đơn vị tính<br/><br>APZ_OUOM: UOM data                                                                                                                                                                     	|
| oPurchaseQuotations                                           	| 540000006  	| Báo giá mua hàng<br/><br>APZ_OPQT: Header data<br/><br>APZ_PQT1: item data                                                                                                                                 	|
| oInventoryTransferRequest                                     	| 1250000001 	| Phiếu yêu cầu chuyển kho<br/><br>APZ_OWTQ: header data<br/><br>APZ_WTQ1: item data                                                                                                                         	|
| oExchangeRate                                                 	| 400005     	| Exchange Rate<br/><br>APZ_ORTT: object data                                                                                                                                                                	|
| [oFormSetting](FormSettingAPI.md)                             	| 810034     	| Form setting<br/><br>apz_ofst: list setting<br/><br>apz_fst1: data setting<br/><br>apz_fst2: valid data setting                                                                                            	|
| [oLicenseManager](LicenseManagerAPI.md)                       	| 10005      	| License Manager<br/><br>apz_oalc: license info<br/><br>apz_lou: license of user                                                                                                                            	|
| [oCompany](MasterData%20API/BaseMasterData/request_body.md)   	| 700007     	| Business Place<br/><br>apz_obpl: business place info                                                                                                                                                       	|
| [oItemComponent](Inventory%20API/ItemMasterDataAPI.md)        	| 400002     	| Item master data và oItem <br/><br>apz_ogit: item cha <br/><br>apz_oitm: item con <br/>                                                                                                                    	|
| oLayoutSetting                                                	| 400003     	|                                                                                                                                                                                                            	|
| [oUDF](UDFAPI.md)                                             	| 152        	| User Define Field <br/><br>apz_oudf: bảng quản lý all udf <br/><br>apz_udf2: bảng valid value                                                                                                              	|
| [oUDO](UDOAPI.md)                                             	| 206        	| User Define Object <br>apz_oudo: object data <br>apz_udo1: UDO child table <br>apz_udo2: UDO find column table<br>apz_udo3: UDO default view column Table<br>apz_udo4: UDO default view child column Table 	|
| [oUDT](UDTAPI.md)                                             	| 153        	| User Define Table<br>apz_oudt: object_data                                                                                                                                                                 	|


# List sub type for choose from list 

## sub type BpGroup

| sub_type                  | value      	| remarks                       |
|----------------------	    |------------	|-----------------------------  |
| oNone         	        | 0          	| Get all                       |
| oSupplier        	        | 1          	| Nhà cung cấp                  |
| oCustomer        	        | 2          	| Khách hàng                    |

## sub type Item master data
| sub_type                  | value      	| remarks                       |
|----------------------	    |------------	|-----------------------------  |
| oNone         	        | 0          	| Get all                       |
| oSale            	        | 3          	| Mặt hành bán                  |
| oPurchase        	        | 4          	| Mặt hàng mua                  |
| oInventory      	        | 5          	| Mặt hàng tồn kho              |

