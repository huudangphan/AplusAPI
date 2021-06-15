using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Mvc;

namespace PosmanErp.Controllers.Commons
{
    /// <summary>
    /// 
    /// </summary>
    public class UdfController : BaseApiController<DataSet>
    {
        /// <summary>
        /// Default user define field controller
        /// </summary>
        /// <param name="accessToken"></param>
        public UdfController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Get udf by conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Get([FromBody] SearchDocumentModel model)
        {
            return UnitOfWork.Udf.Get(model);
        }

        /// <summary>
        /// Create udf
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Create([FromBody] DataSet document)
        {
            return UnitOfWork.Udf.Process(APlusObjectType.oUDF, APlusObjectType.oUDF, GetBranch(), TransactionType.A,
                document, GetUserSign());
        }

        /// <summary>
        /// Create udf
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update([FromBody] DataSet document)
        {
            return UnitOfWork.Udf.Process(APlusObjectType.oUDF, APlusObjectType.oUDF, GetBranch(), TransactionType.U,
                document, GetUserSign());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Delete(SearchDocumentModel model)
        {
            return UnitOfWork.Udf.Delete(APlusObjectType.oUDF, APlusObjectType.oUDF, GetBranch(), GetUserSign(), model);
        }
    }
}