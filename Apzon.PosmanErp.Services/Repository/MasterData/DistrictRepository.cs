using System.Data;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.MasterData;

namespace Apzon.PosmanErp.Services.Repository.MasterData
{
    public class DistrictRepository:BaseMasterData, IDistrict
    {
        public DistrictRepository(IDatabaseClient databaseService, string tableName) : base(databaseService, tableName)
        {
        }

        protected override HttpResult ValidateForeignConstraint(APlusObjectType objtype, APlusObjectType fromObjType, int bplId, int userSign,
            ref TransactionType transtype, DataTable document, ref string objId, ref string objNum)
        {
            return new HttpResult(MessageCode.Success);

        }
    }
}