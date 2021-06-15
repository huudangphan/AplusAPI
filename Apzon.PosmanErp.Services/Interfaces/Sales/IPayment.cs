using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using System.Collections.Generic;
using System.Data;

namespace Apzon.PosmanErp.Services.Interfaces.Sales
{
    public interface IPayment
    {
        HttpResult GetListPaymentReceiptIssueGroup();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objtype"></param>
        /// <param name="bplId"></param>
        /// <param name="userSign"></param>
        /// <param name="transtype"></param>
        /// <param name="document"></param>
        /// <param name="objId"></param>
        /// <param name="objCode"></param>
        /// <returns></returns>
        HttpResult Process(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, TransactionType transtype, DataSet document, int userSign, string objId = "", string objCode = "");

        HttpResult ConfirmPaymentOrder(APlusObjectType paymentType, APlusObjectType objType, int docEntry, DataTable dtPayment, int userSign, int branch, bool subTransaction = false);

        HttpResult ConfirmPaymentReturn(APlusObjectType paymentType, APlusObjectType objType, APlusObjectType baseType, int baseEntry, DataTable dtPayment, int returnEntry, int userSign, int branch, bool subTransaction = false);

        HttpResult GetListPayment(string paymentType, string fromDate, string toDate, List<int> groupType, List<int> docType, string textSearch, string filter, int? pageSize, int? pageIndex);

        HttpResult GetDataPaymentByEntry(int docEntry);

        HttpResult GetDataCashBooks(string fromDate, string toDate, APlusObjectType docType, List<string> payments, int userId, string cardCode, string branch, string isAccounting, string textSearch, int pageSize, int pageIndex);

    }
}
