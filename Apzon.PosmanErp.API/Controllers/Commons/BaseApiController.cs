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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PosmanErp.Attributes;

namespace PosmanErp.Controllers.Commons
{
    /// <summary>
    /// controller base. Xử lý các init data
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [AuthenticationJwtToken]
    public class BaseApiController<TEntity> : ControllerBase where TEntity : class
    {
        /// <summary>
        /// accessToken của request hiệu tại
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// interface để dùng những hàm dùng chung.
        /// </summary>
        public IBaseAction<TEntity> BaseAction { get; set; }

        public APlusObjectType ObjectType { get; set; }

        /// <summary>
        /// service unit được khởi tạo mỗi khi có request được xác thực theo access token
        /// </summary>
        protected IUnitOfWork UnitOfWork;

        /// <summary>
        /// hàm khởi tạo các controller. Sử dụng access token để lấy unitofwork
        /// </summary>
        /// <param name="accessToken"></param>
        public BaseApiController(string accessToken)
        {
            //kiểm tra dữ liệu bearer token
            if(!string.IsNullOrEmpty(accessToken) && accessToken.Length > 7 && accessToken.StartsWith("Bearer "))
            {
                AccessToken = accessToken.Substring(7);
                if (!string.IsNullOrEmpty(AccessToken))
                {
                    //lấy service unit theo accesstoken
                    var result = GetApzUnitOfWork(AccessToken);
                    if (result != null)
                    {
                        UnitOfWork = result;
                    }
                }
            }
        }


        /// <summary>
        /// lấy user id đăng nhập với token hiện tại
        /// </summary>
        /// <returns></returns>
        protected int GetUserSign()
        {
            var userInfo = GlobalData.TokenUserList[AccessToken];
            return userInfo.UserSign;
        }

        /// <summary>
        /// lấy branch đăng nhập với token hiện tại
        /// </summary>
        /// <returns></returns>
        protected int GetBranch()
        {
            var userInfo = GlobalData.TokenUserList[AccessToken];
            return userInfo.BranchCode;
        }

        /// <summary>
        /// lấy database đăng nhập của token hiện tại
        /// </summary>
        /// <returns></returns>
        protected string GetDatabaseName()
        {
            var userInfo = GlobalData.TokenUserList[AccessToken];
            return userInfo.DatabaseName;
        }

        /// <summary>
        /// lấy domain đăng nhập của token hiện tại
        /// </summary>
        /// <returns></returns>
        protected string GetDomainName()
        {
            var userInfo = GlobalData.TokenUserList[AccessToken];
            return userInfo.DomainName;
        }

        /// <summary>
        /// lấy usercode đăng nhập với token hiện tại
        /// </summary>
        /// <returns></returns>
        protected string GetUserCode()
        {
            var userInfo = GlobalData.TokenUserList[AccessToken];
            return userInfo.UserName;
        }

        /// <summary>
        /// Lấy dữ liệu kho đăng nhập với phiên hiện tại
        /// </summary>
        /// <returns></returns>
        protected string GetWhsCode()
        {
            var userInfo = GlobalData.TokenUserList[AccessToken];
            return userInfo.WhsCode;
        }

