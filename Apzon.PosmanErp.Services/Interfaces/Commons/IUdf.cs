using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces.Commons
{
    public interface IUdf
    {
        HttpResult Get(SearchDocumentModel model);

        HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId,
            TransactionType transtype, DataSet document, int userSign, string objId = "", string objCode = "");

        HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, int company, int userSign,
            SearchDocumentModel model);
    }
}