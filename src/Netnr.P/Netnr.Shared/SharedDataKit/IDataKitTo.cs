#if Full || DataKit

using System;
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
        /// 根据连接字符串获取数据库名
        /// </summary>
        /// <returns></returns>
        string DefaultDatabaseName();

        /// <summary>
        /// 获取库
        /// </summary>
        /// <returns></returns>
        List<DatabaseVM> GetDatabase();

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        List<TableVM> GetTable(string DatabaseName);

        /// <summary>
        /// 表DDL
        /// </summary>
        /// <param name="filterTableName">过滤表名，英文逗号分隔，为空时默认所有表</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        Dictionary<string, string> GetTableDDL(string filterTableName = null, string DatabaseName = null);

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="filterTableName">过滤表名，英文逗号分隔，为空时默认所有表</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        List<ColumnVM> GetColumn(string filterTableName = null, string DatabaseName = null);

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        bool SetTableComment(string TableName, string TableComment, string DatabaseName = null);

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="ColumnName">列名</param>
        /// <param name="ColumnComment">列注释</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        bool SetColumnComment(string TableName, string ColumnName, string ColumnComment, string DatabaseName = null);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="sql">脚本</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        Tuple<DataSet, object> ExecuteSql(string sql, string DatabaseName = null);

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
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        Tuple<DataTable, int> GetData(string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, string DatabaseName = null);

        /// <summary>
        /// 查询数据库环境信息
        /// </summary>
        /// <returns></returns>
        DEIVM GetDEI();
    }
}

#endif