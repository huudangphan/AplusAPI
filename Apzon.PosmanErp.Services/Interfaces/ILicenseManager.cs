using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.PosmanErp.Entities;
using System.Data;

namespace Apzon.PosmanErp.Services.Interfaces
{
    public interface ILicenseManager
    {
        /// <summary>
        /// hàm kiểm tra license của user theo database
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="domainName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        HttpResult CheckUserLicense(string domainName, string databaseName, string usercode);

        /// <summary>
        /// hàm lấy dữ liệu giấy phép trên hệ thống
        /// </summary>
        /// <param name="hardwareKey"></param>
        /// <returns></returns>
        HttpResult GetDataLicenseInfomation(string hardwareKey);

        /// <summary>
        /// hàm check dữ liệu trial của datanase. Trong trường hợp trial thì chỉ user đăng kí đăng nhập được hệ thống
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        HttpResult CheckTrialInfomation(string userName);

        /// <summary>
        /// hàm dùng để import license vào hệ thống
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="contentLicenseFile"></param>
        /// <param name="defaultBranch"></param>
        /// <param name="lstRemoveLicesenBranch"></param>
        /// <returns></returns>
        HttpResult ImportLicenseFile(string contentLicenseFile);

        /// <summary>
        /// update dữ liệu license của user 
        /// </summary>
        /// <param name="dsLicUser"></param>
        /// <returns></returns>
        HttpResult MappingUserLicense(DataSet dsLicUser);

        /// <summary>
        /// hàm kiểm tra dữ liệu user trong database
        /// </summary>
        /// <param name="dtUserLic"></param>
        /// <returns></returns>
        HttpResult CheckUserMapping(DataTable dtUserLic);

        /// <summary>
        /// hàm lấy danh sách user của DB hiện tại
        /// </summary>
        /// <returns></returns>
        DataTable GetListLocalUser();
    }
}
