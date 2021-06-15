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
using Npgsql;
using ApzonIrsWeb.Entities;
using Apzon.Libraries.HDBConnection.Interfaces;
using System.IO;
using static Apzon.PosmanErp.Entities.HttpResult;

namespace Apzon.PosmanErp.Services.Repository
{
    public class UserRepository : BaseService<DataSet>, IUser
    {
        private const string TempTableName = "#APZ_OUSR";
        private DataSet _dsLicense { get; set; }

        private string TypeUpdate = string.Empty;

        public UserRepository(IDatabaseClient databaseService) : base(databaseService)
        {

        }

        public HttpResult RemoveOldToken(string userCode)
        {
            try
            {
                using (DatabaseService.OpenConnection())
                {
                    using (var tran = DatabaseService.BeginTransaction())
                    {
                        ExecuteNonQuery(@"delete from user_token where user_code = @_user_code;", CommandType.Text, new SqlParameter("_user_code", userCode));
                        tran.Commit();
                        return new HttpResult
                        {
                            msg_code = MessageCode.Success
                        };
                    }
                }
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

        public void AddNewToken(string token, string userCode, string databaseName, string domainName, DateTime expiryDate, int userSign)
        {
            try
            {
                using (DatabaseService.OpenConnection())
                {
                    using (var tran = DatabaseService.BeginTransaction())
                    {
                        ExecuteNonQuery(@"insert into user_token(user_code, access_token, database_name, domain_name, last_login_date, last_login_time, expiry_date, user_sign) 
                                            values (@userName, @accessToken, @databaseName, @domainName, current_date, cast(TO_CHAR(current_date, 'HHmm') as smallint), @expireDate, @userSign)", CommandType.Text
                                                                , new SqlParameter("@userName", userCode)
                                                                , new SqlParameter("@accessToken", token)
                                                                , new SqlParameter("@databaseName", databaseName ?? "")
                                                                , new SqlParameter("@domainName", domainName ?? "")
                                                                , new SqlParameter("@expireDate", expiryDate)
                                                                , new SqlParameter("@userSign", userSign));
                        tran.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
            }
        }

        public DataTable GetInfoAccessToken(string accessToken)
        {
            try
            {
                return ExecuteDataTable(@"select * from user_token where access_token = @accessToken", CommandType.Text, new SqlParameter("@accessToken", accessToken));
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new DataTable();
            }
        }

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype, DataSet document,
            ref string objId, ref string objNum)
        {
            DataTable dtResult;
            switch (transtype)
            {
                case TransactionType.A:
                case TransactionType.U:
                    #region Update Config
                    if (!string.IsNullOrWhiteSpace(TypeUpdate) && TypeUpdate.Equals("C"))
                    {
                        //Tạo bảng tạm
                        document.Tables[0].Columns.Add("line_num", typeof(int));
                        CreateTempTable(document.Tables[0], "_temp_user_config_2");
                        BulkCopy(document.Tables[0], "_temp_user_config_2");
                        dtResult = ExecuteDataTable(@"aplus_user_updateconfig", CommandType.StoredProcedure);
                        break;
                    }
                    #endregion
                    #region save user
                    //Validate thông tin đầu vào
                    var validate = ValidateDataUser(document);
                    if (validate.msg_code != MessageCode.Success)
                    {
                        return validate;
                    }
                    //Lấy thông tin đầu vào
                    var dtUser = document.Tables["info"];
                    var dtUserCompany = document.Tables["company"];
                    var dtDefaultInfo = document.Tables["system_default"];
                    //kết quả sau khi lưu thông tin
                    //Tạo bảng tạm thông qua thông tin truyền vào, lưu thông tin vào DB
                    //User
                    dtUser = StandardizedTable(dtUser, "user_info");
                    CreateTempTableAsDatabaseTable("user_info", "_temp_user_info");
                    BulkCopy(dtUser, "_temp_user_info");
                    //Company
                    CreateTempTable(dtUserCompany, "_temp_user_company");
                    BulkCopy(dtUserCompany, "_temp_user_company");
                    //Default
                    CreateTempTable(dtDefaultInfo, "_temp_user_config_1");
                    BulkCopy(dtDefaultInfo, "_temp_user_config_1");
                    dtResult = ExecuteDataTable("aplus_user_save", CommandType.StoredProcedure,
                        new SqlParameter("@_trans_type", Function.ToString(transtype)),
                        new SqlParameter("@_user_sign", userSign)
                        );
                    #endregion
                    break;
                case TransactionType.D:
                    // Delete Config
                    CreateTempTable(document.Tables[0], "_temp_user_config_2");
                    BulkCopy(document.Tables[0], "_temp_user_config_2");
                    dtResult = ExecuteDataTable(@"aplus_user_deleteconfig", CommandType.StoredProcedure);
                    break;
                default:
                    return new HttpResult
                    {
                        msg_code = MessageCode.FunctionNotSupport,
                        message = "Phương thức không được hỗ trợ"
                    };
            }
            var msg_code = Function.ParseInt(dtResult.Rows[0][0]);
            objId = Function.ToString(dtResult.Rows[0][1]);
            return new HttpResult
            {
                msg_code = (MessageCode)msg_code,
                content = objId
            };
        }

        public HttpResult ValidateDataUser(DataSet document)
        {
            if (document == null || document.Tables.Count != 3 || !document.Tables["info"].IsNotNull())
            {
                return new HttpResult
                {
                    msg_code = MessageCode.DataUserIsEmpty
                };
            }
            if (document.Tables["info"].Rows.Count == 0 || document.Tables["info"] == null
                || string.IsNullOrWhiteSpace(Function.ToString(document.Tables["info"].Rows[0]["user_code"])))
            {
                return new HttpResult
                {
                    msg_code = MessageCode.DataUserIsEmpty
                };
            }
            if (document.Tables["company"] == null || document.Tables["company"].Rows.Count == 0)
            {
                return new HttpResult
                {
                    msg_code = MessageCode.DataCompanyIsEmpty
                };
            }
            else
            {
                for (int i = 0; i < document.Tables["company"].Rows.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(Function.ToString(document.Tables["company"].Rows[i]["company_code"])))
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.DataCompanyIsEmpty
                        };
                    }
                }
            }
            //if (document.Tables["system_default"] == null || document.Tables["system_default"].Rows.Count == 0
            //    || string.IsNullOrWhiteSpace(Function.ToString(document.Tables["system_default"].Rows[0]["default_company"]))
            //    || string.IsNullOrWhiteSpace(Function.ToString(document.Tables["system_default"].Rows[0]["default_warehouse"])))
            //{
            //    return new HttpResult
            //    {
            //        msg_code = MessageCode.DataDefaultSystemIsEmpty,
            //        message = "Dữ liệu mặc định đang trống."
            //    };
            //}
            return new HttpResult(MessageCode.Success);
        }

