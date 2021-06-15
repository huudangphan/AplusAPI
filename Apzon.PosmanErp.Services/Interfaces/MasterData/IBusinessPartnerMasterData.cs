using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Services.Interfaces.MasterData
{
    public interface IBusinessPartnerMasterData : IBaseAction<DataSet>
    {
        /// <summary>
        /// Hàm lấy thông tin nợ phải trả của BP
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        HttpResult GetLiabilities(string code);
    }
}
