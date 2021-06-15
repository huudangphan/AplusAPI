using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.MasterData;

namespace Apzon.PosmanErp.Services.Repository.MasterData
{
    public class ItemMasterDataRepository : BaseService<DataSet>, IItemMasterData
    {
        #region Constants

        private const string TableGrandItem = "apz_ogit";
        private const string TableInheritItem = "apz_oitm";
        private const string TableAttachment = "apz_atc1";

        private const string TempGrandItem = "_apz_ogit";
        private const string TempInheritItem = "_apz_oitm";
        private const string TempAttachment = "_apz_atc1";


        private const string GrandItem = "item";
        private const string ItemComponents = "item_components";
        private const string SqlGetAtcEntry = "select atc_entry from apz_oitm where item_code = @_item_code";

        #endregion

        public ItemMasterDataRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        #region Get operation

        public HttpResult Get(SearchDocumentModel item)
        {
            try
            {
                var result = ExecuteData("aplus_item_get", CommandType.StoredProcedure,
                    new SqlParameter("_page_index", Function.ParseInt(item.page_index)),
                    new SqlParameter("_page_size", Function.ParseInt(item.page_size)),
                    new SqlParameter("_item_code", item.code),
                    new SqlParameter("_item_name", item.name),
                    new SqlParameter("_from_date", item.from_date),
                    new SqlParameter("_to_date", item.to_date),
                    // TODO find way how to use this (filter by array)
                    new SqlParameter("_groups", item.groups != null ? string.Join(",", item.groups) : null),
                    new SqlParameter("_tags", item.tags != null ? string.Join(",", item.tags) : null),
                    new SqlParameter("_trade_marks",
                        item.trademarks != null ? string.Join(",", item.trademarks) : null),
                    new SqlParameter("_type", item.types != null ? string.Join(",", item.types) : null),
                    new SqlParameter("_status", item.status),
                    new SqlParameter("_search_name", item.search_name));

                result.Tables[0].TableName = Constants.Pagination;
                result.Tables[1].TableName = Constants.ObjectData;
                return new HttpResult(MessageCode.Success, result);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        public HttpResult GetDocumentItems(int? pageIndex, int? pageSize, string tranType, int? priceList,
            string whsCode)
        {
            try
            {
                var result = ExecuteData("aplus_item_get_documents", CommandType.StoredProcedure,
                    new SqlParameter("_page_index", pageIndex),
                    new SqlParameter("_page_size", pageSize),
                    new SqlParameter("_tran_type", tranType),
                    new SqlParameter("_price_list", priceList),
                    new SqlParameter("_whscode", whsCode));

                if (result.Tables.Count < 2)
                {
                    return new HttpResult(MessageCode.RecordNotFound, "No Data!");
                }

                result.Tables[0].TableName = Constants.Pagination;
                result.Tables[1].TableName = Constants.ObjectData;
                return new HttpResult(MessageCode.Success, result);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        public HttpResult GetById(string id)
        {
            try
            {
                var data = ExecuteData("aplus_item_getbyid", CommandType.StoredProcedure, new SqlParameter("_id", id));
                data.Tables[0].TableName = "item";
                data.Tables[1].TableName = "item_components";
                data.Tables[2].TableName = "attachment";
                return new HttpResult(MessageCode.Success, data);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.UnableAccessDatabase, ex.Message);
            }
        }


        public HttpResult GetDocumentById(string id)
        {
            try
            {
                var data = ExecuteData("aplus_item_document_getbyid",
                    CommandType.StoredProcedure,
                    new SqlParameter("_id", id));
                data.Tables[0].TableName = Constants.ObjectData;
                data.Tables[1].TableName = "price_list";
                data.Tables[2].TableName = Constants.Attachment;
                data.Tables[3].TableName = "uoms";
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
        

        #endregion

        #region Process data operation

          public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId,
            int userSign,
            ref TransactionType transtype, DataSet document, ref string objId, ref string objNum)
        {
            try
            {
                switch (objtype)
                {
                    case APlusObjectType.oItems:
                        return ItemProcess(bplId, userSign, ref transtype, document,
                            ref objId);
                    case APlusObjectType.oItemComponent:
                        return ItemComponentProcess(bplId, userSign, ref transtype, document,
                            ref objId);
                    default: return new HttpResult(MessageCode.FunctionNotSupport, "Function not supported!");
                }
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
        /// Process with only inherit item
        /// </summary>
        /// <returns></returns>
        /// 
        private HttpResult ItemProcess(int bplId,
            int userSign, ref TransactionType tranType, DataSet document,
            ref string objId)
        {
            DataTable result;
            switch (tranType)
            {
                case TransactionType.A:
                case TransactionType.U:

                    var dtItem = document.Tables["item"];
                    var dtAttachment = document.Tables[Constants.Attachment];
                    if (dtItem == null || dtItem.Rows.Count == 0)
                    {
                        return new HttpResult(MessageCode.DataNotProvide);
                    }

                    var oldItemCode = Function.ToString(dtItem.Rows[0]["item_code"]);
                    if (tranType == TransactionType.U)
                    {
                        oldItemCode = Function.ToString(dtItem.Rows[0]["o_item_code"]);
                        var itemCode = Function.ToString(dtItem.Rows[0]["item_code"]);
                        if (string.IsNullOrEmpty(oldItemCode))
                        {
                            return new HttpResult(MessageCode.CodeNotProvided);
                        }

                        if (!itemCode.Equals(oldItemCode))
                        {
                            var itemProcessResult = ProcessSubTransaction(APlusObjectType.oItems,
                                APlusObjectType.oItems, bplId,
                                TransactionType.D, null, userSign, oldItemCode);
                            if (itemProcessResult.msg_code != MessageCode.Success)
                            {
                                return itemProcessResult;
                            }

                            tranType = TransactionType.A;
                        }
                    }

                    #region Attachment process

                    var atcEntry = Function.ParseInt(ExecuteScalar(SqlGetAtcEntry, CommandType.Text,
                        new SqlParameter("_item_code", oldItemCode)));
                    dtAttachment = StandardizedTable(dtAttachment, TableAttachment);
                    CreateTempTableAsDatabaseTable(TableAttachment, TempAttachment);
                    BulkCopy(dtAttachment, TempAttachment);
                    atcEntry = AttachmentProcess(dtAttachment, tranType, atcEntry);

                    #endregion

                    dtItem = StandardizedTable(dtItem, TableInheritItem);
                    dtItem.Rows[0]["atc_entry"] = atcEntry;
                    CreateTempTableAsDatabaseTable(TableInheritItem, TempInheritItem);
                    BulkCopy(dtItem, TempInheritItem);
                    result = ExecuteDataTable("aplus_item_document_save", CommandType.StoredProcedure,
                        new SqlParameter("_tran_type", Function.ToString(tranType)),
                        new SqlParameter("_user_id", userSign));
                    break;
                case TransactionType.D:
                    if (string.IsNullOrEmpty(objId))
                    {
                        return new HttpResult(MessageCode.DataNotProvide);
                    }

                    var itemCodeParam = new SqlParameter("_item_code", objId);
                    AttachmentProcess(null, tranType, Function.ParseInt(ExecuteScalar(
                        SqlGetAtcEntry, CommandType.Text, itemCodeParam)));
                    result = ExecuteDataTable("aplus_item_document_delete", CommandType.StoredProcedure,
                        itemCodeParam);
                    break;
                default: return new HttpResult(MessageCode.FunctionNotSupport);
            }

            var messageCode = (MessageCode) Function.ParseInt(result.Rows[0][0]);
            objId = Function.ToString(result.Rows[0][1]);
            return new HttpResult(messageCode);
        }

        /// <summary>
        /// Process with grand item and inherit item document
        /// </summary>
        /// <returns></returns>
        private HttpResult ItemComponentProcess(int bplId, int userSign, ref TransactionType tranType, DataSet document,
            ref string objId)
        {
            DataTable result;
            int msgCode;
            switch (tranType)
            {
                case TransactionType.A:
                case TransactionType.U:

                    var dtGItem = document.Tables[GrandItem];
                    var dtItemDocument = document.Tables[ItemComponents];
                    var dtAttachment = document.Tables[Constants.Attachment];
                    if (!dtGItem.IsNotNull() || !dtItemDocument.IsNotNull())
                    {
                        return new HttpResult(MessageCode.DataNotProvide);
                    }

                    if (tranType == TransactionType.U &&
                        string.IsNullOrEmpty(Function.ToString(dtGItem?.Rows[0]["f_itm_code"])))
                    {
                        return new HttpResult(MessageCode.CodeNotProvided);
                    }

                    #region process grand item

                    dtGItem = StandardizedTable(dtGItem, TableGrandItem);
                    CreateTempTableAsDatabaseTable(TableGrandItem, TempGrandItem);
                    BulkCopy(dtGItem, TempGrandItem);

                    result = ExecuteDataTable("aplus_item_component_save", CommandType.StoredProcedure,
                        new SqlParameter("_user_id", userSign),
                        new SqlParameter("_tran_type", Function.ToString(tranType)));

                    msgCode = (int) result.Rows[0][0];
                    if (msgCode != 200) break;

                    #endregion

                    //Process inherit item
                    var itemCode = result.Rows[0][1];
                    for (var itemCount = 0; itemCount < dtItemDocument?.Rows.Count; itemCount++)
                    {
                        // inherit item define
                        var dsItemDocument = new DataSet();
                        var dtDocument = dtItemDocument.Clone();
                        dtDocument.TableName = "item";


                        dtDocument.ImportRow(dtItemDocument.Rows[itemCount]);
                        //synchronize item components with grand item (make consistence)
                        dtDocument.Rows[0]["f_itm_code"] = itemCode;
                        dtDocument.Rows[0]["tramrk"] = dtGItem.Rows[0]["tramrk"];
                        dtDocument.Rows[0]["itms_grp_cod"] = dtGItem.Rows[0]["itms_grp_cod"];
                        dtDocument.Rows[0]["item_type"] = dtGItem.Rows[0]["item_type"];
                        dtDocument.AcceptChanges();
                        dsItemDocument.Tables.Add(dtDocument);

                        var lineId = Function.ParseInt(dtItemDocument.Rows[itemCount]["line_id"]);
                        // Attachment table                        
                        if (dtAttachment != null && dtAttachment.Rows.Count > 0)
                        {
                            var attachments = dtAttachment.AsEnumerable()
                                .Where(t => Function.ParseInt(t["line_id"]) == lineId);
                            var dtAtc = attachments.Any() ? attachments.CopyToDataTable() : new DataTable();
                            dtAtc.TableName = Constants.Attachment;
                            dsItemDocument.Tables.Add(dtAtc);
                        }

                        // process item components in the same transaction
                        var docResult = ProcessSubTransaction(APlusObjectType.oItems, APlusObjectType.oItems,
                            bplId,
                            tranType, dsItemDocument, userSign);

                        if (docResult.msg_code != MessageCode.Success)
                        {
                            return docResult;
                        }
                    }

                    break;
                case TransactionType.D:
                    result = ExecuteDataTable("aplus_item_component_delete", CommandType.StoredProcedure,
                        new SqlParameter("_item_code", objId));
                    msgCode = Function.ParseInt(result.Rows[0][0]);
                    break;
                default:
                    return new HttpResult(MessageCode.FunctionNotSupport, "Function is not supported!");
            }

            objId = Function.ToString(result.Rows[0][1]);
            return new HttpResult((MessageCode) msgCode);
        }

        #endregion
    }
}