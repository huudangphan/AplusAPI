using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using Apzon.Libraries.HDBConnection.Interfaces;

namespace Apzon.Libraries.HDBConnection
{
    public class DatabaseSqlClient : IDatabaseClient
    {
        /// <summary>
        /// chuỗi kết nối tới database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// schema đang kết nối
        /// </summary>
        public string SchemaName { get; set; }

        public SqlConnection DbConnection { get; set; }

        public SqlTransaction DbTransaction { get; set; }

        public DatabaseSqlClient(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            ConnectionString = connectionString;
            DbTransaction = null;
        }

        public DatabaseSqlClient(SqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            DbConnection = connection;
            DbTransaction = null;
        }

        public virtual string InsertDataTableIntoSql(DataTable dt)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                // Copy the DataTable to SQL Server Table using SqlBulkCopy
                using (var sqlBulkCopy = new SqlBulkCopy(ConnectionString))
                {
                    sqlBulkCopy.DestinationTableName = dt.TableName;
                    foreach (var column in dt.Columns)
                        sqlBulkCopy.ColumnMappings.Add(column.ToString(), column.ToString());

                    sqlBulkCopy.WriteToServer(dt);
                    dt.Rows.Clear();
                    return "";
                }
            }
        }

        public int ExecuteNonQueryNotParse(string query, CommandType commandType = CommandType.Text)
        {
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.Transaction = DbTransaction;
                    command.CommandType = commandType;
                    return command.ExecuteNonQuery();
                }
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var command = conn.CreateCommand();
                command.CommandTimeout = 60000;
                command.CommandText = query;
                command.CommandType = commandType;
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text,
            params SqlParameter[] parameters)
        {
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.Transaction = DbTransaction;
                    command.CommandType = commandType;
                    command.TryAddParameters(parameters);
                    return command.ExecuteNonQuery();
                }
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var command = conn.CreateCommand();
                command.CommandTimeout = 60000;
                command.CommandText = query;
                command.CommandType = commandType;
                command.TryAddParameters(parameters);
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQueryNoParam(string query, CommandType commandType = CommandType.Text)
        {
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.Transaction = DbTransaction;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    return command.ExecuteNonQuery();
                }
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var command = conn.CreateCommand();
                command.CommandTimeout = 60000;
                command.CommandText = query;
                command.CommandType = commandType;
                return command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string query, CommandType commandType = CommandType.Text,
            params SqlParameter[] parameters)
        {
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.Transaction = DbTransaction;
                    command.CommandType = commandType;
                    command.TryAddParameters(parameters);
                    return command.ExecuteScalar();
                }
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var command = conn.CreateCommand();
                command.CommandTimeout = 60000;
                command.CommandText = query;
                command.CommandType = commandType;
                command.TryAddParameters(parameters);
                return command.ExecuteScalar();
            }
        }

        public DataSet ExecuteData(string query, CommandType commandType = CommandType.Text,
            params SqlParameter[] parameters)
        {
            var dtSet = new DataSet();
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (SqlCommand command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.Transaction = DbTransaction;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.TryAddParameters(parameters);
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();
                    return dtSet;
                }
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.TryAddParameters(parameters);
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();
                }
            }
            return dtSet;
        }

        public DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var dtSet = new DataTable();
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (SqlCommand command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.Transaction = DbTransaction;
                    command.CommandType = commandType;
                    command.TryAddParameters(parameters);
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();
                    return dtSet;
                }
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.TryAddParameters(parameters);
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();
                }
            }
            return dtSet;
        }

        public DataTable ExecuteDataTable(string query, string structTable, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var dtSet = new DataTable();
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (SqlCommand command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.Transaction = DbTransaction;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.TryAddParameters(parameters);
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();
                    if (!string.IsNullOrEmpty(structTable))
                    {
                        SetDefaultValueToTable(structTable, dtSet);
                    }
                    return dtSet;
                }
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.TryAddParameters(parameters);
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    if (!string.IsNullOrEmpty(structTable))
                    {
                        SetDefaultValueToTable(structTable, dtSet);
                    }
                    command.Parameters.Clear();
                }
            }
            return dtSet;
        }

        public virtual DataTable GetTableStruct(string tblName)
        {
            const string sql =
                    @"SELECT COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TblName";
            var pr = new SqlParameter("@TblName", tblName);
            var dt = ExecuteDataTable(sql, parameters: pr);
            var ret = new DataTable();
            ret.TableName = tblName;
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Type type;
                    string sqltype = dt.Rows[i]["DATA_TYPE"].To_String().ToLower();
                    var colName = dt.Rows[i]["COLUMN_NAME"].To_String();
                    var dtColumn = new DataColumn();
                    dtColumn.ColumnName = colName;
                    var dflValue = "";
                    var dfltLen = dt.Rows[i]["COLUMN_DEFAULT"].To_String().Length;
                    if (dfltLen >= 4)
                    {
                        dflValue = dt.Rows[i]["COLUMN_DEFAULT"].To_String().Substring(2, dfltLen - 4);
                    }
                    var allownull = dt.Rows[i]["IS_NULLABLE"].To_String().Equals("YES");

                    var info = CultureInfo.InvariantCulture;
                    switch (sqltype)
                    {
                        case "int":
                        case "bigint":
                        case "smallint":
                            type = typeof(int);
                            if (!string.IsNullOrEmpty(dflValue) || !allownull)
                            {
                                dtColumn.DefaultValue = dflValue.Replace("(", "").Replace(")", "").ParseInt();
                            }

                            break;
                        case "decimal":
                            type = typeof(decimal);
                            if (!string.IsNullOrEmpty(dflValue) || !allownull)
                            {
                                dtColumn.DefaultValue = dflValue.Replace("(", "").Replace(")", "").ParseDecimal();
                            }
                            break;

                        case "datetime":
                        case "date":
                            type = typeof(DateTime);
                            if (!string.IsNullOrEmpty(dflValue) || !allownull)
                            {
                                var dfl = dflValue.ParseDateTime();
                                dtColumn.DefaultValue = dfl ?? new DateTime(1900, 1, 1);
                            }
                            break;
                        case "numeric":
                        case "float":
                            type = typeof(double);
                            if (!string.IsNullOrEmpty(dflValue) || !allownull)
                            {
                                dtColumn.DefaultValue = dflValue.Replace("(", "").Replace(")", "").ParseDouble(info.NumberFormat);
                            }
                            break;
                        case "bit":
                            type = typeof(bool);
                            if (!string.IsNullOrEmpty(dflValue) || !allownull)
                            {
                                dtColumn.DefaultValue = dflValue.ParseBool();
                            }
                            break;
                        default:
                            type = typeof(string);
                            if (!string.IsNullOrEmpty(dflValue) || !allownull)
                            {
                                dtColumn.DefaultValue = dflValue.Replace("''", "'").To_String();
                            }
                            break;
                    }

                    dtColumn.AllowDBNull = allownull;

                    dtColumn.DataType = type;
                    ret.Columns.Add(dtColumn);
                }
            }
            if (ret.Columns.Count > 0)
            {
                ret.Rows.Add(ret.NewRow());
                return ret;
            }
            return new DataTable();
        }

        public virtual void SetDefaultValueToTable(string table, DataTable dt)
        {
            if (dt == null || dt.Columns.Count == 0)
            {
                return;
            }
            const string sql = @"SELECT COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TblName and (IS_NULLABLE = 'NO' or COLUMN_DEFAULT is not null)";
            var tabelStruc = ExecuteDataTable(sql, parameters: new SqlParameter("@TblName", table));
            if (tabelStruc != null && tabelStruc.Rows.Count > 0)
            {
                for (var i = 0; i < tabelStruc.Rows.Count; i++)
                {
                    var sqltype = tabelStruc.Rows[i]["DATA_TYPE"].To_String().ToLower();
                    var colName = tabelStruc.Rows[i]["COLUMN_NAME"].To_String();
                    var dflValue = "";
                    var dfltLen = tabelStruc.Rows[i]["COLUMN_DEFAULT"].To_String().Length;
                    if (dfltLen >= 4)
                    {
                        dflValue = tabelStruc.Rows[i]["COLUMN_DEFAULT"].To_String().Substring(2, dfltLen - 4);
                    }
                    var allownull = tabelStruc.Rows[i]["IS_NULLABLE"].To_String().Equals("YES");
                    if (!allownull || !string.IsNullOrEmpty(dflValue))
                    {
                        for (var k = 0; k < dt.Columns.Count; k++)
                        {
                            if (dt.Columns[k].ColumnName == colName)
                            {
                                var info = CultureInfo.InvariantCulture;
                                switch (sqltype)
                                {
                                    case "int":
                                    case "bigint":
                                    case "smallint":
                                        dt.Columns[k].DefaultValue = dflValue.Replace("(", "").Replace(")", "").ParseInt();

                                        break;
                                    case "decimal":
                                        dt.Columns[k].DefaultValue = dflValue.Replace("(", "").Replace(")", "").ParseDecimal();
                                        break;

                                    case "datetime":
                                    case "date":
                                        if (!string.IsNullOrEmpty(dflValue) || !allownull)
                                        {
                                            var dfl = dflValue.ParseDateTime();
                                            dt.Columns[k].DefaultValue = dfl ?? new DateTime(1900, 1, 1);
                                        }
                                        break;
                                    case "numeric":
                                    case "float":
                                        dt.Columns[k].DefaultValue = dflValue.Replace("(", "").Replace(")", "").ParseDouble(info.NumberFormat);
                                        break;
                                    case "bit":
                                        dt.Columns[k].DefaultValue = dflValue.ParseBool();
                                        break;
                                    default:
                                        dt.Columns[k].DefaultValue = dflValue.Replace("''", "'").To_String();
                                        break;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private string GetDefaultString()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                var database = builder.InitialCatalog;
                var sql = string.Format("SELECT DATABASEPROPERTYEX('{0}', 'Collation') as Coll", database);
                var resp = ExecuteDataTable(sql);
                if (resp != null && resp.Rows.Count > 0)
                {
                    return resp.Rows[0]["Coll"].To_String();
                }
            }
            catch (Exception ex)
            {
                //ignore
            }
            return "";
        }

        private string GetSqlType(DataColumn col, string def)
        {
            var sqltype = GetDBType(col.DataType);
            if (col.DataType == typeof(string))
            {
                //todo: Length of nvarchar column
                return string.IsNullOrEmpty(def) ? sqltype + "(MAX) COLLATE DATABASE_DEFAULT " : sqltype + "(MAX) COLLATE " + def;
            }
            if (sqltype == SqlDbType.Decimal)
            {
                return sqltype + " (19,6)";
            }
            return sqltype.ToString();
        }

        private SqlDbType GetDBType(Type theType)
        {
            var pr = new SqlParameter();
            var tc = TypeDescriptor.GetConverter(pr.DbType);
            if (tc.CanConvertFrom(theType))
            {
                var convertFrom = tc.ConvertFrom(theType.Name);
                if (convertFrom != null) pr.DbType = (DbType)convertFrom;
            }
            else
            {
                try
                {
                    var convertFrom = tc.ConvertFrom(theType.Name);
                    if (convertFrom != null) pr.DbType = (DbType)convertFrom;
                }
                catch (Exception)
                {
                    //Do Nothing; will return NVarChar as default
                }
            }
            return pr.SqlDbType;
        }

        public void CreateTempTable(DataTable dt, string tblName)
        {
            var stringDefault = GetDefaultString();
            var sql = string.Format(@"BEGIN TRY
                                DROP TABLE [{0}]  
                                END TRY
                                BEGIN CATCH
                                END CATCH", tblName);
            var cols = new List<string>();
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var col = dt.Columns[i];
                var item = string.Format("[{0}] {1} {2}", col.ColumnName, GetSqlType(col, stringDefault),
                    col.AllowDBNull ? "NULL" : "");
                cols.Add(item);
            }
            sql += string.Format(" CREATE TABLE [{0}]({1})", tblName, string.Join(",", cols.ToArray()));

            if (DbConnection != null)
            {
                if (string.IsNullOrEmpty(DbConnection.ConnectionString))
                {
                    DbConnection.ConnectionString = ConnectionString;
                }
                if (DbConnection.State == ConnectionState.Closed)
                {
                    DbConnection.Open();
                }
            }
            ExecuteNonQuery(sql);
        }

        public void BulkCopy(DataTable dt, string tblName)
        {
            if (DbConnection != null)
            {
                if (string.IsNullOrEmpty(DbConnection.ConnectionString))
                {
                    DbConnection.ConnectionString = ConnectionString;
                }
                if (DbConnection.State == ConnectionState.Closed)
                {
                    DbConnection.Open();
                }
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(DbConnection, SqlBulkCopyOptions.Default, DbTransaction))
                {
                    // The table I'm loading the data to
                    bulkCopy.DestinationTableName = "[" + tblName + "]";
                    // How many records to send to the database in one go (all of them)
                    bulkCopy.BatchSize = 1000;
                    bulkCopy.BulkCopyTimeout = 60000;
                    // Load the data to the database
                    bulkCopy.WriteToServer(dt);
                    // Close up          
                    bulkCopy.Close();
                }
                return;
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConnectionString))
            {
                // The table I'm loading the data to
                bulkCopy.DestinationTableName = "[" + tblName + "]";
                // How many records to send to the database in one go (all of them)
                bulkCopy.BatchSize = 1000;
                bulkCopy.BulkCopyTimeout = 60000;
                // Load the data to the database
                bulkCopy.WriteToServer(dt);
                // Close up          
                bulkCopy.Close();
            }
        }

        public DbConnection OpenConnection()
        {
            DbConnection = DbConnection == null || string.IsNullOrEmpty(DbConnection.ConnectionString) ? new SqlConnection(ConnectionString) : DbConnection;
            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
            return DbConnection;
        }

        public DbTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            DbTransaction = DbConnection.BeginTransaction(level);
            return DbTransaction;
        }

    }
}
