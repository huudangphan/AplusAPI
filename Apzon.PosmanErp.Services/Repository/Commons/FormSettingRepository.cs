using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Services.Repository.Commons
{
    public class FormSettingRepository : BaseService<DataSet>, IFormSetting
    {
        private string _formDataTable = "form_setting";
        private string _settingDataTable = "form_setting_user";
        private string _validDataTable = "form_setting_value";

        private string _tempFormDataTable = "_form_setting";
        private string _tempSettingTable = "_form_setting_user";
        private string _tempValidTable = "_form_setting_value";

        public FormSettingRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }

        public HttpResult Get(string formId, string itemId, int userId, string lngCode)
        {
            try
            {
                var dsSetting = ExecuteData(@"aplus_formsetting_get", CommandType.StoredProcedure, new SqlParameter("_formid",formId), new SqlParameter("_userid", userId)
                                                                                                 , new SqlParameter("_lngcode", lngCode), new SqlParameter("_itemid", itemId));
                if(dsSetting == null || dsSetting.Tables.Count != 3 || !dsSetting.Tables[1].IsNotNull())
                {
                    return new HttpResult(MessageCode.RecordNotFound, "Không tìm thấy dữ liệu cài đặt của chức năng");
                }
                dsSetting.Tables[0].TableName = "form_data";
                dsSetting.Tables[1].TableName = "setting_data";
                dsSetting.Tables[2].TableName = "valid_data";
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
            DataTable saveResult;
            if (objtype == APlusObjectType.oFormSetting)
            {
                if (document == null || document.Tables.Count != 2 || !document.Tables["form_data"].IsNotNull() || !document.Tables["setting_data"].IsNotNull())
                {
                    return new HttpResult(MessageCode.RecordNotFound, "Không tìm thấy dữ liệu cài đặt của chức năng");
                }
                #region khởi tạo data

                var formData = StandardizedTable(document.Tables["form_data"], _formDataTable);
                var settingData = StandardizedTable(document.Tables["setting_data"], _settingDataTable);
                //var validData = StandardizedTable(document.Tables["valid_data"], _validDataTable);

                CreateTempTableAsDatabaseTable(_formDataTable, _tempFormDataTable);
                CreateTempTableAsDatabaseTable(_settingDataTable, _tempSettingTable);
                //CreateTempTableAsDatabaseTable(_validDataTable, _tempValidTable);

                BulkCopy(formData, _tempFormDataTable);
                BulkCopy(settingData, _tempSettingTable);
                //BulkCopy(validData, _tempValidTable);

                #endregion

                saveResult = ExecuteDataTable("aplus_formsetting_save", CommandType.StoredProcedure, new SqlParameter("_user_sign", (Int16)userSign));
            }
            else
            {
                if (document == null || document.Tables.Count != 2 || !document.Tables["form_data"].IsNotNull() || !document.Tables["valid_data"].IsNotNull())
                {
                    return new HttpResult(MessageCode.RecordNotFound, "Không tìm thấy dữ liệu cài đặt valid value của chức năng");
                }

                #region khởi tạo data

                var formData = StandardizedTable(document.Tables["form_data"], _formDataTable);
                var validData = StandardizedTable(document.Tables["valid_data"], _validDataTable);

                CreateTempTableAsDatabaseTable(_formDataTable, _tempFormDataTable);
                CreateTempTableAsDatabaseTable(_validDataTable, _tempValidTable);

                BulkCopy(formData, _tempFormDataTable);
                BulkCopy(validData, _tempValidTable);

                #endregion

                saveResult = ExecuteDataTable("aplus_formsetting_savevalidvalue", CommandType.StoredProcedure);
            }
            if(!saveResult.IsNotNull())
            {
                return new HttpResult(MessageCode.UnableAccessDatabase, "Lỗi kết nối cơ sở dữ liệu");
            }
            var msgCode = Function.ParseInt(saveResult.Rows[0][0]);
            objId = Function.ToString(saveResult.Rows[0][2]);
            return new HttpResult((MessageCode)msgCode, Function.ToString(saveResult.Rows[0][1]), Function.ToString(saveResult.Rows[0][2]));
        }
    }
}
