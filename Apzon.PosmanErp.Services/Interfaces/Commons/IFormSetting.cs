using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Services.Interfaces.Commons
{
    public interface IFormSetting
    {
        /// <summary>
        /// hàm lấy dữ liệu formsetting của form
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <param name="lngCode"></param>
        /// <returns></returns>
        HttpResult Get(string formId, string itemId, int userId, string lngCode);

        /// <summary>
        /// hàm cập nhật form setting của 1 form
        /// dataset truyền vào bao gồm dữ liệu setting của các control và dữ liệu valid value
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
