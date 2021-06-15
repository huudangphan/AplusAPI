using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Commons;

namespace Apzon.PosmanErp.Services.Repository.Commons
{
    public class UdfRepository : BaseService<DataSet>, IUdf
    {
        public UdfRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        private const string UdfTable = "apz_oudf";
        private const string ValidValueTable = "apz_udf2";

        private const string UdfTemp = "_apz_oudf";
        private const string ValidValueTemp = "apz_udf2";

        private const string Udf = "udf";
        private const string ValidValue = "udf2";

        private string _tableName = string.Empty;

        public HttpResult Get(SearchDocumentModel model)
        {
            try
            {
                var data = ExecuteData("aplus_udf_get", CommandType.StoredProcedure,
                    new SqlParameter("_table_name", model.table_name),
                    new SqlParameter("_column_id", model.column_id),
                    new SqlParameter("_column_name", model.column_name),
                    new SqlParameter("_data_type", model.data_type),
                    new SqlParameter("_search_name", model.search_name),
                    new SqlParameter("_page_index", model.page_index),
                    new SqlParameter("_page_size", model.page_size));
                data.Tables[0].TableName = Constants.Pagination;
                data.Tables[1].TableName = Constants.ObjectData;
                return new HttpResult(MessageCode.Success, data);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }


        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId,
            int userSign,
            ref TransactionType transtype, DataSet document, ref string objId, ref string objNum)
        {
            var msgCode = 2;
            try
            {
                //validate udf table
                switch (transtype)
                {
                    case TransactionType.A:
                    case TransactionType.U:
                        var dtUdf = document.Tables[Udf];
                        if (dtUdf == null || dtUdf.Rows.Count == 0)
                        {
                            return new HttpResult(MessageCode.DataNotProvide);
                        }

                        var dtUdf2 = document.Tables[ValidValue];

                        #region Check linked type

                        // check linked data when choose linked_type 05 in udf
                        var linkedType = Function.ToString(dtUdf.Rows[0]["linked_type"]);
                        var linkedObject = Function.ToString(dtUdf.Rows[0]["linked_object"]);

                        switch (linkedType)
                        {
                            // 02 system object in AplusObjectType
                            case "01" when !Enum.IsDefined(typeof(APlusObjectType), linkedObject):
                                return new HttpResult(MessageCode.SystemObjectTypeNotExists);

                            // 04: valid lookup data apz_udf2 (use key),  05: valid list apz_udf2 (not use key)  
                            case "05" or "04" when dtUdf2 == null || dtUdf2.Rows.Count == 0:
                                return new HttpResult(MessageCode.LinkedNotProvide);
                        }

                        #endregion

                        dtUdf = StandardizedTable(document.Tables[Udf], UdfTable);
                        dtUdf2 = StandardizedTable(document.Tables[ValidValue], ValidValueTable);

                        #region Check and mutate data

                        // user id mutation
                        if (Function.ToString(dtUdf.Rows[0]["user_sign"]) == null)
                        {
                            dtUdf.Rows[0]["user_sign"] = userSign;
                        }

                        if (transtype == TransactionType.U)
                        {
                            if (Function.ToString(dtUdf.Rows[0]["user_sign2"]) == null)
                            {
                                dtUdf.Rows[0]["user_sign2"] = userSign;
                            }
                        }

                        #endregion

                        CreateTempTableAsDatabaseTable(UdfTable, UdfTemp);
                        BulkCopy(dtUdf, UdfTemp);
                        if (linkedType is "04" or "05")
                        {
                            CreateTempTableAsDatabaseTable(ValidValueTable, ValidValueTemp);
                            BulkCopy(dtUdf2, ValidValueTemp);
                        }

                        msgCode = (int) ExecuteScalar("aplus_udf_save", CommandType.StoredProcedure,
                            new SqlParameter("_tran_type", Function.ToString(transtype)));
                        break;
                    case TransactionType.D:
                        if (string.IsNullOrEmpty(objId))
                        {
                            return new HttpResult(MessageCode.ColumnIdNotProvide);
                        }

                        if (string.IsNullOrEmpty(_tableName))
                        {
                            return new HttpResult(MessageCode.TableNameNotProvide);
                        }

                        msgCode = (int) ExecuteScalar("aplus_udf_delete", CommandType.StoredProcedure,
                            new SqlParameter("_table_name", _tableName),
                            new SqlParameter("_column_id", objId));
                        break;
                }

                return new HttpResult((MessageCode) msgCode);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        public HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, int company, int userSign,
            SearchDocumentModel model)
        {
            var columnId = model.column_id;
            _tableName = model.table_name;
            return Process(objectType, fromObjectType, company, TransactionType.D, null, userSign,
                columnId);
        }
    }
}