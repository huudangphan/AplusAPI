using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.MasterData;

namespace Apzon.PosmanErp.Services.Repository.MasterData
{
    public class CompanyRepository : BaseMasterData, ICompany
    {
        public CompanyRepository(IDatabaseClient databaseService, string tableName) : base(databaseService, tableName)
        {
        }

        protected override HttpResult ValidateForeignConstraint(APlusObjectType objtype, APlusObjectType fromObjType,
            int bplId, int userSign,
            ref TransactionType transtype, DataTable document, ref string objId, ref string objNum)
        {
            if (transtype == TransactionType.D)
            {
                return new HttpResult(MessageCode.CompanyNoDelete);
            }

            return new HttpResult((MessageCode) ExecuteScalar("aplus_basemasterdata_validate_company_constraints",
                CommandType.StoredProcedure,
                new SqlParameter("_tran_type", Function.ToString(transtype)),
                new SqlParameter("_code", objId)));
        }
    }
}