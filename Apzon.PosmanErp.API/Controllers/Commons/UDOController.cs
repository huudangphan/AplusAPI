using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Controllers.Commons
{
    public class UDOController : BaseApiController<DataSet>
    {
        public UDOController(string accessToken) : base(accessToken)
        {
            if (UnitOfWork != null)
                BaseAction = UnitOfWork.UDOs;
            ObjectType = Apzon.PosmanErp.Commons.APlusObjectType.oUDO;
        }

        [HttpGet]
        public HttpResult GetUdoTable(int type)
        {
            return UnitOfWork.UDOs.GetUdoTable(type);
        }
    }
}
