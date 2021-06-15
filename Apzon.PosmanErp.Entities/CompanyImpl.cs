using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Entities
{
    /// <summary>
    /// Thông tin công ty, và dữ liệu đăng ký hệ thống
    /// </summary>
    public class CompanyImpl
    {
        /// <summary>
        /// Thông tin domain đăng kí với hệ thống
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// Tên công ty/cửa hàng
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Email cửa hàng
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// người dùng quản trị hệ thống
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// Mật khẩu người dùng
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Số điện thoại 1
        /// </summary>
        public string Phone1 { get; set; }

        /// <summary>
        /// Số điện thoại 2
        /// </summary>
        public string Phone2 { get; set; }

        /// <summary>
        /// Địa chỉ công ty, cửa hàng
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Loại hình kinh doanh, loại sản phẩm kinh doanh
        /// </summary>
        public string CompanyType { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string ContactPerson { get; set; }

        public string UserRef { get; set; }

        /// <summary>
        /// thông tin version api
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// mã khuyến mãi của khách hàng
        /// </summary>
        public string PromoCode { get; set; }
    }
}
