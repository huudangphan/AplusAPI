using System.Data;
using System.Data.SqlClient;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.MasterData;

namespace Apzon.PosmanErp.Services.Repository.MasterData
{
    public class LengthRepository:BaseMasterData, ILength
    {
        public LengthRepository(IDatabaseClient databaseService, string tableName) : base(databaseService, tableName)
        {
        }

        protected override HttpResult ValidateForeignConstraint(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign,
            ref TransactionType transtype, DataTable document, ref string objId, ref string objNum)
        {
            return new HttpResult((MessageCode) ExecuteScalar("aplus_basemasterdata_validate_length_constraints",
                CommandType.StoredProcedure, new SqlParameter("_code", objId),
                new SqlParameter("_tran_type", Function.ToString(transtype))));
        }
    }
}