using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Controllers.Administrator
{
    /// <summary>
    /// Controller general setting
    /// </summary>
    public class GeneralSettingController : BaseApiController<DataSet>
    {
        /// <summary>
        /// hàm khởi tạo
        /// </summary>
        /// <param name="accessToken"></param>
        public GeneralSettingController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// hàm lấy dữ liệu general setting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResult Get()
        {
            return UnitOfWork.GeneralSetting.Get();
        }

        /// <summary>
        /// api update dữ liệu setting trên hệ thống
        /// </summary>
        /// <param name="dtSetting"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update(DataSet dtSetting)
        {
            return UnitOfWork.GeneralSetting.Process(APlusObjectType.oGeneralSetting, APlusObjectType.oGeneralSetting, GetBranch(), TransactionType.A, dtSetting, GetUserSign());
        }

    }
}
