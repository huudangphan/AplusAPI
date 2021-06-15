using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;

namespace PosmanErp.Controllers.Sales
{
    public class QuotationsController : BaseApiController<DataSet>
    {
        public QuotationsController(string accessToken) : base(accessToken)
        {
            if (UnitOfWork != null)
            {
                BaseAction = (IBaseAction<DataSet>) UnitOfWork.Quotation;
            }

            ObjectType = APlusObjectType.oQuotations;
        }
    }
}
