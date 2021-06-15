using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Documents;

namespace Apzon.PosmanErp.Services.Repository.Documents
{
    internal class DocumentsRepository : BaseService<DataSet>, IDocuments
    {
        /// <summary>
        /// header table. Được set ở các implement class
        /// </summary>
        protected string DocTable { get; set; }
        /// <summary>
        /// Line table. Được set ở các implement class
        /// </summary>
        protected string LineTable { get; set; }

        /// <summary>
        /// Temp document table. được set ở implement class
        /// </summary>
        protected string TempDocTable = "_temp_doc";

        /// <summary>
        /// Temp line table. được set ở implement class
        /// </summary>
        protected string TempLineTable = "_temp_lines";

        /// <summary>
        /// table tạm chứa dữ liệu của header trong database phục vụ trường hợp update
        /// </summary>
        protected string tempCurrentDoc = "_temp_current_doc";

        /// <summary>
        /// table tạm chứa dữ liệu của lines trong database phục vụ trường hợp update
        /// </summary>
        protected string tempCurrentLines = "_temp_current_lines";

        /// <summary>
        /// têm bảng tạm chứa dữ liệu base data
        /// </summary>
        protected string tempBaseDataTable = "_temp_base_data";

        protected List<APlusObjectType> LstCancelNewDoc = new List<APlusObjectType> { APlusObjectType.oDeliveryNotes, APlusObjectType.oPurchaseReturns, APlusObjectType.oReturns
                            , APlusObjectType.oInventoryGenEntry, APlusObjectType.oInventoryGenExit, APlusObjectType.oInventoryTransfer, APlusObjectType.oInvoices, APlusObjectType.oPurchaseDeliveryNotes
                            , APlusObjectType.oPurchaseInvoices };

        /// <summary>
        /// object Type của object đang xử lí
        /// </summary>
        protected APlusObjectType ObjectTypes { get; set; }

        public DocumentsRepository(IDatabaseClient databaseService) : base(databaseService)
        {

        }

        public HttpResult Get(SearchDocumentModel doc)
        {
            try
            {
                if(doc == null)
                {
                    return new HttpResult(MessageCode.DataNotProvide, "Dữ liệu tìm kiếm chưa được cung cấp");
                }
                var objTable = GetDocumentTableByObjectType(doc.object_type);
                if(objTable == null || string.IsNullOrEmpty(objTable.HeaderTable) || objTable.LineTables == null || objTable.LineTables.Count < 1 || string.IsNullOrEmpty(objTable.LineTables[0]))
                {
                    return new HttpResult(MessageCode.DataNotFound, "không thể lấy cấu trúc của object");
                }
                DocTable = objTable.HeaderTable;
                LineTable = objTable.LineTables[0];
                var dsInvoice = ExecuteData("aplus_document_get", CommandType.StoredProcedure, new SqlParameter("_doc_table", DocTable)
                                                                                                         , new SqlParameter("_line_table", LineTable)
                                                                                                         , new SqlParameter("_f_date", Function.ParseDateTime(doc.from_date, Constants.DatetimeFormat))
                                                                                                         , new SqlParameter("_t_date", Function.ParseDateTime(doc.to_date, Constants.DatetimeFormat))
                                                                                                         , new SqlParameter("_company", doc.company)
                                                                                                         , new SqlParameter("_status", doc.status)
                                                                                                         , new SqlParameter("_search_text", doc.search_name)
                                                                                                         , new SqlParameter("_page_size", doc.page_size)
                                                                                                         , new SqlParameter("_page_index", doc.page_index));
                if (dsInvoice == null || dsInvoice.Tables.Count != 3 || !dsInvoice.Tables[1].IsNotNull())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.RecordNotFound,
                        message = "Không có dữ liệu phù hợp"
                    };
                }
                dsInvoice.Tables[0].TableName = Constants.Pagination;
                dsInvoice.Tables[1].TableName = Constants.ObjectData;
                return new HttpResult(MessageCode.Success, dsInvoice);
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

