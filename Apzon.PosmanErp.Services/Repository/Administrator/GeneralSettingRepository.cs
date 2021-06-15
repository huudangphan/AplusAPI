using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Services.Interfaces.Administrator;
using Apzon.PosmanErp.Entities;
using Apzon.Libraries.HDBConnection.Interfaces;

namespace Apzon.PosmanErp.Services.Repository.Administrator
{
    public class GeneralSettingRepository : BaseService<DataSet>, IGeneralSetting
    {
        /// <summary>
        /// object table
        /// </summary>
        protected string TableName = "general_setting";
        /// <summary>
        /// temp object table
        /// </summary>
        protected string TempTableName = "_general_setting";

        public GeneralSettingRepository(IDatabaseClient databaseService) : base(databaseService)
		{

        }

        public HttpResult GetSettingInfo(int userId)
        {
            try
            {
                var dtInfo = ExecuteData(@"APZ_GeneralSetting_GetUserSetting", CommandType.StoredProcedure, new SqlParameter("@userId", userId));
                if (dtInfo == null || dtInfo.Tables.Count != 4)
                {
                    return new HttpResult
                    {
                        msg_code = MessageCode.Error,
                        message = "Cơ sở dữ liệu không hồi đáp"
                    };
                }
                dtInfo.Tables[0].TableName = "UserInfo";
                dtInfo.Tables[1].TableName = "SettingInfo";
                dtInfo.Tables[2].TableName = "BranchList";
                dtInfo.Tables[3].TableName = "UserAuthList";
                return new HttpResult(MessageCode.Success, dtInfo);
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

        public HttpResult Get()
        {
            try
            {
                var dsSetting = ExecuteData(@"select * from general_setting;
                                            select * from attachment_item a inner join general_setting b on a.doc_entry = b.atc_entry;");
                if(dsSetting == null || dsSetting.Tables.Count != 2)
                {
                    return new HttpResult(MessageCode.UnableAccessDatabase, "Dữ liệu setting hệ thống không tồn tại");
                }
                dsSetting.Tables[0].TableName = "general_setting";
                dsSetting.Tables[1].TableName = "attachments";
                return new HttpResult(MessageCode.Success, dsSetting);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        public override HttpResult ProcessData(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign, ref TransactionType transtype, DataSet document, ref string objId, ref string objNum)
        {
            if(!document.IsNotNull() || document == null || document.Tables.Count != 2 || document.Tables[0].Rows.Count > 1)
            {
                return new HttpResult(MessageCode.DataNotProvide, "Dữ liệu thiết lập hệ thống chưa được cung cấp");
            }
            //chuẩn hóa dữ liệu và khởi tạo dữ liệu table tạm
            var dtDoc = StandardizedTable(document.Tables["general_setting"], "general_setting");
            objId = Function.ToString(dtDoc.Rows[0]["domain"]);
            #region xử lý attachment process
            var actEntry = Function.ParseInt(ExecuteScalar($@"select atc_entry from general_setting", CommandType.Text));
            actEntry = AttachmentProcess(document.Tables["attachments"], transtype, actEntry);
            if (actEntry <= 0)
                dtDoc.Rows[0]["atc_entry"] = DBNull.Value;
            else
                dtDoc.Rows[0]["atc_entry"] = actEntry;
            #endregion
            
            CreateTempTableAsDatabaseTable(TableName, TempTableName);
            BulkCopy(dtDoc, TempTableName);

            //thực thi câu lệnh và kiểm tra kết quả trả về
            var dtResult = ExecuteDataTable("aplus_generalsetting_save", CommandType.StoredProcedure, new SqlParameter("_user_sign", userSign));
            if(!dtResult.IsNotNull())
            {
                return new HttpResult(MessageCode.UnableAccessDatabase, "Lỗi kết nối cơ sở dữ liệu");
            }
            return new HttpResult((MessageCode)Function.ParseInt(dtResult.Rows[0][0]), Function.ToString(dtResult.Rows[0][1]));
        }
    }
}
