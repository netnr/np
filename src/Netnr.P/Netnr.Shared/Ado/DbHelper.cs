#if Full || Ado || AdoFull

using System.ComponentModel;

namespace Netnr
{
    /// <summary>
    /// Db帮助类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        public DbConnection Connection { get; }

        /// <summary>
        /// 事务
        /// </summary>
        public DbTransaction Transaction { get; set; }

        /// <summary>
        /// 执行记录
        /// </summary>
        public List<DbCommand> CommandRecord { get; set; } = new List<DbCommand>();

        /// <summary>
        /// 执行终止
        /// </summary>
        /// <param name="ACID">可选，默认当前</param>
        /// <returns></returns>
        public void CommandAbort(string ACID = null)
        {
            if (string.IsNullOrWhiteSpace(ACID))
            {
                ACID = Activity.Current.Id;
            }

            for (int i = CommandRecord.Count - 1; i >= 0; i--)
            {
                var cmd = CommandRecord[i];
                if (cmd.Site.Name.StartsWith(ACID))
                {
                    cmd.Cancel();
                    CommandRecord.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 执行添加
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public void CommandAdd(DbCommand cmd)
        {
            var ACID = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
            if (CommandRecord.Any(x => x.Site.Name == ACID))
            {
                ACID += "/" + Guid.NewGuid().ToString("N");
            }

            cmd.Site = new DBCSite
            {
                Name = ACID
            };

            CommandRecord.Add(cmd);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbConnection">连接对象</param>
        public DbHelper(DbConnection dbConnection)
        {
            Connection = dbConnection;
        }

        /// <summary>
        /// 执行（查询、新增、修改、删除等）
        /// </summary>
        /// <param name="sql">SQL语句，支持多条</param>
        /// <param name="parameters">带参</param>
        /// <param name="func">回调</param>
        /// <param name="openTransaction">开启事务，默认</param>
        /// <returns>返回 表数据、受影响行数、表结构</returns>
        public ValueTuple<DataSet, int, DataSet> SqlExecuteReader(string sql, DbParameter[] parameters = null, Func<DbCommand, DbCommand> func = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                Transaction = openTransaction ? Connection.BeginTransaction() : null;

                var dsTable = new DataSet();
                var dsSchema = new DataSet();
                int recordsAffected = -1;

                var isOracle = Connection.GetType().Name.ToLower().Contains("oracle");
                var isSplit = false;
                if (isOracle)
                {
                    isSplit = !SqlParserBeginEnd(sql);
                }

                if (isOracle && isSplit)
                {
                    var listSql = sql.Split(';').ToList();
                    var hasRa = false;
                    var ra = 0;

                    foreach (var txt in listSql)
                    {
                        if (string.IsNullOrWhiteSpace(txt))
                        {
                            continue;
                        }

                        var dbc = GetCommand(txt, parameters);
                        if (func != null)
                        {
                            dbc = func(dbc);
                        }
                        var eds = dbc.ExecuteDataSet(tableLoad: false);

                        while (eds.Item1.Tables.Count > 0)
                        {
                            var dt = eds.Item1.Tables[0];
                            dt.TableName = $"table{dsTable.Tables.Count + 1}";
                            eds.Item1.Tables.RemoveAt(0);
                            dsTable.Tables.Add(dt);
                        }

                        if (eds.Item2 != -1)
                        {
                            hasRa = true;
                            ra += eds.Item2;
                        }

                        while (eds.Item3.Tables.Count > 0)
                        {
                            var dt = eds.Item3.Tables[0];
                            dt.TableName = $"table{dsSchema.Tables.Count + 1}";
                            eds.Item3.Tables.RemoveAt(0);
                            dsSchema.Tables.Add(dt);
                        }
                    }

                    if (hasRa)
                    {
                        recordsAffected = ra;
                    }
                }
                else
                {
                    var cmd = GetCommand(sql, parameters);
                    if (func != null)
                    {
                        cmd = func(cmd);
                    }

                    var eds = cmd.ExecuteDataSet(tableLoad: false);
                    dsTable = eds.Item1;
                    recordsAffected = eds.Item2;
                    dsSchema = eds.Item3;
                }

                Transaction?.Commit();

                return new ValueTuple<DataSet, int, DataSet>(dsTable, recordsAffected, dsSchema);
            });
        }

        /// <summary>
        /// 查询 读取行
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="readRow">行数据</param>
        /// <param name="emptyTable">空表</param>
        public void SqlExecuteDataRow(string sql, Action<object[]> readRow, Action<DataTable> emptyTable = null)
        {
            SafeConn(() =>
            {
                var isOracle = Connection.GetType().Name.ToLower().Contains("oracle");
                var isSplit = false;
                if (isOracle)
                {
                    isSplit = !SqlParserBeginEnd(sql);
                }

                var listSql = new List<string>();
                if (isOracle && isSplit)
                {
                    listSql = sql.Split(';').ToList();
                }
                else
                {
                    listSql.Add(sql);
                }

                foreach (var txt in listSql)
                {
                    if (string.IsNullOrWhiteSpace(txt))
                    {
                        continue;
                    }

                    var cmd = GetCommand(sql, timeout: 600);
                    cmd.ExecuteDataRow(readRow, emptyTable);
                }
            });
        }

        /// <summary>
        /// 执行 返回首行首列
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns></returns>
        public object SqlExecuteScalar(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var dbc = GetCommand(sql, parameters);
                var obj = dbc.ExecuteScalar();
                return obj;
            });
        }

