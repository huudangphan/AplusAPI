using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Commons;

namespace Apzon.PosmanErp.Services.Repository.Commons
{
    public class ChooseFromListRepository : BaseService<DataTable>, IChooseFromList
    {
        public ChooseFromListRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }


        public HttpResult ListResult(SearchDocumentModel model)
        {
            if (string.IsNullOrEmpty(model.form_id))
            {
                return new HttpResult(MessageCode.FormIdNotProvide);
            }

            if (string.IsNullOrEmpty(model.item_id))
            {
                return new HttpResult(MessageCode.ItemIdNotProvide);
            }

            if (string.IsNullOrEmpty(model.language_code))
            {
                return new HttpResult(MessageCode.LanguageNotProvide);
            }
            //Get List of table by object_type;
            var tableName = GetDocumentTableByObjectType(model.object_type);

            var result = ExecuteDataTable("aplus_cfl_get", CommandType.StoredProcedure,
                new SqlParameter("_table_name", tableName.HeaderTable),
                new SqlParameter("_form_id", model.form_id),
                new SqlParameter("_item_id", Function.ToString(model.item_id)),
                new SqlParameter("_sub_query", GetConditionQuery(model.object_type, model.sub_type)),
                new SqlParameter("_user_id", model.user_id),
                new SqlParameter("_language_code", model.language_code));
            return new HttpResult(MessageCode.Success, result);
        }

        /// <summary>
        /// Make query filter for choose from list <br/>
        /// Check sub type in object type 
        /// </summary>
        /// <param name="objectType">main object type</param>
        /// <param name="subObjectType">sub object type</param>
        /// <returns></returns>
        private static string GetConditionQuery(APlusObjectType objectType, AplusSubObjectType subObjectType)
        {
            switch (objectType)
            {
                case APlusObjectType.oBpGroup when subObjectType == AplusSubObjectType.oCustomer:
                    return " and type = 'C' ";
                case APlusObjectType.oBpGroup when subObjectType == AplusSubObjectType.oSupplier:
                    return " and type = 'S' ";

                #region Items

                case APlusObjectType.oItems when subObjectType == AplusSubObjectType.oSale:
                    return " and sell_item = 'Y' ";
                case APlusObjectType.oItems when subObjectType == AplusSubObjectType.oPurchase:
                    return " and purchse_item = 'Y' ";
                case APlusObjectType.oItems when subObjectType == AplusSubObjectType.oInventory:
                    return " and invn_item = 'Y' ";

                #endregion

                #region Geography

                case APlusObjectType.oCountry:
                    return " and type = 'C' ";
                case APlusObjectType.oProvince:
                    return " and type = 'P' ";
                case APlusObjectType.oDistrict:
                    return " and type = 'D' ";
                case APlusObjectType.oWards:
                    return " and type = 'W' ";

                #endregion

                default:
                    return "";
            }
        }
    }
}