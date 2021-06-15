// ReSharper disable InconsistentNaming

namespace Apzon.PosmanErp.Commons
{
    public enum APlusObjectType
    {
        oNone = 0,
        oItemGroups = 52,
        oItems = 4,
        oBusinessPartners = 2,
        #region sales
        oInvoices = 13,
        oReserveInvoices = 13000,
        oDeliveryNotes = 15,
        oReturns = 16,
        oOrders = 17,
        oQuotations = 23,
        oReturnRequest = 234000031,
        #endregion

        #region purchase
        oPurchaseInvoices = 18,
        oPurchaseReserveInvoices = 25,
        oPurchaseDeliveryNotes = 20,
        oPurchaseReturns = 21,
        oPurchaseOrders = 22,
        oPurchaseReturnRequest = 234000032,
        oPurchaseQuotations = 540000006,
        #endregion

        #region inventory
        oInventoryGenEntry = 59,
        oInventoryGenExit = 60,
        oInventoryTransfer = 67,
        oInventoryTransferRequest = 1250000001,
        #endregion

        oWarehouses = 64,
        oCurrency = 37,
        oProductTrees = 66,
        oDraft = 112,
        oItemTag = 10001,
        oTrademark = 10002,
        oStore = 10004,
        oBpGroup = 10007,
        oIncommingPayment = 810018,
        oGeneralSetting = 10009,
        oUser = 12,
        oAuth = 10011,
        oMenu = 10014,
        oVatGroup = 10012,
        oOutgoingPayment = 810019,
        oInventoryCounting = 147065,
        oPaymentType = 10016,
        oUoMGroups = 10197,
        oUoM = 10199,
        oItemComponent = 400002,
        oPriceList = 6,
        oLayoutSetting = 400003,
        oExchangeRate = 400005,
        oShift = 810033,
        oFormSetting = 810034,
        oFormValidValue = 810035,
        oLicenseManager = 10005,
        oCompany = 700007,
        oUDF = 152,
        oUDT = 153,
        oUDO = 206,
        oLogin = -1,
        oShippingUnit = 10013,
        oPostingPeriod = 700022,
        oGeographyLocation = 1250000002,
        oCountry = 1250000003,
        oProvince = 1250000004,
        oDistrict = 1250000005,
        oWards = 1250000006,
        oRetailTable = 400004,
        oWeight = 1250000007,
        oLength = 1250000008,
    }

    public enum AplusSubObjectType
    {
        oNone= 0,
        // bp
        oSupplier = 1,
        oCustomer = 2,
        
        //item
        oSale = 3,
        oPurchase = 4,
        oInventory = 5,
       
    }

}
