using Apzon.PosmanErp.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Services.Interfaces.Commons
{
    public interface IUDOs : IBaseAction<DataSet>
    {
        /// <summary>
        /// Lấy thông tin Udt theo object
        /// </summary>
        /// <param name="udo_type"></param>
        /// <param name="udt_type"></param>
        /// <returns></returns>
        HttpResult GetUdoTable(int type);
    }
}
