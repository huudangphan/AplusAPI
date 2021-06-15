using System.Data;
using System.Data.SqlClient;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces.Commons
{
    public interface ICommons
    {
        /// <summary>
        /// hàm lấy dữ liệu database connection của database
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        DataTable GetDataDatabaseConnection(string databaseName);
    }
}
