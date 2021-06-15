using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Commons.Attributes;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Commons;

namespace Apzon.PosmanErp.Services.Repository.Commons
{
    public class MessageCodeRepository : BaseService<DataTable>, IMessageCode
    {
        private const string MsgCodeTable = "message_system";
        private const string ClientMsgCodeTemp = "_client_msgs";
        private const string SystemMsgCodeTemp = "_system_msgs";

        public MessageCodeRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        public HttpResult Get(SearchDocumentModel model)
        {
            try
            {
                return new HttpResult(MessageCode.Success, ExecuteDataTable(
                    "select * from "+ MsgCodeTable +" where lang_code = @_lang_code order by msg_code, msg_identifier", CommandType.Text,
                    new SqlParameter("@_lang_code", model.code)));
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.UnableAccessDatabase, ex.Message);
            }
        }

        private HttpResult GetSystemMessage()
        {
            // check duplicate value 
            var listValues = Enum.GetValues<MessageCode>();
            if (listValues.Length != listValues.Distinct().Count())
                return new HttpResult(MessageCode.DuplicateMessageCode);

            //Define message type and it's members
            var msgCodeType = typeof(MessageCode);
            var msgCodeMembers = msgCodeType.GetMembers();
            var dtLanguage = StandardizedTable(new DataTable(), MsgCodeTable);

            //add row to language data table
            foreach (var member in msgCodeMembers)
            {
                //we only use Enum field and check name
                if (member.MemberType != MemberTypes.Field) continue;
                if (!Enum.IsDefined(msgCodeType, member.Name))
                    continue;

                // get attribute value from this part of enum
                var valueAttributes =
                    (DescriptionAttribute) member.GetCustomAttribute(typeof(DescriptionAttribute), false);
                // add data retrieved to DataRow and add it to dtLanguage
                var row = dtLanguage.NewRow();
                row["lang_code"] = "en-US";
                row["msg"] = valueAttributes == null ? "" : valueAttributes.Value;
                row["msg_code"] = Enum.Parse(msgCodeType, member.Name);
                row["msg_identifier"] = member.Name;
                dtLanguage.Rows.Add(row);
            }

            return new HttpResult(MessageCode.Success, dtLanguage);
        }

        public HttpResult RestoreSystemMessage()
        {
            try
            {
                using var connection = DatabaseService.OpenConnection();
                using var transaction = connection.BeginTransaction();

                var sysMessageCode = GetSystemMessage();
                if (sysMessageCode.msg_code != MessageCode.Success)
                {
                    transaction.Rollback();
                    return sysMessageCode;
                }

                var dtMessage = (DataTable) sysMessageCode.content;
                //Do mutate data 
                CreateTempTableAsDatabaseTable(MsgCodeTable, ClientMsgCodeTemp);
                BulkCopy(dtMessage, ClientMsgCodeTemp);
                ExecuteDataTable(string.Format(@"delete from {0};  insert into {0} (select * from {1})", MsgCodeTable,
                    ClientMsgCodeTemp));
                transaction.Commit();
                return new HttpResult(MessageCode.Success, "Message Restored!");
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.UnableAccessDatabase, ex.Message);
            }
        }

        public HttpResult Update(DataTable document)
        {
            try
            {
                using var conn = DatabaseService.OpenConnection();
                using var transaction = conn.BeginTransaction();

                var systemMsg = GetSystemMessage();
                if (systemMsg.msg_code != MessageCode.Success)
                {
                    transaction.Rollback();
                    return systemMsg;
                }

                var dtSysMessage = (DataTable) systemMsg.content;
                dtSysMessage = StandardizedTable(dtSysMessage, MsgCodeTable);
                document = StandardizedTable(document, MsgCodeTable);

                //system msg code table
                CreateTempTableAsDatabaseTable(MsgCodeTable, SystemMsgCodeTemp);
                BulkCopy(dtSysMessage, SystemMsgCodeTemp);

                // current msg code table
                CreateTempTableAsDatabaseTable(MsgCodeTable, ClientMsgCodeTemp);
                BulkCopy(document, ClientMsgCodeTemp);

                // merge current and system to exists message
                var result = (int) ExecuteScalar("aplus_messagecode_save", CommandType.StoredProcedure);
                if (result != 200)
                {
                    transaction.Rollback();
                    return new HttpResult((MessageCode) result);
                }
                transaction.Commit();
                return new HttpResult((MessageCode) result);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.UnableAccessDatabase, ex.Message);
            }
        }
    }
}