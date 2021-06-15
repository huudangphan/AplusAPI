using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Controllers.Commons
{
    public class UDTController : BaseApiController<DataTable>
    {
        public UDTController(string accessToken) : base(accessToken)
        {
            if (UnitOfWork != null)
            {
                BaseAction = UnitOfWork.UDTs;
            }
            ObjectType = Apzon.PosmanErp.Commons.APlusObjectType.oUDT;
        }

        [HttpPost]
        public override HttpResult Delete([FromBody] SearchDocumentModel deleteinfo)
        {
            try
            {
                return UnitOfWork.UDTs.Delete(ObjectType, ObjectType, deleteinfo.name, GetBranch(), GetUserSign());
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }
    }
}
