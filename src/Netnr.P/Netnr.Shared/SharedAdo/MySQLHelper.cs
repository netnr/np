#if Full || AdoFull || AdoMySQL

using MySqlConnector;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// MySQL操作类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 表批量写入
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="bulkCopy">设置表复制对象</param>
        /// <param name="openTransaction">开启事务，默认不开启</param>
        /// <returns></returns>
        public int BulkCopyMySQL(DataTable dt, Action<MySqlBulkCopy> bulkCopy = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                var connection = (MySqlConnection)Connection;
                MySqlTransaction transaction = openTransaction ? (MySqlTransaction)(Transaction = connection.BeginTransaction()) : null;

                var bulk = new MySqlBulkCopy(connection, transaction)
                {
                    DestinationTableName = dt.TableName,
                    BulkCopyTimeout = 3600
                };

                bulkCopy?.Invoke(bulk);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    bulk.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, dt.Columns[i].ColumnName));
                }

                bulk.WriteToServer(dt);

                transaction?.Commit();

                return dt.Rows.Count;
            });
        }

        /// <summary>
        /// 表批量写入
        /// 根据行数据 RowState 状态新增、修改
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
        /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
        /// <param name="openTransaction">开启事务，默认开启</param>
        /// <returns></returns>
        public int BulkBatchMySQL(DataTable dt, string sqlEmpty = null, Action<MySqlDataAdapter> dataAdapter = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                var connection = (MySqlConnection)Connection;
                MySqlTransaction transaction = openTransaction ? (MySqlTransaction)(Transaction = connection.BeginTransaction()) : null;

                var cb = new MySqlCommandBuilder();
                if (string.IsNullOrWhiteSpace(sqlEmpty))
                {
                    var sntn = SqlSNTN(dt.TableName, dt.Namespace, SharedEnum.TypeDB.MySQL);
                    sqlEmpty = SqlEmpty(sntn);
                }

                cb.DataAdapter = new MySqlDataAdapter
                {
                    SelectCommand = new MySqlCommand(sqlEmpty, connection, transaction)
                };
                cb.ConflictOption = ConflictOption.OverwriteChanges;

                var da = new MySqlDataAdapter
                {
                    InsertCommand = (MySqlCommand)cb.GetInsertCommand(true),
                    UpdateCommand = (MySqlCommand)cb.GetUpdateCommand(true)
                };
                da.InsertCommand.CommandTimeout = 300;
                da.UpdateCommand.CommandTimeout = 300;

                //执行前修改
                dataAdapter?.Invoke(da);

                var num = da.Update(dt);

                transaction?.Commit();

                return num;
            });
        }

        /// <summary>
        /// MySQL写入预检
        /// </summary>
        /// <returns></returns>
        public int PreCheckMySQL()
        {
            var drs = SqlExecuteReader("SHOW VARIABLES").Item1.Tables[0].Select();

            var dicVar1 = new Dictionary<string, string>
            {
                { "local_infile","是否允许加载本地数据，BulkCopy 需要开启"},
                { "innodb_lock_wait_timeout","innodb 的 dml 操作的行级锁的等待时间，事务等待获取资源等待的最长时间，BulkCopy 量大超时设置，单位：秒"},
                { "max_allowed_packet","传输的 packet 大小限制，最大 1G，单位：B"}
            };

            var listBetterSql = new List<string>();
            foreach (var key in dicVar1.Keys)
            {
                var dr = drs.FirstOrDefault(x => x[0].ToString() == key);
                if (dr != null)
                {
                    var val = dr[1]?.ToString();
                    switch (key)
                    {
                        case "local_infile":
                            if (val != "ON")
                            {
                                //ON 开启，OFF 关闭
                                listBetterSql.Add("SET GLOBAL local_infile = ON");
                            }
                            break;
                        case "innodb_lock_wait_timeout":
                            if (Convert.ToInt32(val) < 600)
                            {
                                //10 分钟超时
                                listBetterSql.Add("SET GLOBAL innodb_lock_wait_timeout = 600");
                            }
                            break;
                        case "max_allowed_packet":
                            if (Convert.ToInt32(val) != 1073741824)
                            {
                                //传输的 packet 大小 1G
                                listBetterSql.Add("SET GLOBAL max_allowed_packet = 1073741824");
                            }
                            break;
                    }
                }
            }

            if (listBetterSql.Count > 0)
            {
                Console.WriteLine($"\n执行优化脚本：\n{string.Join(Environment.NewLine, listBetterSql)}\n");
                SqlExecuteNonQuery(listBetterSql);
            }

            return listBetterSql.Count;
        }

    }
}

#endif