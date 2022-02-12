#if Full || Ado || AdoFull

using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// Db帮助类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="typeDB"></param>
        /// <returns></returns>
        public static SharedEnum.TypeDB GetTypeDB(string typeDB)
        {
            Enum.TryParse(typeDB, true, out SharedEnum.TypeDB tdb);
            return tdb;
        }

        /// <summary>
        /// SQL引用符号
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="KeyWord">关键字</param>
        /// <returns></returns>
        public static string SqlQuote(SharedEnum.TypeDB? tdb, string KeyWord)
        {
            return tdb switch
            {
                SharedEnum.TypeDB.SQLite or SharedEnum.TypeDB.SQLServer => $"[{KeyWord}]",
                SharedEnum.TypeDB.MySQL or SharedEnum.TypeDB.MariaDB => $"`{KeyWord}`",
                SharedEnum.TypeDB.Oracle or SharedEnum.TypeDB.PostgreSQL => $"\"{KeyWord}\"",
                _ => KeyWord,
            };
        }

        /// <summary>
        /// 构建查询空表脚本
        /// </summary>
        /// <param name="table">数据库表名</param>
        /// <param name="cb">构建对象，取引用符号</param>
        /// <param name="tdb">数据库类型，取引用符号</param>
        /// <returns></returns>
        public static string SqlEmpty(string table, DbCommandBuilder cb = null, SharedEnum.TypeDB? tdb = null)
        {
            if (cb != null)
            {
                table = $"{cb?.QuotePrefix}{table}{cb?.QuoteSuffix}";
            }
            else if (tdb != null)
            {
                table = SqlQuote(tdb, table);
            }

            return $"SELECT * FROM {table} WHERE 0 = 1";
        }

        /// <summary>
        /// 构建清空表数据脚本
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string SqlClearTable(SharedEnum.TypeDB tdb, string table)
        {
            var fullTableName = SqlQuote(tdb, table);

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
                    if (!connectionString.ToLower().Contains(key.ToLower()))
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
        /// <returns>NULL 或 {dt、dtSchema}</returns>
        public static Tuple<DataTable, DataTable> ReaderAdSchema(DbDataReader reader)
        {
            var hasField = reader.FieldCount > 0;
            if (hasField)
            {
                var dtSchema = reader.GetSchemaTable();
                var dt = new DataTable();

                if (dtSchema.Columns.Contains("BaseTableName"))
                {
                    dt.TableName = dtSchema.Rows[0]["BaseTableName"].ToString();
                    dtSchema.TableName = dt.TableName;
                }

                var keyCols = new List<DataColumn>();
                foreach (DataRow dr in dtSchema.Rows)
                {
                    var column = new DataColumn()
                    {
                        ColumnName = dr["ColumnName"].ToString(),
                        DataType = (Type)dr["DataType"],
                        Unique = (bool)dr["IsUnique"],
                        AllowDBNull = dr["AllowDBNull"] == DBNull.Value || (bool)dr["AllowDBNull"],
                        AutoIncrement = (bool)dr["IsAutoIncrement"]
                    };

                    if (column.DataType == typeof(string))
                    {
                        column.MaxLength = (int)dr["ColumnSize"];
                    }
                    if ((bool)dr["IsKey"])
                    {
                        keyCols.Add(column);
                    }

                    dt.Columns.Add(column);
                }
                if (keyCols.Count > 0)
                {
                    dt.PrimaryKey = keyCols.ToArray();
                }

                return new Tuple<DataTable, DataTable>(dt, dtSchema);
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
        /// <param name="includeSchemaTable"></param>
        /// <returns></returns>
        public static Tuple<DataSet, int, DataSet> ExecuteDataSet(this DbCommand dbCommand, bool includeSchemaTable = false)
        {
            var ds = new DataSet();
            var dsSchema = new DataSet();

            using var reader = dbCommand.ExecuteReader(CommandBehavior.KeyInfo);
            var recordsAffected = reader.RecordsAffected;

            do
            {
                var dt = new DataTable
                {
                    TableName = $"table{ds.Tables.Count + 1}"
                };

                var hasField = reader.FieldCount > 0;
                if (includeSchemaTable && hasField)
                {
                    var st = reader.GetSchemaTable();
                    st.TableName = dt.TableName;
                    dsSchema.Tables.Add(st);
                }

                dt.Load(reader);
                if (hasField)
                {
                    ds.Tables.Add(dt);
                }
            } while (!reader.IsClosed);

            return new Tuple<DataSet, int, DataSet>(ds, recordsAffected, dsSchema);
        }

        /// <summary>
        /// 查询返回数据集
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="readRow"></param>
        /// <returns></returns>
        public static void ExecuteDataRow(this DbCommand dbCommand, Action<DataRow> readRow)
        {
            using var reader = dbCommand.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.CloseConnection);

            do
            {
                var rs = DbHelper.ReaderAdSchema(reader);
                if (rs != null)
                {
                    var dt = rs.Item1;

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
                                col.AllowDBNull = true;
                            }
                        }

                        readRow.Invoke(dr);
                    }
                }
            } while (reader.NextResult());
        }

        /// <summary>
        /// 查询返回数据集
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static DataSet ExecuteData(this DbCommand dbCommand)
        {
            return ExecuteDataSet(dbCommand).Item1;
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