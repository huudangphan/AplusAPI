using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Inventory;

namespace Apzon.PosmanErp.Services.Repository.Inventory
{
    public class WarehouseRepository : BaseService<DataTable>, IWarehouse
    {
        public WarehouseRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        private const string TableWarehouse = "warehouse";
        private const string TempWarehouse = "_warehouse";

        public HttpResult Get(SearchDocumentModel model)
        {
            try
            {
                var data = ExecuteData("aplus_warehouse_get", CommandType.StoredProcedure,
                    new SqlParameter("_page_index", model.page_index), new SqlParameter("_page_size", model.page_size),
                    new SqlParameter("_whs_code", model.code), new SqlParameter("_whs_name", model.name),
                    new SqlParameter("_status", model.status),
                    new SqlParameter("_cpn_codes", string.Join(",", model.cpn_codes)),
                    new SqlParameter("_text_search", model.search_name),
                    new SqlParameter("_from_date", model.from_date), new SqlParameter("_to_date", model.to_date));

                data.Tables[0].TableName = Constants.Pagination;
                data.Tables[1].TableName = Constants.ObjectData;
                return new HttpResult(MessageCode.Success, data);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.UnableAccessDatabase);
            }
        }


        public HttpResult GetById(string id)
        {
            try
            {
                var data = ExecuteDataTable("select * from " + TableWarehouse + " where whs_code = @_whs_code",
                    CommandType.Text,
                    new SqlParameter("_whs_code", id));
                return new HttpResult(data.Rows.Count == 0 ? MessageCode.DataNotFound : MessageCode.Success, data);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.UnableAccessDatabase);
            }
        }


        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId,
            int userSign,
            ref TransactionType transtype, DataTable document, ref string objId, ref string objNum)
        {
            try
            {
                var result = 2;
                switch (transtype)
                {
                    case TransactionType.A:
                    case TransactionType.U:
                        if (document.Rows.Count == 0)
                            return new HttpResult(MessageCode.DataNotProvide);
                        document = StandardizedTable(document, TableWarehouse);

                        #region update user sign

                        if (document.Columns.Contains("user_sign"))
                            document.Rows[0]["user_sign"] = userSign;

                        #endregion

                        CreateTempTableAsDatabaseTable(TableWarehouse, TempWarehouse);
                        BulkCopy(document, TempWarehouse);

                        result = (int) ExecuteScalar("aplus_warehouse_save", CommandType.StoredProcedure,
                            new SqlParameter("_tran_type", Function.ToString(transtype)));
                        break;
                    case TransactionType.D:
                        if (string.IsNullOrEmpty(objId))
                        {
                            return new HttpResult(MessageCode.DataNotProvide);
                        }

                        result = (int) ExecuteScalar("aplus_warehouse_del", CommandType.StoredProcedure,
                            new SqlParameter("_whs_code", objId));
                        break;
                }

                objId = Function.ToString(document.Rows[0]["whs_code"]);
                return new HttpResult((MessageCode) result);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.UnableAccessDatabase);
            }
        }
    }
}