        public HttpResult GetListDatabase()
        {
            try
            {
                var dtDatabase = ExecuteDataTable(@"select b.company_db db_code, b.company_name db_name from domain_database a inner join system_database b on a.database_name = b.company_db");
                if (!dtDatabase.IsNotNull())
                {
                    return new HttpResult(MessageCode.Error, "Hệ thống chưa có cơ sở dữ liệu");
                }
                return new HttpResult(MessageCode.Success, dtDatabase);
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
        public string GetLastVersionFromDB()
        {
            string queryStr = @"select version from general_setting";
            return Function.ToString(ExecuteScalar(queryStr));
        }

        public int SplitVersion(string version, TypeGetVersion type)
        {
            string result = "";
            if (type == TypeGetVersion.First)
            {
                result = version[0].ToString();
                return Function.ParseInt(result);
            }
            if (type == TypeGetVersion.Mid)
            {
                result = version[2].ToString() + version[3].ToString();
                return Function.ParseInt(result);
            }
            else
            {
                result = version[5].ToString();
                return Function.ParseInt(result);
            }

        }
        /// <summary>
        /// Cập nhật lại Database 
        /// </summary>
        /// <param name="version">phiên bản hiện tại,lấy từ GlobalData</param>
        public void UpdateNewversion(string version)
        {
            // lấy phiên bản mới nhất từ database
            string lastVer = GetLastVersionFromDB();
            // split phiên bản hiện tại
            int curLastVer = SplitVersion(version, TypeGetVersion.Last);
            int curMidVer = SplitVersion(version, TypeGetVersion.Mid);
            int curFirstVer = SplitVersion(version, TypeGetVersion.First);
            // split phiên bản mới nhất
            int lLastVer = SplitVersion(lastVer, TypeGetVersion.Last);
            int lMidVer = SplitVersion(lastVer, TypeGetVersion.Mid);
            int lFirstVer = SplitVersion(lastVer, TypeGetVersion.First);
            if (lFirstVer < curFirstVer || lMidVer < curMidVer || lLastVer < curLastVer)
            {                
                UpdateFollowType(lFirstVer, lMidVer, lLastVer, curFirstVer, curMidVer, curLastVer, HttpResult.TypeUpdate.Table);
                UpdateFollowType(lFirstVer, lMidVer, lLastVer, curFirstVer, curMidVer, curLastVer, HttpResult.TypeUpdate.View);
                UpdateFollowType(lFirstVer, lMidVer, lLastVer, curFirstVer, curMidVer, curLastVer, HttpResult.TypeUpdate.Type);
                UpdateFollowType(lFirstVer, lMidVer, lLastVer, curFirstVer, curMidVer, curLastVer, HttpResult.TypeUpdate.Function);
                UpdateFollowType(lFirstVer, lMidVer, lLastVer, curFirstVer, curMidVer, curLastVer, HttpResult.TypeUpdate.Sto);
                UpdateFollowType(lFirstVer, lMidVer, lLastVer, curFirstVer, curMidVer, curLastVer, HttpResult.TypeUpdate.Data);
            }
        }
        /// <summary>
        /// Cập nhật theo từng typeUpdate được truyền vào
        /// </summary>
        /// <param name="lFirst">first mới nhất</param>
        /// <param name="lMid">mid mới nhất</param>
        /// <param name="lLast">last mới nhất</param>
        /// <param name="cFirst">first hiện tại</param>
        /// <param name="cMid">mid hiện tại</param>
        /// <param name="cLast">last hiện tại</param>
        /// <param name="fileName">loại cập nhật</param>
        public void UpdateFollowType(int lFirst, int lMid, int lLast, int cFirst, int cMid, int cLast,TypeUpdate fileName)
        {
            while (lFirst < cFirst || (lMid < cMid && lFirst == cFirst) || (lLast < cLast && lFirst == cFirst))
            {
                while ((lMid <= cMid && lFirst <= cFirst) || (lFirst < cFirst && lMid <= 9))
                {
                    while ((lLast < cLast && lMid == cMid) || (lMid < cMid && lLast < 9) || (lFirst < cFirst && lLast < 9))
                    {
                        lLast++;
                        string path = @"E:\clone\Scripts\" + lFirst + @"\" + lMid + @"\" + lLast + @"\Update" + fileName + ".txt";
                        //ExecuteQueryFromFile(ListUpdate(path));

                    }
                    lLast = 0;
                    lMid++;
                    if (lMid <= cMid || (lFirst < cFirst && lMid < 9))
                    {
                        string path2 = @"E:\clone\Scripts\" + lFirst + @"\" + lMid + @"\" + lLast + @"\Update" + fileName + ".txt";
                        //ExecuteQueryFromFile(ListUpdate(path2));
                    }
                }
                lMid = 0;
                lLast = 0;
                lFirst++;
                if (lFirst <= cFirst)
                {
                    string path3 = @"E:\clone\Scripts\" + lFirst + @"\" + lMid + @"\" + lLast + @"\Update" + fileName + ".txt";
                    //ExecuteQueryFromFile(ListUpdate(path3));
                }
            }
        }            
        
        /// <summary>
        /// đọc các câu query lấy được từ file
        /// </summary>
        /// <param name="path">đường dẫn đến file</param>
        /// <returns></returns>
        public List<string> ListUpdate(string path)
        {
            List<string> strquery = new List<string>();
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    string str;
                    string[] strArray;
                    str = sr.ReadLine();
                    strArray = str.Split(';');
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        strquery.Add(strArray[i]);
                    }
                }
            }
            return strquery;
        }
        /// <summary>
        /// Thực hiện các câu query
        /// </summary>
        /// <param name="lst">List các câu query đọc từ file</param>

