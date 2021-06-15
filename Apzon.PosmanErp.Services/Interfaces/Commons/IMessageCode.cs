using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces.Commons
{
    public interface IMessageCode
    {
        HttpResult Get(SearchDocumentModel model);

        HttpResult RestoreSystemMessage();

        HttpResult Update(DataTable document);
    }
}