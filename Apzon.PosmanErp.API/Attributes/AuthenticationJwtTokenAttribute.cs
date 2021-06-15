using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PosmanErp.Attributes
{
    /// <summary>
    /// Atribute dùng để xác thực request theo jwt token
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthenticationJwtTokenAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Controller không kiểm tra jwt token
        /// </summary>
        private Dictionary<string, List<string>> MapActionNotCheckJwtToken = new Dictionary<string, List<string>>
        {
            {"user", new List<string> {"login", "getlistdatabase"}},
            {"commons", new List<string> { }},
            {"messagecode", new List<string> {"get"}},
            {"licensemanager", new List<string> {"gethwkey"}}
        };

        /// <summary>
        /// chứa danh sách các action được xử lý khi đăng nhập với config user
        /// </summary>
        private Dictionary<string, List<string>> MappingActionConfigUser = new Dictionary<string, List<string>>
        {
            {"user", new List<string> {}},
            {"auth", new List<string> {}},
            {"generalsetting", new List<string> {}},
            {"licensemanager", new List<string> {}}
        };

        /// <summary>
        /// hàm xử lí validate token
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            //lấy thông tin conteoller và action Name
            var controllerName = Function.ToString(actionContext.RouteData.Values["controller"]).ToLower();
            var actionName = Function.ToString(actionContext.RouteData.Values["action"]).ToLower();
            //Kiểm tra xem controller và action có kiểm tra jwt token hay không
            if (!MapActionNotCheckJwtToken.ContainsKey(controllerName) || !MapActionNotCheckJwtToken[controllerName].Exists(t => t.Equals("All") || t.Equals(actionName)))
            {
                #region lấy và validate dữ liệu authorization token theo jwt token

                var request = actionContext?.HttpContext?.Request;
                //lấy dữ liệu token từ request
                var header = request?.Headers;
                if (header == null || !header.ContainsKey("Authorization"))
                {
                    var response = new ObjectResult("Missing header autorization")
                    {
                        StatusCode = 401
                    };
                    actionContext.Result = response;
                    return;
                }

                var authHeader = header["Authorization"];
                //lấy thông tin jwt token từ authorization của request
                var accessToken = authHeader.FirstOrDefault();

                //kiểm tra dữ liệu bearer token
                if (string.IsNullOrEmpty(accessToken) || accessToken.Length <= 7 || !accessToken.StartsWith("Bearer "))
                {
                    var response = new ObjectResult("Invalid autorization scheme")
                    {
                        StatusCode = 401
                    };
                    actionContext.Result = response;
                    return;
                }

                var jwtToken = accessToken.Substring(7);
                if (string.IsNullOrEmpty(jwtToken))
                {
                    var response = new ObjectResult("Missing Token")
                    {
                        StatusCode = 203
                    };
                    actionContext.Result = response;
                    return;
                }

                #endregion

                //validate jwt token
                var validateTokenResult = ApzJwtTokenManager.ValidateToken(jwtToken);

                if (validateTokenResult.msg_code != MessageCode.Success)
                {
                    var response = new ObjectResult(validateTokenResult.message)
                    {
                        StatusCode = 403
                    };
                    actionContext.Result = response;
                    return;
                }

                ///trong trường hợp user là config user thì validate các chức năng được phép truy cập
                if(GlobalData.TokenUserList[jwtToken].UserName.Equals(GlobalData.RootUser))
                {
                    if(!MappingActionConfigUser.ContainsKey(controllerName) || MappingActionConfigUser[controllerName].Any() && !MappingActionConfigUser[controllerName].Contains(actionName))
                    {
                        var response = new ObjectResult("User don't have permission")
                        {
                            StatusCode = 405
                        };
                        actionContext.Result = response;
                        return;
                    }
                }
            }

            base.OnActionExecuting(actionContext);
        }

        //public override Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate cancellationToken)
        //{
        //    var x = ApzJwtTokenManager.GenerateJwtTokenSecretKey();
        //    //lấy thông tin controller và action Name
        //    var controllerName = Function.ToString(actionContext.RouteData.Values["controller"]);
        //    var actionName = Function.ToString(actionContext.RouteData.Values["action"]);
        //    //Kiểm tra action có kiểm tra jwt token hay không
        //    if (!MapActionNotCheckJwtToken.ContainsKey(controllerName) || !MapActionNotCheckJwtToken[controllerName].Exists(t => t.Equals("All") || t.Equals(actionName)))
        //    {
        //        #region lấy và validate dữ liệu authorization token theo jwt token
        //        var request = actionContext?.HttpContext?.Request;
        //        //lấy dữ liệu token từ request
        //        var header = request?.Headers;
        //        if (header == null || !header.ContainsKey("Authorization"))
        //        {
        //            var response = new ObjectResult("Missing header autorization")
        //            {
        //                StatusCode = 401
        //            };
        //            actionContext.Result = response;
        //            Task task = Task.Run(() => Thread.Sleep(1));
        //            return task;
        //        }

        //        var authHeader = header["Authorization"];
        //        //lấy thông tin jwt token từ authorization của request
        //        var accessToken = authHeader.FirstOrDefault();

        //        //kiểm tra dữ liệu bearer token
        //        if (string.IsNullOrEmpty(accessToken) || accessToken.Length <= 7 || !accessToken.StartsWith("Bearer "))
        //        {
        //            var response = new ObjectResult("Invalid autorization scheme")
        //            {
        //                StatusCode = 401
        //            };
        //            actionContext.Result = response;
        //            Task task = Task.Run(() => Thread.Sleep(1));
        //            return task;
        //        }

        //        var jwtToken = accessToken.Substring(7);
        //        if (string.IsNullOrEmpty(jwtToken))
        //        {
        //            var response = new ObjectResult("Missing Token")
        //            {
        //                StatusCode = 203
        //            };
        //            actionContext.Result = response;
        //            Task task = Task.Run(() => Thread.Sleep(1));
        //            return task;
        //        }
        //        #endregion
        //        //validate jwt token
        //        var validateTokenResult = ApzJwtTokenManager.ValidateToken(jwtToken);

        //        if (validateTokenResult.MsgCode != MessageCode.Success)
        //        {
        //            var response = new ObjectResult(validateTokenResult.Message)
        //            {
        //                StatusCode = 406
        //            };
        //            actionContext.Result = response;
        //            Task task = Task.Run(() => Thread.Sleep(1));
        //            return task;
        //        }
        //    }
        //    return base.OnActionExecutionAsync(actionContext, cancellationToken);
        //}
    }
}