using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Libraries.HDBConnection;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Npgsql;

namespace Apzon.PosmanErp.Services
{
    public class BaseService<TEntity> : BaseServiceClient where TEntity : class
    {
		public IDatabaseClient DatabaseService { get; set; }

		public BaseService(IDatabaseClient databaseService) : base(databaseService)
		{
            DatabaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
		}

		#region Các hàm xử lý dữ liệu khi Cập nhật document

		// ReSharper disable once FunctionComplexityOverflow
		public virtual HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, TEntity document, int userSign, string objId = "", string objCode = "")
		{
			try
			{
				using (DatabaseService.OpenConnection())
                {
                    #region xử lý dữ liệu object
                    HttpResult result;
					using (var tran = DatabaseService.BeginTransaction())
					{
                        //DatabaseService.LockNumbering(lockNumbering, objtype);
					    result = ProcessObject(objtype, fromObjType, bplId, transtype, document, userSign, ref objId, ref objCode);
					    if (result.msg_code != MessageCode.Success)
					        return result;
                        if (ProcessObjectLog(objtype, objId))
                        {
                            tran.Commit();
                        }
                        else
                        {
                            return new HttpResult(MessageCode.CreateObjectLogFailed);
                        }
                    }
                    #endregion

                    return result;
				}
			}
			catch (SqlException ex)
			{
				Logging.Write(Logging.ERROR,
					new StackTrace(new StackFrame(0)).ToString()
						.Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
				return new HttpResult
				{
				    msg_code = MessageCode.Exception,
                    message = "Could not commit transaction"
                };
			}
            catch(PostgresException ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = string.Format(@"{0} - {1}", ex.Where ,ex.Message)
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

        public virtual HttpResult ProcessSubTransaction(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, TEntity document, int userSign, string objId = "", string objCode = "")
        {
            var result = ProcessObject(objtype, fromObjType, bplId, transtype, document, userSign, ref objId, ref objCode);
            if (result.msg_code != MessageCode.Success)
                return result;
            #region xử lý dữ liệu log
            if (!ProcessObjectLog(objtype, objId))
            {
                return new HttpResult(MessageCode.CreateObjectLogFailed);
            }
            #endregion
            return result;
        }

	    private HttpResult ProcessObject(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, TEntity document, int userSign, ref string objId, ref string objCode)
	    {
            int errorCode;
            string errorMessage;
            var objNum = "";
            // insert, update, delete
            var check = ProcessData(objtype, fromObjType, bplId, userSign, ref transtype, document, ref objId, ref objNum);
            if (check.msg_code != MessageCode.Success)
            {
                return check;
            }
            string obj;
            try
            {
                obj = ((int)objtype).ToString();
            }
            catch (Exception)
            {
                obj = objtype.ToString();
            }

            var checkProcess = ProcessProcedure(obj, transtype, -1, "", objId);
            if (checkProcess.msg_code != MessageCode.Success)
            {
                return checkProcess;
            }
            var dtProcess = (DataTable)checkProcess.content;
            if (dtProcess != null && dtProcess.Rows.Count > 0)
            {
                errorCode = Function.ParseInt(dtProcess.Rows[0][0]);
                errorMessage = Function.ToString(dtProcess.Rows[0][1]);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = errorCode + "-" + errorMessage
                    };
                }
            }
            var checkNotify = NotifyProcedure(obj, transtype, -1, "", objId);
            if (checkNotify.msg_code != MessageCode.Success)
            {
                return checkNotify;
            }
            var dtNotify = (DataTable)checkNotify.content;
            if (dtNotify != null && dtNotify.Rows.Count > 0)
            {
                errorCode = Function.ParseInt(dtNotify.Rows[0][0]);
                errorMessage = Function.ToString(dtNotify.Rows[0][1]);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = errorCode + "-" + errorMessage
                    };
                }
            }
            return new HttpResult(MessageCode.Success, obj: objId);
        }

	    private bool ProcessObjectLog(APlusObjectType objtype, string objId = "")
	    {
            bool checkCommitLog = true;
            try
            {
                var lstKey = objId.Split(';');
                var ojectDefined = GetDocumentTableByObjectType(objtype);
                if (ojectDefined == null || string.IsNullOrEmpty(ojectDefined.HeaderTable))
                {
                    return true;
                }
                //LẶP THEo danh sách key để insert log
                foreach (var key in lstKey)
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }
                    ProcessChangeLog(ojectDefined, key);
                }
                #region thực hiện xóa log theo dữ liệu setting số bản ghi tối đa
                const string queryMaxHistory = @"select max_history from general_setting;";
                var maxHistoryEntry = Function.ParseInt(ExecuteScalar(queryMaxHistory));
                maxHistoryEntry = maxHistoryEntry <= 0 ? 1000 : maxHistoryEntry;
                var keySelect = "";
                var keyCondition = "";
                for (int i = 0; i < ojectDefined.ObjectColumnKeys.Count; i++)
                {
                    var keyName = ojectDefined.ObjectColumnKeys[i];
                    if (string.IsNullOrWhiteSpace(keyName))
                    {
                        continue;
                    }
                    keySelect += @" ," + keyName;
                    keyCondition += string.Format(" and a.{0} = b.{0}", keyName);
                }
                var queryDelete = $@"delete from {ojectDefined.HeaderTable}_log a using (select log_instance_id {keySelect}, row_number() over(order by create_date desc, create_time desc) row_id from {ojectDefined.HeaderTable}_log) b where a.log_instance_id = b.log_instance_id {keyCondition} and b.row_id > {maxHistoryEntry};";
                if (ojectDefined.LineTables != null)
                {
                    for (var i = 0; i < ojectDefined.LineTables.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(ojectDefined.LineTables[i]))
                        {
                            continue;
                        }
                        queryDelete += $@"
                            delete from {ojectDefined.LineTables[i]}_log a using (select log_instance_id {keySelect}, row_number() over(order by create_date desc, create_time desc) row_id from {ojectDefined.LineTables[i]}_log) b where a.log_instance_id = b.log_instance_id {keyCondition} and b.row_id > {maxHistoryEntry};";
                    }
                }
                ExecuteNonQuery(queryDelete);
                #endregion
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, $@"create log failed, throw exception. object {objtype} id {objId}.", ex);
                checkCommitLog = false;
            }
            return checkCommitLog;
	    }

        /// <summary>
        /// hàm lấy dữ liệu log entry của object
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="objectDefined"></param>
        /// <returns></returns>
        private int GetNewLogEntry(string objectKey, ObjectDefinedModel objectDefined)
        {
            var listHeadParam = new List<SqlParameter>();
            string headerWhereClause = "";
            var headerParamValues = objectKey.Split(',');
            //lấy điều kiện where của log object
            for (int i = 0; i < objectDefined.ObjectColumnKeys.Count; i++)
            {
                var columnKey = objectDefined.ObjectColumnKeys[i];
                if (string.IsNullOrWhiteSpace(columnKey) || headerParamValues.Length <= i)
                {
                    continue;
                }
                var pr = headerParamValues[i];
                headerWhereClause += (" AND " + columnKey + " :: character varying=@_objId" + i + " :: character varying");
                listHeadParam.Add(new SqlParameter("@_objId" + i, pr));
            }
            //Lấy Max Instance Key tương ứng với bảng được khai báo TableLogInstanceKey
            var sqlQuery =
                $@" select coalesce(max(log_instance_id),0)+1 from {objectDefined.HeaderTable}_log where 1=1 {headerWhereClause}";
            return Function.ParseInt(ExecuteScalar(sqlQuery, CommandType.Text, parameters: listHeadParam.ToArray()));
        }

        /// <summary>
        /// hàm lấy query insert log của table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="logEntry"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private void ProcessChangeLog(ObjectDefinedModel objectDefined, string key)
        {
            var logEntry = GetNewLogEntry(key, objectDefined);
            string whereClause = "";
            var lstParam = new List<SqlParameter>();
            var paramValues = key.Split(',');
            for (int i = 0; i < objectDefined.ObjectColumnKeys.Count; i++)
            {
                var keyName = objectDefined.ObjectColumnKeys[i];
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }
                var pr = paramValues[i];
                //Dựa vào cấu hình Key và giá trị được lấy ra từ ObjId (các giá trị cách nhau bằng dấu ',') để tạo ra mệnh để where để lọc
                whereClause += (@" AND  " + keyName + @" :: character varying = @objId" + i + " :: character varying");
                lstParam.Add(new SqlParameter("@objId" + i, pr));
            }
            //Lưu vào bảng log những bản ghi từ bảng chính để lưu vào bảng log với LogInstance Key được lấy bằng với maxInstanceKey được lấy ở trên
            var sqlHistory = $@" insert into {objectDefined.HeaderTable}_log select {logEntry}, * from {objectDefined.HeaderTable}  where 1=1 {whereClause};";
            if(objectDefined.LineTables != null)
            {
                for(var i = 0; i < objectDefined.LineTables.Count; i++)
                {
                    if(string.IsNullOrWhiteSpace(objectDefined.LineTables[i]))
                    {
                        continue;
                    }
                    sqlHistory += $@"
                    insert into {objectDefined.LineTables[i]}_log select {logEntry}, * from {objectDefined.LineTables[i]}  where 1=1 {whereClause};";
                }
            }
            ExecuteNonQuery(sqlHistory, parameters: lstParam.ToArray());
        }
        
        /// <summary>
        /// hàm xử lý modified dữ liệu của các object
        /// </summary>
        /// <param name="objtype"></param>
        /// <param name="fromObjType"></param>
        /// <param name="bplId"></param>
        /// <param name="userSign"></param>
        /// <param name="transtype"></param>
        /// <param name="document"></param>
        /// <param name="objId"></param>
        /// <param name="objNum"></param>
        /// <returns></returns>
		public virtual HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype,
			TEntity document, ref string objId, ref string objNum)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Hàm validate, xử lý dữ liệu [APZON USE ONLY]
		/// </summary>
		/// <param name="objtype"></param>
		/// <param name="transtype">[A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose</param>
		/// <param name="numOfkey">Số cột (count) là khóa chính</param>
		/// <param name="keyColumns">Các cột khóa chính</param>
		/// <param name="objId">Giá trị khóa chính</param>
		/// <returns></returns>
		public HttpResult ProcessProcedure(string objtype, TransactionType transtype, int numOfkey,
			string keyColumns, string objId)
		{
			try
			{
				const string sql = @"apz_process";
				var lst = new List<SqlParameter>();
				lst.Add(new SqlParameter("@objtype", string.IsNullOrEmpty(objtype) ? "" : objtype));
				lst.Add(new SqlParameter("@transtype", string.IsNullOrEmpty(transtype.ToString()) ? "" : transtype.ToString()));
				lst.Add(new SqlParameter("@num_of_key", numOfkey > 0 ? numOfkey : 0));
				lst.Add(new SqlParameter("@key_columns", string.IsNullOrEmpty(keyColumns) ? "" : keyColumns));
				lst.Add(new SqlParameter("@obj_id", string.IsNullOrEmpty(objId) ? "" : objId));
				return new HttpResult(MessageCode.Success , ExecuteDataTable(sql, CommandType.StoredProcedure, lst.ToArray()));
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

		/// <summary>
		///     Hàm validate, xử lý dữ liệu [ENDED USER]
		/// </summary>
		/// <param name="objtype"></param>
		/// <param name="transtype">[A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose</param>
		/// <param name="numOfKey">Số cột (count) là khóa chính</param>
		/// <param name="keyColumns">Các cột khóa chính</param>
		/// <param name="objId">Giá trị khóa chính</param>
		/// <returns></returns>
		public HttpResult NotifyProcedure(string objtype, TransactionType transtype, int numOfKey,
			string keyColumns, string objId)
		{
			try
			{
				const string sql = @"apz_notify";
				var lst = new List<SqlParameter>();
				lst.Add(new SqlParameter("@objtype", string.IsNullOrEmpty(objtype) ? "" : objtype));
				lst.Add(new SqlParameter("@transtype", string.IsNullOrEmpty(transtype.ToString()) ? "" : transtype.ToString()));
				lst.Add(new SqlParameter("@num_of_key", numOfKey >= 0 ? numOfKey : 0));
				lst.Add(new SqlParameter("@key_columns", string.IsNullOrEmpty(keyColumns) ? "" : keyColumns));
				lst.Add(new SqlParameter("@obj_id", string.IsNullOrEmpty(objtype) ? "" : objId));
				return new HttpResult(MessageCode.Success, ExecuteDataTable(sql, CommandType.StoredProcedure, lst.ToArray()));
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
        
        #endregion

        /// <summary>
        /// hàm update lại các trạng thái của phiếu và xử lí timeline process
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="docEntry"></param>
        //protected virtual void UpdateTimeLineStatus(APlusObjectType objType, int docEntry)
        //{
        //    var tableData = GetDocumentTableByObjectType(objType);
        //    if (tableData == null || tableData.Count < 2)
        //    {
        //        return;
        //    }
        //    #region get base object

        //    var dtBaseObj = ExecuteDataTable(string.Format("Select BaseType, BaseEntry from {0} where DocEntry = @docEntry group by BaseType, BaseEntry", tableData[1])
        //                                                    , CommandType.Text, new SqlParameter("@docEntry", docEntry));
        //    if (!dtBaseObj.IsNotNull())
        //    {
        //        return;
        //    }
        //    var sqlText = "";
        //    if(objType != APlusObjectType.oOutgoingPayment && objType != APlusObjectType.oIncommingPayment)
        //        sqlText +=
        //                string.Format(@"INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
        //                                    values('{1}', {0}, isnull((select max(LineId) from APZ_OOTL where ObjType = '{1}' and DocEntry = {0}),-1) + 1, 1, @currentDate, @currentTime)
        //                            if exists(select 1 from {2} where DocEntry = {0} and IsIns = 'Y')
        //                            BEGIN
        //                                update {2} set DelStatus = 'Y' where DocEntry = {0};
        //                                    INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
        //                                        values('{1}', {0}, isnull((select max(LineId) from APZ_OOTL where ObjType = '{1}' and DocEntry = {0}),-1) + 1, 10, @currentDate, @currentTime)
        //                            END"
        //                                            , docEntry, (int)objType, tableData[0]);
        //    for (var i = 0; i < dtBaseObj.Rows.Count; i++)
        //    {
        //        var baseObjType = (APlusObjectType)Function.ParseInt(dtBaseObj.Rows[i][0]);
        //        var baseDocEntry = Function.ParseInt(dtBaseObj.Rows[0][1]);
        //        var tableBase = GetDocumentTableByObjectType(baseObjType);
        //        if (tableBase == null || tableBase.Count < 2)
        //        {
        //            continue;
        //        }
        //        if (baseObjType == objType)
        //        {
        //            sqlText +=
        //                string.Format(
        //                    @"
        //                            if exists(select 1 from {2} where DocEntry = {1} and CANCELED = 'Y')
        //                                INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
        //                                            values({0}, {1}, isnull((select max(LineId) from APZ_OOTL where ObjType = '{0}' and DocEntry = {1}),-1) + 1, 4, @currentDate, @currentTime)"
        //                                        , (int)objType, baseDocEntry, tableData[0]);
        //            sqlText += @"
        //                        " + UpdateTimeLineStatusWhenCancel(baseObjType, baseDocEntry);
        //            break;
        //        }
        //        switch (objType)
        //        {
        //            case APlusObjectType.oDeliveryNotes:
        //                sqlText += string.Format(@"
        //                                Update {0} set DelStatus = 'D' where DocEntry = {1}
        //                                INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
        //                                    values({2}, {1}, isnull((select max(LineId) from APZ_OOTL where ObjType = '{2}' and DocEntry = {1}),-1) + 1, 4, @currentDate, @currentTime)"
        //                    , tableBase[0], baseDocEntry, (int)baseObjType);
        //                break;
        //            case APlusObjectType.oPurchaseDeliveryNotes:
        //                sqlText += string.Format(@"
        //                                Update {0} set DelStatus = 'Y' where DocEntry = {1}
        //                                INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
        //                                    values({2}, {1}, isnull((select max(LineId) from APZ_OOTL where ObjType = '{2}' and DocEntry = {1}),-1) + 1, 5, @currentDate, @currentTime)"
        //                    , tableBase[0], baseDocEntry, (int)baseObjType);
        //                break;
        //            case APlusObjectType.oReturns:
        //            case APlusObjectType.oPurchaseReturns:
        //                sqlText += string.Format(@"
        //                                            update {0} set RetStatus = 'H' where DocEntry = {1}
	       //                                         update {0} set RetStatus = 'A' where DocEntry = {1} and 
								//			                        not exists(select 1 from {2} x 
								//					                        left JOin (select a1.BaseLine, SUM(a1.Quantity) Quantity from {3} a1 inner join {5} a2 On a1.DocEntry = a2.DocEntry 
        //                                                                                where isnull(a2.CANCELED, 'N') = 'N' and a1.BaseType = '{4}' and a1.BaseEntry = {1} group by a1.BaseLine) y 
								//                        on x.LineNum = y.BaseLine where x.DocEntry = {1} and x.Quantity - isnull(y.Quantity,0) > 0)
        //                                            INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
        //                                                values({4}, {1}, isnull((select max(LineId) from APZ_OOTL where ObjType = '{4}' and DocEntry = {1}),-1) + 1, 6, @currentDate, @currentTime)"
        //                    , tableBase[0], baseDocEntry, tableBase[1], tableData[1], (int)baseObjType, tableData[0]);
        //                break;
        //            case APlusObjectType.oOutgoingPayment:
        //            case APlusObjectType.oIncommingPayment:
        //                sqlText += string.Format(@"
        //                                            if exists(select 1 from APZ_RCT1 where PayMth <> 'COD' and DocEntry = {0})
	       //                                         BEGIN

		
		      //                                          declare @lineId int = isnull((select max(LineId) from APZ_OOTL where DocEntry = {1} and ObjType = '{2}'),-1) + 1
		      //                                          INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime, TextInfo)
		      //                                          values ('{2}', {1}, @lineId, 2, @currentDate, @currentTime, isnull(convert(nvarchar,(select sum(isnull(LineTotal, 0)) from APZ_RCT1 where PayMth <> 'COD'and DocEntry = {0})), '') + ' VND')
        //                                                declare @paymentTotal numeric(19,6) = isnull((select sum(isnull(LineTotal,0)) from APZ_RCT1 a inner join APZ_RCT2 b ON a.DocEntry = b.DocEntry where b.BaseType ='{2}' and b.BaseEntry = {1} and a.PayMth <> 'COD'),0)
		      //                                          update {3} set PmtStatus = case when DocTotal > @paymentTotal then 'P' else 'Y' end where DocEntry = {1}

	       //                                         END", docEntry, baseDocEntry, (int)baseObjType, tableBase[0]);
        //                break;
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(sqlText))
        //    {
        //        ExecuteNonQuery(sqlText, CommandType.Text, new SqlParameter("@currentDate", DateTime.Now.Date)
        //            , new SqlParameter("@currentTime", DateTime.Now.Hour*100 + DateTime.Now.Minute));
        //    }

        //    #endregion
        //}

        /// <summary>
        /// hàm update lại các trạng thái của phiếu và xử lí timeline process
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="docEntry"></param>
        //private string UpdateTimeLineStatusWhenCancel(APlusObjectType objType, int docEntry)
        //{
        //    var tableData = GetDocumentTableByObjectType(objType);
        //    if (tableData == null || string.IsNullOrEmpty(tableData.HeaderTable) || tableData.LineTables == null || tableData.LineTables.Count < 1 || string.IsNullOrEmpty(tableData.LineTables[0]))
        //    {
        //        return "";
        //    }
        //    #region get base object

        //    var dtBaseObj = ExecuteDataTable(string.Format("Select BaseType, BaseEntry from {0} where DocEntry = @docEntry group by BaseType, BaseEntry", tableData[1])
        //                                                    , CommandType.Text, new SqlParameter("@docEntry", docEntry));
        //    if (!dtBaseObj.IsNotNull())
        //    {
        //        return "";
        //    }
        //    var sqlText = "";
        //    for (var i = 0; i < dtBaseObj.Rows.Count; i++)
        //    {
        //        var baseObjType = (APlusObjectType)Function.ParseInt(dtBaseObj.Rows[i][0]);
        //        var baseDocEntry = Function.ParseInt(dtBaseObj.Rows[0][1]);
        //        var tableBase = GetDocumentTableByObjectType(baseObjType);
        //        if (tableBase == null || tableBase.Count < 2)
        //        {
        //            continue;
        //        }
        //        switch (objType)
        //        {
        //            case APlusObjectType.oDeliveryNotes:
        //            case APlusObjectType.oPurchaseDeliveryNotes:
        //                sqlText += string.Format(@"
        //                                Update {0} set DelStatus = 'N' where DocEntry = {1}
        //                                INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
        //                                    values({2}, {1}, isnull((select max(LineId) from APZ_OOTL where ObjType = '{2}' and DocEntry = {1}),-1) + 1, 6, @currentDate, @currentTime)"
        //                    , tableBase[0], baseDocEntry, (int)baseObjType);
        //                break;
        //            case APlusObjectType.oReturns:
        //            case APlusObjectType.oPurchaseReturns:
        //                sqlText += string.Format(@"
        //                                            update {0} set RetStatus = 'H' where DocEntry = {1}
	       //                                         update {0} set RetStatus = 'A' where DocEntry = {1} and 
								//			                        not exists(select 1 from {2} x 
								//					                        left JOin (select a1.BaseLine, SUM(a1.Quantity) Quantity from {3} a1 inner join {5} a2 On a1.DocEntry = a2.DocEntry 
        //                                                                                where isnull(a2.CANCELED, 'N') = 'N' and a1.BaseType = '{4}' and a1.BaseEntry = {1} group by a1.BaseLine) y 
								//                        on x.LineNum = y.BaseLine where x.DocEntry = {1} and x.Quantity - isnull(y.Quantity,0) > 0)
        //                                            INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
        //                                                values({4}, {1}, isnull((select max(LineId) from APZ_OOTL where ObjType = '{4}' and DocEntry = {1}),-1) + 1, 4, @currentDate, @currentTime)"
        //                    , tableBase[0], baseDocEntry, tableBase[1], tableData[1], (int)baseObjType, tableData[0]);
        //                break;
        //            case APlusObjectType.oOutgoingPayment:
        //            case APlusObjectType.oIncommingPayment:
        //                sqlText += string.Format(@"
        //                                            if not exists(select 1 from APZ_RCT1 a inner join APZ_RCT2 b ON a.DocEntry = b.DocEntry inner inner join APZ_ORCT c ON a.DocEntry = c.DocEntry
        //                                                                where isnull(c.CANCELED,'N') = 'N' and a.PayMth <> 'COD' and b.BaseEntry = {0} and b.BaseType = '{2}')
	       //                                         BEGIN

		
		      //                                          declare @lineId int = isnull((select max(LineId) from APZ_OOTL where DocEntry = {1} and ObjType = '{2}'),-1) + 1
		      //                                          INSERT INTO APZ_OOTL(ObjType, DocEntry, LineId, ProcessId, ChangeDate, ChangeTime)
		      //                                          values ('{2}', {1}, @lineId, 12, @currentDate, @currentTime)

		      //                                          update {3} set PmtStatus = 'N' where DocEntry = {1}

	       //                                         END", docEntry, baseDocEntry, (int)baseObjType, tableBase[0]);
        //                break;
        //        }
        //    }
        //    #endregion

        //    return sqlText;
        //}

        /// <summary>
        /// hàm lấy thông tin bảng của document theo object type
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        protected ObjectDefinedModel GetDocumentTableByObjectType(APlusObjectType objType)
        {
            var objectDefined = new ObjectDefinedModel();
            switch (objType)
            {
                case APlusObjectType.oDeliveryNotes:
                    objectDefined.HeaderTable = "s_delivery";
                    objectDefined.LineTables = new List<string> { "s_delivery_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oInventoryGenEntry:
                    objectDefined.HeaderTable = "receipt";
                    objectDefined.LineTables = new List<string> { "receipt_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oInventoryGenExit:
                    objectDefined.HeaderTable = "issue";
                    objectDefined.LineTables = new List<string> { "issue_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oIncommingPayment:
                case APlusObjectType.oOutgoingPayment:
                    objectDefined.HeaderTable = "payment";
                    objectDefined.LineTables = new List<string> { "payment_item", "payment_base" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
		        #region Base master data
                
                case APlusObjectType.oBpGroup:
                    objectDefined.HeaderTable = "partner_group";
                    objectDefined.LineTables = new List<string> {  };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oItemGroups:
                    objectDefined.HeaderTable = "item_group";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oTrademark:
                    objectDefined.HeaderTable = "trademark";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oCurrency:
                    objectDefined.HeaderTable = "currency";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oShift:
                    objectDefined.HeaderTable = "shift";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oItemTag:
                    objectDefined.HeaderTable = "item_tag";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oUoM:
                    objectDefined.HeaderTable = "unit";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oVatGroup:
                    objectDefined.HeaderTable = "tax_group";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oCompany:
                    objectDefined.HeaderTable = "company";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oShippingUnit:
                    objectDefined.HeaderTable = "shipping_unit";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                case APlusObjectType.oGeographyLocation:
                case APlusObjectType.oCountry:
                case APlusObjectType.oProvince:
                case APlusObjectType.oDistrict:
                case APlusObjectType.oWards:
                    objectDefined.HeaderTable = "geography";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
				case APlusObjectType.oRetailTable:
                    objectDefined.HeaderTable = "table_info";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
				case APlusObjectType.oPostingPeriod:
                    objectDefined.HeaderTable = "financial_period";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
				case APlusObjectType.oWeight:
                    objectDefined.HeaderTable = "weight";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
				case APlusObjectType.oLength:
                    objectDefined.HeaderTable = "length";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                #endregion

                case APlusObjectType.oWarehouses:
                    objectDefined.HeaderTable = "warehouse";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "whs_code" };
                    break;
                case APlusObjectType.oItems:
                    objectDefined.HeaderTable = "item";
                    objectDefined.LineTables = new List<string> { "item_price" };
                    objectDefined.ObjectColumnKeys = new List<string> { "item_code" };
                    break;
                case APlusObjectType.oInventoryTransfer:
                    objectDefined.HeaderTable = "transfer";
                    objectDefined.LineTables = new List<string> { "transfer_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oInventoryTransferRequest:
                    objectDefined.HeaderTable = "transfer_request";
                    objectDefined.LineTables = new List<string> { "transfer_request_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oInvoices:
                case APlusObjectType.oReserveInvoices:
                    objectDefined.HeaderTable = "s_invoice";
                    objectDefined.LineTables = new List<string> { "s_invoice_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oOrders:
                    objectDefined.HeaderTable = "s_order";
                    objectDefined.LineTables = new List<string> { "s_order_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oPurchaseDeliveryNotes:
                    objectDefined.HeaderTable = "p_delivery";
                    objectDefined.LineTables = new List<string> { "p_delivery_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oPurchaseInvoices:
                case APlusObjectType.oPurchaseReserveInvoices:
                    objectDefined.HeaderTable = "p_invoice";
                    objectDefined.LineTables = new List<string> { "p_invoice_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oPurchaseOrders:
                    objectDefined.HeaderTable = "p_order";
                    objectDefined.LineTables = new List<string> { "p_order_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oPurchaseQuotations:
                    objectDefined.HeaderTable = "p_quotation";
                    objectDefined.LineTables = new List<string> { "p_quotation_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oPurchaseReturnRequest:
                    objectDefined.HeaderTable = "p_return_request";
                    objectDefined.LineTables = new List<string> { "p_return_request_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oPurchaseReturns:
                    objectDefined.HeaderTable = "p_return";
                    objectDefined.LineTables = new List<string> { "p_return_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oQuotations:
                    objectDefined.HeaderTable = "s_quotation";
                    objectDefined.LineTables = new List<string> { "s_quotation_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oReturnRequest:
                    objectDefined.HeaderTable = "s_return_request";
                    objectDefined.LineTables = new List<string> { "s_return_request_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oReturns:
                    objectDefined.HeaderTable = "s_return";
                    objectDefined.LineTables = new List<string> { "s_return_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oBusinessPartners:
                    objectDefined.HeaderTable = "partner";
                    objectDefined.LineTables = new List<string> { "partner_address" };
                    objectDefined.ObjectColumnKeys = new List<string> { "card_code" };
                    break;
                case APlusObjectType.oDraft:
                    objectDefined.HeaderTable = "draft";
                    objectDefined.LineTables = new List<string> { "draft_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oGeneralSetting:
                    objectDefined.HeaderTable = "general_setting";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "domain" };
                    break;
                case APlusObjectType.oUser:
                    objectDefined.HeaderTable = "user_info";
                    objectDefined.LineTables = new List<string> { "user_company", "user_config_1", "user_config_2" };
                    objectDefined.ObjectColumnKeys = new List<string> { "user_id" };
                    break;
                case APlusObjectType.oAuth:
                    objectDefined.HeaderTable = "auth";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "user_id" };
                    break;
                case APlusObjectType.oInventoryCounting:
                    objectDefined.HeaderTable = "whs_counting";
                    objectDefined.LineTables = new List<string> { "whs_counting_item" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oUoMGroups:
                    objectDefined.HeaderTable = "unit_group";
                    objectDefined.LineTables = new List<string> { "unit_group_1" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oPriceList:
                    objectDefined.HeaderTable = "price_list";
                    objectDefined.LineTables = new List<string> { "item_price" };
                    objectDefined.ObjectColumnKeys = new List<string> { "doc_entry" };
                    break;
                case APlusObjectType.oExchangeRate:
                    objectDefined.HeaderTable = "exchange_rate";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "due_date", "currency" };
                    break;
                case APlusObjectType.oFormSetting:
                    objectDefined.HeaderTable = "form_setting_user";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "setting_entry", "user_id"};
                    break;
                case APlusObjectType.oUDF:
                    objectDefined.HeaderTable = "user_field";
                    objectDefined.LineTables = new List<string> { "user_field_layout", "user_field_value" };
                    objectDefined.ObjectColumnKeys = new List<string> { "table_name", "column_id" };
                    break;
                case APlusObjectType.oUDT:
                    objectDefined.HeaderTable = "user_table";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { "table_name" };
                    break;
                case APlusObjectType.oUDO:
                    objectDefined.HeaderTable = "user_object";
                    objectDefined.LineTables = new List<string> { "user_object_child", "user_object_search", "user_object_default", "user_object_child_default" };
                    objectDefined.ObjectColumnKeys = new List<string> { "code" };
                    break;
                default:
                    objectDefined.HeaderTable = "";
                    objectDefined.LineTables = new List<string> { };
                    objectDefined.ObjectColumnKeys = new List<string> { };
                    break;
            }
            return objectDefined;
        }

        /// <summary>
        /// chuẩn hóa cấu trúc table theo cấu trúc bảng trong database
        /// </summary>
        /// <param name="dtClient"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
	    protected DataTable StandardizedTable(DataTable dtClient, string tableName)
	    {
            var dtStand = GetTableStruct(tableName);
            if (!dtStand.IsNotNull())
            {
                throw new Exception("Không thể lấy cấu trúc bảng");
            }
            dtStand.Rows.Clear();
            for(var i = 0; i < dtClient.Rows.Count; i++)
            {
                dtStand.ImportRow(dtClient.Rows[i]);
            }
            return dtStand;
        }

        /// <summary>
        /// tạo bảng tạo dựa vào cấu trúc bảng trong cơ sở dữ liệu
        /// </summary>
        /// <param name="databaseTableName"></param>
        /// <param name="tempTableName"></param>
	    protected void CreateTempTableAsDatabaseTable(string databaseTableName, string tempTableName)
        {
            DropTempTable(tempTableName);
            var sqlString = string.Format(@"create local temporary table ""{1}"" as select * from ""{0}"" limit 0;", databaseTableName, tempTableName);
            ExecuteNonQuery(sqlString);
        }

        protected void DropTempTable(string tempTableName)
	    {
	        var sqlDrop = string.Format(@"do $$ begin 
												    if exists(select 1 from information_schema.tables where table_name = '{0}' and table_type = 'LOCAL TEMPORARY') then
												        drop table {0};
												    end if;
												end;$$;", tempTableName);
	        ExecuteNonQuery(sqlDrop);
	    }

        /// <summary>
        /// lấy dữ liệu new entry
        /// </summary>
        /// <param name="headerTable"></param>
        /// <returns></returns>
	    protected int GetDocumentNewEntry(string headerTable)
	    {
            var docentry = Function.ParseInt(ExecuteScalar($"select max(doc_entry) from {headerTable};")) + 1;
            if (docentry < 100000)
            {
                docentry = 100000;
            }
	        return docentry;
	    }

	    /// <summary>
	    /// hàm xử lí lưu attachment
	    /// </summary>
	    /// <param name="dtAttachment"></param>
	    /// <param name="transType"></param>
	    /// <param name="atcEntry"></param>
	    /// <returns></returns>
	    protected int AttachmentProcess(DataTable dtAttachment, TransactionType transType, int atcEntry = 0)
	    {
	        if (atcEntry > 0)
	        {
	            ExecuteNonQuery(@"delete from attachment_item Where doc_entry = @_docEntry;
                                  delete from attachment where doc_entry = @_docEntry;", CommandType.Text, new SqlParameter("@_docEntry", atcEntry));
	        }
	        if (transType == TransactionType.D)
	        {
	            return -1;
	        }
            if (!dtAttachment.IsNotNull())
            {
                return -1;
            }
            dtAttachment = StandardizedTable(dtAttachment, "attachment_item");
            CreateTempTableAsDatabaseTable("attachment_item", "_apz_atc1");
            BulkCopy(dtAttachment, "_apz_atc1");
            return Function.ParseInt(ExecuteScalar("aplus_attachment_save", CommandType.StoredProcedure, new SqlParameter("_docentry", atcEntry)));
	    }
    }
}