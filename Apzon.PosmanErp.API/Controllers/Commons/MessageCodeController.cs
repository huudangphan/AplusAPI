using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Mvc;

namespace PosmanErp.Controllers.Commons
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageCodeController : BaseApiController<DataTable>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        public MessageCodeController(string accessToken) : base(accessToken)
        {
            UnitOfWork = GetCommonUnitOfWork();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Get(SearchDocumentModel model)
        {
            return UnitOfWork.MessageCode.Get(model);
        }

        /// <summary>
        /// Restore System Message
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResult RestoreSystemMessage()
        {
            return UnitOfWork.MessageCode.RestoreSystemMessage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update(DataTable document)
        {
            return UnitOfWork.MessageCode.Update(document);
        }
    }
}