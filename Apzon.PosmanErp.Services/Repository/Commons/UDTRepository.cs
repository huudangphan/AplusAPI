using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Administrator.Setup;
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
    public class UDTRepository : BaseService<DataTable>, IUDTs
    {
        private const string TableName = "_temp_user_table";

        public UDTRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype,
           DataTable document, ref string objId, ref string objNum)
        {
            DataTable dtResult;
            switch (transtype)
            {
                case TransactionType.A:
                case TransactionType.U:
                    //Validate input
                    if (document == null)
                        return new HttpResult(MessageCode.DataNotProvide);
                    var dtUDT = document;
                    dtUDT = StandardizedTable(dtUDT, "user_table");
                    CreateTempTableAsDatabaseTable("user_table", TableName);
                    BulkCopy(dtUDT, TableName);
                    dtResult = ExecuteDataTable(@"aplus_udt_save", CommandType.StoredProcedure,
                        new SqlParameter("@_trans_type", Function.ToString(transtype)),
                        new SqlParameter("@_user_sign", Function.ParseInt(userSign))
                        );
                    break;
                case TransactionType.D:
                    if (string.IsNullOrWhiteSpace(objId))
                        return new HttpResult(MessageCode.DataNotProvide);
                    //Check
                    dtResult = ExecuteDataTable(@"aplus_udt_delete", CommandType.StoredProcedure, new SqlParameter("@_table_name", Function.ToString(objId)));
                    break;
                default:
                    return new HttpResult(MessageCode.FunctionNotSupport);
            }
            if (!dtResult.IsNotNull())
                return new HttpResult(MessageCode.UnableAccessDatabase);
            
            var msgCode = Function.ParseInt(dtResult.Rows[0][0]);
            objId = Function.ToString(dtResult.Rows[0][1]);
            return new HttpResult((MessageCode)msgCode,objId);
        }

        #region Base Action
        public HttpResult Get(SearchDocumentModel Search)
        {
            try
            {
                if (Search == null)
                    Search = new SearchDocumentModel();
                var dtUDT = ExecuteData(@"aplus_udt_get", CommandType.StoredProcedure, 
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
                var dtUDT = ExecuteDataTable(@"Select * from user_table where table_name = @_table_name;", CommandType.Text,
                      new SqlParameter("@_table_name", Function.ToString(id)));
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

        public HttpResult Create(APlusObjectType objectType, APlusObjectType fromObjectType, DataTable document, int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.A, document, userSign);
        }

        public HttpResult Update(APlusObjectType objectType, APlusObjectType fromObjectType, DataTable document, int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.U, document, userSign);
        }

        public HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.D, null, userSign,objId: code);
        }

        public HttpResult Cancel(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            return new HttpResult(MessageCode.FunctionNotSupport, "Function doesn't support!");
        }
        #endregion
    }
}
