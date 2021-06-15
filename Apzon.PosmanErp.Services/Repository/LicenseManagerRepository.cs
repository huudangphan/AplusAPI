using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Services.Interfaces;
using Apzon.PosmanErp.Entities;
using Apzon.Libraries.HDBConnection.Interfaces;
using Npgsql;

namespace Apzon.PosmanErp.Services.Repository
{
    public class LicenseManagerRepository : BaseService<DataTable>, ILicenseManager
    {

        public LicenseManagerRepository(IDatabaseClient databaseService) : base(databaseService)
        {
            
        }

        public HttpResult CheckUserLicense(string domainName, string databaseName, string usercode)
        {
            try
            {
                var hardwareKeyServer = Function.GetHardwareKey();
                //lấy thông tin database date
                var dateserver = ExecuteScalar(@"select TO_CHAR(current_date, 'yyyyMMdd');");
                //lấy dữ liệu license của hệ thống
                var dtlicense = ExecuteDataTable(@"select * from license_info where hardware_key = @_hardware_key;", CommandType.Text, new SqlParameter("@_hardware_key", hardwareKeyServer));
                //trong trương hợp hệ thống có import license
                if (dtlicense.IsNotNull())
                {
                    //trong trường hợp ngày tạo license lớn hơn current date của server thì không báo lỗi
                    //tránh trượng hợp user sửa ngày hệ thống để gian lận license
                    if (Function.ParseInt(Function.Decrypt(Function.ToString(dtlicense.Rows[0]["date_request"]))) > Function.ParseInt(dateserver))
                    {
                        return new HttpResult(MessageCode.ServerDateIsInvalid);
                    }
                    //khai báo các thôn tin để đọc license file
                    string bpCode, installNumber, hardwareKey, sysType, fileId, dateCreate, versionIrs;
                    List<LicenseInfoImpl> lstLicenseInfo;
                    //đọc dữ liệu license
                    Function.ReadLicense(Function.ToString(dtlicense.Rows[0]["license_info"]), out bpCode, out installNumber, out hardwareKey, out fileId, out sysType, out dateCreate, out versionIrs, out lstLicenseInfo);
                    //kiểm tra hardwarekey của hệ thống
                    if (!hardwareKey.Equals(hardwareKeyServer))
                    {
                        return new HttpResult(MessageCode.LicenseInvalid);
                    }
                    //kiểm tra installnumber của file và của thông tin lưu trên hệ thống
                    if (!Function.ToString(dtlicense.Rows[0]["install_number"]).Equals(installNumber))
                    {
                        return new HttpResult(MessageCode.LicenseInvalid);
                    }
                    #region lấy thông tin license
                    var userLicense = new UserLicenseImpl();
                    userLicense.user_code = usercode;
                    userLicense.is_user_license = false;
                    userLicense.list_license_type = new List<LicenseInfoImpl>();
                    userLicense.expiry_date = DateTime.Now.Date.AddDays(-1);
                    //lấy dữ liệu license của user trên hệ thống
                    var dtUserLicense = ExecuteData(@"select count(1) quantity, license_type from user_license where install_number = @_install_number group by license_type;
                                                      select * from user_license where user_code = @_user_code and install_number = @_install_number"
                                                    , CommandType.Text, new SqlParameter("@_install_number", installNumber), new SqlParameter("@_user_code", usercode));
                    if(dtUserLicense == null || dtUserLicense.Tables.Count != 2)
                    {
                        return new HttpResult(MessageCode.UnableAccessDatabase);
                    }
                    if(!dtUserLicense.Tables[0].IsNotNull() || !dtUserLicense.Tables[1].IsNotNull())
                    {
                        return new HttpResult(MessageCode.LicenseNotGranted);
                    }
                    //lấy dữ liệu mapping license user
                    var dtLicenseUser = dtUserLicense.Tables[1];
                    //dữ liệu sử dụng license
                    var dtUsed = dtUserLicense.Tables[0];
                    //khởi tạo dữ liệu license Rule
                    var licenseRule = LicenseRule.Service;
                    #region lấy dữ liệu mapping license user
                    if (dtLicenseUser.IsNotNull())
                    {
                        //khởi tạo dữ liệu license user
                        var userRule = LicenseRule.Service;
                        #region lặp theo dữ liệu setting license
                        for (var i = 0; i < dtLicenseUser.Rows.Count; i++)
                        {
                            //license Type
                            var licenseType = Function.ToString(dtLicenseUser.Rows[i]["license_type"]);
                            //lấy dữ liệu sử dụng licnese tương ứng
                            var licenseUse = dtUsed.AsEnumerable().FirstOrDefault(t => Function.ToString(t["license_type"]).Equals(licenseType));
                            //số lượng license đã sữ dụng
                            var countUsed = licenseUse == null ? 0 : Function.ParseInt(licenseUse["quantity"]);
                            //thông tin license trong license file
                            var licenseInfo = lstLicenseInfo.FirstOrDefault(t => t.type_code.Equals(licenseType));
                            //nếu không tồn tại thông tin license type trong file license hoặc số lượng license sử dụng lớn hơn số lượng Amount trong file
                            if (licenseInfo == null || licenseInfo.amount < countUsed)
                            {
                                return new HttpResult(MessageCode.DatabaseCastingArbitrarityChang);
                            }
                            //lấy thông tin status license. So sánh giữa ngày expired date và ngày hiện tại
                            licenseInfo.license_status = licenseInfo.is_mapping_user = Function.ParseDateTimes(dateserver, "yyyyMMdd") <= licenseInfo.expiry_date;
                            licenseInfo.used = countUsed;
                            #region lấy thông tin license rule
                            if (licenseInfo.license_status)
                            {
                                userLicense.is_user_license = true;
                                userLicense.expiry_date = userLicense.expiry_date < DateTime.Now.Date ? licenseInfo.expiry_date : userLicense.expiry_date > licenseInfo.expiry_date ? licenseInfo.expiry_date : userLicense.expiry_date;
                                if (licenseInfo.license_rule == LicenseRule.Normal)
                                {
                                    if (userRule == LicenseRule.Service)
                                    {
                                        userRule = LicenseRule.Normal;
                                    }
                                    if (licenseRule == LicenseRule.Service)
                                    {
                                        licenseRule = LicenseRule.Normal;
                                    }
                                }
                                if (licenseInfo.license_rule == LicenseRule.Super)
                                {
                                    userRule = licenseRule = LicenseRule.Super;
                                }
                            }
                            #endregion
                            userLicense.list_license_type.Add(licenseInfo);
                        }
                        #endregion
                        userLicense.user_rule = userRule;
                        //khởi tạo dữ liệu access from của user
                        userLicense.list_access_form = new List<string>();
                        #region lặp theo thông tin license để lấy list access form
                        for (var i = 0; i < userLicense.list_license_type.Count; i++)
                        {
                            //Nếu license type chứa config về list access form
                            if (userLicense.list_license_type[i].list_access_form.Any())
                            {
                                //Khi license đang valid thì lấy thông tin access form
                                if (userLicense.list_license_type[i].license_status)
                                {
                                    //khi license được mapping cho user
                                    if (userLicense.list_license_type[i].is_mapping_user)
                                        userLicense.list_access_form = userLicense.list_access_form.Union(userLicense.list_license_type[i].list_access_form).ToList();
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #endregion
                    //khi user không được mapping license
                    if (!userLicense.is_user_license)
                    {
                        return new HttpResult(dtLicenseUser.IsNotNull() ? MessageCode.LicenseExpired : MessageCode.LicenseNotGranted, obj: userLicense);
                    }

                    return new HttpResult(MessageCode.Success, userLicense);
                }
                return new HttpResult(MessageCode.LicenseNotFound);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        public HttpResult GetDataLicenseInfomation(string systemHwk)
        {
            try
            {
                //lấy dữ liệu license trên hệ thống
                var dtLicense = ExecuteDataTable(@"select a.license_info from license_info a where a.hardware_key = @_hardware_key;", CommandType.Text, new SqlParameter("@_hardware_key", systemHwk));
                //không tồn tại dữ liệu license thò bỏ qua việc check license
                if (!dtLicense.IsNotNull())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.LicenseNotFound,
                        message = "Hệ thống chưa có giấy phép. Vui lòng mua giấy phép để tiếp tục sử dụng dịch vụ"
                    };
                }
                //khởi tạo dữ liệu đọc license
                string bpCode, installNumber, licHardwareKey, sysType, fileId, dateCreate, versionIrs;
                List<LicenseInfoImpl> lstLicenseInfo;
                //đọc dữ liệu license
                try
                {
                    //đọc license file
                    Function.ReadLicense(Function.ToString(dtLicense.Rows[0][0]), out bpCode, out installNumber, out licHardwareKey, out fileId, out sysType, out dateCreate, out versionIrs, out lstLicenseInfo);
                }
                catch (Exception ex)
                {
                    Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                    return new HttpResult
                    {
                        msg_code = MessageCode.LicenseInvalid,
                        message = "Giấy phép không hợp lệ. Vui lòng sử dụng giấy phép khác để tiếp tục sử dụng dịch vụ"
                    };
                }
                if (lstLicenseInfo == null || !lstLicenseInfo.Any())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.LicenseInvalid,
                        message = "Giấy phép không hợp lệ. Vui lòng sử dụng giấy phép khác để tiếp tục sử dụng dịch vụ"
                    };
                }
                #region kiểm tra số lượng license sử dụng và số lượng license trong file license
                //kiểm tra dữ liệu license của user
                var dtLicenseBranch = ExecuteDataTable(@"select license_type, Count(1) amount from user_license where install_number = @_install_number group by license_type;", CommandType.Text
                                                                                                                                                     , new SqlParameter("@_install_number", installNumber));
                //không lấy được thông tin licese user hoặc user không có license
                if (dtLicenseBranch.IsNotNull())
                {
                    var rowError = dtLicenseBranch.AsEnumerable().FirstOrDefault(t => lstLicenseInfo.Exists(x => x.type_code.Equals(Function.ToString(t["license_type"])) && x.amount < Function.ParseInt(t["amount"])));
                    if (rowError != null)
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.LicenseInvalid,
                            message = "Thông tin giấy phép không chính xác. Vui lòng liên hệ quản trị viên để được hỗ trợ"
                        };
                    }
                }
                #endregion

                if (lstLicenseInfo.All(t=>t.expiry_date < DateTime.Now.Date))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.LicenseExpired,
                        message = "Giấy phép hệ thống đã hết hạn. Vui lòng gia hạn thêm giấy phép để sử dụng dịch vụ"
                    };
                }
                var minExpiredDate = DateTime.Now.Date.AddMonths(1);
                #region lấy thông tin số lượng người dùng theo từng type license
                var dtLicenseType = new DataTable();
                dtLicenseType.Columns.Add("license_type", typeof(string));
                dtLicenseType.Columns.Add("type_name", typeof(string));
                dtLicenseType.Columns.Add("amount", typeof(int));
                dtLicenseType.Columns.Add("use_amount", typeof(int));
                dtLicenseType.Columns.Add("rem_amount", typeof(int));
                dtLicenseType.Columns.Add("exp_date", typeof(DateTime));
                dtLicenseType.Columns.Add("install_number", typeof(string));
                for (var i = 0; i < lstLicenseInfo.Count; i++)
                {
                    var row = dtLicenseType.NewRow();
                    row[0] = lstLicenseInfo[i].type_code;
                    row[1] = lstLicenseInfo[i].type_name;
                    row[2] = lstLicenseInfo[i].amount;
                    var existsRow = dtLicenseBranch.AsEnumerable().FirstOrDefault(t => Function.ToString(t["license_type"]).Equals(lstLicenseInfo[i].type_code));
                    row[3] = existsRow == null ? 0 : Function.ParseInt(existsRow["amount"]);
                    row[4] = lstLicenseInfo[i].amount - Function.ParseInt(row[3]);
                    row[5] = lstLicenseInfo[i].expiry_date;
                    row[6] = installNumber;
                    dtLicenseType.Rows.Add(row);
                    if (lstLicenseInfo[i].expiry_date >= DateTime.Now.Date && lstLicenseInfo[i].expiry_date < minExpiredDate)
                    {
                        minExpiredDate = lstLicenseInfo[i].expiry_date;
                    }
                }
                dtLicenseType.AcceptChanges();
                #endregion
                
