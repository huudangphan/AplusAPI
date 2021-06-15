using System.Data;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces.Commons
{
    public interface IChooseFromList
    {
        HttpResult ListResult(SearchDocumentModel model);
    }
}