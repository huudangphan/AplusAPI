using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Entities
{
	public class LogInstance
	{
		/// <summary>
		/// Key được dùng để lọc lấy bản ghi lưu Log
		/// </summary>
		public string Key { get; set; }
		/// <summary>
		/// Key được dùng để lọc khi Show Difference
		/// </summary>
		public string LineKey { get; set; }
		/// <summary>
		/// (Bắt buộc) Bảng lưu logInstance key (có thể cùng hoặc khác bảng log CHA). Mặc định object đầu tiên có TableLogInstanceKey = HistoryTableName sẽ được lấy làm bảng log cha
		/// </summary>
		public string TableLogInstanceKey { get; set; }
		/// <summary>
		/// (Bắt buộc)Trường lưu LogInstance
		/// </summary>
		public string LogInstanceKey { get; set; }

		/// <summary>
		/// (Bắt buộc)Bảng chính
		/// </summary>
		public string TableName { get; set; }

		/// <summary>
		/// (Bắt buộc)Bảng lưu dữ liệu lịch sử
		/// </summary>
		public string HistoryTableName { get; set; }

		/// <summary>
		/// Trường người tạo (User ID)
		/// </summary>
		public string UserSignKey { get; set; }
		/// <summary>
		/// (Bắt buộc) Trường người sửa  (User ID)
		/// </summary>
		public string UserSign2Key { get; set; }
		/// <summary>
		/// Trường người tạo (User Name)
		/// </summary>
		public string UserSignCodeKey { get; set; }
		/// <summary>
		/// Trường người sửa (User Name)
		/// </summary>
		public string UserSignCode2Key { get; set; }
		/// <summary>
		/// Trường ngày tạo
		/// </summary>
		public string CreateDateKey { get; set; }
		/// <summary>
		/// (bắt buộc)Trường ngày sửa 
		/// </summary>
		public string UpdateDateKey { get; set; }
		/// <summary>
		/// Trường lưu giờ tạo
		/// </summary>
		public string CreateTimeKey { get; set; }
		/// <summary>
		/// Trường lưu giờ sửa
		/// </summary>
		public string UpdateTimeKey { get; set; }
		/// <summary>
		/// Những trường không muốn hiển thị trên Log Differences (vì 1 lí do nào đó)
		/// </summary>
		public string[] NotShowLogKey { get; set; }
	}
}
