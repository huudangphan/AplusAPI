using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;

namespace PosmanErp.Controllers.MasterData
{
    /// <inheritdoc />
    public class ItemMasterDataController : BaseApiController<DataSet>
    {
        /// <inheritdoc />
        public ItemMasterDataController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Get List Grand item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Get([FromBody] SearchDocumentModel item)
        {
            return UnitOfWork.Item.Get(item);
        }

        /// <summary>
        /// Get List Inherit item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult GetDocumentItems([FromBody] SearchDocumentModel item)
        {
            return UnitOfWork.Item.GetDocumentItems(item.page_index, item.page_size, item.tran_type, item.price_list,
                item.whs_code);
        }

        /// <summary>
        /// Get detail grand item by id and it's inheritances
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public override HttpResult GetById(string id)
        {
            return UnitOfWork.Item.GetById(id);
        }

        /// <summary>
        /// Get detail inherit item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult GetDocumentById(string id)
        {
            return UnitOfWork.Item.GetDocumentById(id);
        }

        /// <summary>
        /// Create grand item include inherit item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Create([FromBody] DataSet item)
        {
            return UnitOfWork.Item.Process(APlusObjectType.oItemComponent, APlusObjectType.oItemComponent, GetBranch(),
                TransactionType.A, item, GetUserSign());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update([FromBody] DataSet item)
        {
            return UnitOfWork.Item.Process(APlusObjectType.oItemComponent, APlusObjectType.oItemComponent, GetBranch(),
                TransactionType.U, item, GetUserSign());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Delete([FromBody] SearchDocumentModel item)
        {
            return UnitOfWork.Item.Process(APlusObjectType.oItemComponent, APlusObjectType.oItemComponent, GetBranch(),
                TransactionType.D, null, GetUserSign(), item.code);
        }

        /// <summary>
        /// Create Inherit item only
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult CreateDocumentItem([FromBody] DataSet item)
        {
            return UnitOfWork.Item.Process(APlusObjectType.oItems, APlusObjectType.oItems, GetBranch(),
                TransactionType.A, item, GetUserSign());
        }
        /// <summary>
        /// Update inherit item only
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult UpdateDocumentItem([FromBody] DataSet item)
        {
            return UnitOfWork.Item.Process(APlusObjectType.oItems, APlusObjectType.oItems, GetBranch(),
                TransactionType.U, item, GetUserSign());
        }
        /// <summary>
        /// Delete inherit item only
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult DeleteDocumentItem([FromBody] SearchDocumentModel item)
        {
            return UnitOfWork.Item.Process(APlusObjectType.oItems, APlusObjectType.oItems, GetBranch(),
                TransactionType.D, null, GetUserSign(), item.code);
        }
    }
}