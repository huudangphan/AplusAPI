using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Administrator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Services.Repository.Administrator
{
    internal class AuthRepository : BaseService<DataTable>, IAuth
    {
        public AuthRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        public HttpResult Get(int? user_id)
        {
            try
            {
                var dtAuth = ExecuteDataTable(@"select a.*, b.auth, @_user_id user_id from menu a left join auth b on a.menu_code = b.menu_code and b.user_id = @_user_id where a.status = 'A';"
                                                    , CommandType.Text, new System.Data.SqlClient.SqlParameter("_user_id", user_id));
                if(!dtAuth.IsNotNull())
                {
                    return new HttpResult(MessageCode.RecordNotFound, "Dữ liệu danh sách chức năng chưa được khởi tạo");
                }
                return new HttpResult(MessageCode.Success, dtAuth);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype, DataTable document, ref string objId, ref string objNum)
        {
            if(!document.IsNotNull())
            {
                return new HttpResult(MessageCode.DataNotProvide, "Dữ liệu chưa được cung cấp");
            }
            //khai báo dữ liệu table vật lý và temp table
            var tableName = "auth";
            var tempTableName = "_auth";
            var storeName = "aplus_auth_save";
            if(transtype == TransactionType.M)
            {
                tableName = "menu";
                tempTableName = "_menu";
                storeName = "aplus_auth_menusave";
            }
            else
            {
                objId = Function.ToString(document.Rows[0]["user_id"]);
            }
            //chuẩn hóa dữ liệu theo table vật lý
            document = StandardizedTable(document, tableName);
            //khởi tạo dữ liệu cho table tạm
            CreateTempTableAsDatabaseTable(tableName, tempTableName);
            BulkCopy(document, tempTableName);
            var saveResult = ExecuteDataTable(storeName, CommandType.StoredProcedure, new System.Data.SqlClient.SqlParameter("_user_sign", userSign));
            if(!saveResult.IsNotNull())
            {
                return new HttpResult(MessageCode.UnableAccessDatabase, "không thể kết nối cơ sở dữ liệu");
            }
            return new HttpResult((MessageCode)Function.ParseInt(saveResult.Rows[0][0]), Function.ToString(saveResult.Rows[0][1]));
        }
    }
}