        /// <summary>
        /// 执行 返回受影响行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <returns></returns>
        public int SqlExecuteNonQuery(string sql, DbParameter[] parameters = null)
        {
            return SafeConn(() =>
            {
                var dbc = GetCommand(sql, parameters);
                var num = dbc.ExecuteNonQuery();
                return num;
            });
        }

        /// <summary>
        /// 执行（批量、事务）
        /// </summary>
        /// <param name="listSql">SQL语句</param>
        /// <param name="sqlBatchSize">脚本分批大小，单位：字节（byte），默认：1024 * 100 = 100KB</param>
        /// <returns>返回受影响行数</returns>
        public int SqlExecuteNonQuery(List<string> listSql, int sqlBatchSize = 1024 * 100)
        {
            return SafeConn(() =>
            {
                Transaction = Connection.BeginTransaction();
                var num = 0;

                var listBatchSql = new List<string>();
                StringBuilder sbsql = new();
                var currSqlSize = 0;
                foreach (var sql in listSql)
                {
                    currSqlSize += Encoding.Default.GetBytes(sql).Length;
                    sbsql.AppendLine(sql.TrimEnd(';') + ";");
                    if (currSqlSize > sqlBatchSize)
                    {
                        listBatchSql.Add(sbsql.ToString());
                        sbsql.Clear();
                        currSqlSize = 0;
                    }
                }
                if (currSqlSize != 0)
                {
                    listBatchSql.Add(sbsql.ToString());
                }

                foreach (var bs in listBatchSql)
                {
                    var dbc = GetCommand(bs);
                    num += dbc.ExecuteNonQuery();
                }

                Transaction.Commit();
                return num;
            });
        }

        /// <summary>
        /// 拿到 DbCommand
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">带参</param>
        /// <param name="timeout">超时，默认 300 秒</param>
        /// <param name="commandType">类型</param>
        /// <returns></returns>
        public DbCommand GetCommand(string sql, DbParameter[] parameters = null, int timeout = 300, CommandType commandType = CommandType.Text)
        {
            var cmd = Connection.CreateCommand().ToFix();

            if (Transaction != null)
            {
                cmd.Transaction = Transaction;
            }

            cmd.CommandTimeout = timeout;
            cmd.CommandType = commandType;
            cmd.CommandText = sql;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            //记录添加
            CommandAdd(cmd);

            return cmd;
        }

        class DBCSite : ISite
        {
            public DBCSite(string name = null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = Guid.NewGuid().ToString("N");
                }
                Name = name;
            }

            public IComponent Component => null;

            public IContainer Container => null;

            public bool DesignMode => false;

            public string Name { get; set; }

            public object GetService(Type serviceType)
            {
                return null;
            }
        }

        /// <summary>
        /// 连接包装
        /// </summary>
        /// <param name="action"></param>
        public void SafeConn(Action action)
        {
            SafeConn(() => { action(); return 0; });
        }

        /// <summary>
        /// 连接包装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public T SafeConn<T>(Func<T> action)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }

                return action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Transaction?.Rollback();
                throw;
            }
            finally
            {
                Transaction?.Dispose();
                Transaction = null;
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }
            }
        }
    }
}

#endif