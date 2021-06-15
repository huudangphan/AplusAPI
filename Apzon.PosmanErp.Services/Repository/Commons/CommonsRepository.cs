using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Commons;

namespace Apzon.PosmanErp.Services.Repository.Commons
{
    public class CommonsRepository : BaseService<DataTable>, ICommons
    {
        public CommonsRepository(IDatabaseClient databaseService) : base(databaseService)
		{

        }

        public DataTable GetDataDatabaseConnection(string databaseName)
        {
            try
            {
                return ExecuteDataTable(@"select company_db, db_server, db_user, db_pass, db_schema, db_server_port from system_database where company_db = @_databaseName", CommandType.Text, new SqlParameter("_databaseName", databaseName));
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new DataTable();
            }
        }

    }
}
