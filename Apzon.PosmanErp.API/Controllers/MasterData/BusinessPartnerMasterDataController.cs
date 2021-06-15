using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Controllers.MasterData
{
    public class BusinessPartnerMasterDataController : BaseApiController<DataSet>
    {
        public BusinessPartnerMasterDataController(string accessToken) : base(accessToken)
        {
            //Khai báo interface và ObjectType để dùng BaseAction
            if (UnitOfWork != null)
            {
                BaseAction = UnitOfWork.BusinessPartnerMasterData;
            }
            ObjectType = Apzon.PosmanErp.Commons.APlusObjectType.oBusinessPartners;
        }

        /// <summary>
        /// Hàm lấy thông tin nợ của BP
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult GetLiabilities(string code)
        {
            return UnitOfWork.BusinessPartnerMasterData.GetLiabilities(code);
        }
    }
}
