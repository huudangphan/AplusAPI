using System;
using Apzon.PosmanErp.Services.Interfaces;
using Apzon.PosmanErp.Services.Interfaces.Administrator;
using Apzon.PosmanErp.Services.Interfaces.Administrator.Setup;
using Apzon.PosmanErp.Services.Interfaces.Commons;
using Apzon.PosmanErp.Services.Interfaces.Documents;
using Apzon.PosmanErp.Services.Interfaces.Inventory;
using Apzon.PosmanErp.Services.Interfaces.MasterData;
using Apzon.PosmanErp.Services.Interfaces.Sales;
using ICompany = Apzon.PosmanErp.Services.Interfaces.MasterData.ICompany;

namespace Apzon.PosmanErp.Services
{
    public interface IUnitOfWork : IDisposable
    {
        ICommons Commons { get; }

        IUser User { get; }

        ILicenseManager LicenseManager { get; }

        IGeneralSetting GeneralSetting { get; }

        #region BaseMasterData

        IBpGroup BpGroup { get; }

        IItemGroup ItemGroup { get; }

        ICurrency Currency { get; }
        IItemTag ItemTag { get; }

        IShift Shift { get; }

        IUnitOfMeasure UnitOfMeasure { get; }

        ITradeMark TradeMark { get; }

        IVatGroup VatGroup { get; }

        ICompany Company { get; }

        IShippingUnit ShippingUnit { get; }
        IGeographyLocation GeographyLocation { get; }
        ICountry Country { get; }
        IProvince Province { get; }
        IDistrict District { get; }
        IWards Wards { get; }
        IRetailTable RetailTable { get; }
        IWeight Weight { get; }
        ILength Length { get; }

        #endregion

        IItemMasterData Item { get; }
        IWarehouse Warehouse { get; }
        IPriceList PriceList { get; }

        /// <summary>
        /// formsetting repository
        /// </summary>
        IFormSetting FormSetting { get; }

        /// <summary>
        /// Choose from list
        /// </summary>
        IChooseFromList ChooseFromList { get; }

        IUdf Udf { get; }

        IAuth Auth { get; }

        #region Documents

        IQuotation Quotation { get; }

        IPayment Payment { get; }

        #endregion

        IBusinessPartnerMasterData BusinessPartnerMasterData { get; }

        IUnitOfMesureGroup UnitOfMesureGroup { get; }


        IPostingPeriod PostingPeriod { get; }

        IUDTs UDTs { get; }

        IUDOs UDOs { get; }

        IMessageCode MessageCode { get; }

        IDocuments Document { get; }
    }
}