using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Services.Repository.Commons
{
    public class UDORepository : BaseService<DataSet>, IUDOs 
    {
        private const string TableName = "_temp_user_object";
        private const string TableName1 = "_temp_user_object_child";
        private const string TableName2 = "_temp_user_object_search";
        private const string TableName3 = "_temp_user_object_default";
        private const string TableName4 = "_temp_user_object_child_default";

        public UDORepository(IDatabaseClient databaseService) : base(databaseService)
        { }

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype,
           DataSet document, ref string objId, ref string objNum)
        {
            DataTable dtResult;
            switch (transtype)
            {
                case TransactionType.A:
                case TransactionType.U:
                    //Validate input
                    if (document == null || document.Tables["udo_info"] == null || document.Tables["udo_info"].Columns.Count == 0)
                        return new HttpResult(MessageCode.DataNotProvide);
                    if ( document.Tables["udo_info"].Rows.Count != 1)
                        return new HttpResult(MessageCode.DataNotProvide);
                    #region Create temp table
                    var dtUDO = document.Tables["udo_info"];
                    var dtchild = document.Tables["udo_child"];
                    var dtfind = document.Tables["default_find"];
                    var dthdv = document.Tables["header_view"];
                    var dtchv= document.Tables["child_view"];
                    //oudo
                    dtUDO = StandardizedTable(dtUDO, "user_object");
                    CreateTempTableAsDatabaseTable("user_object", TableName);
                    BulkCopy(dtUDO, TableName);
                    //udo1
                    if (dtchild == null)
                        dtchild = GetTableStruct("user_object_child");
                    dtchild = StandardizedTable(dtchild, "user_object_child");
                    CreateTempTableAsDatabaseTable("user_object_child", TableName1);
                    BulkCopy(dtchild, TableName1);
                    //udo2
                    if (dtfind == null)
                        dtfind = GetTableStruct("user_object_search");
                    dtfind = StandardizedTable(dtfind, "user_object_search");
                    CreateTempTableAsDatabaseTable("user_object_search", TableName2);
                    BulkCopy(dtfind, TableName2);
                    //udo3
                    if (dthdv == null)
                        dthdv = GetTableStruct("user_object_default");
                    dthdv = StandardizedTable(dthdv, "user_object_default");
                    CreateTempTableAsDatabaseTable("user_object_default", TableName3);
                    BulkCopy(dthdv, TableName3);
                    //udo4
                    if (dtchv == null)
                        dtchv = GetTableStruct("user_object_child_default");
                    dtchv = StandardizedTable(dtchv, "user_object_child_default");
                    CreateTempTableAsDatabaseTable("user_object_child_default", TableName4);
                    BulkCopy(dtchv, TableName4);
                    #endregion
                    dtResult = ExecuteDataTable(@"aplus_udo_save", CommandType.StoredProcedure,
                        new SqlParameter("@_trans_type", Function.ToString(transtype)),
                        new SqlParameter("@_user_sign", Function.ParseInt(userSign))
                        );
                    break;
                case TransactionType.D:
                    if (string.IsNullOrWhiteSpace(objId))
                        return new HttpResult(MessageCode.DataNotProvide);
                    dtResult = ExecuteDataTable(@"aplus_udo_delete", CommandType.StoredProcedure, new SqlParameter("@_code", Function.ToString(objId)));
                    break;
                default:
                    return new HttpResult(MessageCode.FunctionNotSupport);
            }
            if (!dtResult.IsNotNull())
            {
                return new HttpResult
                {
                    msg_code = MessageCode.UnableAccessDatabase,
                    message = "Database didn't responsed."
                };
            }
            var msgCode = Function.ParseInt(dtResult.Rows[0][0]);
            objId = Function.ToString(dtResult.Rows[0][1]);
            return new HttpResult((MessageCode)msgCode, objId);
        }

        public HttpResult GetUdoTable(int type)
        {
            try
            {
                //Validate Input
                if (type == null)
                    return new HttpResult(MessageCode.DataNotProvide);
                if (type < 1 || type > 4)
                    return new HttpResult(MessageCode.DataNotCorrect);
                //Danh sách bảng header chưa sử dụng
                string sql = @"Select x.*
                               From user_table x left join user_object y on x.table_name = y.header_table 
                               Where y.code is null 
                                and x.table_type = @_type
                               Order By x.create_date,x.create_time desc;";
                var dtUdoTable = ExecuteDataTable(sql, CommandType.Text,
                      new SqlParameter("@_type", Function.ParseInt(type))
                      );
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success.",
                    content = dtUdoTable
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Exception, ex.Message);
            }
        }

        #region Base Action
        public HttpResult Get(SearchDocumentModel Search)
        {
            try
            {
                if (Search == null)
                    Search = new SearchDocumentModel();
                var dtUDT = ExecuteData(@"aplus_udo_get", CommandType.StoredProcedure,
                    new SqlParameter("@_code", Function.ToString(Search.code)),
                    new SqlParameter("@_name", Function.ToString(Search.name)),
                    new SqlParameter("@_user_sign", Function.ParseInt(Search.user_sign)),
                    new SqlParameter("@_page_index", Function.ParseInt(Search.page_index)),
                    new SqlParameter("@_page_size", Function.ParseInt(Search.page_size)),
                    new SqlParameter("@_from_date", Search.from_date),
                    new SqlParameter("@_to_date", Search.to_date));
                //Rename
                dtUDT.Tables[0].TableName = Constants.Pagination;
                dtUDT.Tables[1].TableName = Constants.ObjectData;
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success.",
                    content = dtUDT
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Exception, ex.Message);
            }
        }

        public HttpResult GetById(string id)
        {
            try
            {
                string sql = @"Select * from user_object x where x.code = @_code;
                               Select * from user_object_child x where x.code = @_code;
                               Select * from user_object_search x where x.code = @_code;
                               Select * from user_object_default x where x.code = @_code;
                               Select * from user_object_child_default x where x.code = @_code;";
                var dtUDO = ExecuteData(sql, CommandType.Text,
                      new SqlParameter("@_code", Function.ToString(id)));
                dtUDO.Tables[0].TableName = "udo_info";
                dtUDO.Tables[1].TableName = "udo_child";
                dtUDO.Tables[2].TableName = "default_find";
                dtUDO.Tables[3].TableName = "header_view";
                dtUDO.Tables[4].TableName = "child_view";
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success.",
                    content = dtUDO
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Exception, ex.Message);
            }
        }

        public HttpResult Create(APlusObjectType objectType, APlusObjectType fromObjectType, DataSet document, int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.A, document, userSign);
        }

        public HttpResult Update(APlusObjectType objectType, APlusObjectType fromObjectType, DataSet document, int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.U, document, userSign);
        }

        public HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.D, null, userSign, objId: code);
        }

        public HttpResult Cancel(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            return new HttpResult(MessageCode.FunctionNotSupport, "Function doesn't support!");
        }
        #endregion
    }
}