        public HttpResult GetDocumentLineByEntry(APlusObjectType objType, int docEntry)
        {
            try
            {
                var objTable = GetDocumentTableByObjectType(objType);
                if (objTable == null || string.IsNullOrEmpty(objTable.HeaderTable) || objTable.LineTables == null || objTable.LineTables.Count < 1 || string.IsNullOrEmpty(objTable.LineTables[0]))
                {
                    return new HttpResult(MessageCode.DataNotFound, "không thể lấy cấu trúc của object");
                }
                DocTable = objTable.HeaderTable;
                LineTable = objTable.LineTables[0];

                var query = string.Format(@"select * from {0} where doc_entry = @docEntry;", LineTable);
                var dtDocumentLines = ExecuteDataTable(query, parameters: new SqlParameter("@docEntry", docEntry));
                if (!dtDocumentLines.IsNotNull())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.RecordNotFound,
                        message = "Không có bản ghi phù hợp"
                    };
                }
                dtDocumentLines.TableName = "document_lines";
                return new HttpResult(MessageCode.Success, dtDocumentLines);
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

        public HttpResult GetById(string docEntry)
        {
            try
            {

                var query = string.Format(@"select * from {0} where doc_entry = @docEntry;
                                            create local temp table ""#tempLines"" as select * from {1} where doc_entry = @docEntry;
                                            select a.*, (select top 1 uri from apz_atc1 where abs_entry = b.atc_entry) uri, 
			(select top 1 dwl_uri from apz_atc1 where abs_entry = b.atc_entry) dwl_uri from ""#tempLines"" a inner join apz_oitm b ON a.Item_Code = b.Item_Code
                                            select * from apz_atc1 where abs_entry = coalesce((select top 1 atc_entry from {0} where doc_entry = @docEntry),0);", DocTable, LineTable);
                var dtDocument = ExecuteData(query, parameters: new SqlParameter("@docEntry", docEntry));
                if (dtDocument == null || dtDocument.Tables.Count != 3 || !dtDocument.Tables[0].IsNotNull())
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.RecordNotFound,
                        message = "Không có bản ghi phù hợp"
                    };
                }
                dtDocument.Tables[0].TableName = DocTable;
                dtDocument.Tables[1].TableName = LineTable;
                dtDocument.Tables[2].TableName = "Attachments";
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

        public HttpResult Create(APlusObjectType objectType, APlusObjectType fromObjectType, DataSet document, int bplId,
            int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.A, document, userSign);
        }

        public HttpResult Update(APlusObjectType objectType, APlusObjectType fromObjectType, DataSet document, int bplId,
            int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.U, document, userSign);
        }

