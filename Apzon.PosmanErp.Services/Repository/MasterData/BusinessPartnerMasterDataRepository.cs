using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services;
using Apzon.PosmanErp.Services.Interfaces.MasterData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Services.Repository.MasterData
{
    public class BusinessPartnerMasterDataRepository : BaseService<DataSet>, IBusinessPartnerMasterData
    {
        private const string TempTableName = "_temp_partner";
        private const string TempTableAddress = "_temp_partner_address";
        private DataSet _dsLicense { get; set; }
        public BusinessPartnerMasterDataRepository(IDatabaseClient databaseService) : base(databaseService)
        { 
        }

        public HttpResult GetLiabilities(string code)
        {
            try
            {
                //Validate đầu vào
                if (string.IsNullOrWhiteSpace(code))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.DataNotProvide,
                        message = "Code must has value."
                    };
                }
                var dtBplb = ExecuteDataTable(@"aplus_businesspartnermasterdata_getliabilities", CommandType.StoredProcedure, new SqlParameter("@code", code)
                    );
                if (string.IsNullOrWhiteSpace(Function.ToString(dtBplb.Rows[0]["liab_total"])))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.RecordNotFound,
                        message = "Customer's Liabilities not found."
                    };
                }
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success",
                    content = dtBplb
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

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, 
            ref TransactionType transtype, DataSet document, ref string objId, ref string objNum)
        {
            DataTable dtResult;
            switch (transtype)
            {
                case TransactionType.A:
                case TransactionType.U:
                    //Validate thông tin
                    var validate = ValidateBP(document);
                    if (validate.msg_code != MessageCode.Success)
                    {
                        return validate;
                    }
                    #region Create Temp Table
                    var dtBp = document.Tables["info"];
                    dtBp = StandardizedTable(dtBp, "partner");
                    var actEntry = Function.ParseInt(ExecuteScalar("select atc_entry from partner where card_code = @_code", 
                        CommandType.Text,
                        new SqlParameter("@_code", Function.ToString(dtBp.Rows[0]["card_code"]))));
                    actEntry = AttachmentProcess(document.Tables[2], transtype, actEntry);
                    if (actEntry == 0)
                        dtBp.Rows[0]["atc_entry"] = DBNull.Value;
                    else
                        dtBp.Rows[0]["atc_entry"] = actEntry;

                    var dtAddress = document.Tables["address"];
                    if (dtAddress == null || dtAddress.Columns.Count == 0)
                    {
                        dtAddress = GetTableStruct("partner_address");
                    }
                    dtAddress = StandardizedTable(dtAddress, "partner_address");
                    CreateTempTableAsDatabaseTable("partner", TempTableName);
                    BulkCopy(dtBp, TempTableName);
                    CreateTempTableAsDatabaseTable("partner_address", TempTableAddress);
                    BulkCopy(dtAddress, TempTableAddress);
                    #endregion
                    //Save
                    dtResult = ExecuteDataTable(@"aplus_businesspartnermasterdata_save", CommandType.StoredProcedure, 
                        new SqlParameter("@_trans_type", Function.ToString(transtype)),
                        new SqlParameter("@_user_sign", Function.ParseInt(userSign))
                        );
                    break;
                case TransactionType.D:
                    DataTable dtActt = new DataTable();
                    AttachmentProcess(dtActt, transtype);
                    if (string.IsNullOrWhiteSpace(objId))
                        return new HttpResult(MessageCode.DataNotProvide);
                    //Check APZ_OIVL, OVPM
                    dtResult = ExecuteDataTable(@"aplus_businesspartnermasterdata_delete", CommandType.StoredProcedure, new SqlParameter("@_code", objId) );
                    break;
                default:
                    return new HttpResult(MessageCode.FunctionNotSupport);
            }
            if (!dtResult.IsNotNull())
                return new HttpResult(MessageCode.UnableAccessDatabase);

            var msgCode = Function.ParseInt(dtResult.Rows[0][0]);
            objId = Function.ToString(dtResult.Rows[0][1]);
            
            return new HttpResult
            {
                msg_code = (MessageCode)msgCode,
                content = Function.ToString(dtResult.Rows[0][1])
            };
        }

        private HttpResult ValidateBP(DataSet document)
        {
            //Check thông tin đầu vào
            if (document == null || document.Tables.Count < 3 || !document.Tables["info"].IsNotNull())
            {
                return new HttpResult
                {
                    msg_code = MessageCode.DataNotProvide,
                    message = "Data is empty."
                };
            }
            if (document.Tables["info"].Rows.Count > 1)
            {
                return new HttpResult
                {
                    msg_code = MessageCode.DataNotProvide,
                    message = "Too many record."
                };
            }
            return new HttpResult(MessageCode.Success);
        }

        #region Base Action
        public HttpResult Get(SearchDocumentModel Search)
        {
            try
            {
                if (Search == null)
                    Search = new SearchDocumentModel();
                var dtBp = ExecuteData(@"aplus_businesspartnermasterdata_get", CommandType.StoredProcedure,
                    new SqlParameter("@_code", Function.ToString(Search.code)),
                    new SqlParameter("@_name", Function.ToString(Search.name)),
                    new SqlParameter("@_type", Function.ToString(Search.type)),
                    new SqlParameter("@_branch", Function.ParseInt(Search.branch)),
                    new SqlParameter("@_user_sign", Function.ParseInt(Search.user_sign)),
                    new SqlParameter("@_page_size", Function.ParseInt(Search.page_size)),
                    new SqlParameter("@_page_index", Function.ParseInt(Search.page_index)),
                    new SqlParameter("@_from_date", Search.from_date),
                    new SqlParameter("@_to_date", Search.to_date)
                    );
                //Đặt tên table trả về
                dtBp.Tables[0].TableName = Constants.Pagination;
                dtBp.Tables[1].TableName = Constants.ObjectData;
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success",
                    content = dtBp
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
                var dtBp = ExecuteData(@"SELECT * FROM partner Where card_code = @code;
                                        SELECT * FROM partner_address Where card_code = @code", CommandType.Text, new SqlParameter("@code",id));
                //Đặt tên table trả về
                dtBp.Tables[0].TableName = "bussiness_partner";
                dtBp.Tables[1].TableName = "address";
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    message = "Success",
                    content = dtBp
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
            throw new NotImplementedException();
        }
        #endregion
    }
}