                dtLicenseType.TableName = "license_info";
                //lấy thông tin mapping license của toàn bộ người dùng trên hệ thống
                var dtUserLic = ExecuteDataTable(@"select user_code, license_type from user_license where install_number = @_install_number", CommandType.Text
                                                                                                                                          , new SqlParameter("@_install_number", installNumber));
                dtUserLic.TableName = "users_license";
                var result = new DataSet();
                result.Tables.Add(dtLicenseType);
                result.Tables.Add(dtUserLic);

                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    content = result,
                    message = minExpiredDate.ToString("yyyyMMdd")
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = "Dữ liệu giấy phép không chính xác"
                };
            }
        }

        public HttpResult CheckTrialInfomation(string userName)
        {
            try
            {
                #region lấy thông tin ngày đăng kí và user supper của database để check trial
                var dataTrial = ExecuteData(@"select ""CreateDate"" from ""APZ_CINF"";
                                              select ""UserCode"" from APZ_OUSR where ""UserType"" = 'S';");
                if (dataTrial == null || dataTrial.Tables.Count != 2 || !dataTrial.Tables[0].IsNotNull() || !dataTrial.Tables[1].IsNotNull())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Không thể kểm tra dữ liệu dùng thử của hệ thống"
                    };
                }
                var registerDate = Function.ParseDateTimes(dataTrial.Tables[0].Rows[0][0]);
                var databaseSupperUser = Function.ToString(dataTrial.Tables[1].Rows[0][0]);
                #endregion

                if (!userName.ToLower().Equals(databaseSupperUser.ToLower()))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Hệ thống đang ở trạng thái dùng thử. Sử dụng tài khoản quản trị để đăng nhập hệ thống"
                    };
                }
                if (registerDate.AddMonths(1) < DateTime.Now.Date)
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Hệ thống hết thời gian dùng thử. Vui lòng mua giấy phép để tiếp tục sử dụng hệ thống"
                    };
                }
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    content = registerDate.AddMonths(1)
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        public HttpResult ImportLicenseFile(string contentLicenseFile)
        {
            try
            {
                if (string.IsNullOrEmpty(contentLicenseFile))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Nội dung giấy phép chưa được cung cấp"
                    };
                }
                
                string bpCode, installNumber, hardwareKey, sysType, fileId, dateCreate, versionIrs;
                List<LicenseInfoImpl> lstLicense;
                //dọc dữ liệu license file
                try
                {
                    Function.ReadLicense(contentLicenseFile, out bpCode, out installNumber, out hardwareKey, out fileId, out sysType, out dateCreate, out versionIrs, out lstLicense);
                }
                catch (Exception ex)
                {
                    Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                    return new HttpResult
                    {
                        msg_code = MessageCode.Exception,
                        message = "Nội dụng giấy phép không đúng. Không thể đọc dữ liệu giấy phép"
                    };
                }
                //lấy dữ liệu hardwareKey từ hệ thống
                var domainHarwareKey = Function.GetHardwareKey();
                #region kiểm tra dữ liệu key
                if (string.IsNullOrEmpty(domainHarwareKey))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.SystemInfoInvalid,
                        message = "Không thể lưu giấy phép. Thông tin hệ thống không chính xác"
                    };
                }
                if (!hardwareKey.Equals(domainHarwareKey))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.LicenseInvalid,
                        message = "Giấy phép sử dụng cho hệ thống khác. Vui lòng chọn giấy phép khác"
                    };
                }
                #endregion
                #region lưu dữ liệu giấy phép vào hệ thống

                var lst = new List<SqlParameter>
                {
                    new SqlParameter("_bpcode", bpCode),
                    new SqlParameter("_install_number", installNumber),
                    new SqlParameter("_hardware_key", hardwareKey),
                    new SqlParameter("_license_info", contentLicenseFile),
                    new SqlParameter("_file_id", fileId),
                    new SqlParameter("_license_type", sysType),
                    new SqlParameter("_date_request", Function.Encrypt(DateTime.Now.Date.ToString("yyyyMMdd")))

                };
                var dtLicenseType = new DataTable();
                dtLicenseType.Columns.Add("license_type", typeof (string));
                dtLicenseType.Columns.Add("amount", typeof (int));
                for (var i = 0; i < lstLicense.Count; i++)
                {
                    var row = dtLicenseType.NewRow();
                    row[0] = lstLicense[i].type_code;
                    row[1] = lstLicense[i].amount;
                    dtLicenseType.Rows.Add(row);
                }
                dtLicenseType.AcceptChanges();
                using (DatabaseService.OpenConnection())
                {
                    using (var tran = DatabaseService.BeginTransaction())
                    {
                        CreateTempTable(dtLicenseType, "_temp_license_type");
                        BulkCopy(dtLicenseType, "_temp_license_type");
                        var result = ExecuteDataTable(@"aplus_licensemanager_import", CommandType.StoredProcedure, lst.ToArray());
                        if (!result.IsNotNull())
                        {
                            return new HttpResult
                            {
                                msg_code = MessageCode.UnableAccessDatabase,
                                message = "Cơ sở dữ liệu không phản hồi vui lòng thử lại sau"
                            };
                        }
                        var msgCode = (MessageCode)Function.ParseInt(result.Rows[0][0]);
                        if(msgCode == MessageCode.Success)
                            tran.Commit();
                        return new HttpResult
                        {
                            msg_code = msgCode,
                            message = Function.ToString(result.Rows[0][1])
                        };
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        public HttpResult MappingUserLicense(DataSet dsLicUser)
        {
            try
            {
                //lấy system hardwarekey
                var systemHwk = Function.GetHardwareKey();
                //lấy dữ liệu license của hệ thống
                var licenseInfoResult = GetDataLicenseInfomation(systemHwk);
                if (licenseInfoResult.msg_code != MessageCode.Success)
                {
                    return licenseInfoResult;
                }
                var dsLicense = licenseInfoResult.content as DataSet;
                using (DatabaseService.OpenConnection())
                {
                    using (var tran = DatabaseService.BeginTransaction())
                    {
                        #region khởi tạo bảng tạm
                        CreateTempTable(dsLicUser.Tables["users"], "_temp_user");
                        BulkCopy(dsLicUser.Tables["users"], "_temp_user");
                        var dtUserLic = dsLicUser.Tables["users_license"];
                        if (dtUserLic == null || dtUserLic.Columns.Count == 0)
                        {
                            dtUserLic.Columns.Add("license_type");
                            dtUserLic.Columns.Add("user_code");
                        }
                        CreateTempTable(dtUserLic, "_temp_user_lic");
                        BulkCopy(dtUserLic, "_temp_user_lic");
                        CreateTempTable(dsLicense.Tables[0], "_temp_lic_info");
                        BulkCopy(dsLicense.Tables[0], "_temp_lic_info");
                        #endregion
                        var dtResult = ExecuteDataTable("aplus_licensemanager_mappinguserlicense", CommandType.StoredProcedure, new SqlParameter("_hwk", systemHwk));
                        if(!dtResult.IsNotNull())
                        {
                            return new HttpResult(MessageCode.UnableAccessDatabase, "Không thể kết nối cơ sở dữ liệu");
                        }
                        var msgCode = (MessageCode)Function.ParseInt(dtResult.Rows[0][0]);
                        if(msgCode == MessageCode.Success)
                        {
                            tran.Commit();
                        }
                        return new HttpResult(msgCode, Function.ToString(dtResult.Rows[0][1]));
                    }
                }
            }
            catch (PostgresException ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = string.Format(@"{0} - {1}", ex.Where, ex.Message)
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        public HttpResult CheckUserMapping(DataTable dtUserLic)
        {
            try
            {
                using (DatabaseService.OpenConnection())
                {
                    CreateTempTable(dtUserLic, "_temp_user");
                    BulkCopy(dtUserLic, "_temp_user");
                    var dtResult = ExecuteDataTable("select a.user_code from _temp_user a left join user_info b on a.user_code = b.user_code where b.user_code is null", CommandType.Text);
                    if(dtResult.IsNotNull())
                    {
                        return new HttpResult(MessageCode.UserNotCorrect, string.Format(@"Người dùng {0} không chính xác", Function.ToString(dtResult.Rows[0][0])));
                    }
                    return new HttpResult(MessageCode.Success);
                }
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        public DataTable GetListLocalUser()
        {
            return ExecuteDataTable("select user_code, user_name, email, phone1, status from user_info");
        }
    }
}
