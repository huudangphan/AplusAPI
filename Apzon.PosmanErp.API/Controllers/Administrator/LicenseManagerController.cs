using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosmanErp.Controllers.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PosmanErp.Controllers.Administrator
{
    /// <summary>
    /// api license manager
    /// </summary>
    public class LicenseManagerController : BaseApiController<DataSet>
    {
        /// <summary>
        /// khởi tạo controller
        /// </summary>
        /// <param name="accessToken"></param>
        public LicenseManagerController(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// api import license cho hệ thống
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResult Import([FromBody]dynamic dataLic)
        {
            var commonUnit = GetCommonUnitOfWork();
            return commonUnit.LicenseManager.ImportLicenseFile(Function.ToString(dataLic.license_info));
        }

        /// <summary>
        /// api update license cho 1 user 
        /// </summary>
        /// <param name="dsUserLic"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult UpdateUsersLicense([FromBody]DataSet dsUserLic)
        {
            try
            {
                //kiểm tra dữ liệu đầu vào
                if(dsUserLic == null || dsUserLic.Tables.Count != 2 || !dsUserLic.Tables[0].IsNotNull())
                {
                    return new HttpResult(MessageCode.DataNotProvide, "Chưa cung cấp dữ liệu giấy phép của người dùng");
                }
                //validate thông tin người dùng trên database hiện tại để mapping license.
                //chỉ có thể update thông tin license của người dùng có trên database hiện tại
                var userInfo = UnitOfWork.LicenseManager.CheckUserMapping(dsUserLic.Tables["users"]);
                if(userInfo.msg_code != MessageCode.Success)
                {
                    return userInfo;
                }
                var commonUnit = GetCommonUnitOfWork();
                //thực hiện lưu mapping license trên database common
                return commonUnit.LicenseManager.MappingUserLicense(dsUserLic);
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        /// <summary>
        /// lấy thông tin license trên hệ thống
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResult Get()
        {
            try
            {
                //khởi tạo commonunit
                var commonUnit = GetCommonUnitOfWork();
                //lấy dữ liệu license và danh sách user mapping license trên toàn hệ thống
                var licenseInfo = commonUnit.LicenseManager.GetDataLicenseInfomation(Function.GetHardwareKey());
                if(licenseInfo.msg_code != MessageCode.Success)
                {
                    return licenseInfo;
                }
                //lấy danh sách người dùng trên database hiện tại đang đăng nhập
                var dtLocalUserLicense = UnitOfWork.LicenseManager.GetListLocalUser();
                dtLocalUserLicense.TableName = "database_users";
                var dsLic = licenseInfo.content as DataSet;
                dsLic.Tables.Add(dtLocalUserLicense);
                return new HttpResult
                {
                    msg_code = MessageCode.Success,
                    content = dsLic,
                    message = licenseInfo.message
                };
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return new HttpResult(MessageCode.Error, ex.Message);
            }
        }

        [HttpGet]
        public HttpResult GetHwKey()
        {
            return new HttpResult(MessageCode.Success, obj: Function.GetHardwareKey());
        }
    }
}
