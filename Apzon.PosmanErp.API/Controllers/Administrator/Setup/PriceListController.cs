using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;

namespace PosmanErp.Controllers.Administrator.Setup
{
    /// <summary>
    /// Price List controller 
    /// </summary>
    public class PriceListController : BaseApiController<DataSet>
    {
        /// <summary>
        /// Default controller
        /// </summary>
        /// <param name="accessToken"></param>
        public PriceListController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Get by conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Get(SearchDocumentModel model)
        {
            return UnitOfWork.PriceList.Get(model);
        }

        /// <summary>
        /// Get by doc_entry and user_sign
        /// </summary>
        /// <param name="entry">Price list entry</param>
        /// <param name="user_id">User create</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult GetByEntry(string entry, string user_id)
        {
            return UnitOfWork.PriceList.GetByEntry(Function.ParseInt(entry), Function.ParseInt(user_id));
        }

        /// <summary>
        /// Create new price list
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Create([FromBody] DataSet document)
        {
            return UnitOfWork.PriceList.Process(APlusObjectType.oPriceList, APlusObjectType.oPriceList, GetBranch(),
                TransactionType.A, document, GetUserSign());
        }

        /// <summary>
        /// Update price list
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update([FromBody] DataSet document)
        {
            return UnitOfWork.PriceList.Process(APlusObjectType.oPriceList, APlusObjectType.oPriceList, GetBranch(),
                TransactionType.U, document, GetUserSign());
        }

        /// <summary>
        /// Delete price list
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Delete([FromBody] SearchDocumentModel model)
        {
            return UnitOfWork.PriceList.Process(APlusObjectType.oPriceList, APlusObjectType.oPriceList, GetBranch(),
                TransactionType.D, null, GetUserSign(), Function.ToString(model.doc_entry));
        }
    }
}