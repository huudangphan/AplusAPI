using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApzonIrsWeb.Entities
{
    public class TokenUserInfoImpl
    {
        public string UserName { get; set; }

        public string DatabaseName { get; set; }

        public string DomainName { get; set; }

        public string UserId { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int UserSign { get; set; }

        public string WhsCode { get; set; }

        public int BranchCode { get; set; }

        public string Validated { get; set; }
    }
}
