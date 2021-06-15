using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.Libraries.HDBConnection
{
    /// <summary>
    /// apzon postgesql bulkcopy
    /// </summary>
    public class ApzNpgsqlBulk : IDisposable
    {
        /// <summary>
        /// connection string
        /// </summary>
        private NpgsqlConnection _npgsqlConnection;

        /// <summary>
        /// table copy dữ liệu vào
        /// </summary>
        public string TargetTable { get; set; }

        /// <summary>
        /// chứa danh sách mapping column khi copy data
        /// </summary>
        public List<ApzColumnMapping> ColumnMappings = new List<ApzColumnMapping>();

        /// <summary>
        /// hàm khởi tạo
        /// </summary>
        /// <param name="npgsqlConnection"></param>
        public ApzNpgsqlBulk(NpgsqlConnection npgsqlConnection)
        {
            _npgsqlConnection = npgsqlConnection;
        }

        /// <summary>
        /// insert data from table source to target table
        /// </summary>
        /// <param name="dtSource"></param>
        public void BulkInsert(DataTable dtSource)
        {
            if(dtSource == null || dtSource.Rows.Count == 0)
            {
                return;
            }
            var sqlQuery = string.Format(@"insert into {0}", TargetTable);
            var lstParamater = new List<NpgsqlParameter>();
            if (ColumnMappings.Any())
            {
                sqlQuery = string.Format(@"{0}({1})", sqlQuery, string.Join(",", ColumnMappings.Select(t=>string.Format(@"""{0}""", t.TargetField))));
            }
            sqlQuery += @"
                        VALUES
";
            var lstInsertQuery = new List<string>();
            for (var i = 0; i < dtSource.Rows.Count; i++)
            {
                if (ColumnMappings.Any())
                {
                    lstInsertQuery.Add(string.Format(@"({0})", string.Join(",", ColumnMappings.Select(t => string.Format(@"@__{0}_{1}", t.SourceField, i)))));
                    for(var j = 0; j < ColumnMappings.Count; j++)
                    {
                        lstParamater.Add(new NpgsqlParameter(string.Format(@"@__{0}_{1}", ColumnMappings[j].SourceField, i), dtSource.Rows[i][ColumnMappings[j].SourceField]));
                    }
                }
                else
                {
                    lstInsertQuery.Add(string.Format(@"({0})", string.Join(",", dtSource.Columns.Cast<DataColumn>().Select(t => string.Format(@"@__{0}_{1}", t.ColumnName, i)))));
                    for (var j = 0; j < dtSource.Columns.Count; j++)
                    {
                        lstParamater.Add(new NpgsqlParameter(string.Format(@"@__{0}_{1}", dtSource.Columns[j].ColumnName, i), dtSource.Rows[i][j]));
                    }
                }
            }
            sqlQuery = string.Format(@"{0} {1};", sqlQuery, string.Join(",", lstInsertQuery));
            using (var command = _npgsqlConnection.CreateCommand())
            {
                command.CommandTimeout = 60000;
                command.CommandText = sqlQuery;
                command.Parameters.AddRange(lstParamater.ToArray());
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            
        }
    }

    public class ApzColumnMapping
    {
        /// <summary>
        /// source column table
        /// </summary>
        public string SourceField { get; set; }

        /// <summary>
        /// tarrget column table
        /// </summary>
        public string TargetField { get; set; }

        /// <summary>
        /// hàm khởi tạo
        /// </summary>
        /// <param name="sourceField"></param>
        /// <param name="targetField"></param>
        public ApzColumnMapping(string sourceField,  string targetField)
        {
            SourceField = sourceField;
            TargetField = targetField;
        }
    }
}
