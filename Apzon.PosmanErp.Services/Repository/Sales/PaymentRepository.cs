using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Sales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Apzon.PosmanErp.Services.Repository.Sales
{
    internal class PaymentRepository : BaseService<DataSet>, IPayment
    {
        /// <summary>
        /// Temp document table. được set ở implement class
        /// </summary>
        protected string TempDocTable = "_temp_doc";

        /// <summary>
        /// Temp line table. được set ở implement class
        /// </summary>
        protected string TempLineTable = "_temp_lines";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseClient"></param>
        protected string TempItemTable = "_temp_item";

        public PaymentRepository(IDatabaseClient databaseClient) : base(databaseClient)
        {

        }

        public HttpResult ConfirmPaymentOrder(APlusObjectType paymentType, APlusObjectType objType, int docEntry, DataTable dtPayment, int userSign, int branch, bool subTransaction = false)
        {
            try
            {
                var lstTable = GetDocumentTableByObjectType(objType);
                if (lstTable == null || string.IsNullOrEmpty(lstTable.HeaderTable))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Không thể kiểm tra dữ liệu thanh toán từ phiếu"
                    };
                }
                var dsPayment = ExecuteData("Aplus_Payment_GetOrderPaymentData", CommandType.StoredProcedure, new SqlParameter("_obj_type", (int)objType)
                                                                                                           , new SqlParameter("_doc_entry", docEntry)
                                                                                                           , new SqlParameter("_doc_table", lstTable.HeaderTable)
                                                                                                           , new SqlParameter("_payment_type", (int)paymentType));
                var dtDoc = dsPayment.Tables[0].Copy();
                dtDoc.TableName = "apz_orct";
                dtPayment.TableName = "apz_rct1";
                var dtItem = dsPayment.Tables[1].Copy();
                dtItem.TableName = "apz_rct2";
                var dtAttachment = new DataTable("attachment");
                var dsSavePayment = new DataSet();
                dsSavePayment.Tables.AddRange(new[] { dtDoc, dtPayment, dtItem, dtAttachment });
                return subTransaction ? ProcessSubTransaction(paymentType, paymentType, branch, TransactionType.A, dsSavePayment, userSign)
                                      : Process(paymentType, paymentType, branch, TransactionType.A, dsSavePayment, userSign);
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

        public HttpResult ConfirmPaymentReturn(APlusObjectType paymentType, APlusObjectType objType, APlusObjectType baseType, int baseEntry, DataTable dtPayment, int returnEntry, int userSign, int branch, bool subTransaction = false)
        {
            try
            {
                var lstTable = GetDocumentTableByObjectType(objType);
                if (lstTable == null || string.IsNullOrEmpty(lstTable.HeaderTable))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Không thể kiểm tra dữ liệu thanh toán từ phiếu"
                    };
                }
                var resultCheckReturnPayment = ExecuteDataTable(@"select 1 from apz_orct a inner join apz_rct1 b on a.doc_entry = b.doc_entry 
                                                                                            inner join apz_rct2 c ON a.doc_entry = c.doc_entry 
                                                                        where c.base_type = _base_type and c.base_entry = _order_entry and b.pay_mth <> 'COD';", CommandType.Text
                                                                                , new SqlParameter("_base_type", (int)baseType), new SqlParameter("_order_entry", baseEntry));
                if (!resultCheckReturnPayment.IsNotNull())
                {
                    if (dtPayment.IsNotNull())
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.Error,
                            message = @"Đơn hàng chưa thanh toán không thể tạo phiếu hoàn tiền cho đơn trả hàng"
                        };
                    }
                    return new HttpResult(MessageCode.Success);
                }

                var dsPayment = ExecuteData("Aplus_Return_GetPayment", CommandType.StoredProcedure, new SqlParameter("_return_entry", returnEntry)
                                                                                                , new SqlParameter("_users_ign", userSign)
                                                                                                , new SqlParameter("_payment_type", (int)paymentType)
                                                                                                , new SqlParameter("_obj_type", (int)objType)
                                                                                                , new SqlParameter("_doc_table", lstTable.HeaderTable));
                if (dtPayment.IsNotNull())
                {
                    var totalPayment = dtPayment.AsEnumerable().Sum(t => Function.ParseDecimal(t["line_total"]));
                    var dtDoc = dsPayment.Tables[0].Copy();
                    var dtBase = dsPayment.Tables[2].Copy();
                    dtDoc.Rows[0]["doc_total"] = totalPayment;
                    dtDoc.TableName = "document";
                    dtPayment.TableName = "lines";
                    dtBase.TableName = "bases";
                    dsPayment = new DataSet();
                    dsPayment.Tables.Add(dtDoc);
                    dsPayment.Tables.Add(dtPayment);
                    dsPayment.Tables.Add(dtBase);
                }
                dsPayment.Tables.Add(new DataTable());
                return subTransaction ? ProcessSubTransaction(paymentType, paymentType, branch, TransactionType.A, dsPayment, userSign)
                                      : Process(paymentType, paymentType, branch, TransactionType.A, dsPayment, userSign);
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

        public HttpResult GetDataCashBooks(string fromDate, string toDate, APlusObjectType docType, List<string> payments, int userId, string cardCode, string branch, string isAccounting, string textSearch, int pageSize, int pageIndex)
        {
            try
            {
                var dsCashBooks = ExecuteData("Aplus_Payment_GetDataCashBooks", CommandType.StoredProcedure, new SqlParameter("_f_date", Function.ParseDateTime(fromDate, Constants.DatetimeFormat))
                                                                                                      , new SqlParameter("_t_date", Function.ParseDateTime(toDate, Constants.DatetimeFormat))
                                                                                                      , new SqlParameter("_text_search", textSearch)
                                                                                                      , new SqlParameter("_doc_type", (int)docType)
                                                                                                      , new SqlParameter("_payments", payments != null && payments.Any() ? string.Join(";", payments) : "")
                                                                                                      , new SqlParameter("_user_id", userId)
                                                                                                      , new SqlParameter("_card_code", cardCode)
                                                                                                      , new SqlParameter("_branch", branch)
                                                                                                      , new SqlParameter("_is_accounting", isAccounting)
                                                                                                      , new SqlParameter("_page_size", pageSize)
                                                                                                      , new SqlParameter("_page_index", pageIndex));
                if (dsCashBooks == null || dsCashBooks.Tables.Count != 3)
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Không tìm thấy dữ liệu phù hợp"
                    };
                }
                dsCashBooks.Tables[0].TableName = "overview";
                dsCashBooks.Tables[1].TableName = "result_info";
                dsCashBooks.Tables[2].TableName = "documents";
                return new HttpResult(MessageCode.Success, dsCashBooks);
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

        public HttpResult GetDataPaymentByEntry(int docEntry)
        {
            try
            {
                var dtDocument = ExecuteData("Aplus_Payment_GetPaymentByEntry", CommandType.StoredProcedure, new SqlParameter("_doc_entry", docEntry));
                if (dtDocument == null || dtDocument.Tables.Count != 4 || !dtDocument.Tables[0].IsNotNull())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.RecordNotFound,
                        message = "Không có bản ghi phù hợp"
                    };
                }
                dtDocument.Tables[0].TableName = "apz_orct";
                dtDocument.Tables[1].TableName = "apz_rct1";
                dtDocument.Tables[2].TableName = "apz_rct2";
                dtDocument.Tables[3].TableName = "attachments";
                return new HttpResult(MessageCode.Success, dtDocument);
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

        public HttpResult GetListPayment(string paymentType, string fromDate, string toDate, List<int> groupType, List<int> docType, string textSearch, string filter, int? pageSize, int? pageIndex)
        {
            try
            {
                var dsTransfer = ExecuteData("Aplus_Payment_GetListPayment", CommandType.StoredProcedure, new SqlParameter("_f_date", Function.ParseDateTime(fromDate, Constants.DatetimeFormat))
                                                                                                      , new SqlParameter("_t_date", Function.ParseDateTime(toDate, Constants.DatetimeFormat))
                                                                                                      , new SqlParameter("_text_search", textSearch)
                                                                                                      , new SqlParameter("_filter", filter)
                                                                                                      , new SqlParameter("_group_type", groupType != null && groupType.Any() ? string.Join(";", groupType) : "")
                                                                                                      , new SqlParameter("_doc_type", docType != null && docType.Any() ? string.Join(";", docType) : "")
                                                                                                      , new SqlParameter("_page_size", pageSize)
                                                                                                      , new SqlParameter("_page_index", pageIndex)
                                                                                                      , new SqlParameter("_obj_type", paymentType.Equals("I") ? Function.ToString((int)APlusObjectType.oIncommingPayment)
                                                                                                                                                                : paymentType.Equals("O") ? Function.ToString((int)APlusObjectType.oOutgoingPayment) : ""));
                if (dsTransfer == null || dsTransfer.Tables.Count != 2 || !dsTransfer.Tables[1].IsNotNull())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Không tìm thấy dữ liệu phù hợp"
                    };
                }
                dsTransfer.Tables[0].TableName = "result_info";
                dsTransfer.Tables[1].TableName = "documents";
                return new HttpResult(MessageCode.Success, dsTransfer);
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

        public HttpResult GetListPaymentReceiptIssueGroup()
        {
            try
            {
                var dtResult = ExecuteDataTable("select * from apz_opmr;");
                if (!dtResult.IsNotNull())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Không tìm thấy dữ liệu nhóm đối tượng"
                    };
                }
                return new HttpResult(MessageCode.Success, dtResult);
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

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype, DataSet document,
            ref string objId, ref string objNum)
        {
            DataTable dtResult;
            switch (transtype)
            {
                case TransactionType.A:
                case TransactionType.U:
                    if (document == null || document.Tables.Count != 4 || !document.Tables[0].IsNotNull() || !document.Tables[1].IsNotNull())
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.DataNotProvide,
                            message = "Dữ liệu phiếu thanh toán chưa được cung cấp"
                        };
                    }
                    var dtDoc = StandardizedTable(document.Tables[0], "apz_orct");
                    var dtLines = StandardizedTable(document.Tables[1], "apz_rct1");
                    var dtItem = StandardizedTable(document.Tables[2], "apz_rct2");
                    if (Function.ParseInt(dtDoc.Rows[0]["user_sign"]) == 0)
                    {
                        dtDoc.Rows[0]["user_sign"] = userSign;
                    }
                    var actEntry = Function.ParseInt(ExecuteScalar(string.Format(@"select atc_entry from apz_orct where doc_entry = _doc_entry;"), CommandType.Text, new SqlParameter("_doc_entry", Function.ParseInt(dtDoc.Rows[0]["doc_entry"]))));
                    actEntry = AttachmentProcess(document.Tables[2], transtype, actEntry);
                    if (actEntry == 0)
                        dtDoc.Rows[0]["atc_entry"] = DBNull.Value;
                    else
                        dtDoc.Rows[0]["atc_entry"] = actEntry;

                    CreateTempTableAsDatabaseTable("apz_orct", TempDocTable);
                    BulkCopy(dtDoc, TempDocTable);

                    CreateTempTableAsDatabaseTable("apz_rct1", TempLineTable);
                    BulkCopy(dtLines, TempLineTable);

                    CreateTempTableAsDatabaseTable("apz_rct2", TempItemTable);
                    BulkCopy(dtItem, TempItemTable);

                    objId = Function.ToString(dtDoc.Rows[0]["doc_entry"]);
                    if (transtype == TransactionType.A || transtype == TransactionType.L)
                    {
                        var doc_entry = GetDocumentNewEntry("apz_orct");
                        objId = Function.ToString(doc_entry);
                    }
                    dtResult = ExecuteDataTable("Aplus_Payment_BeforeAction", CommandType.StoredProcedure, new SqlParameter("_obj_type", (int)objtype)
                                                                                                        , new SqlParameter("_tran_type", Function.ToString(transtype))
                                                                                                        , new SqlParameter("_doc_entry", objId)
                                                                                                        , new SqlParameter("_user_sign", userSign)
                                                                                                        , new SqlParameter("_branch", bplId));
                    if (!dtResult.IsNotNull() || Function.ParseInt(dtResult.Rows[0][0]) > 0)
                    {
                        break;
                    }
                    var query = string.Format(@"delete from apz_orct where doc_entry = _doc_entry;
                                                delete from apz_rct1 where doc_entry = _doc_entry;
                                                delete from apz_rct2 where doc_entry = _doc_entry;
                                                insert into apz_orct select * from {0};
                                                insert into apz_rct1 select * from {1};
                                                insert into apz_rct2 select * from {2};", TempDocTable, TempLineTable, TempItemTable);
                    ExecuteNonQuery(query, CommandType.Text, new SqlParameter("_doc_entry", objId));
                    //if (transtype == TransactionType.A)
                    //    UpdateTimeLineStatus(objtype, Function.ParseInt(objId));
                    break;
                case TransactionType.L:
                    dtResult = ExecuteDataTable("Aplus_Payment_Cancel", CommandType.StoredProcedure, new SqlParameter("_obj_type", Function.ToString((int)objtype)), new SqlParameter("_doc_entry", Function.ParseInt(objId)));
                    break;
                default:
                    return new HttpResult(MessageCode.FunctionNotSupport, "Phương thức không được hỗ trợ");
            }
            if (!dtResult.IsNotNull())
            {
                return new HttpResult
                {
                    msg_code = MessageCode.Error,
                    message = "Thao tác không thành công. Cơ sở dữ liệu không hồi đáp"
                };
            }
            var msgCode = Function.ParseInt(dtResult.Rows[0][0]);
            if (msgCode > 0)
            {
                return new HttpResult
                {
                    msg_code = MessageCode.Error,
                    message = Function.ToString(dtResult.Rows[0][1])
                };
            }
            objId = Function.ToString(dtResult.Rows[0][1]);
            return new HttpResult(MessageCode.Success);
        }
    }
}
