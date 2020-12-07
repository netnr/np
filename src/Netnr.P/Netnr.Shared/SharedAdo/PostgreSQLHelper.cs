#if Full || AdoPostgreSQL

using Npgsql;
using System.Data;
using System.Linq;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// PostgreSQL操作类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 表批量写入
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <returns></returns>
        public int BulkCopyPostgreSQL(DataTable dt, string table)
        {
            return SafeConn(() =>
            {
                var columns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                string copyString = "COPY " + "\"" + table + "\"" + " (\"" + string.Join("\",\"", columns) + "\") FROM STDIN (FORMAT BINARY)";

                var connection = (NpgsqlConnection)Connection;
                using (var writer = connection.BeginTextImport(copyString))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var rv = string.Empty;
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (i > 0)
                            {
                                rv += ",";
                            }

                            var rc = dr[columns[i]].ToString();
                            if (rc.Contains(","))
                            {
                                rc = "\"" + rc.Replace("\"", "\"\"") + "\"";
                            }

                            rv += rc;
                        }
                        writer.WriteLine(rv);
                    }
                }

                return dt.Rows.Count;
            });
        }
    }
}

#endif