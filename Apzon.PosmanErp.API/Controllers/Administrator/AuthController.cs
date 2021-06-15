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

namespace PosmanErp.Controllers.Administrator
{
    /// <summary>
    /// controller xử lý phân quyền người dùng
    /// </summary>
    public class AuthController : BaseApiController<DataTable>
    {
        /// <summary>
        /// hàm khởi tạo
        /// </summary>
        /// <param name="accessToken"></param>
        public AuthController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// API lấy dữ liệu phân quyền của một user
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult Get(int? user_id)
        {
            return UnitOfWork.Auth.Get(user_id);
        }

        /// <summary>
        /// hamf update dữ liệu phân quyền của người dùng
        /// </summary>
        /// <param name="dtUserAuth"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update([FromBody]DataTable dtUserAuth)
        {
            return UnitOfWork.Auth.Process(APlusObjectType.oAuth, APlusObjectType.oAuth, GetBranch(), TransactionType.U, dtUserAuth, GetUserSign());
        }

        /// <summary>
        /// hàm update dữ liệu menu trên hệ thống
        /// </summary>
        /// <param name="dtMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult UpdateMenuItem([FromBody] DataTable dtMenu)
        {
            return UnitOfWork.Auth.Process(APlusObjectType.oMenu, APlusObjectType.oMenu, GetBranch(), TransactionType.M, dtMenu, GetUserSign());
        }
    }
}