        public HttpResult ExecuteQueryFromFile(List<string> lst)
        {
            try
            {
                foreach (var item in lst)
                {
                    ExecuteNonQueryNotParse(item);
                }              
                return new HttpResult
                {
                    msg_code = MessageCode.Success
                    

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
        /// thực hiện cập nhật Database
        /// </summary>
        /// <param name="version">phiên bản hiện tại của API</param>
        /// <returns></returns>
        public HttpResult UpdateAuto(string version)
        {
            try
            {
                UpdateNewversion(version);
                ExecuteNonQuery("update general_setting set version = @version", CommandType.Text, new SqlParameter("@version",version));
                return new HttpResult
                {
                    msg_code = MessageCode.Success                 
                   
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
        public HttpResult GetByCode(string code)
        {
            try
            {
                //Validate
                if (string.IsNullOrWhiteSpace(code))
                    return new HttpResult(MessageCode.DataUserIsEmpty);
                var dt = ExecuteData(@"select * from user_info where Lower(User_Code) = Lower(@_user_code) ;
                                        Select u1.company_code,u1.company_name from user_info ou, user_company u1 where ou.user_id = u1.user_id And Lower(ou.user_code) = Lower(@_user_code);
                                         Select u2.default_company, u2.default_warehouse from user_info ou, user_config_1 u2 where ou.user_id = u2.user_id And Lower(ou.user_code) = Lower(@_user_code);",
                    CommandType.Text, new SqlParameter("@_user_code", code));
                //Rename Table
                dt.Tables[0].TableName = "info";
                dt.Tables[1].TableName = "company";
                dt.Tables[2].TableName = "system_default";
                //Trả kết quả
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success.",
                    content = dt
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

        public HttpResult GetListCompany(string user_code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user_code))
                    return new HttpResult(MessageCode.DataUserIsEmpty);

                var dt = ExecuteDataTable(@"SELECT us1.company_code, us1.company_name, us1.create_date, us1.create_time,us1.update_date,us1.update_time
                                    FROM user_company us1 INNER JOIN user_info ou ON us1.user_id = ou.User_Id
                                    Where ou.user_code = @_user_code;", CommandType.Text,
                        new SqlParameter("@_user_code", user_code)
                     );
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success",
                    content = dt
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

        public HttpResult GetListConfig(string user_code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user_code))
                    return new HttpResult
                    {
                        msg_code = MessageCode.DataNotProvide,
                        message = "user_code is empty."
                    };

                var dt = ExecuteDataTable(@"SELECT u2.code, u2.name
                                    FROM user_config_2 u2 INNER JOIN user_info u ON u2.user_id = u.User_Id
                                    Where u.User_Code = @_user_code;", CommandType.Text,
                     new SqlParameter("@_user_code", user_code)
                     );
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success",
                    content = dt
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

        public HttpResult UpdatePassword(DataTable dtval)
        { 
            var dt = ExecuteNonQuery(@"UPDATE user_info 
                                        SET password = @_newpassword
                                        WHERE Lower(user_code) = Lower(@_usercode); ", CommandType.Text,
                             new SqlParameter("@_usercode",Function.ToString(dtval.Rows[0]["user_code"])),
                             new SqlParameter("@_newpassword",Function.GetMd5Hash(Function.ToString(dtval.Rows[0]["new_password"])))
                             );
            if (dt == 0)
                return new HttpResult(MessageCode.UserNotCorrect);
            return new HttpResult
            {
                msg_code = MessageCode.Success,
                message = "Update Successful."
            };
        }

        public HttpResult UpdateStatus(DataTable dtval)
        {
            try
            {
                #region validate
                if (string.IsNullOrWhiteSpace(Function.ToString(dtval.Rows[0]["user_code"])))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.DataUserIsEmpty,
                        message = "user_code đang trống."
                    };
                }
                if (string.IsNullOrWhiteSpace(Function.ToString(dtval.Rows[0]["user_code"])) ||
                    (Function.ToString(dtval.Rows[0]["user_code"]).Equals("A") && Function.ToString(dtval.Rows[0]["user_code"]).Equals("I")))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.StatusUserNotCorrect,
                        message = "Trạng thái đang trống hoặc không đúng định dạng."
                    };
                }
                #endregion
                var dt = ExecuteNonQuery(@"UPDATE user_info 
                                            SET status = @_status_update
	                                        WHERE Lower(user_code) = Lower(@_usercode); ", CommandType.Text,
                            new SqlParameter("@_usercode", Function.ToString(dtval.Rows[0]["user_code"])),
                            new SqlParameter("@_status_update", Function.ToString(dtval.Rows[0]["status"]))
                            );
                if (dt == 0)
                    return new HttpResult
                    {
                        msg_code = MessageCode.UserNotFound,
                        message = "Update fail, user_code my not be wrong."
                    };
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Update Successful."
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

        public HttpResult UpdateConfig(DataTable dtval)
        {
            try
            {
                #region validate
                if (dtval == null || dtval.Rows.Count == 0)
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.DataUserConfigIsEmpty
                    };
                }
                #endregion
                TypeUpdate = "C";
                //Declare Data Set
                DataSet InputDocument = new DataSet();
                InputDocument.Tables.Add(dtval);
                return Process(APlusObjectType.oUser, APlusObjectType.oUser, -1, TransactionType.U, InputDocument, -1);
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

        public HttpResult DeleteConfig(DataTable dtval)
        {
            try
            {
                #region validate
                if (dtval == null || dtval.Rows.Count != 1)
                {
                    return new HttpResult(MessageCode.DataUserConfigIsEmpty);
                }
                if (string.IsNullOrWhiteSpace(Function.ToString(dtval.Rows[0]["user_code"])) ||
                    string.IsNullOrWhiteSpace(Function.ToString(dtval.Rows[0]["code"])))
                    return new HttpResult(MessageCode.DataUserConfigIsEmpty);
                #endregion
                DataSet InputDocument = new DataSet();
                InputDocument.Tables.Add(dtval);
                return Process(APlusObjectType.oUser, APlusObjectType.oUser, -1, TransactionType.D, InputDocument,-1);
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

        #region Base action
        public HttpResult Get(SearchDocumentModel Search)
        {
            try
            {
                if (Search == null)
                    Search = new SearchDocumentModel();
                var dt = ExecuteData("aplus_user_get", CommandType.StoredProcedure,
                new SqlParameter("@_search_name", Function.ToString(Search.search_name)),
                new SqlParameter("@_status", Function.ToString(Search.status)),
                new SqlParameter("@_user_type", Function.ToString(Search.type)),
                new SqlParameter("@_company", Search.company),
                new SqlParameter("@_user_sign", Function.ParseInt(Search.user_sign)),
                new SqlParameter("@_page_size", Function.ParseInt(Search.page_size)),
                new SqlParameter("@_page_index", Function.ParseInt(Search.page_index)),
                new SqlParameter("@_from_date", Search.from_date),
                new SqlParameter("@_to_date", Search.to_date)
                );
                //Set lại tên bảng
                dt.Tables[0].TableName = Constants.Pagination;
                dt.Tables[1].TableName = Constants.ObjectData;
                //Trả kết quả
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success.",
                    content = dt
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

        public HttpResult GetById(string id)
        {
            try
            {
                //Validate
                if (string.IsNullOrWhiteSpace(id))
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "user_id is empty."
                    };

                var dt = ExecuteData(@"select * from user_info where user_id = @_user_id;
                    Select u1.company_code,u1.company_name from user_info ou, user_company u1 where ou.user_id = u1.user_id And ou.user_id = @_user_id;
                    Select u2.default_company, u2.default_warehouse from user_info ou, user_config_1 u2 where ou.user_id = u2.user_id And ou.user_id = @_user_id;", 
                    CommandType.Text, new SqlParameter("@_user_id",Function.ParseInt(id)));
                //Rename Table
                dt.Tables[0].TableName = "info";
                dt.Tables[1].TableName = "company";
                dt.Tables[2].TableName = "system_default";
                //Trả kết quả
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success.",
                    content = dt
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

        public HttpResult Create(APlusObjectType objectType, APlusObjectType fromObjectType, DataSet dtvalue, int bplId, int userSign)
        {
            return Process(APlusObjectType.oUser, APlusObjectType.oUser, -1, TransactionType.A, dtvalue, userSign);
        }

        public HttpResult Update(APlusObjectType objectType, APlusObjectType fromObjectType, DataSet dtvalue,  int bplId, int userSign)
        {
            return Process(APlusObjectType.oUser, APlusObjectType.oUser, -1, TransactionType.U, dtvalue, userSign);
        }

        public HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, string code,  int bplId, int userSign)
        {
            throw new NotImplementedException();
        }

        public HttpResult Cancel(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
