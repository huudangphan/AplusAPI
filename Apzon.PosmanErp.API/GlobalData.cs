using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Services;
using ApzonIrsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp
{
    public static class GlobalData
    {
        #region Connection Information

        /// <summary>
        /// User database common của hệ thống
        /// </summary>
        public static string CommonDbUserName { get; set; }
        /// <summary>
        /// Password user common database của hệ thống
        /// </summary>
        public static string CommonDbPassword { get; set; }
        /// <summary>
        /// Version của cơ sở dữ liệu
        /// </summary>
        public static string CommonDbServerType { get; set; }
        /// <summary>
        /// Ngôn ngữ sử dụng cửa cơ sở dữ liệu
        /// </summary>
        public static string CommonLanguage { get; set; }
        /// <summary>
        /// Địa chỉ database server của hệ thống
        /// </summary>
        public static string CommonDbServer { get; set; }

        /// <summary>
        /// Thông tin port kết nối đến database db server
        /// </summary>
        public static string CommonDbPort { get; set; }

        /// <summary>
        /// Tên common database
        /// </summary>
        public static string CommonDb { get; set; }

        /// <summary>
        /// số lượng tối da request trong hàng đợi. Vượt quá số lượng này các request tiếp theo sẽ bị reject
        /// </summary>
        public static int QueueRequestWait { get; set; }
        /// <summary>
        /// số lương tối đa request xử lí cùng lúc. Vượt quá số lượng này các request tiếp theo sẽ được đưa vào hàng đợi
        /// </summary>
        public static int MaxRequestProcess { get; set; }
        /// <summary>
        /// dữ liệu time out của các request ở trong hàng đợi
        /// </summary>
        public static int RequestTimeout { get; set; }

        /// <summary>
        /// thông tin OpenApi
        /// </summary>
        public static string OpenApiAdress { get; set; }

        /// <summary>
        /// Loại database system sử dụng trên hệ thống
        /// </summary>
        public static DatabaseSystemType DatabaseSystemType { get; set; }

        /// <summary>
        /// connection string
        /// </summary>
        public static string CommonConnectionString { get; set; }

        /// <summary>
        /// common schema
        /// </summary>
        public static string CommonSchemaName { get; set; }

        /// <summary>
        /// Lưu thông tin database và connectionstring
        /// </summary>
        public static Dictionary<string, ConnectionData> PosWorkList = new Dictionary<string, ConnectionData>();

        /// <summary>
        /// danh sách token=> user infomation 
        /// </summary>
        public static Dictionary<string, TokenUserInfoImpl> TokenUserList = new Dictionary<string, TokenUserInfoImpl>();

        #endregion

        /// <summary>
        /// chứa giá trị link folder chạy web api
        /// </summary>
        public static string WebRootPath { get; set; }

        /// <summary>
        /// user system admin để config và import license khi hệ thống chưa có license
        /// </summary>
        public static string RootUser { get; set; }

        /// <summary>
        /// Pass user root
        /// </summary>
        public static string RootPass { get; set; }
        /// <summary>
        /// Phiên bản hiện tại của API
        /// </summary>
        public static string version = "1.02.0";
    }

    /// <summary>
    /// object chữa dữ liệu conection string của 1 schema
    /// </summary>
    public class ConnectionData
    {
        /// <summary>
        /// hàm khởi tạo
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="schemaName"></param>
        public ConnectionData(string connectionString, string schemaName)
        {
            ConnectionString = connectionString;
            SchemaName = schemaName;
        }

        /// <summary>
        /// connection string đến database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// schemna trong database connect tới
        /// </summary>
        public string SchemaName { get; set; }
     
    }
}
