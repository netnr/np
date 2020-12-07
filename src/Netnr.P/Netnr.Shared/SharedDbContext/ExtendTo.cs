# if Full || DbContext

using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Netnr.SharedAdo;

namespace Netnr.SharedDbContext
{
    /// <summary>
    /// EF上下文执行SQL扩展
    /// </summary>
    public static class ExtendTo
    {
        /// <summary>
        /// 执行SQL返回表
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet SqlQuery(this DbContext context, string sql, DbParameter[] parameters = null)
        {
            var connection = context.Database.GetDbConnection();
            var db = new DbHelper(connection);

            return db.SafeConn(() =>
            {
                return db.SqlQuery(sql, parameters);
            });
        }

        /// <summary>
        /// 执行SQL返回受影响行数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int SqlExecute(this DbContext context, string sql, DbParameter[] parameters = null)
        {
            var connection = context.Database.GetDbConnection();
            var db = new DbHelper(connection);

            return db.SafeConn(() =>
            {
                return db.SqlExecute(sql, parameters);
            });
        }

        /// <summary>
        /// 事务执行批量SQL，可自定义SQL分批大小
        /// </summary>
        /// <param name="context"></param>
        /// <param name="listSql">批量脚本</param>
        /// <param name="sqlBatchSize">脚本分批大小，单位：字节（byte），默认：1024 * 100 = 100KB</param>
        /// <returns></returns>
        public static int SqlExecute(this DbContext context, List<string> listSql, int sqlBatchSize = 1024 * 100)
        {
            var connection = context.Database.GetDbConnection();
            var db = new DbHelper(connection);

            return db.SafeConn(() =>
            {
                return db.SqlExecute(listSql, sqlBatchSize);
            });
        }

        /// <summary>
        /// 引用对象符号
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Ros(this DbContext context)
        {
            var pn = context.Database.ProviderName.ToLower();
            if (pn.Contains("mysql"))
            {
                return "`";
            }
            else if (pn.Contains("postgresql"))
            {
                return "\"";
            }
            return "";
        }
    }
}

#endif