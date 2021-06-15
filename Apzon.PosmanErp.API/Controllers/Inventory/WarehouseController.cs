using System.Data;
using System.Diagnostics;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;

namespace PosmanErp.Controllers.Inventory
{
    /// <summary>
    /// Warehouse Api
    /// </summary>
    public class WarehouseController : BaseApiController<DataTable>
    {
        /// <summary>
        /// Default controller
        /// </summary>
        /// <param name="accessToken"></param>
        public WarehouseController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Get warehouse by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public override HttpResult GetById(string id)
        {
            return UnitOfWork.Warehouse.GetById(id);
        }


        /// <summary>
        /// Get all warehouse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Get(SearchDocumentModel model)
        {
            return UnitOfWork.Warehouse.Get(model);
        }

        /// <summary>
        /// Create new warehouse
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Create(DataTable document)
        {
            return UnitOfWork.Warehouse.Process(APlusObjectType.oWarehouses, APlusObjectType.oWarehouses, GetBranch(),
                TransactionType.A, document, GetUserSign());
        }

        /// <summary>
        /// Update warehouse
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update(DataTable document)
        {
            return UnitOfWork.Warehouse.Process(APlusObjectType.oWarehouses, APlusObjectType.oWarehouses, GetBranch(),
                TransactionType.U, document, GetUserSign());
        }

        /// <summary>
        /// Delete warehouse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Delete(SearchDocumentModel model)
        {
            return UnitOfWork.Warehouse.Process(APlusObjectType.oWarehouses, APlusObjectType.oWarehouses, GetBranch(),
                TransactionType.D, null, GetUserSign(), model.code);
        }
    }
}