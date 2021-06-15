using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.PosmanErp.Entities;
using ApzonIrsWeb.Entities;

namespace Apzon.PosmanErp.Services.Interfaces
{
    public interface IUser : IBaseAction<DataSet>
    {
        /// <summary>
        /// xóa dữ liệu login của các user cùng database và cùng user đăng nhập
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        HttpResult RemoveOldToken(string userCode);

        /// <summary>
        /// hàm thêm mới dữ liệu token vào hệ thống khi login
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="userCode"></param>
        /// <param name="databaseName"></param>
        /// <param name="domainName"></param>
        /// <param name="expiryDate"></param>
        /// <param name="userSign"></param>
        void AddNewToken(string token, string userCode, string databaseName, string domainName, DateTime expiryDate, int userSign);

        /// <summary>
        /// lấy dữ liệu token từ database
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        DataTable GetInfoAccessToken(string accessToken);

        /// Lấy thông tin người dùng theo code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        HttpResult GetByCode(string code);

        /// <summary>
        /// hàm lấy danh sách database trên hệ thống
        /// </summary>
        /// <returns></returns>
        HttpResult GetListDatabase();

        /// <summary>
        /// Lấy danh sách company của user
        /// </summary>
        /// <param name="user_code"></param>
        /// <returns></returns>
        HttpResult GetListCompany(string user_code);

        /// <summary>
        /// Lấy danh sách config front end của user
        /// </summary>
        /// <param name="user_code"></param>
        /// <returns></returns>
        HttpResult GetListConfig(string user_code);

        /// <summary>
        /// Cập nhật mật khẩu mới
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        HttpResult UpdatePassword(DataTable dtval);

        /// <summary>
        /// Cập nhật trạng thái user
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        HttpResult UpdateStatus(DataTable dtval);

        /// <summary>
        /// Cập nhật config user, nếu chưa có thì thêm mới
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        HttpResult UpdateConfig(DataTable dtval);

        /// <summary>
        /// Xóa config interface user 
        /// </summary>
        /// <param name="dtval"></param>
        /// <returns></returns>
        HttpResult DeleteConfig(DataTable dtval);
        /// <summary>
        /// Cập nhật phiên bản database 
        /// </summary>
        /// <param name="version">phiên bản hiện tại của API</param>
        /// <returns></returns>
        HttpResult UpdateAuto(string version);
    }
}
