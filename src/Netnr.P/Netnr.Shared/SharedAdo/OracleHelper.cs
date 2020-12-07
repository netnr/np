#if Full || AdoOracle

using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// Oracle操作类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 表批量写入
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <returns></returns>
        public int BulkCopyOracle(DataTable dt, string table)
        {
            return SafeConn(() =>
            {
                var connection = (OracleConnection)Connection;
                using var bulk = new OracleBulkCopy(connection)
                {
                    DestinationTableName = table,
                    BatchSize = dt.Rows.Count
                };

                foreach (DataColumn dc in dt.Columns)
                {
                    bulk.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }

                bulk.WriteToServer(dt);

                return dt.Rows.Count;
            });
        }

        /// <summary>
        /// 执行SQL语句，返回打印信息
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns>返回打印信息</returns>
        public List<string> SqlPrintOracle(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var connection = (OracleConnection)Connection;

                var listPrint = new List<string>();
                connection.InfoMessage += (s, e) => listPrint.Add(e.Message);

                var cmd = GetCommand(sql, parameters);
                cmd.ExecuteNonQuery();

                return listPrint;
            });
        }
    }
}

#endif