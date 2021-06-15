using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Controllers.Commons
{
    /// <summary>
    /// Thông tin cài đặt trên màn hình chức năng
    /// </summary>
    public class FormSettingController : BaseApiController<DataSet>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        public FormSettingController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Lấy dữ liệu formsetting theo tham số truyền vào
        /// </summary>
        /// <param name="form_id"></param>
        /// <param name="item_id"></param>
        /// <param name="lng_code"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult Get(string form_id, string item_id, string lng_code, int user_id)
        {
            return UnitOfWork.FormSetting.Get(form_id, item_id, user_id, lng_code);
        }

        /// <summary>
        /// hàm cập nhật thông tin setting chức năng vào hệ thống
        /// nếu chưc có dữ liệu thì thêm mới
        /// </summary>
        /// <param name="dtSetting">dữ liệu setting chức năng bào gồm 3 object: form_data, setting_dât, valid_data</param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update([FromBody]DataSet dtSetting)
        {
            return UnitOfWork.FormSetting.Process(APlusObjectType.oFormSetting, APlusObjectType.oFormSetting, -1, TransactionType.U, dtSetting, GetUserSign());
        }

        /// <summary>
        /// hàm update dữ liệu valid value của các trường trên form
        /// </summary>
        /// <param name="dtValidValue"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult UpdateValidValue([FromBody]DataSet dtValidValue)
        {
            return UnitOfWork.FormSetting.Process(APlusObjectType.oFormValidValue, APlusObjectType.oFormValidValue, -1, TransactionType.U, dtValidValue, GetUserSign());
        }
    }
}
