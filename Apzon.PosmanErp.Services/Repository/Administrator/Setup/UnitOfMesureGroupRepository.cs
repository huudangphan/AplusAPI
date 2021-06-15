
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services;
using Apzon.PosmanErp.Services.Interfaces.Administrator.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.Libraries.HDBConnection.Interfaces;

namespace Apzon.PosmanErp.Services.Repository.Administrator.Setup
{
    public class UnitOfMesureGroupRepository : BaseService<DataSet>, IUnitOfMesureGroup
    {
        private const string TempTableName = "_temp_ougp";
        private const string TempDetailsTableName = "_temp_ugp1";

        public UnitOfMesureGroupRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype,
            DataSet document, ref string objId, ref string objNum)
        {
            DataTable dtResult;
            switch (transtype)
            {
                case TransactionType.A:
                case TransactionType.U:
                    //Validate thông tin
                    var validate = ValidateUg(document);
                    if (validate.msg_code != MessageCode.Success)
                        return validate;
                    #region Tạo bảng tạm
                    var dtUgp = document.Tables["uom_group"];
                    dtUgp = StandardizedTable(dtUgp, "unit_group");
                    var dtUgp1 = document.Tables["uom_group_data"];
                    dtUgp1 = StandardizedTable(dtUgp1, "unit_group_1");
                    //Set lại user_sign
                    CreateTempTableAsDatabaseTable("unit_group", TempTableName);
                    BulkCopy(dtUgp, TempTableName);
                    CreateTempTableAsDatabaseTable("unit_group_1", TempDetailsTableName);
                    BulkCopy(dtUgp1, TempDetailsTableName);
                    #endregion
                    //Save
                    dtResult = ExecuteDataTable(@"aplus_unitofmesuregroup_save", CommandType.StoredProcedure,
                        new SqlParameter("@_trans_type", Function.ToString(transtype)),
                        new SqlParameter("@_user_sign", Function.ParseInt(userSign))
                        );
                    break;
                case TransactionType.D:
                    if (string.IsNullOrWhiteSpace(objId))
                        return new HttpResult(MessageCode.UnitOfMesureGroupIsEmpty);
                    //Check
                    dtResult = ExecuteDataTable(@"aplus_unitofmesuregroup_delete", CommandType.StoredProcedure, new SqlParameter("@_code", Function.ParseInt(objId)));
                    break;
                default:
                    return new HttpResult(MessageCode.FunctionNotSupport);
            }
            if (!dtResult.IsNotNull())
                return new HttpResult(MessageCode.UnableAccessDatabase);
            
            var msgCode = Function.ParseInt(dtResult.Rows[0][0]);
            objId = Function.ToString(dtResult.Rows[0][1]);
            return new HttpResult((MessageCode)msgCode,obj: objId);
        }

        private HttpResult ValidateUg(DataSet document)
        {
            if (document.Tables.Count != 2 || !document.IsNotNull())
                return new HttpResult(MessageCode.UnitOfMesureGroupIsEmpty);

            if (document.Tables["uom_group"].Rows.Count != 1)
                return new HttpResult(MessageCode.UnitOfMesureGroupIsEmpty);

            if (string.IsNullOrEmpty(Function.ToString(document.Tables[0].Rows[0]["ugp_name"])))
                return new HttpResult(MessageCode.UnitOfMesureGroupIsEmpty);

            return new HttpResult(MessageCode.Success);
        }

        #region Default Action
        public HttpResult Get(SearchDocumentModel Search)
        {
            try
            {
                if (Search == null)
                    Search = new SearchDocumentModel();
                var dtUg = ExecuteData(@"aplus_unitofmesuregroup_get", CommandType.StoredProcedure,
                    new SqlParameter("@_code", Function.ParseInt(Search.code)),
                    new SqlParameter("@_name",Search.name),
                    new SqlParameter("@_user_sign", Function.ParseInt(Search.user_sign)),
                    new SqlParameter("@_page_size", Function.ParseInt(Search.page_size)),
                    new SqlParameter("@_page_index", Function.ParseInt(Search.page_index)),
                    new SqlParameter("@_from_date", Search.from_date),
                    new SqlParameter("@_to_date", Search.to_date)
                );
                //Đặt lại tên bảng kết quả trả về
                dtUg.Tables[0].TableName = "pagination";
                dtUg.Tables[1].TableName = "data";
                //Trả kết quả
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success.",
                    content = dtUg
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
                var dtUg = ExecuteData(@"Select * from unit_group ug where ug.ugp_entry = @_id;
                    Select u1.*, uo.name from unit_group_1 u1, apz_ouom uo where u1.uom_code = uo.code and u1.ugp_entry = @_id",
                    CommandType.Text, new SqlParameter("@_id", Function.ParseInt(id))
                );
                dtUg.Tables[0].TableName = "group";
                dtUg.Tables[1].TableName = "group_details";
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success.",
                    content = dtUg
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

        public HttpResult Create(APlusObjectType objectType, APlusObjectType fromObjectType, DataSet document, int bplId, int userSign)
        {
            return Process(APlusObjectType.oUser, APlusObjectType.oUser, -1, TransactionType.A, document, userSign);
        }

        public HttpResult Update(APlusObjectType objectType, APlusObjectType fromObjectType, DataSet document, int bplId, int userSign)
        {
            return Process(APlusObjectType.oUser, APlusObjectType.oUser, -1, TransactionType.U, document, userSign);
        }

        public HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            return Process(APlusObjectType.oUser, APlusObjectType.oUser, -1, TransactionType.D, null, userSign, code);
        }

        public HttpResult Cancel(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
