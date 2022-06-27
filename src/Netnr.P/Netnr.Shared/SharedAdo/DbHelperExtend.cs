#if Full || Ado || AdoFull

using System.Text.RegularExpressions;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// Db帮助类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// SQL引用符号，支持点分隔
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="KeyWord">关键字</param>
        /// <returns></returns>
        public static string SqlQuote(SharedEnum.TypeDB? tdb, string KeyWord)
        {
            if (string.IsNullOrWhiteSpace(KeyWord))
            {
                return KeyWord;
            }

            return tdb switch
            {
                SharedEnum.TypeDB.SQLite or SharedEnum.TypeDB.SQLServer =>
                string.Join('.', KeyWord.Replace("[", "").Replace("]", "").Split('.').Select(x => $"[{x}]")),

                SharedEnum.TypeDB.MySQL or SharedEnum.TypeDB.MariaDB =>
                string.Join('.', KeyWord.Replace("`", "").Split('.').Select(x => $"`{x}`")),

                SharedEnum.TypeDB.Oracle or SharedEnum.TypeDB.PostgreSQL =>
                string.Join('.', KeyWord.Replace("\"", "").Split('.').Select(x => $"\"{x}\"")),

                _ => KeyWord,
            };
        }

        /// <summary>
        /// 模式及表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">模式，可选</param>
        /// <param name="tdb">类型</param>
        /// <returns></returns>
        public static string SqlSNTN(string tableName, string schemaName = null, SharedEnum.TypeDB? tdb = null)
        {
            var sntn = tableName;

            if (!string.IsNullOrEmpty(schemaName))
            {
                sntn = $"{schemaName}.{tableName}";
            }

            if (tdb != null)
            {
                sntn = SqlQuote(tdb, sntn);
            }

            return sntn;
        }

        /// <summary>
        /// 判断 是否相等
        /// </summary>
        /// <param name="sntn"></param>
        /// <param name="tableName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static bool SqlEqualSNTN(string sntn, string tableName, string schemaName = null)
        {
            var sntnArray = sntn.Split('.');
            if (sntnArray.Length == 2)
            {
                return schemaName == sntnArray[0] && tableName == sntnArray[1];
            }
            else
            {
                return tableName == sntnArray[0];
            }
        }

        /// <summary>
        /// 构建查询空表脚本
        /// </summary>
        /// <param name="table">数据库表名</param>
        /// <param name="tdb">数据库类型，取引用符号</param>
        /// <returns></returns>
        public static string SqlEmpty(string table, SharedEnum.TypeDB? tdb = null)
        {
            return $"SELECT * FROM {SqlQuote(tdb, table)} WHERE 0 = 1";
        }

        /// <summary>
        /// 构建清空表数据脚本
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="sntn">模式名.表名</param>
        /// <returns></returns>
        public static string SqlClearTable(SharedEnum.TypeDB tdb, string sntn)
        {
            var fullTableName = SqlQuote(tdb, sntn);

            if (tdb == SharedEnum.TypeDB.SQLite)
            {
                return $"DELETE FROM {fullTableName}";
            }
            else
            {
                return $"TRUNCATE TABLE {fullTableName}";
            }
        }

        /// <summary>
        /// SQL连接字符串预检
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static string SqlConnPreCheck(SharedEnum.TypeDB? tdb, string connectionString)
        {
            var citem = new Dictionary<string, string>();

            switch (tdb)
            {
                case SharedEnum.TypeDB.MySQL:
                case SharedEnum.TypeDB.MariaDB:
                    citem.Add("AllowLoadLocalInfile", "true");
                    break;
                case SharedEnum.TypeDB.SQLServer:
                    citem.Add("TrustServerCertificate", "true");
                    break;
            }

            if (citem.Count > 0)
            {
                foreach (var key in citem.Keys)
                {
                    if (!connectionString.ToLower().Replace(" ", "").Contains(key.ToLower()))
                    {
                        connectionString = connectionString.TrimEnd(';') + $";{key}={citem[key]}";
                    }
                }
            }

            return connectionString;
        }

        /// <summary>
        /// SQL连接字符串加密/解密
        /// </summary>
        /// <param name="conn">连接字符串</param>
        /// <param name="pwd">密码</param>
        /// <param name="isDecrypt">是解密，加密 false</param>
        public static string SqlConnEncryptOrDecrypt(string conn, string pwd, bool isDecrypt = true)
        {
            if (isDecrypt)
            {
                var ckey = "CONNED" + conn.GetHashCode();
                if (Core.CacheTo.Get(ckey) is not string cval)
                {
                    var clow = conn.ToLower();
                    var pts = new List<string> { "database", "server", "filename", "source", "user" };
                    if (!pts.Any(x => clow.Contains(x)))
                    {
                        cval = Core.CalcTo.AESDecrypt(conn, pwd);
                    }
                    else
                    {
                        cval = conn;
                    }

                    Core.CacheTo.Set(ckey, cval);
                }
                return cval;
            }
            else
            {
                return conn = Core.CalcTo.AESEncrypt(conn, pwd);
            }
        }

        /// <summary>
        /// 解析 Heroku 环境变量的连接字符串
        /// 遵循规则：postgres://{username}:{password}@{host}:{port}/{dbname}
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        public static string SqlConnFromHeroku(string ev)
        {
            string conn = string.Empty;

            string pattern = @"(\w+):\/\/(\w+):(\w+)@(.*):(\d+)\/(\w+)";
            var val = Environment.GetEnvironmentVariable(ev);
            Match m = Regex.Match(val, pattern);
            if (m.Success)
            {
                string username = m.Groups[2].Value.ToString();
                string password = m.Groups[3].Value.ToString();
                string host = m.Groups[4].Value.ToString();
                string port = m.Groups[5].Value.ToString();
                string dbname = m.Groups[6].Value.ToString();

                switch (m.Groups[1].ToString().ToLower())
                {
                    case "postgres":
                        conn = $"Server={host};Port={port};User Id={username};Password={password};Database={dbname};SslMode=Require;Trust Server Certificate=true;";
                        break;
                    case "mysql":
                        conn = $"Server={host};Port={port};Uid={username};Pwd={password};Database={dbname};";
                        break;
                }
            }
            else
            {
                Console.WriteLine($"解析 Heroku 环境变量连接字符串失败，环境变量：{ev}，值：{val}");
            }

            return conn;
        }

        /// <summary>
        /// 解析 begin ... end;
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool SqlParserBeginEnd(string sql)
        {
            string pattern = @"begin(.*)end(\s+||\s\S+);";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline;
            return Regex.Matches(sql, pattern, options).Count > 0;
        }

        /// <summary>
        /// 解析 open:name for
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static HashSet<string> SqlParserCursors(string sql)
        {
            var list = new HashSet<string>();

            string pattern = @"open(\s+|):(\S+)\s+for";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline;

            var mcs = Regex.Matches(sql, pattern, options);
            for (int i = 0; i < mcs.Count; i++)
            {
                var mc = mcs[i];
                if (mc.Success)
                {
                    list.Add(mc.Groups[2].ToString().ToLower());
                }
            }

            return list;
        }

        /// <summary>
        /// 获取空表结构、元信息
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>NULL 或 {Table, Schema}</returns>
        public static Tuple<DataTable, DataTable> ReaderTableSchema(DbDataReader reader)
        {
            var hasField = reader.FieldCount > 0;
            if (hasField)
            {
                var dtSchema = reader.GetSchemaTable();
                var dtTable = new DataTable();

                if (dtSchema.Columns.Contains("BaseSchemaName"))
                {
                    dtTable.Namespace = dtSchema.Rows[0]["BaseSchemaName"].ToString();
                    dtSchema.Namespace = dtTable.Namespace;
                }
                if (dtSchema.Columns.Contains("BaseTableName"))
                {
                    dtTable.TableName = dtSchema.Rows[0]["BaseTableName"].ToString();
                    dtSchema.TableName = dtTable.TableName;
                }

                var keyCols = new List<DataColumn>();
                foreach (DataRow dr in dtSchema.Rows)
                {
                    //跳过隐藏列（针对配置 CommandBehavior.KeyInfo 添加了额外的列）
                    if (dtSchema.Columns.Contains("IsHidden") && Convert.ToBoolean(dr["IsHidden"] == DBNull.Value ? false : dr["IsHidden"]))
                    {
                        continue;
                    }

                    var columnName = dr["ColumnName"].ToString();
                    var dataType = dr["DataType"];

                    //处理相同的列
                    int num = 0;
                    while (dtTable.Columns.Contains(columnName))
                    {
                        columnName = $"{dr["ColumnName"]}{++num}";
                    }

                    var column = new DataColumn()
                    {
                        ColumnName = columnName,
                        DataType = (Type)dataType,
                        Unique = Convert.ToBoolean(dr["IsUnique"] == DBNull.Value ? false : dr["IsUnique"]),
                        AllowDBNull = Convert.ToBoolean(dr["AllowDBNull"] == DBNull.Value ? true : dr["AllowDBNull"]),
                        AutoIncrement = Convert.ToBoolean(dr["IsAutoIncrement"] == DBNull.Value ? false : dr["IsAutoIncrement"])
                    };
                    if (dataType.ToString() == "System.DBNull")
                    {
                        column.DataType = typeof(object);
                    }

                    if (column.DataType == typeof(string))
                    {
                        column.MaxLength = (int)dr["ColumnSize"];
                    }
                    if (Convert.ToBoolean(dr["IsKey"] == DBNull.Value ? false : dr["IsKey"]))
                    {
                        keyCols.Add(column);
                    }

                    dtTable.Columns.Add(column);
                }
                if (keyCols.Count > 0)
                {
                    dtTable.PrimaryKey = keyCols.ToArray();
                }

                return new Tuple<DataTable, DataTable>(dtTable, dtSchema);
            }

            return null;
        }
    }

    /// <summary>
    /// 扩展
    /// </summary>
    public static class DbHelperExtend
    {
        /// <summary>
        /// 查询返回数据集
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="tableLoad">默认使用 Load 模式，False 则逐行读取（针对Load模式出错时用）</param>
        /// <returns>表数据、受影响行数、表结构</returns>
        public static Tuple<DataSet, int, DataSet> ExecuteDataSet(this DbCommand dbCommand, bool tableLoad = true)
        {
            var dsTable = new DataSet();
            var dsSchema = new DataSet();

            using var reader = dbCommand.ExecuteReader(CommandBehavior.KeyInfo);
            var recordsAffected = reader.RecordsAffected;

            if (tableLoad)
            {
                do
                {
                    var dt = new DataTable
                    {
                        TableName = $"table{dsTable.Tables.Count + 1}"
                    };

                    var hasField = reader.FieldCount > 0;
                    if (hasField)
                    {
                        var st = reader.GetSchemaTable();
                        st.TableName = dt.TableName;
                        dsSchema.Tables.Add(st);
                    }

                    dt.Load(reader); // 就算没字段也需要执行，触发 Close

                    if (hasField)
                    {
                        dsTable.Tables.Add(dt);
                    }
                } while (!reader.IsClosed);
            }
            else
            {
                do
                {
                    var hasField = reader.FieldCount > 0;
                    if (hasField)
                    {
                        var rts = DbHelper.ReaderTableSchema(reader);

                        var dt = rts.Item1;
                        dt.TableName = $"table{dsTable.Tables.Count + 1}";

                        var st = rts.Item2;
                        st.TableName = dt.TableName;
                        dsSchema.Tables.Add(st);

                        while (reader.Read())
                        {
                            var dr = dt.NewRow();
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                var col = dt.Columns[i];
                                var cellValue = reader[i];

                                if (cellValue != DBNull.Value)
                                {
                                    dr[i] = cellValue;
                                }
                                else if (col.AllowDBNull == false)
                                {
                                    Console.WriteLine($"{col.ColumnName}:Column does not allow null but value is Null !!!");
                                    dr[i] = Convert.ChangeType("0", col.DataType);
                                }
                            }

                            dt.Rows.Add(dr.ItemArray);
                        }

                        dsTable.Tables.Add(dt);
                    }

                } while (reader.NextResult());
            }

            return new Tuple<DataSet, int, DataSet>(dsTable, recordsAffected, dsSchema);
        }

        /// <summary>
        /// 查询返回数据集
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static Dictionary<string, List<Dictionary<string, object>>> ExecuteDataOnly(this DbCommand dbCommand)
        {
            var dic = new Dictionary<string, List<Dictionary<string, object>>>();
            using var reader = dbCommand.ExecuteReader();
            do
            {
                var hasField = reader.FieldCount > 0;
                if (hasField)
                {
                    var dicKey = $"table{dic.Keys.Count + 1}";
                    var dicVal = new List<Dictionary<string, object>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader[i]);
                        }
                        dicVal.Add(row);
                    }
                    dic.Add(dicKey, dicVal);
                }
            } while (reader.NextResult());

            return dic;
        }

        /// <summary>
        /// 查询读取数据行
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="readRow">读取行，dt.Namespace = SchemaName，dt.TableName = TableName</param>
        /// <returns></returns>
        public static void ExecuteDataRow(this DbCommand dbCommand, Action<DataRow> readRow)
        {
            using var reader = dbCommand.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.CloseConnection);

            do
            {
                var rts = DbHelper.ReaderTableSchema(reader);
                if (rts != null)
                {
                    var dt = rts.Item1;

                    while (reader.Read())
                    {
                        var dr = dt.NewRow();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var col = dt.Columns[i];
                            var cellValue = reader[i];

                            if (cellValue != DBNull.Value)
                            {
                                dr[i] = cellValue;
                            }
                            else if (col.AllowDBNull == false)
                            {
                                dr[i] = cellValue.ToString();

                                /*
                                 * 目前遇到 Oracle 的两种情况：
                                 * Oracle 的字段约束状态为 Disable
                                 * Oracle 不为 Null 的 Empty CLOB
                                 */
                            }
                        }

                        readRow.Invoke(dr);
                    }
                }
            } while (reader.NextResult());
        }

        /// <summary>
        /// 修复：避免内存泄露
        /// ref: https://stackoverflow.com/questions/3699143
        /// ref: https://support.oracle.com/knowledge/Oracle%20Database%20Products/1050515_1.html
        /// </summary>
        /// <param name="cmd"></param>
        public static DbCommand ToFix(this DbCommand cmd)
        {
            var gtCmd = cmd.GetType();
            if (gtCmd.Name == "OracleCommand")
            {
                var gpLob = gtCmd.GetProperty("InitialLOBFetchSize");
                gpLob.SetValue(cmd, -1);

                var gpLong = gtCmd.GetProperty("InitialLONGFetchSize");
                gpLong.SetValue(cmd, -1);
            }

            return cmd;
        }
    }
}

#endif