using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Entities
{
    /// <summary>
    /// thông tin connection database
    /// </summary>
    public class DatabaseConnectionImpl
    {
        /// <summary>
        /// Thông tin database server
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Thông tin user đăng nhập database
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// thông tin mật khẩu đăng nhập
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// database name
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
