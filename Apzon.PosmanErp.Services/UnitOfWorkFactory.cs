using Apzon.PosmanErp.Commons;

namespace Apzon.PosmanErp.Services
{
    public static class UnitOfWorkFactory
    {
        /// <summary>
        /// trả về unitofword theo loại system
        /// </summary>
        /// <param name="strConnection"></param>
        /// <param name="schemaName"></param>
        /// <param name="databaseSystemType"></param>
        /// <returns></returns>
        public static IUnitOfWork GetUnitOfWork(string strConnection, string schemaName, DatabaseSystemType databaseSystemType)
        {
            return new UnitOfWork(strConnection, schemaName, databaseSystemType);
        }
    }
}