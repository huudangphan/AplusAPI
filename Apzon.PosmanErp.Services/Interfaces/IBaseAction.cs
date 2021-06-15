using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;

namespace Apzon.PosmanErp.Services.Interfaces
{
    public interface IBaseAction<TEntity> where TEntity : class
    {
        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <returns></returns>
        HttpResult Get(SearchDocumentModel Search);

        /// <summary>
        /// Lấy dữ liệu theo code của record
        /// </summary>
        /// <returns></returns>
        HttpResult GetById(string id);

        /// <summary>
        /// Thêm mới dữ liệu
        /// </summary>
        /// <returns></returns>
        HttpResult Create(APlusObjectType objectType, APlusObjectType fromObjectType, TEntity document, int bplId,
            int userSign);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <returns></returns>
        HttpResult Update(APlusObjectType objectType, APlusObjectType fromObjectType, TEntity document, int bplId,
            int userSign);

        /// <summary>
        /// Xóa dữ liệu
        /// </summary>
        /// <returns></returns>
        HttpResult Delete(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId,
            int userSign);

        /// <summary>
        /// Document: Hủy phiếu
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="fromObjectType"></param>
        /// <param name="code"></param>
        /// <param name="bplId"></param>
        /// <param name="userSign"></param>
        /// <returns></returns>
        HttpResult Cancel(APlusObjectType objectType, APlusObjectType fromObjectType, string code, int bplId,
            int userSign);
    }
}