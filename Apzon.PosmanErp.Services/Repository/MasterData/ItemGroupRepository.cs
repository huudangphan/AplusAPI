using System.Data;
using System.Data.SqlClient;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.MasterData;

namespace Apzon.PosmanErp.Services.Repository.MasterData
{
    public class ItemGroupRepository : BaseMasterData, IItemGroup
    {
        public ItemGroupRepository(IDatabaseClient databaseService, string tableName) : base(databaseService, tableName)
        {
        }

        protected override HttpResult ValidateForeignConstraint(APlusObjectType objectType, APlusObjectType fromObjType,
            int bplId,
            int userSign,
            ref TransactionType tranType, DataTable document, ref string objId, ref string objNum)
        {
            return new ((MessageCode) ExecuteScalar("aplus_basemasterdata_validate_itemgroup_constraints",
                CommandType.StoredProcedure, new SqlParameter("_tran_type", Function.ToString(tranType)),
                new SqlParameter("_code", objId)));
        }
    }
}