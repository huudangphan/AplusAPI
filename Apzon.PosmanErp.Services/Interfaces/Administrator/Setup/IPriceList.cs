using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces.Administrator.Setup
{
    public interface IPriceList
    {
        HttpResult Get(SearchDocumentModel model);

        HttpResult GetByEntry(int id, int userId);
        
        HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, DataSet document, int userSign, string objId = "", string objCode = "");
    }
}