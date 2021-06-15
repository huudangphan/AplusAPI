using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services
{
    public abstract class BaseMasterData : BaseService<DataTable>
    {
        /// <summary>
        /// Table name in database
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Temporary table use for Process data
        /// </summary>
        private static string TempTable => "_tmp_master_data";


        public BaseMasterData(IDatabaseClient databaseService, string tableName) : base(databaseService)
        {
            TableName = tableName;
        }


        #region Get data

        public virtual HttpResult Get(SearchDocumentModel model)
        {
            try
            {
                // case process type for geography location (because only it have to use _type)
                string type;
                switch (model.object_type)
                {
                    case APlusObjectType.oCountry:
                        type = "C";
                        break;
                    case APlusObjectType.oProvince:
                        type = "P";
                        break;
                    case APlusObjectType.oDistrict:
                        type = "D";
                        break;
                    case APlusObjectType.oWards:
                        type = "W";
                        break;
                    default:
                        type = null;
                        break;
                }

                var resultSet = ExecuteData("aplus_basemasterdata_getall", CommandType.StoredProcedure,
                    new SqlParameter("_table_name", TableName),
                    new SqlParameter("_code", model.code),
                    new SqlParameter("_name", model.name),
                    new SqlParameter("_status", model.status),
                    new SqlParameter("_page_index", model.page_index),
                    new SqlParameter("_page_size", model.page_size),
                    new SqlParameter("_order_by", model.order_by),
                    new SqlParameter("_is_ascending", model.is_ascending),
                    new SqlParameter("_from_date", model.from_date),
                    new SqlParameter("_to_date", model.to_date),
                    new SqlParameter("_type", type));

                resultSet.Tables[0].TableName = Constants.Pagination;
                resultSet.Tables[1].TableName = Constants.ObjectData;
                return new HttpResult(MessageCode.Success, resultSet);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        /// <summary>
        /// Get specific record by Code of Table.
        /// <br/>
        /// Warning: the name of column Code must is "Code"
        /// </summary>
        /// <param name="code">value for Code column</param>
        /// <returns>HttpResult</returns>
        public virtual HttpResult GetById(string code)
        {
            try
            {
                var query = $@"select * from {TableName} where code=@_code";
                return new HttpResult
                {
                    content = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@_code", code)),
                    msg_code = MessageCode.Success,
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString()
                    .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        #endregion

        #region Process Data

        public HttpResult Create(APlusObjectType objectType, APlusObjectType fromObjectType, DataTable document,
            int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.A, document, userSign);
        }

        public HttpResult Update(APlusObjectType objectType, APlusObjectType fromObjectType, DataTable document,
            int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.U, document, userSign);
        }

        public HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, string code,
            int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.D, null, userSign, code);
        }

        public HttpResult Cancel(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId,
            int userSign)
        {
            throw new NotImplementedException();
        }


        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId,
            int userSign,
            ref TransactionType transtype, DataTable document, ref string objId, ref string objNum)
        {
            try
            {
                DataTable result;
                switch (transtype)
                {
                    case TransactionType.A:
                    case TransactionType.U:

                        if (document == null || document.Rows.Count == 0)
                        {
                            return new HttpResult(MessageCode.DataNotProvide);
                        }

                        break;
                    case TransactionType.D:
                        var deleteValidate = ValidateForeignConstraint(objtype, fromObjType, bplId, userSign,
                            ref transtype, null, ref objId, ref objNum);
                        if (deleteValidate.msg_code != MessageCode.Success)
                        {
                            return deleteValidate;
                        }

                        result = ExecuteDataTable("aplus_basemasterdata_savedoc", CommandType.StoredProcedure,
                            new SqlParameter("_table_name", TableName),
                            new SqlParameter("_tran_type", Function.ToString(transtype)),
                            new SqlParameter("_code", objId));

                        return new HttpResult((MessageCode) Function.ParseInt(result.Rows[0][0]));
                    default:
                        return new HttpResult(MessageCode.FunctionNotSupport);
                }

                // mutate data phase
                document = StandardizedTable(document, TableName);

                if (transtype == TransactionType.A)
                {
                    document.Rows[0]["user_sign"] = userSign;
                }
                else
                {
                    document.Rows[0]["user_sign2"] = userSign;
                }

                //execute phase
                CreateTempTableAsDatabaseTable(TableName, TempTable);
                BulkCopy(document, TempTable);

                // validate input pre-process phase 
                var validate = ValidateForeignConstraint(objtype, fromObjType, bplId, userSign,
                    ref transtype, document, ref objId, ref objNum);
                if (validate.msg_code != MessageCode.Success)
                {
                    return validate;
                }

                result = ExecuteDataTable("aplus_basemasterdata_savedoc", CommandType.StoredProcedure,
                    new SqlParameter("_table_name", TableName),
                    new SqlParameter("_tran_type", Function.ToString(transtype)),
                    new SqlParameter("_code", DBNull.Value));
                objId = Function.ToString(result.Rows[0][1]);
                return new HttpResult((MessageCode) result.Rows[0][0],
                    Function.ToString(result.Rows[0][1]).Split(",").ToList());
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString()
                    .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }


        /// <summary>
        /// Validate the record is foreign key of other table has been existed in pre-process phase 
        /// </summary>
        /// <param name="objtype"></param>
        /// <param name="fromObjType"></param>
        /// <param name="bplId"></param>
        /// <param name="userSign"></param>
        /// <param name="transtype"></param>
        /// <param name="document"></param>
        /// <param name="objId"></param>
        /// <param name="objNum"></param>
        /// <returns>new HttpResult(MessageCode.Success)</returns>
        protected abstract HttpResult ValidateForeignConstraint(APlusObjectType objtype, APlusObjectType fromObjType,
            int bplId,
            int userSign,
            ref TransactionType transtype, DataTable document, ref string objId, ref string objNum);

        #endregion
    }
}