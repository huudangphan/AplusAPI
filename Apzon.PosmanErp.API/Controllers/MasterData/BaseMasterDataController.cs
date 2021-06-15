using System;
using System.Data;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces;
using Apzon.PosmanErp.Services.Interfaces.MasterData;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;

namespace PosmanErp.Controllers.MasterData
{
    /// <summary>
    /// Operation All base master data with object_type and data
    /// </summary>
    public class BaseMasterDataController : BaseApiController<ModifyMasterDataModel>
    {
        /// <summary>
        /// Default controller
        /// </summary>
        /// <param name="accessToken"></param>
        public BaseMasterDataController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Get master data by object_type and data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Get([FromBody] SearchDocumentModel model)
        {
            return GetMasterDataByObjectType(model.object_type).Get(model);
        }

        /// <summary>
        /// Get master data by id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult GetById([FromBody] SearchDocumentModel model)
        {
            return GetMasterDataByObjectType(model.object_type).GetById(model.code);
        }

        /// <summary>
        /// create master data with object_type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Create([FromBody] ModifyMasterDataModel model)
        {
            return GetMasterDataByObjectType(model.object_type)
                .Create(model.object_type, model.object_type, model.data, GetBranch(), GetUserSign());
        }

        /// <summary>
        /// update master data with object_type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Update([FromBody] ModifyMasterDataModel model)
        {
            return GetMasterDataByObjectType(model.object_type).Update(model.object_type, model.object_type, model.data,
                GetBranch(), GetUserSign());
        }

        /// <summary>
        /// delete master data with object_type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override HttpResult Delete([FromBody] SearchDocumentModel model)
        {
            return GetMasterDataByObjectType(model.object_type).Delete(model.object_type, model.object_type, model.code,
                GetBranch(), GetUserSign());
        }


        /// <summary>
        /// Return base master data
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private IBaseAction<DataTable> GetMasterDataByObjectType(APlusObjectType objectType)
        {
            switch (objectType)
            {
                case APlusObjectType.oBpGroup:
                    return UnitOfWork.BpGroup;
                case APlusObjectType.oItemGroups:
                    return UnitOfWork.ItemGroup;
                case APlusObjectType.oCurrency:
                    return UnitOfWork.Currency;
                case APlusObjectType.oItemTag:
                    return UnitOfWork.ItemTag;
                case APlusObjectType.oShift:
                    return UnitOfWork.Shift;
                case APlusObjectType.oUoM:
                    return UnitOfWork.UnitOfMeasure;
                case APlusObjectType.oVatGroup:
                    return UnitOfWork.VatGroup;
                case APlusObjectType.oTrademark:
                    return UnitOfWork.TradeMark;
                case APlusObjectType.oCompany:
                    return UnitOfWork.Company;
                case APlusObjectType.oShippingUnit:
                    return UnitOfWork.ShippingUnit;
                case APlusObjectType.oGeographyLocation:
                    return UnitOfWork.GeographyLocation;
                case APlusObjectType.oPostingPeriod:
                    return UnitOfWork.PostingPeriod;
                case APlusObjectType.oCountry:
                    return UnitOfWork.Country;
                case APlusObjectType.oProvince:
                    return UnitOfWork.Province;
                case APlusObjectType.oDistrict:
                    return UnitOfWork.District;
                case APlusObjectType.oWards:
                    return UnitOfWork.Wards;
                case APlusObjectType.oRetailTable:
                    return UnitOfWork.RetailTable;
                case APlusObjectType.oWeight:
                    return UnitOfWork.Weight;
                case APlusObjectType.oLength:
                    return UnitOfWork.Length;
                default:
                    return null;
            }
        }
    }

    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 
    /// </summary>
    public class ModifyMasterDataModel
    {
        /// <summary>
        /// object_type to specify the master data
        /// </summary>
        public APlusObjectType object_type { get; set; }

        /// <summary>
        /// data of this master data
        /// </summary>
        public DataTable data { get; set; }
    }
}