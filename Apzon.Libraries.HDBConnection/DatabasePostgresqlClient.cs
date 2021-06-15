using Apzon.Libraries.HDBConnection.Interfaces;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.Libraries.HDBConnection
{
    public class DatabasePostgresqlClient : IDatabaseClient
    {
        /// <summary>
        /// chuỗi kết nối tới database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// schema đang kết nối
        /// </summary>
        public string SchemaName { get; set; }

        public NpgsqlConnection DbConnection { get; set; }

        public NpgsqlTransaction DbTransaction { get; set; }

        public DatabasePostgresqlClient(string connectionString, string schemaName)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            if (schemaName == null)
                throw new ArgumentNullException("schemaName");
            ConnectionString = connectionString;
            SchemaName = schemaName;
            DbTransaction = null;
        }

        public DatabasePostgresqlClient(NpgsqlConnection connection, string schemaName)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (schemaName == null)
                throw new ArgumentNullException("schemaName");
            DbConnection = connection;
            SchemaName = schemaName;
            DbTransaction = null;
        }

        public virtual string InsertDataTableIntoSql(DataTable dt)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                // Copy the DataTable to SQL Server Table using SqlBulkCopy
                using (var npgsqlBulkCopy = new ApzNpgsqlBulk(conn))
                {
                    npgsqlBulkCopy.TargetTable = @"""" + dt.TableName + @"""";
                    foreach (var column in dt.Columns)
                        npgsqlBulkCopy.ColumnMappings.Add(new ApzColumnMapping(column.ToString(), column.ToString()));

                    npgsqlBulkCopy.BulkInsert(dt);
                    dt.Rows.Clear();
                    return "";
                }
            }
        }

        #region Các hàm thực thi câu lệnh postgresql

        public int ExecuteNonQueryNotParse(string query, CommandType commandType = CommandType.Text)
        {
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.Transaction = DbTransaction;
                    return command.ExecuteNonQuery();
                }
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                var command = conn.CreateCommand();
                command.CommandTimeout = 60000;
                command.CommandText = query;
                command.CommandType = commandType;
                var result = command.ExecuteNonQuery();
                //HanaConnection.ClearPool(conn);
                return result;
            }
        }

        public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var lst = ParseParamater(parameters);
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.Transaction = DbTransaction;
                    command.TryAddParameters(lst);
                    return command.ExecuteNonQuery();
                }
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                var command = conn.CreateCommand();
                command.CommandTimeout = 60000;
                command.CommandText = query;
                command.CommandType = commandType;
                command.TryAddParameters(lst);
                var result = command.ExecuteNonQuery();
                //HanaConnection.ClearPool(conn);
                return result;
            }
        }

        public int ExecuteNonQueryNoParam(string query, CommandType commandType = CommandType.Text)
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
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                var command = conn.CreateCommand();
                command.CommandTimeout = 60000;
                command.CommandText = query;
                command.CommandType = commandType;
                var result = command.ExecuteNonQuery();
                //HanaConnection.ClearPool(conn);
                return result;
            }
        }

        public object ExecuteScalar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var lst = ParseParamater(parameters);
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.Transaction = DbTransaction;
                    command.CommandType = commandType;
                    command.TryAddParameters(lst);
                    return command.ExecuteScalar();
                }
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                var command = conn.CreateCommand();
                command.CommandTimeout = 60000;
                command.CommandText = query;
                command.CommandType = commandType;
                command.TryAddParameters(lst);
                var result = command.ExecuteScalar();
                //HanaConnection.ClearPool(conn);
                return result;
            }
        }

        public DataSet ExecuteData(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var lst = ParseParamater(parameters);
            var dtSet = new DataSet();
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.Transaction = DbTransaction;
                    command.TryAddParameters(lst);
                    var adapter = new NpgsqlDataAdapter(command);

                    if (commandType == CommandType.Text)
                    {
                        adapter.Fill(dtSet);
                    }
                    else
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Columns.Count == 1)
                        {
                            //Kiểm tra kiểu trả về của function
                            command.CommandType = CommandType.Text;
                            command.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function";
                            command.Parameters.Clear();
                            command.TryAddParameters(new NpgsqlParameter("@function", query));
                            var type = ToString(command.ExecuteScalar());

                            //Nếu là refcursor thì fetch data từ list cursor
                            if (type == "refcursor")
                            {
                                if (dt.Rows.Count == 0)
                                {
                                    return new DataSet();
                                }

                                query = "";
                                for (var i = 0; i < dt.Rows.Count; i++)
                                {
                                    query += $@"FETCH ALL IN ""{dt.Rows[i][0]}""; ";
                                }

                                command.CommandText = query;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Clear();
                                adapter.Fill(dtSet);

                                return dtSet;
                            }
                        }

                        //Nếu bảng có nhiều hơn 1 cột hoặc kiểu trả về của function không phải refcursor
                        //thì dataset trả về chỉ có 1 bảng
                        dtSet.Tables.Add(dt);
                    }
                }
                return dtSet;
            }

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                using (var command = conn.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.TryAddParameters(lst);
                    var adapter = new NpgsqlDataAdapter(command);

                    if (commandType == CommandType.Text)
                    {
                        adapter.Fill(dtSet);
                    }
                    else
                    {
                        //Phải có transaction thì mới fetch được data từ cursor
                        var trans = conn.BeginTransaction();
                        command.Transaction = trans;
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Columns.Count == 1)
                        {
                            //Kiểm tra kiểu trả về của function
                            command.CommandType = CommandType.Text;
                            command.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function";
                            command.Parameters.Clear();
                            command.TryAddParameters(new NpgsqlParameter("@function", query));
                            var type = ToString(command.ExecuteScalar());

                            //Nếu là refcursor thì fetch data từ list cursor
                            if (type == "refcursor")
                            {
                                if (dt.Rows.Count == 0)
                                {
                                    return new DataSet();
                                }

                                query = "";
                                for (var i = 0; i < dt.Rows.Count; i++)
                                {
                                    query += $@"FETCH ALL IN ""{dt.Rows[i][0]}""; ";
                                }

                                command.CommandText = query;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Clear();
                                adapter.Fill(dtSet);

                                trans.Commit();
                                return dtSet;
                            }
                        }

                        //Nếu dt có nhiều hơn 1 cột hoặc kiểu trả về của function không phải refcursor
                        //thì dataset trả về chỉ có 1 bảng
                        dtSet.Tables.Add(dt);
                        trans.Commit();
                    }
                }
                return dtSet;
            }
        }

        public DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var lst = ParseParamater(parameters);
            var dtSet = new DataTable();
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.Transaction = DbTransaction;
                    command.TryAddParameters(lst);
                    var adapter = new NpgsqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();

                    if (commandType == CommandType.Text || dtSet.Columns.Count > 1)
                    {
                        return dtSet;
                    }

                    #region Get return type of function

                    command.CommandType = CommandType.Text;
                    command.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function";
                    command.TryAddParameters(new NpgsqlParameter("@function", query));
                    var type = ToString(command.ExecuteScalar());

                    #endregion

                    #region If type is 'refcursor', fetch data from cursors

                    if (type == "refcursor")
                    {
                        if (dtSet.Rows.Count == 0) return new DataTable();

                        command.CommandText = $@"FETCH ALL IN ""{ToString(dtSet.Rows[0][0])}"";";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        dtSet = new DataTable();
                        adapter.Fill(dtSet);
                    }

                    #endregion

                    return dtSet;
                }
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                using (var command = conn.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.TryAddParameters(lst);
                    var adapter = new NpgsqlDataAdapter(command);

                    if (commandType == CommandType.Text)
                    {
                        adapter.Fill(dtSet);
                    }
                    else
                    {
                        var trans = conn.BeginTransaction();
                        command.Transaction = trans;
                        adapter.Fill(dtSet);
                        command.Parameters.Clear();

                        if (dtSet.Columns.Count == 1)
                        {
                            #region Get return type of function

                            command.CommandType = CommandType.Text;
                            command.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function;";
                            command.TryAddParameters(new NpgsqlParameter("@function", query));
                            var type = ToString(command.ExecuteScalar());

                            #endregion

                            #region If type is 'refcursor', fetch data from cursors

                            if (type == "refcursor")
                            {
                                if (dtSet.Rows.Count == 0) return new DataTable();

                                command.CommandText = $@"FETCH ALL IN ""{ToString(dtSet.Rows[0][0])}"";";
                                command.Parameters.Clear();
                                command.CommandType = CommandType.Text;
                                dtSet = new DataTable();
                                adapter.Fill(dtSet);
                            }

                            #endregion
                        }

                        trans.Commit();
                    }
                }
            }
            return dtSet;
        }

        public DataTable ExecuteDataTable(string query, string structTable, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var lst = ParseParamater(parameters);
            var dtSet = new DataTable();
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (var command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.Transaction = DbTransaction;
                    command.TryAddParameters(lst);
                    var adapter = new NpgsqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();

                    if (commandType == CommandType.Text || dtSet.Columns.Count > 1)
                    {
                        return dtSet;
                    }

                    #region Get return type of function

                    command.CommandType = CommandType.Text;
                    command.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function";
                    command.TryAddParameters(new NpgsqlParameter("@function", query));
                    var type = ToString(command.ExecuteScalar());

                    #endregion

                    #region If type is 'refcursor', fetch data from cursors

                    if (type == "refcursor")
                    {
                        if (dtSet.Rows.Count == 0) return new DataTable();

                        command.CommandText = $@"FETCH ALL IN ""{ToString(dtSet.Rows[0][0])}"";";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        dtSet = new DataTable();
                        adapter.Fill(dtSet);
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(structTable))
                    {
                        SetDefaultValueToTable(structTable, dtSet);
                    }
                    return dtSet;
                }
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                using (var command = conn.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.TryAddParameters(lst);
                    var adapter = new NpgsqlDataAdapter(command);

                    if (commandType == CommandType.Text)
                    {
                        adapter.Fill(dtSet);
                    }
                    else
                    {
                        var trans = conn.BeginTransaction();
                        command.Transaction = trans;
                        adapter.Fill(dtSet);
                        command.Parameters.Clear();

                        if (dtSet.Columns.Count == 1)
                        {
                            #region Get return type of function

                            command.CommandType = CommandType.Text;
                            command.CommandText = "select lower(data_type) from information_schema.routines where routine_type = 'FUNCTION' and routine_name = @function;";
                            command.TryAddParameters(new NpgsqlParameter("@function", query));
                            var type = ToString(command.ExecuteScalar());

                            #endregion

                            #region If type is 'refcursor', fetch data from cursors

                            if (type == "refcursor")
                            {
                                if (dtSet.Rows.Count == 0) return new DataTable();

                                command.CommandText = $@"FETCH ALL IN ""{ToString(dtSet.Rows[0][0])}"";";
                                command.Parameters.Clear();
                                command.CommandType = CommandType.Text;
                                dtSet = new DataTable();
                                adapter.Fill(dtSet);
                            }

                            #endregion
                        }

                        trans.Commit();
                    }
                }
            }

            if (!string.IsNullOrEmpty(structTable))
            {
                SetDefaultValueToTable(structTable, dtSet);
            }
            return dtSet;
        }

        #endregion

        #region Postgresql bulkcopy

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tblName"></param>
        public void CreateTempTable(DataTable dt, string tblName)
        {
            var sql = "";
            var cols = new List<string>();
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var col = dt.Columns[i];
                var maxValueColumn = dt.AsEnumerable().Max(t => t[col] == null ? "" : t[col].ToString());
                var item = string.Format(@"""{0}"" {1} {2}", col.ColumnName, GetSqlType(col, maxValueColumn == null ? 50 : maxValueColumn.Length),
                    col.AllowDBNull ? "NULL" : "");
                cols.Add(item);
            }
            sql += string.Format(@" CREATE local temporary TABLE ""{0}""({1});", tblName, string.Join(",", cols.ToArray()));
            if (DbConnection != null)
            {
                if (string.IsNullOrEmpty(DbConnection.ConnectionString))
                {
                    DbConnection.ConnectionString = ConnectionString;
                }
                if (DbConnection.State == ConnectionState.Closed)
                {
                    DbConnection.Open();
                    SetConnectionSchema(DbConnection);
                }
            }
            ExecuteNonQuery(sql);
        }

        private string GetSqlType(DataColumn col, int columnLength)
        {
            var postgrestype = GetDBType(col.DataType);
            if (col.DataType == typeof(string))
            {
                return columnLength < 5001 ? "character varying(5000)" : "text";
            }
            else if (col.DataType == typeof(char))
            {
                return "CHAR(1)";
            }

            if (postgrestype == NpgsqlDbType.Double || postgrestype == NpgsqlDbType.Numeric)
            {
                return "numeric(19,6)";
            }
            else if (postgrestype == NpgsqlDbType.Date || postgrestype == NpgsqlDbType.Timestamp || postgrestype == NpgsqlDbType.TimestampTz || postgrestype == NpgsqlDbType.TimestampTZ)
            {
                return "date";
            }
            return postgrestype.ToString();
        }

        private NpgsqlDbType GetDBType(Type theType)
        {
            var pr = new NpgsqlParameter();
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
                    if (convertFrom != null)
                        pr.DbType = (DbType)convertFrom;
                }
                catch (Exception ex)
                {
                    if (theType.Name.Equals("TimeSpan"))
                    {
                        pr.DbType = DbType.Time;
                    }
                    //Do Nothing; will return NVarChar as default
                }
            }
            return pr.NpgsqlDbType;
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
                    SetConnectionSchema(DbConnection);
                }
                using (var bulkCopy = new ApzNpgsqlBulk(DbConnection))
                {
                    // The table I'm loading the data to
                    bulkCopy.TargetTable = tblName;
                    // How many records to send to the database in one go (all of them)
                    //bulkCopy.BatchSize = 1000;
                    //bulkCopy.BatchTimeout = 60000;
                    // Load the data to the database
                    bulkCopy.BulkInsert(dt);
                }
                return;
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SetConnectionSchema(conn);
                // Copy the DataTable to SQL Server Table using SqlBulkCopy
                using (var bulkCopy = new ApzNpgsqlBulk(conn))
                {
                    // The table I'm loading the data to
                    bulkCopy.TargetTable = tblName;
                    // How many records to send to the database in one go (all of them)
                    //bulkCopy.BatchSize = 1000;
                    //bulkCopy.BatchTimeout = 60000;
                    // Load the data to the database
                    bulkCopy.BulkInsert(dt);
                }
            }
        }

        #endregion

        /// <summary>
        /// set default value for table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dt"></param>
        public virtual void SetDefaultValueToTable(string table, DataTable dt)
        {
            if (dt == null || dt.Columns.Count == 0)
            {
                return;
            }
            const string sql = @"SELECT column_name ""COLUMN_NAME"",column_default ""COLUMN_DEFAULT"",is_nullable ""IS_NULLABLE"",data_type ""DATA_TYPE FROM"" information_schema.columns
                WHERE table_catalog = current_database() and table_schema = current_schema() and table_name= @tblName and (is_nullable = 'NO' or column_default is not null);";
            var tabelStruc = ExecuteDataTable(sql, CommandType.Text, new SqlParameter("@tblName", table));
            if (tabelStruc != null && tabelStruc.Rows.Count > 0)
            {
                for (var i = 0; i < tabelStruc.Rows.Count; i++)
                {
                    var colName = tabelStruc.Rows[i]["COLUMN_NAME"].To_String();
                    for (var k = 0; k < dt.Columns.Count; k++)
                    {
                        if (dt.Columns[k].ColumnName == colName)
                        {
                            if (tabelStruc.Rows[i]["COLUMN_DEFAULT"] != null && tabelStruc.Rows[i]["COLUMN_DEFAULT"].ToString() != "")
                            {
                                var defaultValue = tabelStruc.Rows[i]["COLUMN_DEFAULT"].ToString();
                                var indexDataType = defaultValue.LastIndexOf("::");
                                defaultValue = indexDataType > 0 ? defaultValue.Substring(0, indexDataType) : defaultValue;
                                dt.Columns[k].DefaultValue = defaultValue;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public virtual DataTable GetTableStruct(string tblName)
        {
            const string sql =
                    @"SELECT column_name ""COLUMN_NAME"",column_default ""COLUMN_DEFAULT"",is_nullable ""IS_NULLABLE"",data_type ""DATA_TYPE"" FROM information_schema.columns 

    where table_catalog = current_database() and table_schema = current_schema() and table_name = @tblName order by ordinal_position;";
            var dt = ExecuteDataTable(sql, CommandType.Text, new SqlParameter("@tblName", tblName));
            var ret = new DataTable { TableName = tblName };
            if (dt != null && dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    Type type;
                    var sqltype = dt.Rows[i]["DATA_TYPE"].To_String().ToLower();
                    var colName = dt.Rows[i]["COLUMN_NAME"].To_String();
                    var dtColumn = new DataColumn { ColumnName = colName };
                    switch (sqltype)
                    {
                        case "int":
                        case "integer":
                        case "bigint":
                        case "smallint":
                        case "tinyint":
                            type = typeof(int);
                            break;
                        case "decimal":
                        case "smalldecimal":
                            type = typeof(decimal);
                            break;
                        case "datetime":
                        case "date":
                        case "timestamp":
                        case "timestamp without time zone":
                        case "timestamp with time zone":
                            type = typeof(DateTime);
                            break;
                        case "numeric":
                        case "float":
                        case "real":
                            type = typeof(double);
                            break;
                        case "bit":
                            type = typeof(bool);
                            break;
                        default:
                            type = typeof(string);
                            break;
                    }

                    dtColumn.DataType = type;
                    if (dt.Rows[i]["COLUMN_DEFAULT"] != null && dt.Rows[i]["COLUMN_DEFAULT"].ToString() != "")
                    {
                        var defaultValue = dt.Rows[i]["COLUMN_DEFAULT"].ToString();
                        var indexDataType = defaultValue.LastIndexOf("::");
                        defaultValue = indexDataType > 0 ? defaultValue.Substring(1, indexDataType - 2) : defaultValue;
                        dtColumn.DefaultValue = defaultValue;
                    }
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

        public DbConnection OpenConnection()
        {
            DbConnection = DbConnection == null || DbConnection.State == ConnectionState.Closed ? new NpgsqlConnection(ConnectionString) : DbConnection;
            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
            SetConnectionSchema(DbConnection);
            return DbConnection;
        }

        public DbTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            DbTransaction = DbConnection.BeginTransaction(level);
            return DbTransaction;
        }

        private NpgsqlParameter[] ParseParamater(SqlParameter[] arrSqlPara)
        {
            if (arrSqlPara == null || arrSqlPara.Length == 0)
            {
                return null;
            }
            var paramaterCount = arrSqlPara.Length;
            var arrPostgresPara = new NpgsqlParameter[paramaterCount];
            for (var i = 0; i < paramaterCount; i++)
            {
                arrPostgresPara[i] = new NpgsqlParameter(arrSqlPara[i].ParameterName, arrSqlPara[i].Value ?? DBNull.Value);
            }
            return arrPostgresPara;
        }

        /// <summary>
        /// hàm set schema cho connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="schemaName"></param>
        private void SetConnectionSchema(NpgsqlConnection connection)
        {
            //Khi có thông tin schema thì call set command. Nếu không thì để schema mặc định (public)
            if (!string.IsNullOrEmpty(SchemaName))
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = string.Format(@"set schema '{0}'", SchemaName);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
        }

        private string ToString(object obj)
        {
            try
            {
                if (obj == null) return string.Empty;
                return obj.ToString().Trim();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
