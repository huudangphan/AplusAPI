using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Administrator.Setup;

namespace Apzon.PosmanErp.Services.Repository.Administrator.Setup
{
    public class PriceListRepository : BaseService<DataSet>, IPriceList
    {
        // Table name for sql   
        private const string TablePriceList = "apz_opln";
        private const string TableItemPrice = "apz_itm1";
        private const string TempPriceList = "_apz_opln";
        private const string TempItemPrice = "_apz_itm1";

        // Table name for data table
        private const string DtPriceList = "price_list";
        private const string DtItemPrice = "item_prices";

        public PriceListRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        public HttpResult Get(SearchDocumentModel model)
        {
            try
            {
                var data = ExecuteData("aplus_pricelist_get", CommandType.StoredProcedure,
                    new SqlParameter("_doc_entry", model.doc_entry), new SqlParameter("_list_name", model.name),
                    new SqlParameter("_from_date", model.from_date), new SqlParameter("_to_date", model.to_date),
                    new SqlParameter("_search_name", model.search_name));
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

        public HttpResult GetByEntry(int id, int userId)
        {
            try
            {
                var data = ExecuteData("aplus_pricelist_getbyid", CommandType.StoredProcedure,
                    new SqlParameter("_doc_entry", id), new SqlParameter("_user_id", userId));
                data.Tables[0].TableName = DtPriceList;
                data.Tables[1].TableName = DtItemPrice;
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
            try
            {
                DataTable result = new DataTable();
                switch (transtype)
                {
                    case TransactionType.A:
                    case TransactionType.U:
                        if (document.Tables.Count < 2)
                        {
                            return new HttpResult(MessageCode.DataNotCorrect, "Data input not correct!");
                        }

                        var dtPriceList = document.Tables[DtPriceList];
                        var dtItemPrices = document.Tables[DtItemPrice];

                        if (!dtPriceList.IsNotNull() || dtPriceList.Rows.Count == 0)
                        {
                            return new HttpResult(MessageCode.DataNotProvide, "Price list not provided!");
                        }

                        if (transtype == TransactionType.A &&
                            (!dtItemPrices.IsNotNull() || dtItemPrices.Rows.Count == 0))
                        {
                            return new HttpResult(MessageCode.DataNotProvide, "Item price not provided!");
                        }

                        dtPriceList = StandardizedTable(dtPriceList, TablePriceList);
                        dtItemPrices = StandardizedTable(dtItemPrices, TableItemPrice);

                        switch (transtype)
                        {
                            case TransactionType.A:
                                dtPriceList.Rows[0]["user_sign"] = userSign;
                                break;
                            case TransactionType.U:
                                dtPriceList.Rows[0]["user_sign2"] = userSign;
                                break;
                        }


                        CreateTempTableAsDatabaseTable(TablePriceList, TempPriceList);
                        BulkCopy(dtPriceList, TempPriceList);

                        CreateTempTableAsDatabaseTable(TableItemPrice, TempItemPrice);
                        BulkCopy(dtItemPrices, TempItemPrice);

                        result = ExecuteDataTable("aplus_pricelist_save", CommandType.StoredProcedure,
                            new SqlParameter("_tran_type", Function.ToString(transtype)));
                        break;
                    case TransactionType.D:
                        if (string.IsNullOrEmpty(objId))
                        {
                            return new HttpResult(MessageCode.DataNotProvide, "doc_entry not provided!");
                        }

                        result = ExecuteDataTable("aplus_pricelist_delete", CommandType.StoredProcedure,
                            new SqlParameter("_doc_entry", Function.ParseInt(objId)));
                        break;
                }

                var msgCode = (MessageCode) result.Rows[0][0];
                return new HttpResult(msgCode, Function.ToString(result.Rows[0][1]));
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }
    }
}