using System.Data;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Mvc;

namespace PosmanErp.Controllers.Commons
{
    /// <summary>
    /// 
    /// </summary>
    public class ChooseFromListController : BaseApiController<DataTable>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        public ChooseFromListController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Get List result by object_type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Get([FromBody] SearchDocumentModel model)
        {
            return UnitOfWork.ChooseFromList.ListResult(model);
        }
    }
}