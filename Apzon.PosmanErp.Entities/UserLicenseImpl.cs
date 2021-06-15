using Apzon.PosmanErp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Entities
{
    /// <summary>
    /// entity dùng để lưu thông tin license của user và hệ thống
    /// </summary>
    public class UserLicenseImpl
    {
        /// <summary>
        /// user check license
        /// </summary>
        public string user_code { get; set; }

        /// <summary>
        /// trạng thái được mapping license của user
        /// </summary>
        public bool is_user_license { get; set; }

        /// <summary>
        /// đánh dấu user được mapping vào super license
        /// </summary>
        public LicenseRule user_rule { get; set; }

        /// <summary>
        /// đánh dấu license chứa super license
        /// </summary>
        public LicenseRule rule { get; set; }

        /// <summary>
        /// danh sách license được mapping cho user
        /// </summary>
        public List<LicenseInfoImpl> list_license_type { get; set; }

        /// <summary>
        /// danh sách access form của license file
        /// </summary>
        public List<string> list_access_form { get; set; }

        /// <summary>
        /// thời gian hết hạn nhỏ nhất của tất cả các license
        /// </summary>
        public DateTime expiry_date { get; set; }
    }
}
