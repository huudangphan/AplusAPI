using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces.MasterData
{
    public interface IItemMasterData    
    {
        /// <summary>
        /// Get grand items by conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        HttpResult Get(SearchDocumentModel model);
        
        /// <summary>
        /// Get inherit items by conditions
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="tranType"></param>
        /// <param name="priceList"></param>
        /// <param name="whsCode"></param>
        /// <returns></returns>
        HttpResult GetDocumentItems(int? pageIndex, int? pageSize, string tranType, int? priceList, string whsCode);
        
        /// <summary>
        /// Get grand item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResult GetById(string id);

        /// <summary>
        /// Get inherit item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResult GetDocumentById(string id);

        /// <summary>
        /// Data processing with both grand item and inherit item in the same transaction
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
        HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId,
            TransactionType transtype, DataSet document, int userSign, string objId = "", string objCode = "");
    }
}