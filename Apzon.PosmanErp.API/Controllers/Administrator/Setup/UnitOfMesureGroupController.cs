using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Controllers.Administrator.Setup
{
    
    public class UnitOfMesureGroupController : BaseApiController<DataSet>
    {
        public UnitOfMesureGroupController(string accessToken) : base(accessToken)
        {
            //Khai báo interface và ObjectType để dùng BaseAction
            if (UnitOfWork != null)
            {
                BaseAction = UnitOfWork.UnitOfMesureGroup;
            }
            ObjectType = Apzon.PosmanErp.Commons.APlusObjectType.oUoMGroups;
        }
    }
}