        public HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            throw new NotImplementedException();
        }

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype, DataSet document,
            ref string objId, ref string objNum)
        {
            ////// nếu là lưu draft thì xử lí ở draft process
            if (objtype == APlusObjectType.oDraft)
            {
                return DocumentAsDraftProcess(objtype, bplId, userSign, ref transtype, document, ref objId, ref objNum);
            }
            //lấy dữ liệu table của object theo object type
            var objTable = GetDocumentTableByObjectType(objtype);
            if (objTable == null || string.IsNullOrEmpty(objTable.HeaderTable) || objTable.LineTables == null || objTable.LineTables.Count < 1 || string.IsNullOrEmpty(objTable.LineTables[0]))
            {
                return new HttpResult(MessageCode.DataNotFound, "không thể lấy cấu trúc của object");
            }
            DocTable = objTable.HeaderTable;
            LineTable = objTable.LineTables[0];

            var lstParamater = new List<SqlParameter>();
            var baseEntry = -1;
            var baseObjectType = -1;
            ///biến chứa dữ liệu action chính xác. sử dụng để update trạng thái phiếu
            var currentTran_type = transtype;
            if(transtype == TransactionType.C && !LstCancelNewDoc.Contains(objtype))
            {
                transtype = TransactionType.L;
            }
            #region lấy dữ liệu phiếu cancel và close
            if (transtype == TransactionType.C && document == null || transtype == TransactionType.L)
            {
                lstParamater.Add(new SqlParameter("@_close_doc_entry", Function.ParseInt(objId)));
                var sqlQueryGetCancelData = $@"select * from {DocTable} where doc_entry = @_close_doc_entry;
                                                    select * from {LineTable} where doc_entry = @_close_doc_entry;";
                document = ExecuteData(sqlQueryGetCancelData, CommandType.Text, lstParamater.ToArray());
                if (document == null || document.Tables.Count != 2 || !document.Tables[0].IsNotNull() || !document.Tables[1].IsNotNull())
                {
                    return new HttpResult(MessageCode.DocumentDoesNotExit, "Không tìm thấy dữ liệu phiếu. Không thể hủy");
                }
                document.Tables[0].TableName = "document";
                document.Tables[1].TableName = "document_lines";
                if (Function.ToString(document.Tables[0].Rows[0]["canceled"]).Equals("Y") || !Function.ToString(document.Tables[0].Rows[0]["doc_status"]).Equals("O"))
                {
                    return new HttpResult(currentTran_type == TransactionType.L ? MessageCode.DocumentClosedCannotClose : MessageCode.DocumentClosedCannotCancel, "Không thể hủy. Phiếu đã đóng hoặc đã bị hủy");
                }
            }

            ///kiểm tra và update lại dữ liệu của phiếu cancel
            if(currentTran_type == TransactionType.C)
            {
                ///kiểm tra số lượng. Nếu dữ liệu đã được copy qua phiếu khác thì không thể hủy
                for (var i = 0; i < document.Tables[1].Rows.Count; i++)
                {
                    var row = document.Tables[1].Rows[i];
                    if (Function.ParseInt(row["open_qty"]) > 0 && Function.ParseInt(row["open_qty"]) < Function.ParseInt(row["quantity"]))
                    {
                        return new HttpResult(MessageCode.RecordInUseInOtherTable, "Không thể hủy. Hủy phiếu đích trước");
                    }
                }
            }

            if(transtype == TransactionType.C)
            {
                /////////// update lại dữ liệu của phiếu cancel
                document.Tables[0].Rows[0]["canceled"] = "C";
                document.Tables[0].Rows[0]["doc_status"] = "C";
                document.Tables[0].Rows[0]["atc_entry"] = DBNull.Value;
                document.Tables[0].Rows[0]["doc_entry"] = DBNull.Value;
                document.Tables[0].Rows[0]["doc_date"] = DateTime.Now.Date;
                for (var i = 0; i < document.Tables[1].Rows.Count; i++)
                {
                    baseObjectType = Function.ParseInt(document.Tables[1].Rows[i]["base_type"]);
                    baseEntry = Function.ParseInt(document.Tables[1].Rows[i]["base_entry"]);
                    document.Tables[1].Rows[i]["line_status"] = "C";
                    document.Tables[1].Rows[i]["base_type"] = document.Tables[0].Rows[0]["obj_type"];
                    document.Tables[1].Rows[i]["base_entry"] = document.Tables[1].Rows[i]["doc_entry"];
                    document.Tables[1].Rows[i]["base_line"] = document.Tables[1].Rows[i]["line_num"];
                }

                document.Tables.Add(new DataTable());
                document.Tables[2].TableName = "Attachments";
            }
            #endregion

            DataTable dtResult;
            switch (transtype)
            {
                case TransactionType.A:
                case TransactionType.U:
                case TransactionType.C:
                    ///validate số lượng table truyền lên
                    if (document == null || document.Tables.Count < 3 || !document.Tables[0].IsNotNull())
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.DataNotProvide,
                            message = "Dữ liệu không đủ. không thể lưu"
                        };
                    }
                    var dtDoc = document.Tables["document"];
                    var dtLines = document.Tables["document_lines"];
                    dtDoc = StandardizedTable(dtDoc, DocTable);

                    //lấy docentry của document xử lí
                    if (transtype == TransactionType.A || transtype == TransactionType.C)
                    {
                        var docentry = GetDocumentNewEntry(DocTable);
                        objId = Function.ToString(docentry);
                    }
                    else
                    {
                        objId = Function.ToString(dtDoc.Rows[0]["doc_entry"]);
                    }

                    #region xử lý attachment process
                    var actEntry = Function.ParseInt(ExecuteScalar($@"select atc_entry from {DocTable} where cast(doc_entry as varchar) = @docEntry;", CommandType.Text, new SqlParameter("@docEntry", objId)));
                    actEntry = AttachmentProcess(document.Tables["Attachments"], transtype, actEntry);
                    if (actEntry <= 0)
                        dtDoc.Rows[0]["atc_entry"] = DBNull.Value;
                    else
                        dtDoc.Rows[0]["atc_entry"] = actEntry;
                    #endregion
                    ///chuẩn hóa dữ liệu table lines và linum của document_lines
                    dtLines = StandardizedTable(dtLines, LineTable);
                    var maxLineNum = dtLines.AsEnumerable().Max(t => Function.ParseInt(t["line_num"]));
                    for (var i = 0; i < dtLines.Rows.Count; i++)
                    {
                        if (dtLines.Rows[i]["line_num"] == DBNull.Value || dtLines.Rows[i]["line_num"] == null || Function.ParseInt(dtLines.Rows[i]["line_num"]) < 0)
                        {
                            dtLines.Rows[i]["line_num"] = ++maxLineNum;
                        }
                    }

                    ////set vis_order cho các line chưa có giá trị vis_order
                    var maxVisOrder = dtLines.AsEnumerable().Max(t => Function.ParseInt(t["vis_order"]));
                    for (var i = 0; i < dtLines.Rows.Count; i++)
                    {
                        if (dtLines.Rows[i]["vis_order"] == DBNull.Value || dtLines.Rows[i]["vis_order"] == null || Function.ParseInt(dtLines.Rows[i]["vis_order"]) < 0)
                        {
                            dtLines.Rows[i]["vis_order"] = ++maxVisOrder;
                        }
                    }

                    ///khởi tạo table tạm và đổ dữ liệu vào table tạm
                    CreateTempTableAsDatabaseTable(DocTable, TempDocTable);
                    BulkCopy(dtDoc, TempDocTable);
                    CreateTempTableAsDatabaseTable(LineTable, TempLineTable);
                    BulkCopy(dtLines, TempLineTable);

                    #region lấy dữ liệu đã lưu trên hệ thống để xử lí khi update
                    var dsCurrentData = ExecuteData(string.Format(@"select * from {0} where doc_entry = {2};
                                select * from {1} where doc_entry = {2};", DocTable, LineTable, objId));

                    CreateTempTableAsDatabaseTable(DocTable, tempCurrentDoc);
                    BulkCopy(dsCurrentData.Tables[0], tempCurrentDoc);
                    CreateTempTableAsDatabaseTable(LineTable, tempCurrentLines);
                    BulkCopy(dsCurrentData.Tables[1], tempCurrentLines);
                    #endregion

                    #region lấy dữ liệu của các phiếu base

                    //tạo table tạm chứa dữ liệu của base data
                    ExecuteNonQuery(string.Format(@"create local temporary table {0} (obj_type int, doc_entry int, line_num int, open_qty numeric(19,6)
                                                        , whs_code character varying(50), item_code character varying(50), unit_msr character varying(50), ugp_entry int
                                                        , uom_code character varying(50), num_per_msr numeric(19,6), base_type character varying(20)
                                                        , base_entry integer, base_line integer, price numeric(19,6), currency character varying(3)
                                                        , rate numeric(19,6), item_cost numeric(19,6));", tempBaseDataTable));
                    /////query cập nhật lại dữ liệu open quantity của phiếu base
                    var updateOpenQuantityQuery = "";

                    //lấy danh sách baseobject type
                    var lstBaseObj = dtLines.AsEnumerable().Where(t => Function.ParseInt(t["base_type"]) != -1).Select(t => Function.ParseInt(t["base_type"])).Distinct();
                    if(lstBaseObj.Any())
                    {
                        var executeQuery = new List<string>();
                        //lặp theo danh sách base object để lấy dữ liệu vào table tạm
                        foreach(var objType in lstBaseObj)
                        {
                            //lấy table tương ứng base object
                            var baseTable = GetDocumentTableByObjectType((APlusObjectType)objType);
                            if (baseTable == null || string.IsNullOrEmpty(baseTable.HeaderTable) || baseTable.LineTables == null || baseTable.LineTables.Count < 1 || string.IsNullOrEmpty(baseTable.LineTables[0]))
                            {
                                return new HttpResult(MessageCode.DataNotFound, "không thể lấy cấu trúc của object");
                            }
                            //lấy các bản ghi có base object type tương ứng
                            var lstBaseLine = dtLines.AsEnumerable().Where(t => Function.ParseInt(t["base_type"]) == objType);
                            foreach (var rowBase in lstBaseLine)
                            {
                                executeQuery.Add(string.Format(@"select {3}, doc_entry, line_num, open_inv_qty, whs_code, item_code, unit_msr, ugp_entry, uom_code, num_per_msr
                                                                    , base_type, base_entry, base_line, price, currency, rate, item_cost from {0} where doc_entry = {1} and line_num = {2}"
                                                                    , baseTable.LineTables[0]
                                                                    , Function.ToString(rowBase["base_entry"])
                                                                    , Function.ToString(rowBase["base_line"])
                                                                    , Function.ToString(objType)));
                            }

                            updateOpenQuantityQuery = string.Format(@"update {0} a set open_qty = case when b.quantity - coalesce(c.quantity, 0) > a.open_qty then 0 else a.open_qty - b.quantity + coalesce(c.quantity, 0) end
                                                                        from {1} b left join {2} c on b.line_num = c.line_num 
                                                                            where a.doc_entry = b.base_entry and a.line_num = b.base_line and b.base_type = '{3}';
                                                                      update {0} a set open_inv_qty = a.num_per_msr * a.open_qty
                                                                                       , line_status = case when a.open_qty > 0 then a.line_status else 'C' end
                                                                            from {1} b where a.doc_entry = b.base_entry and a.line_num = b.base_line and b.base_type = '{3}';
                                                                      update {4} a set doc_status = 'C' where exists(select 1 from {1} b where a.doc_entry = b.base_entry and b.base_type = '{3}')
                                                                                                        and not exists(select 1 from {0} c where c.line_status = 'O' and c.doc_entry = a.doc_entry);", baseTable.LineTables[0], TempLineTable, tempCurrentLines, objType, baseTable.HeaderTable);
                        }
                        var queryBase = string.Format(@"insert into {0} {1}", tempBaseDataTable, string.Join(@"
                            UNION ALL
                            ", executeQuery));
                        ExecuteNonQuery(queryBase);
                    }
                    #endregion

                    if (string.IsNullOrEmpty(objId))
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.CannotGetEntry,
                            message = "Số phiếu không được cung cấp. Vui lòng thử lại"
                        };
                    }
                    ///////////// Validate và xư lý dữ liệu trước khi lưu
                    dtResult = ExecuteDataTable("aplus_document_beforeaction", CommandType.StoredProcedure, new SqlParameter("@_object_type", (int)objtype)
                                                                                    , new SqlParameter("@_transaction_type", Function.ToString(currentTran_type))
                                                                                    , new SqlParameter("@_doc_entry", Function.ParseInt(objId))
                                                                                    , new SqlParameter("@_user_sign", userSign));
                    if (!dtResult.IsNotNull() || Function.ParseInt(dtResult.Rows[0][0]) != 200)
                    {
                        break;
                    }

                    /////////////////// gọi đến store update lại itemcost
                    ExecuteNonQuery("call aplus_document_itemcost()");

                    /////////////////// Cập nhập tại dữ liệu kho
                    dtResult = ExecuteDataTable("aplus_document_item_whs", CommandType.StoredProcedure, new SqlParameter("@_obj_type", (int)objtype)
                                                                                    , new SqlParameter("@_trans_type", Function.ToString(currentTran_type))
                                                                                    , new SqlParameter("@_obj_id", objId));
                    if (!dtResult.IsNotNull() || Function.ParseInt(dtResult.Rows[0][0]) != 200)
                    {
                        break;
                    }

                    ///thực thi câu lệnh update lại dữ liệu của phiếu base
                    if(!string.IsNullOrEmpty(updateOpenQuantityQuery))
                    {
                        ExecuteNonQuery(updateOpenQuantityQuery);
                    }

                    var query = $@"delete from {DocTable} where doc_entry = @docEntry;
                                        delete from {LineTable} where doc_entry = @docEntry;
                                        insert into {DocTable} select * from {TempDocTable};
                                        insert into {LineTable} select * from {TempLineTable};";
                    lstParamater.Add(new SqlParameter("@docEntry", Function.ParseInt(objId)));
                    //cancel phiếu base
                    if (transtype == TransactionType.C)
                    {
                        query += $@"update {DocTable} set canceled ='Y', doc_status = 'C', update_date = current_date where doc_entry = @_close_doc_entry;
                                    update {LineTable} set line_status = 'C', open_qty = 0, open_inv_qty = 0 where doc_entry = @_close_doc_entry;";
                        var baseTable = GetDocumentTableByObjectType((APlusObjectType)baseObjectType);
                        if (baseTable != null && !string.IsNullOrEmpty(baseTable.HeaderTable) && baseTable.LineTables != null && baseTable.LineTables.Count > 0 && !string.IsNullOrEmpty(baseTable.LineTables[0]))
                        {
                            query += $@"update {baseTable.HeaderTable} set doc_status = 'O' where doc_entry = @_base_boc_entry;
                                        update {baseTable.LineTables[0]} set line_status = 'O' where doc_entry = @_base_boc_entry;";
                            lstParamater.Add(new SqlParameter("@_base_boc_entry", baseEntry));
                        }
                    }
                    ExecuteNonQuery(query, CommandType.Text, lstParamater.ToArray());
                    break;
                case TransactionType.L:
                    ///khởi tạo table tạm và đổ dữ liệu vào table tạm
                    CreateTempTableAsDatabaseTable(DocTable, TempDocTable);
                    BulkCopy(document.Tables["document"], TempDocTable);
                    CreateTempTableAsDatabaseTable(LineTable, TempLineTable);
                    BulkCopy(document.Tables["document_lines"], TempLineTable);
                    //// thực thi update lại dữ liệu commit và order của item trên kho
                    dtResult = ExecuteDataTable("aplus_document_close_item_whs", CommandType.StoredProcedure, new SqlParameter("@_object_type", (int)objtype)
                                                                                    , new SqlParameter("@_doc_entry", Function.ParseInt(objId))
                                                                                    , new SqlParameter("@_user_sign", userSign)
                                                                                    , new SqlParameter("@_trans_type", Function.ToString(currentTran_type)));
                    ///update lại dữ liệu object
                    ExecuteNonQuery(string.Format(@"update {0} set doc_status = 'C' {2}  where doc_entry = @_doc_entry;
                                                    update {1} set open_qty = 0, open_inv_qty = 0, line_status = 'C' where doc_entry = @_doc_entry;", DocTable, LineTable, currentTran_type == TransactionType.C ? ", canceled = 'Y'" : "")
                                , CommandType.Text, new SqlParameter("@_doc_entry", Function.ParseInt(objId)));
                    break;
                default:
                    return new HttpResult(MessageCode.FunctionNotSupport, "Phương thức không được hỗ trợ");
            }
            if (!dtResult.IsNotNull())
            {
                return new HttpResult
                {
                    msg_code = MessageCode.UnableAccessDatabase,
                    message = "Cơ sở dữ liệu không hồi đáp"
                };
            }
            var msgCode = Function.ParseInt(dtResult.Rows[0][0]);
            if (msgCode > 0)
            {
                return new HttpResult
                {
                    msg_code = (MessageCode)msgCode,
                    message = Function.ToString(dtResult.Rows[0][1])
                };
            }
            return new HttpResult(MessageCode.Success);
        }

        private HttpResult DocumentAsDraftProcess(APlusObjectType objtype, int bplId, int userSign, ref TransactionType transtype, DataSet document,
            ref string objId, ref string objNum)
        {
            DocTable = "apz_odrf";
            LineTable = "apz_drf1";
            switch (transtype)
            {
                case TransactionType.A:
                case TransactionType.U:
                    if (document == null || document.Tables.Count < 3 || !document.Tables[0].IsNotNull())
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.DataNotProvide,
                            message = "Dữ liệu không đủ. không thể lưu"
                        };
                    }
                    var dtDoc = document.Tables["document"];
                    var dtLines = document.Tables["document_lines"];
                    dtDoc = StandardizedTable(dtDoc, "apz_odrf");
                    #region xử lí attachment
                    var actEntry = Function.ParseInt(ExecuteScalar(string.Format(@"select atc_entry from apz_odrf where doc_entry = @_docEntry;"), CommandType.Text
                                                                                    , new SqlParameter("@_docEntry", Function.ParseInt(dtDoc.Rows[0]["doc_entry"]))));
                    actEntry = AttachmentProcess(document.Tables["Attachments"], transtype, actEntry);
                    if (actEntry <= 0)
                        dtDoc.Rows[0]["atc_entry"] = DBNull.Value;
                    else
                        dtDoc.Rows[0]["atc_entry"] = actEntry;
                    #endregion

                    dtLines = StandardizedTable(dtLines, "apz_drf1");

                    #region khởi tạo lineNum cho các row không có lineNum
                    var maxLineNum = dtLines.AsEnumerable().Max(t=>Function.ParseInt(t["line_num"]));
                    for(var i = 0; i < dtLines.Rows.Count; i++)
                    {
                        if(dtLines.Rows[i]["line_num"] == DBNull.Value || dtLines.Rows[i]["line_num"] == null)
                        {
                            dtLines.Rows[i]["line_num"] = ++maxLineNum;
                        }
                    }
                    dtLines.AcceptChanges();
                    #endregion

                    //khởi tạo table tạm và bulkcopy data to table tạm
                    CreateTempTableAsDatabaseTable(DocTable, TempDocTable);
                    BulkCopy(dtDoc, TempDocTable);
                    CreateTempTableAsDatabaseTable(LineTable, TempLineTable);
                    BulkCopy(dtLines, TempLineTable);

                    //lấy docentry của document xử lí
                    objId = Function.ToString(dtDoc.Rows[0]["doc_entry"]);
                    if (transtype == TransactionType.A || transtype == TransactionType.L)
                    {
                        var docentry = GetDocumentNewEntry("apz_odrf");
                        objId = Function.ToString(docentry);
                    }
                    if (string.IsNullOrEmpty(objId))
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.Error,
                            message = "Số phiếu không được cung cấp. Vui lòng thử lại"
                        };
                    }
                    var dtResult = ExecuteDataTable("aplus_document_beforeaction", CommandType.StoredProcedure, new SqlParameter("@_object_type", (int)objtype)
                                                                                            , new SqlParameter("@_transaction_type", Function.ToString(transtype))
                                                                                            , new SqlParameter("@_doc_entry", objId)
                                                                                            , new SqlParameter("@_user_sign", userSign)
                                                                                            , new SqlParameter("@_branch", bplId));
                    if (!dtResult.IsNotNull())
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.UnableAccessDatabase,
                            message = "Cơ sở dữ liệu không hồi đáp"
                        };
                    }
                    var msgCode = Function.ParseInt(dtResult.Rows[0][0]);
                    if (msgCode > 0)
                    {
                        return new HttpResult
                        {
                            msg_code = MessageCode.Error,//(MessageCode)msgCode,
                            message = Function.ToString(dtResult.Rows[0][1])
                        };
                    }
                    var query = $@"delete from apz_odrf where doc_entry = @docEntry;
                                        delete from apz_drf1 where doc_entry = @docEntry;
                                        insert into apz_odrf select * from {TempDocTable};
                                        insert into apz_drf1 select * from {TempLineTable};";

                    ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@docEntry", objId));
                    return new HttpResult(MessageCode.Success);
                case TransactionType.D:
                    ExecuteNonQuery(@"delete from apz_odrf where doc_entry = @docEntry;
                                      delete from apz_drf1 where doc_entry = @docEntry;", CommandType.Text, new SqlParameter("@docEntry", objId));
                    return new HttpResult(MessageCode.Success);
                default:
                    return new HttpResult(MessageCode.FunctionNotSupport, "Phương thức không được hỗ trợ");
            }
        }

        public HttpResult Cancel(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId, int userSign)
        {
            return Process(objectType, fromObjectType, bplId, TransactionType.L, null, userSign);
        }

    }
}
