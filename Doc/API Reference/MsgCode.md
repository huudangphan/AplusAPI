#HttpResult Message Code
```c#
None = 0,                                           Lỗi không xác định
Success = 200,
Error = 2,                                          Lỗi chung chung
Exception = 3,                                      Exception hệ thống
LicenseNotGranted = 100054,                         Người dùng chưa được gán giấy phép
LicenseInvalid = 100055,                            Giấy phép hệ thống không hợp lệ
LicenseNotFound = 1000503,                          Hệ thống không có giấy phép
LicenseTypeInvalid = 1000506,                       Loại giấy phép không hợp lệ
LicenseExpired = 1000507,                           Giấy phép hết hạn
SystemInfoInvalid = 1000520,                        Dữ liệu hệ thống không chính xác
TokenExpired = 9,                                   Token hết hạn
TokenInvalid = 10,                                  Token không tồn tại
RecordNotFound = 11,                                Không tồn tại bản ghi
DataNotProvide = 12,                                Dữ liệu chưa được cung cấp
FunctionNotSupport = 13,                            Phương thức không được hỗ trợ
UserLock = 15,                                      Nguời dùng bị khóa
TableNotExists = 16,                                bảng không tồn tại
ColumnNotExistsInTable = 17,                        Cột không tồn tại
CodeNotProvided = 18,                               code/id không được cung cấp
RecordInUseInOtherTable = 19,                       Dữ liệu đã được sử dụng
UnableAccessDatabase = 500001,                      Không thể kết nối csdl
DataUserNotProvide = 20,                            Dữ liệu người dùng không được cung cấp
DataBranchNotProvide = 21,                          dữ liệu chi nhánh chưa được cung cấp
DataDefaultSystemNotProvide = 22,                   Dữ liệu mặc định hệ thống chưa được cung cấp
DataNotCorrect = 400020,                            Dữ liệu không đúng
UserNotCorrect = 1001101,                           Người dùng không chính xác
FunctionNotCorrect = 1001102,                       CHức năng không chính xác
CurrencyNotValid = 25,                              Đồng tiền không tồn tại
NumberDecimalNotValid = 26,                         Setting số lượng chữ số không chính xác
DateFormatNotValid = 27,                            Định dạng ngày tháng không chính xác
TimeFormatNotValid = 28,                            Định dạng thời gian không chính xác
CustomerNotExists = 29,                             Khách hàng không tồn tại
NotValidValue = 30,                                 Giá trị không chính xác
SepaNotValid = 1000901,                             Kí tự phân cách không đúng
CannotSettingCurrency = 1000902,                    Không thể thiết lập đồng tiền
CannotSettingDecimal = 1000903,                     không thể thiết lập số chữ số thập phân
```