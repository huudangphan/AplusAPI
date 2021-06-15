using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Services.Interfaces.Administrator
{
    public interface IAuth
    {
        /// <summary>
        /// lấy dữ liệu phân quyển của người dùng
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        HttpResult Get(int? user_id);

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
        HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, DataTable document, int userSign, string objId = "", string objCode = "");
    }
}
