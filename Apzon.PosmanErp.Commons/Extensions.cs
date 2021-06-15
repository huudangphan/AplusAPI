using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Commons
{
    /// <summary>
    /// chứa các extenstion trên hệ thống
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// check tồn tạo column trong table
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static bool IsExistsColumn(this DataTable dt, string colName)
        {
            try
            {
                if (null != dt && dt.Columns.Contains(colName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// check datanull
        /// dt = null hoặc không có row
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsNotNull(this DataTable dt)
        {
            try
            {
                return null != dt && dt.Rows.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// check dataset null
        /// có giá trị null hoặc không có table nào
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static bool IsNotNull(this DataSet ds)
        {
            try
            {
                return null != ds && ds.Tables.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
