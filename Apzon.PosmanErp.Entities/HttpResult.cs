using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons.Attributes;

namespace Apzon.PosmanErp.Entities
{
    /// <summary>
    /// Class định nghĩa cấu trúc dữ liệu trả về cho các request
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// Mã mesage
        /// </summary>
        public MessageCode msg_code { get; set; }

        /// <summary>
        /// Kết quả trả về
        /// </summary>
        public object content { get; set; }

        /// <summary>
        /// Message trả về
        /// </summary>
        public string message { get; set; }

        public HttpResult()
        {
        }

        /// <summary>
        /// hàm khởi tạo với message code, vả content
        /// </summary>
        /// <param name="messageCode"></param>
        /// <param name="obj"></param>
        public HttpResult(MessageCode messageCode, object obj)
        {
            msg_code = messageCode;
            content = obj;
        }

        public HttpResult(MessageCode messageCode)
        {
            msg_code = messageCode;
        }
        public enum TypeGetVersion
        {
            First,
            Mid,
            Last
        }
        public enum TypeUpdate
        {
            Table,
            View,
            Type,
            Function,
            Sto,
            Data
        }

        /// <summary>
        /// khởi tạo với message lỗi và mã lôi
        /// </summary>
        /// <param name="messageCode"></param>
        /// <param name="mess"></param>
        public HttpResult(MessageCode messageCode, string mess)
        {
            msg_code = messageCode;
            message = mess;
        }

