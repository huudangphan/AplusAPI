using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces.Inventory
{
    public interface IWarehouse
    {
        HttpResult Get(SearchDocumentModel model);   
        HttpResult GetById(string id);
        HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId,
            TransactionType transtype, DataTable document, int userSign, string objId = "", string objCode = "");
    }
}