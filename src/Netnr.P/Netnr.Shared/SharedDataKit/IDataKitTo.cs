#if Full || DataKit

using System.Data;
using System.Collections.Generic;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// 获取数据库各类信息的接口定义
    /// </summary>
    public interface IDataKitTo
    {
        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <returns></returns>
        List<TableNameVM> GetTable();

        /// <summary>
        /// 获取表列信息
        /// </summary>
        /// <param name="listTableName">表名</param>
        /// <returns></returns>
        List<TableColumnVM> GetColumn(List<string> listTableName = null);

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        bool SetTableComment(string TableName, string TableComment);

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="FieldName">列名</param>
        /// <param name="FieldComment">列注释</param>
        /// <returns></returns>
        bool SetColumnComment(string TableName, string FieldName, string FieldComment);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="page">页码</param>
        /// <param name="rows">页量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序方式</param>
        /// <param name="listFieldName">查询列，默认为 *</param>
        /// <param name="whereSql">条件</param>
        /// <param name="total">返回总条数</param>
        /// <returns></returns>
        DataTable GetData(string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, out int total);

        /// <summary>
        /// 查询数据库环境信息
        /// </summary>
        /// <returns></returns>
        DEIVM GetDEI();
    }
}

#endif