        /// <summary>
        /// hàm khởi tạo với mã lỗi, nội dung lỗi và content trả về
        /// </summary>
        /// <param name="messageCode"></param>
        /// <param name="mess"></param>
        /// <param name="obj"></param>
        public HttpResult(MessageCode messageCode, string mess, object obj)
        {
            msg_code = messageCode;
            message = mess;
            content = obj;
        }
    }

    public enum MessageCode
    {
        #region System Code
        [Description("None")] 
        None = 0,
        
        [Description("Success")] 
        Success = 200,

        [Description("Error")] 
        Error = 2,

        [Description("Exception")]
        Exception = 3,

        [Description("Token has been expired")]
        TokenExpired = 9,

        [Description("Invalid Token")] 
        TokenInvalid = 10,

        [Description("Function not supported")]
        FunctionNotSupport = 13,

        #region tập lỗi liên quan database

        [Description("Unable access to database")]
        UnableAccessDatabase = 20,
        
        [Description("MessageCode in database not consistence with defined enums")]
        EnumDataInConsistence = 21,
        
        [Description("object_type not exists in system")]
        SystemObjectTypeNotExists = 22,
        
        [Description("MessageCode Enums Duplicated. Please contact administrator")]
        DuplicateMessageCode = 23,

        #endregion

        #endregion


        #region General Error

        [Description("Data insert not correct")]
        DataNotCorrect = 10000,
        [Description("Data is not found")]
        DataNotFound = 10001,
        [Description("Data is existed")]
        DataExisted = 10002,
        [Description("Record not found")]
        RecordNotFound = 10003,
        [Description("Data not provide")]
        DataNotProvide = 10004,
        [Description("Code not provided")]
        CodeNotProvided = 10005,
        [Description("Language not provide")]
        LanguageNotProvide = 10007,
        
        #endregion
        
        #region UDF
        [Description("Linked data not provide")]
        LinkedNotProvide = 10008,
        [Description("column_id not provide")]
        ColumnIdNotProvide = 10009,
        [Description("column_id not exists")]
        ColumnIdNotExists = 10010,
        [Description("column_id has been existed")]
        ColumnIdExisted = 10011,
        [Description("UDF existed")]
        UdfExisted = 10012,
        [Description("UDF not exists")]
        UdfNotExists = 10013,
        [Description("Linked Table not provide")]
        LinkedTableNotProvide = 10014,
        [Description("Lookup key existed")]
        LookupKeyExisted = 10015,
        [Description("Lookup key not exists")]
        LookupKeyNotExists = 10016,
        [Description("UDF in use on form setting. Could not operate")]
        UdfInUseByFormSetting = 10017,
        [Description("Field already exists on database table. Could not insert")]
        FieldExistsInTable = 10018,
        [Description("Field not exits on database table. Could not alter")]
        FieldNotExistsInTable = 10019,
        [Description("Could not modify data type")]
        CouldNotModifyColumnDataType = 10020,
        [Description("Column size must be greater than 0")]
        ColumnSizeMustGreaterThanZero = 10021,
        [Description("Table name not provide")]
        TableNameNotProvide = 10022,
      
            
        #endregion

        #region tập lỗi liên quan tới item master data

        [Description("Data on transaction, could not modify")]
        DataOnTransaction = 10023,
        [Description("Item already in used by other table")]
        RecordInUseInOtherTable = 10024,
        [Description("Item group not existed")]
        ItemGroupNotExists = 10025,
        [Description("Item trademark not existed")]
        ItemTradeMarkNotExists = 10026,
        [Description("Item property not provide")]
        ItemPropertyNotProvide = 10027,
        [Description("Item Property not correct")]
        ItemPropertyNotCorrect = 10028,
        [Description("Item Quantity must greater than 0")]
        QuantityMustGreaterThanZero = 10029,
        [Description("Unit of measure not existed")]
        MeasureUnitNotExists = 10030,
        [Description("Item on transaction, could not update")]
        NotUpdateOnTransaction = 10031,
        [Description("Could not update item property")]
        PropertyNotUpdate = 10032,
        [Description("Properties is already used by item components. Could not modify")]
        PropertiesUsedByComponents = 10034,
        [Description("Item property is existed")]
        PropertyExisted = 10035, 
        [Description("Item code is not found")]
        ItemCodeNotFound = 10036,
        
        #endregion
        
        #region Base Master data constraints
        
        [Description("Business partner group used in Business Partner. Could not delete")]
        BpGrpUsedInBpNoDelete = 10037, 
        [Description("Company cannot delete!")]
        CompanyNoDelete = 10038,
        [Description("Currency used in Business Partner. Could not delete")]
        CurrencyUsedByBpNoDelete = 10039,
        [Description("Currency used. Could not delete")]
        CurrencyUsedNoDelete = 10040,
        [Description("Currency used. Could not delete")]
        CurrencyUsedByItemPriceNoDelete = 10041,
        [Description("Item group used by item. Could not delete")]
        ItmGrpUsedByItemNoDelete = 10042,
        [Description("Item tag used by item. Could not delete")]
        ItmTagUsedByItemNoDelete = 10043,
        [Description("Trademark used by item. Could not delete")]
        TrmUsedByItemNoDelete = 10044,
        [Description("Unit of measure used by ite. Could not delete")]
        UomsUsedByItemNoDelete = 10045,
        [Description("Vat group used in business partner. Could not delete")]
        VatGrpUsedByBpNoDelete = 10046,
        [Description("Vat group used in item. Could not delete")]
        VatGrpUsedByItemNoDelete = 10047,
        [Description("Weight used in item. Could not delete")]
        WeightUsedByItemNoDelete = 10048,
        [Description("Length is used in item. Could not delete")]
        LengthUsedByItemNoDelete = 10049,
        [Description("Duplicated code found. Please try again")]
        CodeDuplicated = 10052,
        
        #endregion

        
        #region Choose from list code

        [Description("object_type not provide")]
        ObjectTypeNotProvide = 10053,
        [Description("Form Id not provide")]
        FormIdNotProvide = 10054,
        [Description("Item Id not provide")]
        ItemIdNotProvide = 10055,
        [Description("Conflict form setting")]
        FormSettingConflict = 10056,

        #endregion

        #region Warehouse  
        
        [Description("Warehouse code already existed. Please choose some difference one!")]
        WarehouseExisted = 10057,
        [Description("Country not existed")]
        CountryNotExisted = 10058,
        [Description("City not existed")]
        CityNotExisted = 10059,
        [Description("District not existed")]
        DistrictNotExisted = 10060,
        [Description("Ward not existed")]
        WardNotExisted = 10061,
        [Description("Warehouse is linked to transaction, Cannot update warehouse status")]
        WarehouseOnTransactionNoUpdateStatus = 10062, 
        [Description("Warehouse is linked to transaction, Cannot delete")]
        WarehouseOnTransactionNoDelete = 10063,

        #endregion
        
        // max = 10062
        
        #region danh sách lỗi general setting

        [Description("Sepa is not valid")]
        SepaNotValid = 10200,
        [Description("Cannot setting currency")]
        CannotSettingCurrency = 10201,
        [Description("Cannot setting decimal")]
        CannotSettingDecimal = 10202,

        #endregion
        
        [Description("User Lock")]
        UserLock = 10100,
        [Description("User not correct")]
        UserNotCorrect = 10101,
        [Description("Function not correct")]
        FunctionNotCorrect = 10102,
        [Description("Currency not correct")]
        CurrencyNotValid = 10103,
        [Description("Invalid decimal number")]
        NumberDecimalNotValid = 10104,
        [Description("Invalid date format")]
        DateFormatNotValid = 10105,
        [Description("Invalid time format")]
        TimeFormatNotValid = 10106,
        [Description("Customer not existed")]
        CustomerNotExists = 10107,
        [Description("Invalid Value")]
        NotValidValue = 10108,

        #region User
        [Description("User can not be found")] 
        UserNotFound = 10109,
        [Description("Email has been exists.")]
        EmailExists = 10110,
        [Description("Phone has been exists.")]
        PhoneExists = 10111,
        [Description("User already exists")] 
        UserExists = 10112,
        [Description("Old Password user is not correct")]
        OldPasswordNotCorrect = 10113,
        [Description("Old password and new password must not coincide")]
        OldPasswordAndNewPasswordMustNotCoincide = 10114,
        [Description("User is inactive")] 
        UserInactived = 10115,
        [Description("User Info is required")]
        DataUserIsEmpty = 10116,
        [Description("Email wrong format")] 
        EmailWrongFormat = 10117,
        [Description("Phone wrong format")] 
        PhoneWrongFormat = 10118,
        [Description("Data company is required")]
        DataCompanyIsEmpty = 10119,
        [Description("Data default system is required")]
        DataDefaultSystemIsEmpty = 10120,
        [Description("Data user fron-ent config is required")]
        DataUserConfigIsEmpty = 10121,
        [Description("Data user fron-ent config not found")]
        DataUserConfigNotFound = 10122,
        [Description("Status user not correct")]
        StatusUserNotCorrect = 10123,
        [Description("Company not found")]
        CompanyNotFound = 10124,
        [Description("Warehouse not found")]
        WarehouseNotFound = 10125,
        #endregion

        #region Unit Of Mesure Group
        [Description("Unit Of Mesure group data is empty")]
        UnitOfMesureGroupIsEmpty = 10130,
        [Description("Unit Of Mesure group not found")]
        UnitOfMesureGroupNotFound = 10131,
        [Description("Unit Of Mesure group has been used by Item")]
        UnitOfMesureGroupUsedByItem = 10132,
        [Description("Unit Of Mesure has been used by Item")]
        UnitOfMesureUsedByItem = 10133,
        [Description("Unit Of Mesure not found")]
        UnitOfMesureNotFound = 10134,
        #endregion

        #region Business Partner Master Data
        [Description("Business partner code is empty")]
        CardCodeIsEmpty = 10140,
        [Description("Business partner type not provide or not correct with old values")]
        CardTypeIsEmptyOrNotCorrect = 10141,
        [Description("Address type must be in ('S','B','L')")]
        CardTypeWrongFormat = 10142,
        [Description("Group business partner not found")]
        GroupBusinessPartnerNotFound = 10143,
        [Description("Price list not found")]
        PriceListNotFound = 10144,
        [Description("VAT group not found")]
        VATGroupNotFound = 10145,
        [Description("Address data not found")]
        AddressInvalid = 10146,
        [Description("Address type must be in ('S','B')")]
        AddressTypeWrongFormat = 10147,
        [Description("Business partner not found")]
        BusinessPartnerNotFound = 10148,
        [Description("Business partner has been used in another table")]
        BusinessPartnerUsedInAnotherTable = 10149,
        [Description("Business partner must have one default address")]
        BusinessPartnerMustHaveOneDefaultAddress = 10150,
        #endregion

        #region User Define Table
        [Description("Table name is empty")]
        TableNameIsEmpty = 10160,
        [Description("Table type is empty")]
        TableTypeIsEmpty = 10161,
        [Description("Table type must be in {0,1,2,3,4}")]
        TableTypeWrongFormat = 10162,
        [Description("Table define not found")]
        TableNotFound = 10163,
        [Description("Table already exists")]
        TableExists = 10164,
        [Description("Table define has been used in another table")]
        TableUsedInAnotherTable = 10165,
        #endregion

        #region User Define Object
        [Description("Object code is empty")]
        ObjectCodeIsEmpty = 10170,
        [Description("Object header table is empty")]
        ObjectTableHeaderIsEmpty = 10171,
        [Description("Object data is empty")]
        ObjectTypeIsEmpty = 10172,
        [Description("Type udo is empty")]
        TypeIsEmpty = 10173,
        [Description("Type must be in {'Document','MasterData'}")]
        TypeWrongFormat = 10174,
        [Description("Object code in child table is empty")]
        ObjectCodeChildIsEmpty = 10175,
        [Description("Object code in default find is empty")]
        ObjectCodeDefaultFindIsEmpty = 10176,
        [Description("Object code in header view is empty")]
        ObjectCodeHeaderViewIsEmpty = 10177,
        [Description("Column not exists in table")]
        ColumnNotExists = 10178,
        [Description("Object code in child view is empty")]
        ObjectCodeChildViewIsEmpty = 10179,
        [Description("Table in child view empty or not correct with object child")]
        TableInChildIsEmptyOrNotCorrect = 10180,
        [Description("Object already exists")]
        ObjectExists = 10181,
        [Description("Object not found")]
        ObjectNotFound = 10182,
        [Description("Header Table has been used in another object")]
        HeaderTableUsedInAnotherObject = 10183,
        [Description("Object_type has been used in another object")]
        ObjectTypeUseInAnotherObject = 10184,
        [Description("Object has been used in another table")]
        ObjectUsedInAnotherTable = 10185,
        #endregion


        [Description("Line number not correct")]
        LineNumNotCorrect = 10300,
        [Description("Data BOM sell error")]
        DataBomSellError = 10301,
        [Description("Table data not existed")]
        TableDataNotExists = 10302,
        [Description("Price list not exits")]
        PriceListNotExists = 10303,
        [Description("Cannot get entry")]
        CannotGetEntry = 10304,
        [Description("Closed Document cannot be canceled")]
        DocumentClosedCannotCancel = 10305,
        

        #region lỗi license

        [Description("License not granted")]
        LicenseNotGranted = 11000,
        [Description("Invalid License")]
        LicenseInvalid = 11001,
        [Description("License not found")]
        LicenseNotFound = 11002,
        [Description("Invalid License type")]
        LicenseTypeInvalid = 11003,
        [Description("License expired")]
        LicenseExpired = 11004,
        [Description("Invalid system info")]
        SystemInfoInvalid = 11005,

        #endregion

        //thanhnv
        [Description("Closed Document cannot be updated")]
        DocumentClosedCannotUpdate = 10306,
        [Description("Cannot read base data")]
        CannotReadBaseData = 10307,
        [Description("Cannot update Business Partner")]
        CannotUpdateBP = 10308,
        [Description("Cannot change company of document")]
        CannotChangeCompany = 10309,
        [Description("Cannot change VAT of document")]
        CannotChangeVat = 10310,
        [Description("Cannot change VAT Percent of document")]
        CannotChangeVatPercent = 10311,
        [Description("Cannot change Discount Percent of document")]
        CannotChangeDiscountPercent = 10312,
        [Description("Cannot change items of document lines")]
        CannotUpdateDocumentItem = 10313,
        [Description("Cannot change linked data of document lines")]
        CannotUpdateLinkedData = 10314,
        [Description("Cannot change item quantity of document lines")]
        CannotUpdateItemQuantity = 10315,
        [Description("Cannot change Posting Date of document")]
        CannotChangePostingDateDocument = 10316,
        [Description("Cannot change Due Date of document")]
        CannotChangeDueDateDocument = 10317,
        [Description("Cannot change shipping Date of document lines")]
        CannotChangeShippingDateDocumentLines = 10318,
        [Description("Cannot change currency of document lines")]
        CannotChangeDocumentCurrency = 10319,
        [Description("Cannot change document rate")]
        CannotChangeDocumentRate = 10320,
        [Description("Cannot change price of document lines")]
        CannotChangePriceDocumentLines = 10321,
        [Description("Cannot change currency of document lines")]
        CannotChangeCurrencyDocumentLines = 10322,
        [Description("Cannot change rate of document lines")]
        CannotChangeRateDocumentLines = 10323,
        [Description("Cannot change discount percent of document lines")]
        CannotChangeDiscountPercentDocumentLines = 10324,
        [Description("Cannot change warehouse of document lines")]
        CannotChangeWarehouseDocumentLines = 10325,
        [Description("Cannot change Tree Type of document lines")]
        CannotChangeTreeTypeDocumentLines = 10326,
        [Description("Cannot change Vat Group of document lines")]
        CannotChangeVatGroupDocumentLines = 10327,
        [Description("Cannot change Vat Percent of document lines")]
        CannotChangeVatPercentDocumentLines = 10328,
        [Description("Cannot change item unit of document lines")]
        CannotChangeUnitDocumentLines = 10329,
        [Description("Cannot change unit convert rate of document lines")]
        CannotChangeConvertRateDocumentLines = 10330,
        [Description("Linked document does not exists")]
        LinkedBaseNotExists = 10331,
        [Description("Linked item cannot be changed")]
        LinkedItemCannotBeChange = 10332,
        [Description("Linked warehouse cannot be changed")]
        LinkedWarehouseCannotBeChange = 10333,
        [Description("Linked unit cannot be changed")]
        LinkedUnitCannotBeChange = 10334,
        [Description("Copy quantity cannot greater than base quantity")]
        CopyQtyCannotGreaterThanBaseQty = 10335,
        [Description("Quantity of item is not enough")]
        QuantityItemNotEnough = 10336,
        [Description("Cannot change status of closed line")]
        CannotChangeStatusClosedLine = 10337,
        [Description("Document company is not correct")]
        CompanyDocumentNotCorrect = 10338,
        [Description("Please choose a active business partner")]
        ChooseActiveBusinessPartner = 10339,
        [Description("Item does not exists")]
        ItemDoesNotExists = 10340,
        [Description("Item is not a inventory item")]
        ItemIsNotInventoryItem = 10341,
        [Description("Item is not a sale item")]
        ItemIsNotSaleItem = 10342,
        [Description("Item is not a purchase item")]
        ItemIsNotPurchaseItem = 10343,
        [Description("Warehouse does not exists")]
        WarehouseDoesNotExists = 10344,
        [Description("Item does not used in warehouse")]
        ItemNotUseInWarehouse = 10345,
        [Description("Cannot transfer item in the same warehouse")]
        CannotTransferItemInSameWhs = 10346,
        [Description("Due date cannot be empty")]
        DueDateCannotBeEmpty = 10347,
        [Description("Unit of item is not correct")]
        ItemUnitNotCorrect = 10348,
        [Description("Tree type is not correct")]
        TreeTypeNotCorrect = 10349,
        [Description("Father num is not correct")]
        FatherNumNotCorrect = 10350,
        [Description("Combo data is not correct")]
        ComboDataNotCorrect = 10351,
        [Description("Data menu list is not correct")]
        DataMenuListNotCorrect = 10352,
        [Description("Data authorization menu is not correct")]
        DataAuthorizationMenuNotCorrect = 10353,
        [Description("Data setting is not enough")]
        DataSettingNotEnough = 10354,
        [Description("Data setting is overlap")]
        DataSettingOvelap = 10355,
        [Description("Warehouse is negative. Cannot change setting negative warehouse")]
        WarehouseIsNegativeCannotChangeSetting = 10356,
        [Description("Closed Document cannot be closeed")]
        DocumentClosedCannotClose = 10357,
        [Description("Document does not exists")]
        DocumentDoesNotExit = 10358,
        [Description("Suplier not existed")]
        SuplierNotExists = 10359,
        [Description("Cannot add or delete line of inventory effect document")]
        ChangeLineInventoryEffect = 10360,
        [Description("Visible order is not correct")]
        VisOrderNotCorrect = 10361,
        [Description("License type is not corret")]
        LicenseTypeNotCorrect = 10362,
        [Description("Used license is greater than license quantity")]
        LicenseUsedGreaterThanLicenseQty = 10363,
        [Description("Server date is invalid")]
        ServerDateIsInvalid = 10364,
        [Description("Data base casting arbitrarily be changed")]
        DatabaseCastingArbitrarityChang = 10365,
        [Description("Create object log failed")]
        CreateObjectLogFailed = 10366,
    }
}