        /// <summary>
        /// hàm lấy dữ unitofwork theo token truy cập
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        protected IUnitOfWork GetApzUnitOfWork(string accessToken)
        {
            try
            {
                //kiểm tra access token
                if (!string.IsNullOrEmpty(accessToken))
                {
                    //lock dữ liệu để xử lí tránh xunh đột với các request khác
                    lock (GlobalData.TokenUserList)
                    {
                        #region khởi tạo lại giá trị các thông số trong trường hợp bị null
                        if (GlobalData.TokenUserList == null)
                        {
                            GlobalData.TokenUserList = new Dictionary<string, TokenUserInfoImpl>();
                        }
                        if (GlobalData.PosWorkList == null)
                        {
                            GlobalData.PosWorkList = new Dictionary<string, ConnectionData>();
                        }
                        #endregion
                        //khởi tạo common unit
                        var commonUnitOfWork = UnitOfWorkFactory.GetUnitOfWork(GlobalData.CommonConnectionString, GlobalData.CommonSchemaName, GlobalData.DatabaseSystemType);
                        #region lấy dữ liệu databaseName theo accesstoken
                        string dbName;
                        //nếu tồn tại trong Global thì trả về dữ liệu
                        if (GlobalData.TokenUserList.ContainsKey(accessToken))
                        {
                            dbName = GlobalData.TokenUserList[accessToken].DatabaseName;
                        }
                        //trong trường hợp không tồn tại trong global thì trả về null
                        else
                        {
                            var dataToken = commonUnitOfWork.User.GetInfoAccessToken(accessToken);
                            if (!dataToken.IsNotNull() || Function.ToString(dataToken.Rows[0]["validated"]).Equals("N"))
                            {
                                return null;
                            }
                            dbName = Function.ToString(dataToken.Rows[0]["database_name"]);
                            GlobalData.TokenUserList.Add(accessToken, new TokenUserInfoImpl
                            {
                                DatabaseName = dbName,
                                DomainName = Function.ToString(dataToken.Rows[0]["domain_name"]),
                                UserName = Function.ToString(dataToken.Rows[0]["user_code"]).ToLower(),
                                ExpiryDate = Function.ParseDateTimes(dataToken.Rows[0]["expiry_date"]),
                                UserSign = Function.ParseInt(dataToken.Rows[0]["user_sign"]),
                                WhsCode = Function.ToString(dataToken.Rows[0]["whs_code"]),
                                BranchCode = Function.ParseInt(dataToken.Rows[0]["branch_code"]),
                                Validated = "Y"
                            });
                        }
                        #endregion

                        //Kiểm tra service unit nếu tồn tại trả về service unit trong global
                        if (GlobalData.PosWorkList.ContainsKey(dbName))
                        {
                            var dataConnection = GlobalData.PosWorkList[dbName];
                            return UnitOfWorkFactory.GetUnitOfWork(dataConnection.ConnectionString, dataConnection.SchemaName, GlobalData.DatabaseSystemType);
                        }
                        #region trong trường hợp không tồn tại service unit thì khởi tạo dữ liệu từ common database

                        IUnitOfWork databaseUnitOfWork;
                        if (dbName.Equals(GlobalData.CommonDb))
                        {
                            databaseUnitOfWork = UnitOfWorkFactory.GetUnitOfWork(GlobalData.CommonConnectionString, GlobalData.CommonSchemaName, GlobalData.DatabaseSystemType);
                            GlobalData.PosWorkList.Add(dbName, new ConnectionData(GlobalData.CommonConnectionString, GlobalData.CommonSchemaName));
                            return databaseUnitOfWork;
                        }
                        //lấy thông số kết nối database từ common DB
                        var dataDatabaseConnection = commonUnitOfWork.Commons.GetDataDatabaseConnection(dbName);
                        //trong trường hợp tồn tại dữ liệu thì khài báo service unit trong global
                        if (dataDatabaseConnection.IsNotNull())
                        {
                            var dbServerName = Function.Decrypt(Function.ToString(dataDatabaseConnection.Rows[0]["db_server"]));
                            var dbUserName = Function.Decrypt(Function.ToString(dataDatabaseConnection.Rows[0]["db_user"]));
                            var dbPassword = Function.Decrypt(Function.ToString(dataDatabaseConnection.Rows[0]["db_pass"]));
                            var dbSchema = Function.Decrypt(Function.ToString(dataDatabaseConnection.Rows[0]["db_schema"]));
                            var dbPort = Function.Decrypt(Function.ToString(dataDatabaseConnection.Rows[0]["db_server_port"]));
                            var databaseConnectionString = GetConnectionStringByDatabaseInfo(dbServerName, dbPort, dbUserName, dbPassword, dbName);
                            databaseUnitOfWork = UnitOfWorkFactory.GetUnitOfWork(databaseConnectionString, dbSchema, GlobalData.DatabaseSystemType);
                            GlobalData.PosWorkList.Add(dbName, new ConnectionData(databaseConnectionString, dbSchema));
                            return databaseUnitOfWork;
                        }
                        //trả về null trong trường hợp không tồn tại service unit
                        Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), null, "Cannot get connection data from common database;" + dbName);
                        #endregion
                    }
                }
                else
                {
                    Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), null, "Session Id is null");
                }
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
            }
            return null;
        }

        /// <summary>
        /// hàm lấy common unit theo thông tin config hệ thống
        /// </summary>
        /// <returns></returns>
        protected IUnitOfWork GetCommonUnitOfWork()
        {
            return UnitOfWorkFactory.GetUnitOfWork(GlobalData.CommonConnectionString, GlobalData.CommonSchemaName, GlobalData.DatabaseSystemType);
        }

        /// <summary>
        /// hàm lấy thông in unit of work theo thông tin của database truyền vào
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="serverPort"></param>
        /// <param name="dbUserName"></param>
        /// <param name="dbPassword"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        protected string GetConnectionStringByDatabaseInfo(string serverName, string serverPort, string dbUserName, string dbPassword, string databaseName)
        {
            var dbConnectionString = GlobalData.DatabaseSystemType == DatabaseSystemType.SQL 
                                ? string.Format(@"Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", serverName, dbUserName, dbPassword, databaseName)
                                : string.Format(@"Server={0};Port={1};Database={4};User id={2};Password={3};Pooling=false;MinPoolSize=5;MaxPoolSize=50;", serverName, serverPort
                                                                                , dbUserName, dbPassword, databaseName);
            return dbConnectionString;
        }

        #region Default Controller
        /// <summary>
        /// Lấy danh sách object theo điều kiện
        /// </summary>
        /// <param name="searchinfo"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResult Get([FromBody] SearchDocumentModel searchinfo)
        {
            try
            {
                return BaseAction.Get(searchinfo);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        /// <summary>
        /// Lấy object theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual HttpResult GetById(string id)
        {
            try
            {
                return BaseAction.GetById(id);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        /// <summary>
        /// Tạo mới dữ liệu
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResult Create([FromBody] TEntity dtval)
        {
            try
            {
                return BaseAction.Create(ObjectType, ObjectType, dtval,  GetBranch(),GetUserSign());
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResult Update([FromBody] TEntity dtval)
        {
            try
            {
                return BaseAction.Update(ObjectType, ObjectType, dtval,  GetBranch(), GetUserSign());
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        /// <summary>
        /// Xóa dữ liệu
        /// </summary>
        /// <param name="deleteinfo"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResult Delete([FromBody] SearchDocumentModel deleteinfo)
        {
            try
            {
                return BaseAction.Delete(ObjectType, ObjectType, deleteinfo.code,  GetBranch(), GetUserSign());
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
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
