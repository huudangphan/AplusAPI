using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApzonIrsWeb.Entities
{
    public class OpenApiHttpResult<TEntity> where TEntity : class
    {
        public int MessageCode { get; set; }

        public string Message { get; set; }

        public TEntity Content { get; set; }
    }

    public class OpenApiTokenInfo
    {
        public string Token { get; set; }

        public string Email { get; set; }

        public DateTime ExpiredTime { get; set; }

        public string Status { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }
    }


    public class OpenApiUserInfo
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public class OpenApiTokenResetPass
    {
        public string TokenId { get; set; }
        public DateTime ExpiredTime { get; set; }
    }

    public class OpenApiTokenNewPass
    {
        public string NewPassword { get; set; }
        public DateTime ExpiredTime { get; set; }
    }
}
