using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services;
using Apzon.PosmanErp.Services.Interfaces;
using ApzonIrsWeb.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Attributes;
using PosmanErp.Controllers.Commons;
using PosmanErp.Helper;

namespace PosmanErp.Controllers.Administrator
{
    /// <summary>
    /// controller user
    /// </summary>
    public class UserController : BaseApiController<DataSet>
    {
        /// <summary>
        /// hàm khởi tạo
        /// </summary>
        /// <param name="accessToken"></param>
        public UserController(string accessToken) : base(accessToken)
        {
            //Cần khai báo interface và ObjectType để dùng BaseAction trong BaseApiController 
            if (UnitOfWork != null)
            {
                BaseAction = UnitOfWork.User;
            }
            ObjectType = Apzon.PosmanErp.Commons.APlusObjectType.oUser;
        }
        public void UpdateAuto()
        {
            UnitOfWork.User.UpdateAuto(GlobalData.version);
        }
             
        /// <summary>
        /// lấy danh sách database trên hệ thống
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResult GetListDatabase()
        {
            var commonUnit = GetCommonUnitOfWork();
            return commonUnit.User.GetListDatabase();
        }
        
        /// <summary>
        /// đăng nhập hệ thống
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult Login([FromBody] dynamic loginInfo)
        {
            try
            {
                string userName = Function.ToString(loginInfo.user_name);
                string password = Function.ToString(loginInfo.password);
                string databaseName = Function.ToString(loginInfo.db_code);

                #region validate login infomation theo open API

                OpenApiHttpResult<OpenApiTokenInfo> openApiLogin;
                using (var http = new HttpClientHelper<OpenApiHttpResult<OpenApiTokenInfo>>(GlobalData.OpenApiAdress, "User"))
                {
                    openApiLogin = http.Post("Login", new List<Object> { new { UserName = userName, Password = password } });
                }

                #endregion

                if (openApiLogin.MessageCode != 200 && (!userName.ToLower().Equals(GlobalData.RootUser) || !password.Equals(GlobalData.RootPass)))
                {
                    return new HttpResult(MessageCode.UserNotCorrect, openApiLogin.Message);
                }

                var commonUnitOfWork = GetCommonUnitOfWork();
                IUnitOfWork dataService;

                #region kiểm tra dữ liệu database
                lock (GlobalData.PosWorkList)
                {
                    if (GlobalData.PosWorkList.ContainsKey(databaseName.ToLower()))
                    {
                        var infoDb = GlobalData.PosWorkList[databaseName.ToLower()];
                        dataService = UnitOfWorkFactory.GetUnitOfWork(infoDb.ConnectionString, infoDb.SchemaName, GlobalData.DatabaseSystemType);
                    }
                    else
                    {
                        var dtDatabase = commonUnitOfWork.Commons.GetDataDatabaseConnection(databaseName);
                        if (!dtDatabase.IsNotNull())
                        {
                            return new HttpResult(MessageCode.UnableAccessDatabase, "Thông tin đăng nhâp không đúng vui lòng thử lại");
                        }

                        var dbServerName = Function.Decrypt(Function.ToString(dtDatabase.Rows[0]["db_server"]));
                        var dbUserName = Function.Decrypt(Function.ToString(dtDatabase.Rows[0]["db_user"]));
                        var dbPassword = Function.Decrypt(Function.ToString(dtDatabase.Rows[0]["db_pass"]));
                        var dbSchema = Function.Decrypt(Function.ToString(dtDatabase.Rows[0]["db_schema"]));
                        var dbPort = Function.Decrypt(Function.ToString(dtDatabase.Rows[0]["db_server_port"]));

                        var connectionString = GetConnectionStringByDatabaseInfo(dbServerName, dbPort, dbUserName, dbPassword, databaseName);
                        GlobalData.PosWorkList.Add(databaseName.ToLower(), new ConnectionData(connectionString, dbSchema));
                        dataService = UnitOfWorkFactory.GetUnitOfWork(connectionString, dbSchema, GlobalData.DatabaseSystemType);
                    }
                }
                // cập nhật lại API
                var updateResult = dataService.User.UpdateAuto(GlobalData.version);
                if (updateResult.msg_code != MessageCode.Success)
                {
                    return updateResult;
                }

                #endregion
                var userSign = -1;
                UserLicenseImpl licenseInfo = null;
                if (!userName.ToLower().Equals(GlobalData.RootUser))
                {
                    //check dữ liệu user ở database login
                    var resultUser = dataService.User.GetByCode(userName);
                    if (resultUser.msg_code != MessageCode.Success)
                    {
                        return resultUser;
                    }

                    var dtUser = resultUser.content as DataSet;
                    if (!dtUser.IsNotNull())
                    {
                        return new HttpResult(MessageCode.UserNotCorrect, "Người dùng hoặc mật khẩu không chính xác");
                    }

                    if (!Function.ToString(dtUser.Tables[0].Rows[0]["status"]).Equals("A"))
                    {
                        return new HttpResult(MessageCode.UserLock, "Người dùng bị khóa trên hệ thống");
                    }

                    userSign = Function.ParseInt(dtUser.Tables[0].Rows[0]["user_id"]);

                    //validate dữ liệu licese của user trên hệ thống
                    var licenseValidateInfo = commonUnitOfWork.LicenseManager.CheckUserLicense("", databaseName, userName);

                    if (licenseValidateInfo.msg_code != MessageCode.Success)
                    {
                        return licenseValidateInfo;
                    }
                    licenseInfo = licenseValidateInfo.content as UserLicenseImpl;
                }
                #region Update dữ liệu token trên hệ thống

                //lấy accesstoken từ API login
                var accessToken = userName.ToLower().Equals(GlobalData.RootUser) ? Function.RandomSession(userName, databaseName) : openApiLogin.Content.Token;
                //lock dữ liệu tránh xung đột
                lock (GlobalData.TokenUserList)
                {
                    //khởi tạo dữ liệu trong trường hợp bị null
                    if (GlobalData.TokenUserList == null)
                    {
                        GlobalData.TokenUserList = new Dictionary<string, TokenUserInfoImpl>();
                    }
                    #region xóa dữ liệu token của user cùng tên đã đăng nhâp trước đó
                    var resultUpdateOldToken = commonUnitOfWork.User.RemoveOldToken(userName);
                    if (resultUpdateOldToken.msg_code != MessageCode.Success)
                    {
                        return resultUpdateOldToken;
                    }
                    var oldToken = GlobalData.TokenUserList.FirstOrDefault(t => t.Value.UserName.Equals(userName.ToLower())).Key;
                    if (!string.IsNullOrEmpty(oldToken))
                    {
                        GlobalData.TokenUserList.Remove(oldToken);
                    }
                    #endregion
                    //cập nhật dữ liệu accesstoken mới vào common database 
                    commonUnitOfWork.User.AddNewToken(accessToken, userName, databaseName, "", userName.ToLower().Equals(GlobalData.RootUser) ? DateTime.Now.AddDays(1) : openApiLogin.Content.ExpiredTime, userSign);
                    //khởi tạo dữ liệu token info mới
                    GlobalData.TokenUserList.Add(accessToken, new TokenUserInfoImpl
                    {
                        DatabaseName = databaseName,
                        UserName = userName.ToLower(),
                        ExpiryDate = userName.ToLower().Equals(GlobalData.RootUser) ? DateTime.Now.AddDays(1) : openApiLogin.Content.ExpiredTime,
                        UserSign = userSign,
                        Validated = "N"
                    });
                }

                #endregion
               
                //đăng nhập thành công trả về mã code và accesstoken
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    content = new
                    {
                        access_token = accessToken,
                        user_info =  new {                            
                            user_code = userName,
                            user_name = userName.ToLower().Equals(GlobalData.RootUser) ? GlobalData.RootUser : openApiLogin.Content.FullName,
                            user_id = userSign
                        },
                        license_info = licenseInfo
                    }
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

        #region Register
        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Create([FromBody] DataSet dtval)
        {
            try
            {
                //Register in Open Api
                //Tạo mới DataTable to request to Open Api
                #region open API
                DataTable dtuser = new DataTable();
                dtuser.Columns.Add("UserName", typeof(string));
                dtuser.Columns.Add("Password", typeof(string));
                dtuser.Columns.Add("Email", typeof(string));
                dtuser.Columns.Add("FullName", typeof(string));
                dtuser.Columns.Add("Phone", typeof(string));
                dtuser.Columns.Add("Address", typeof(string));
                dtuser.Rows.Add(Function.ToString(dtval.Tables["info"].Rows[0]["user_code"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["password"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["email"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["user_name"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["phone1"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["address"])
                    );
                var openApiRegister = new OpenApiHttpResult<OpenApiUserInfo>();
                using (var http = new HttpClientHelper<OpenApiHttpResult<OpenApiUserInfo>>(GlobalData.OpenApiAdress, "User"))
                {
                    openApiRegister = http.Post("Register", dtuser);
                }
                #endregion
                if (openApiRegister.MessageCode != 200 && openApiRegister.MessageCode != 1086)
                {
                    var msg_code = ConvertMsgCode(openApiRegister.MessageCode);
                    return new HttpResult
                    {
                        msg_code = (MessageCode)msg_code,
                        message = openApiRegister.Message,
                        content = openApiRegister.Content
                    };
                }
                //Create in Posman Database
                var createUser = UnitOfWork.User.Create(ObjectType, ObjectType, dtval,  GetBranch(), GetUserSign());
                if (createUser.msg_code != MessageCode.Success)
                {
                    return createUser;
                }
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Created Successfully.",
                    content = new
                    {
                        user_code = Function.ToString(dtval.Tables[0].Rows[0]["user_code"]),
                        password = "**************"
                    }
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

        /// <summary>
        /// Lấy thông tin người dùng theo Id
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet]
        public override HttpResult GetById(string user_id)
        {
            return UnitOfWork.User.GetById(user_id);
        }

        /// <summary>
        /// Lấy thông tin người dùng theo Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult GetByCode(string user_code)
        {
            return UnitOfWork.User.GetByCode(user_code);
        }

        /// <summary>
        /// Lấy danh sách branch của người dùng
        /// </summary>
        /// <param name="user_code"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult GetListCompany(string user_code)
        {
            return UnitOfWork.User.GetListCompany(user_code);
        }

        /// <summary>
        /// Lấy thông tin config front end của người dùng
        /// </summary>
        /// <param name="user_code"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult GetListConfig(string user_code)
        {
            return UnitOfWork.User.GetListConfig(user_code);
        } 
        
        /// <summary>
        /// Cập nhật thông tin config front end của người dùng
        /// </summary>
        /// <param name="UpdateInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult UpdateConfig([FromBody] DataTable UpdateInfo)
        {
            return UnitOfWork.User.UpdateConfig(UpdateInfo);
        }

        [HttpPost]
        public HttpResult DeleteConfig([FromBody] DataTable DeleteInfo)
        {
            return UnitOfWork.User.DeleteConfig(DeleteInfo);
        }
        #endregion

        #region Update user
        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update([FromBody] DataSet dtval)
        {
            try
            {
                #region Update thông tin lên Open Api
                //Tạo mới DataTable to request to Open Api
                DataTable dtuser = new DataTable();
                dtuser.Columns.Add("UserName", typeof(string));
                dtuser.Columns.Add("FullName", typeof(string));
                dtuser.Columns.Add("Email", typeof(string));
                dtuser.Columns.Add("Phone", typeof(string));
                dtuser.Columns.Add("Address", typeof(string));
                dtuser.Rows.Add(Function.ToString(dtval.Tables["info"].Rows[0]["user_code"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["user_name"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["email"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["phone1"]),
                    Function.ToString(dtval.Tables["info"].Rows[0]["address"])
                    );
                var openApiUpdateUser = new OpenApiHttpResult<OpenApiUserInfo>();
                using (var http = new HttpClientHelper<OpenApiHttpResult<OpenApiUserInfo>>(GlobalData.OpenApiAdress, "User", AccessToken))
                {
                    openApiUpdateUser = http.Post("UpdateUser", dtuser);
                }
                if (openApiUpdateUser.MessageCode != 200)
                {
                    var msg_code = ConvertMsgCode(openApiUpdateUser.MessageCode);
                    return new HttpResult
                    {
                        msg_code = (MessageCode)msg_code,
                        message = openApiUpdateUser.Message,
                        content = openApiUpdateUser.Content
                    };
                }
                #endregion
                #region Update thông tin lên database
                var updateUser = UnitOfWork.User.Update(ObjectType, ObjectType, dtval, GetBranch(), GetUserSign());
                if (updateUser.msg_code != MessageCode.Success)
                    return updateUser;
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Update Successfully.",
                    content = new
                    {
                        user_code = Function.ToString(dtval.Tables["info"].Rows[0]["user_code"]),
                        user_name = Function.ToString(dtval.Tables["info"].Rows[0]["user_name"]),
                        email = Function.ToString(dtval.Tables["info"].Rows[0]["email"]),
                        phone1 = Function.ToString(dtval.Tables["info"].Rows[0]["phone1"]),
                        phone2 = Function.ToString(dtval.Tables["info"].Rows[0]["phone2"]),
                        address = Function.ToString(dtval.Tables["info"].Rows[0]["address"])
                    }
                };
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

        /// <summary>
        /// Cập nhật mật khẩu người dùng
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult UpdatePassword([FromBody] DataTable dtval)
        {
            try
            {
                #region UpdatePassword lên OpenApi
                //Tạo bảng để request sang Open Api
                DataTable dtuser = new DataTable();
                dtuser.Columns.Add("UserName", typeof(string));
                dtuser.Columns.Add("OldPassword", typeof(string));
                dtuser.Columns.Add("NewPassword", typeof(string));
                dtuser.Rows.Add(Function.ToString(dtval.Rows[0]["user_code"]),
                    Function.ToString(dtval.Rows[0]["old_password"]),
                    Function.ToString(dtval.Rows[0]["new_password"])
                    );
                var openApiUpdatePass = new OpenApiHttpResult<OpenApiUserInfo>();
                using (var http = new HttpClientHelper<OpenApiHttpResult<OpenApiUserInfo>>(GlobalData.OpenApiAdress, "User", AccessToken))
                {
                    openApiUpdatePass = http.Post("UpdatePassword", dtuser);
                }
                if (openApiUpdatePass.MessageCode != 200)
                {
                    //Convert msg_code Open to Aplus
                    var msg_code = ConvertMsgCode(openApiUpdatePass.MessageCode);
                    return new HttpResult
                    {
                        msg_code = (MessageCode)msg_code,
                        message = openApiUpdatePass.Message,
                        content = openApiUpdatePass.Content
                    };
                }
                #endregion
                #region Update Password in Posman database
                var updatePass = UnitOfWork.User.UpdatePassword(dtval);
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = openApiUpdatePass.Message,
                    content = openApiUpdatePass.Content
                };
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

        private int ConvertMsgCode(int messageCode)
        {
            if (messageCode != null)
            {
                switch (messageCode)
                {
                    //Email Exists
                    case 1084:
                        messageCode = 10110;
                        break;
                    //Phone Exists
                    case 1085:
                        messageCode = 10111;
                        break;
                    //User Exists
                    case 1086:
                        messageCode = 10112;
                        break;
                    //Old Password Not Correct
                    case 1087:
                        messageCode = 10113;
                        break;
                    //Old Password And New Password Must Not Coincide
                    case 1088:
                        messageCode = 10114;
                        break;
                    //User Not Exists
                    case 1089:
                        messageCode = 10109;
                        break;
                    //Data User Not Provide
                    case 1090:
                        messageCode = 10116;
                        break;
                    //Email Wrong Format
                    case 1091:
                        messageCode = 10117;
                        break;
                    //Phone Wrong Format
                    case 1092:
                        messageCode = 10118;
                        break;
                }
            }
            return messageCode;
        }

        /// <summary>
        /// Cập nhật trạng thái người dùng
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult UpdateStatus([FromBody] DataTable dtval)
        {
            return UnitOfWork.User.UpdateStatus(dtval);
        }

        /// <summary>
        /// Đặt password cho tài khoản người dùng
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult SetPassword([FromBody] dynamic userinfo)
        {
            try
            {
                string userName = Function.ToString(userinfo.user_code);
                string password = Function.ToString(userinfo.password);
                //Request to OpenApi
                DataTable dtuser = new DataTable();
                dtuser.Columns.Add("UserName", typeof(string));
                dtuser.Columns.Add("Password", typeof(string));
                dtuser.Rows.Add(Function.ToString(userName), Function.ToString(password));
                OpenApiHttpResult<OpenApiTokenInfo> openApiSetPass;
                using (var http = new HttpClientHelper<OpenApiHttpResult<OpenApiTokenInfo>>(GlobalData.OpenApiAdress, "User", AccessToken))
                {
                    openApiSetPass = http.Post("SetPassword", dtuser);
                }
                int msg_code = openApiSetPass.MessageCode;
                if (openApiSetPass.MessageCode != 200)
                {
                    msg_code = ConvertMsgCode(openApiSetPass.MessageCode);
                }
                return new HttpResult
                {
                    msg_code = (MessageCode)msg_code,
                    message = openApiSetPass.Message,
                    content = openApiSetPass.Content
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
        #endregion
    }
}
