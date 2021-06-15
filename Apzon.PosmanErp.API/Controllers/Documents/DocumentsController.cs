using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Controllers.Documents
{
    /// <summary>
    /// document controller
    /// </summary>
    public class DocumentsController : BaseApiController<DocumentModifiedDataModel>
    {
        /// <summary>
        /// hàm khởi tạo
        /// </summary>
        /// <param name="accessToken"></param>
        public DocumentsController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// lấy dữ liệu danh sách document trên hệ thống
        /// </summary>
        /// <param name="searchinfo"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Get([FromBody] SearchDocumentModel searchinfo)
        {
            return UnitOfWork.Document.Get(searchinfo);
        }

        /// <summary>
        /// lấy chi tiết lines của document
        /// </summary>
        /// <param name="obj_type"></param>
        /// <param name="doc_entry"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult GetLinesByEntry(APlusObjectType obj_type,int doc_entry)
        {
            return UnitOfWork.Document.GetDocumentLineByEntry(obj_type, doc_entry);
        }

        /// <summary>
        /// api tạo mới document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Create([FromBody] DocumentModifiedDataModel document)
        {
            return UnitOfWork.Document.Process(document.object_type, document.object_type, -1, TransactionType.A, document.document, GetUserSign());
        }

        /// <summary>
        /// api update dữ liệu document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update([FromBody] DocumentModifiedDataModel document)
        {
            return UnitOfWork.Document.Process(document.object_type, document.object_type, -1, TransactionType.U, document.document, GetUserSign());
        }

        /// <summary>
        /// hàm cancel document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult Cancel([FromBody] SearchDocumentModel document)
        {
            return UnitOfWork.Document.Process(document.object_type, document.object_type, -1, TransactionType.C, null, GetUserSign(), Function.ToString(document.doc_entry));
        }

        /// <summary>
        /// hàm close document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult Close([FromBody] SearchDocumentModel document)
        {
            return UnitOfWork.Document.Process(document.object_type, document.object_type, -1, TransactionType.L, null, GetUserSign(), Function.ToString(document.doc_entry));
        }
    }

    /// <summary>
    /// document modified object
    /// </summary>
    public class DocumentModifiedDataModel
    {
        /// <summary>
        /// object type
        /// </summary>
        public APlusObjectType object_type { get; set; }

        /// <summary>
        /// object data
        /// </summary>
        public DataSet document { get; set; }
    }
}
