using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services;
using ApzonIrsWeb.Entities;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using PosmanErp.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PosmanErp.Attributes
{
    /// <summary>
    /// lớp hổ trợ các xử lí liên quan tới jwt token
    /// </summary>
    public class ApzJwtTokenManager
    {
        /// <summary>
        /// scret key
        /// </summary>
        private const string Secret = "uumyVH5V5kQYNhZ2p8u73ovYHQrLq9YVSPr9YIqVO32uf35Dn67B8tXy1f/4JRxjUuCPYsaxa+lHi1OfcSdWoA==";

        /// <summary>
        /// hàm tạo jwt token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="databaseName"></param>
        /// <param name="userId"></param>
        /// <param name="expiryDate">format theo định dạng yyyyMMđ</param>
        /// <returns></returns>
        public static string GenerateToken(string username, string userId, DateTime expiryDate)
        {
            var key = Convert.FromBase64String(Secret);
            var securityKey = new SymmetricSecurityKey(key);
            var descriptor = new SecurityTokenDescriptor
            {
                //các tham số để validate token
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Name, userId), new Claim(ClaimTypes.Name, expiryDate.ToString("yyyyMMdd")), }),
                //exprired date của token. Lấy theo ngày hết hạn license
                Expires = expiryDate,
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        /// <summary>
        /// hàm đọc dữ liệu jwt token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            IdentityModelEventSource.ShowPII = true;
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            if (jwtToken == null)
                return null;
            var key = Convert.FromBase64String(Secret);
            var parameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token,
                  parameters, out securityToken);
            return principal;
        }

        /// <summary>
        /// Hàm validate jwt token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static HttpResult ValidateToken(string token)
        {
            try
            {
                //thông tin token khai báo trên hệ thống
                TokenUserInfoImpl systemTokenInfo;
                //lock dữ liệu để xử lí tránh xung đột giữa các request
                lock (GlobalData.TokenUserList)
                {
                    //khởi tạo lại dữ liệu trong trường hợp bị null
                    if (GlobalData.TokenUserList == null)
                    {
                        GlobalData.TokenUserList = new Dictionary<string, TokenUserInfoImpl>();
                    }
                    //nếu không tồn tại dữ liệu trên global thì check thông tin từ database
                    if (!GlobalData.TokenUserList.ContainsKey(token))
                    {
                        //khởi tạo common unit
                        var commonUnitOfWork = UnitOfWorkFactory.GetUnitOfWork(GlobalData.CommonConnectionString, GlobalData.CommonSchemaName, GlobalData.DatabaseSystemType);
                        //lấy dữ liệu token theo thông tin đã lưu ở database
                        var dataToken = commonUnitOfWork.User.GetInfoAccessToken(token);
                        if (!dataToken.IsNotNull() || Function.ToString(dataToken.Rows[0]["validated"]).Equals("N"))
                        {
                            return new HttpResult
                            {
                                msg_code = MessageCode.Error,
                                message = "Token không đúng vui lòng đăng nhập lại"
                            };
                        }

                        if(Function.ParseDateTimes(dataToken.Rows[0]["expiry_date"]) < DateTime.Now.Date)
                        {
                            return new HttpResult
                            {
                                msg_code = MessageCode.TokenExpired,
                                message = "Token hết hạn vui lòng đăng nhập lại"
                            };
                        }

                        if(!Function.ToString(dataToken.Rows[0]["user_name"]).Equals(GlobalData.RootUser))
                        {
                            #region validate login infomation theo open API

                            OpenApiHttpResult<OpenApiTokenInfo> openApiLoginResult;
                            using (var http = new HttpClientHelper<OpenApiHttpResult<OpenApiTokenInfo>>(GlobalData.OpenApiAdress, "Token"))
                            {
                                openApiLoginResult = http.Post("Validate", new { TokenId = token });
                            }

                            if (openApiLoginResult.MessageCode != 200)
                            {
                                return new HttpResult(MessageCode.TokenInvalid, "Vui lòng đăng nhập lại");
                            }

                            #endregion
                        }

                        //insert dữ liệu token vào global trong trường hợp bị mất token
                        GlobalData.TokenUserList.Add(token, new TokenUserInfoImpl
                        {
                            DatabaseName = Function.ToString(dataToken.Rows[0]["database_name"]),
                            DomainName = Function.ToString(dataToken.Rows[0]["domain_name"]),
                            UserName = Function.ToString(dataToken.Rows[0]["user_name"]).ToLower(),
                            ExpiryDate = Function.ParseDateTimes(dataToken.Rows[0]["expiry_date"]),
                            UserSign = Function.ParseInt(dataToken.Rows[0]["user_sign"]),
                            WhsCode = Function.ToString(dataToken.Rows[0]["whs_code"]),
                            BranchCode = Function.ParseInt(dataToken.Rows[0]["branch_code"]),
                            Validated = "Y"
                        });
                    }
                    //lấy thông tin token từ hệ thống
                    systemTokenInfo = GlobalData.TokenUserList[token];
                }
                return new HttpResult
                {
                    msg_code = MessageCode.Success
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult
                {
                    msg_code = MessageCode.Exception,
                    message = ex.Message
                };
            }
        }

        /// <summary>
        /// hàm gen jwt token scret key
        /// </summary>
        /// <returns></returns>
        public static string GenerateJwtTokenSecretKey()
        {
            var hmac = new HMACSHA256();
            return Convert.ToBase64String(hmac.Key);
        }
    }
}
