using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces.Documents
{
    public interface IDocuments : IBaseAction<DataSet>
    {
        /// <summary>
        /// hàm lấy dữ liệu lines của document
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="docEntry"></param>
        /// <returns></returns>
        HttpResult GetDocumentLineByEntry(APlusObjectType objType ,int docEntry);

        /// <summary>
        /// hàm get danh sách document trên hệ thống
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        HttpResult Get(SearchDocumentModel searchData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objtype"></param>
        /// <param name="bplId"></param>
        /// <param name="userSign"></param>
        /// <param name="transtype"></param>
        /// <param name="document"></param>
        /// <param name="objId"></param>
        /// <param name="objCode"></param>
        /// <returns></returns>
        HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, DataSet document, int userSign, string objId = "", string objCode = "");

        HttpResult ProcessSubTransaction(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, DataSet document, int userSign, string objId = "", string objCode = "");
    }
}
