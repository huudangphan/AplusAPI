using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using System.Data;

namespace Apzon.PosmanErp.Services.Interfaces.Administrator
{
    public interface IGeneralSetting
    {
        /// <summary>
        /// lấy dữ liệu setting của user 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        HttpResult GetSettingInfo(int userId);

        /// <summary>
        /// lấy dữ liệu general setting
        /// </summary>
        /// <returns></returns>
        HttpResult Get();

        /// <summary>
        /// update dữ liệu phân quyền của người dùng
        /// với trantype là U: lưu dữ liệu phân quyền của người dùng
        /// với trantype là M: lưu dữ liệu setting menu trên hệ thống
        /// </summary>
        /// <param name="objtype"></param>
        /// <param name="fromObjType"></param>
        /// <param name="bplId"></param>
        /// <param name="transtype"></param>
        /// <param name="document"></param>
        /// <param name="userSign"></param>
        /// <param name="objId"></param>
        /// <param name="objCode"></param>
        /// <returns></returns>
        HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, DataSet document, int userSign, string objId = "", string objCode = "");
    }
}
