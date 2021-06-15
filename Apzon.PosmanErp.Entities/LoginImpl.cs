using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Entities
{
    /// <summary>
    /// Entity thông tin đăng nhập hệ thống của user
    /// </summary>
    public class LoginImpl
    {
        /// <summary>
        /// Domain của cửa hàng
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Database đăng nhập
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// UserName đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// mật khẩu của user đăng nhập
        /// </summary>
        public string Password { get; set; }
    }
}
