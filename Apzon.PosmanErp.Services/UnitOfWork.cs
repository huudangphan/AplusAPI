using System;
using System.Data;
using Apzon.Libraries.HDBConnection;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Services.Interfaces;
using Apzon.PosmanErp.Services.Interfaces.Administrator;
using Apzon.PosmanErp.Services.Interfaces.Administrator.Setup;
using Apzon.PosmanErp.Services.Interfaces.Commons;
using Apzon.PosmanErp.Services.Interfaces.Documents;
using Apzon.PosmanErp.Services.Interfaces.Inventory;
using Apzon.PosmanErp.Services.Interfaces.MasterData;
using Apzon.PosmanErp.Services.Interfaces.Sales;
using Apzon.PosmanErp.Services.Repository;
using Apzon.PosmanErp.Services.Repository.Administrator;
using Apzon.PosmanErp.Services.Repository.Administrator.Setup;
using Apzon.PosmanErp.Services.Repository.Commons;
using Apzon.PosmanErp.Services.Repository.Documents;
using Apzon.PosmanErp.Services.Repository.Inventory;
using Apzon.PosmanErp.Services.Repository.MasterData;
using Apzon.PosmanErp.Services.Repository.Sales;
using CompanyRepository = Apzon.PosmanErp.Services.Repository.MasterData.CompanyRepository;
using ICompany = Apzon.PosmanErp.Services.Interfaces.MasterData.ICompany;

namespace Apzon.PosmanErp.Services
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        #region Initialize

        private string _connectionString;
        private IDatabaseClient _databaseClient;
        private string _schemaName;
        private DatabaseSystemType _databaseType;

        private bool _disposed;

        public UnitOfWork(string connectionString, string schemaName, DatabaseSystemType databaseSystemType)
        {
            _connectionString = connectionString;
            _schemaName = schemaName;
            _databaseType = databaseSystemType;
            if (databaseSystemType == DatabaseSystemType.SQL)
            {
                _databaseClient = new DatabaseSqlClient(_connectionString);
            }
            else
            {
                _databaseClient = new DatabasePostgresqlClient(connectionString, schemaName);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // new DatabaseServiceSql(_connectionString).TryDispose();
                }
            }

            _disposed = true;
        }

        #endregion


        public ICommons Commons
        {
            get { return new CommonsRepository(_databaseClient); }
        }

        public IUser User
        {
            get { return new UserRepository(_databaseClient); }
        }

        public ILicenseManager LicenseManager
        {
            get { return new LicenseManagerRepository(_databaseClient); }
        }

        public IGeneralSetting GeneralSetting
        {
            get { return new GeneralSettingRepository(_databaseClient); }
        }

        #region BaseMasterData

        public IBpGroup BpGroup => new BpGroupRepository(_databaseClient, "partner_group");
        public IItemGroup ItemGroup => new ItemGroupRepository(_databaseClient, "item_group");
        public ICurrency Currency => new CurrencyRepository(_databaseClient, "currency");
        public IItemTag ItemTag => new ItemTagRepository(_databaseClient, "item_tag");
        public IShift Shift => new ShiftRepository(_databaseClient, "shift");
        public IUnitOfMeasure UnitOfMeasure => new UnitOfMeasureRepository(_databaseClient, "unit");
        public ITradeMark TradeMark => new TradeMarkRepository(_databaseClient, "trademark");
        public IVatGroup VatGroup => new VatGroupRepository(_databaseClient, "tax_group");
        public ICompany Company => new CompanyRepository(_databaseClient, "company");
        public IShippingUnit ShippingUnit => new ShippingUnitRepository(_databaseClient, "shipping_unit");
        public IGeographyLocation GeographyLocation => new GeographyLocationRepository(_databaseClient, "geography");

        public ICountry Country => new CountryRepository(_databaseClient, "geography");
        public IProvince Province => new ProvinceRepository(_databaseClient, "geography");
        public IDistrict District => new DistrictRepository(_databaseClient, "geography");
        public IWards Wards => new WardsRepository(_databaseClient, "geography");
        public IRetailTable RetailTable => new RetailTableRepository(_databaseClient, "table_info");
        public ILength Length => new LengthRepository(_databaseClient, "length");
        public IWeight Weight => new WeightRepository(_databaseClient, "width");
        
        public IPostingPeriod PostingPeriod
        {
            get { return new PostingPeriodRepository(_databaseClient, "financial_period"); }
        }

        #endregion

        public IFormSetting FormSetting => new FormSettingRepository(_databaseClient);
        public IChooseFromList ChooseFromList => new ChooseFromListRepository(_databaseClient);
        public IUdf Udf => new UdfRepository(_databaseClient);


        public IItemMasterData Item => new ItemMasterDataRepository(_databaseClient);

        public IWarehouse Warehouse => new WarehouseRepository(_databaseClient);
        public IPriceList PriceList => new PriceListRepository(_databaseClient);

        public IAuth Auth
        {
            get { return new AuthRepository(_databaseClient); }
        }

        #region Documents

        public IQuotation Quotation => new QuotationRepository(_databaseClient);

        public IPayment Payment => new PaymentRepository(_databaseClient);

        #endregion

        public IBusinessPartnerMasterData BusinessPartnerMasterData
        {
            get { return new BusinessPartnerMasterDataRepository(_databaseClient); }
        }

        public IUnitOfMesureGroup UnitOfMesureGroup
        {
            get { return new UnitOfMesureGroupRepository(_databaseClient); }
        }

        

        public IUDTs UDTs
        {
            get { return new UDTRepository(_databaseClient); }
        }

        public IUDOs UDOs
        {
            get { return new UDORepository(_databaseClient); }
        }

        public IMessageCode MessageCode => new MessageCodeRepository(_databaseClient);

        public IDocuments Document => new DocumentsRepository(_databaseClient);
    }
}