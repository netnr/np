#if Full || Ado || AdoAll

using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// 静态扩展
/// </summary>
public static partial class DbKitExtensions
{
    #region 基础方法

    /// <summary>
    /// SQL引用符号，支持点分隔
    /// </summary>
    /// <param name="tdb">数据库类型</param>
    /// <param name="KeyWord">关键字</param>
    /// <returns></returns>
    public static string SqlQuote(EnumTo.TypeDB? tdb, string KeyWord)
    {
        if (string.IsNullOrWhiteSpace(KeyWord))
        {
            return KeyWord;
        }

        return tdb switch
        {
            EnumTo.TypeDB.SQLite or EnumTo.TypeDB.SQLServer =>
            string.Join('.', KeyWord.Replace("[", "").Replace("]", "").Split('.').Select(x => $"[{x}]")),

            EnumTo.TypeDB.MySQL or EnumTo.TypeDB.MariaDB =>
            string.Join('.', KeyWord.Replace("`", "").Split('.').Select(x => $"`{x}`")),

            EnumTo.TypeDB.Oracle or EnumTo.TypeDB.PostgreSQL =>
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
    public static string SqlSNTN(string tableName, string schemaName = null, EnumTo.TypeDB? tdb = null)
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
    public static string SqlEmpty(string table, EnumTo.TypeDB? tdb = null)
    {
        return $"SELECT * FROM {SqlQuote(tdb, table)} WHERE 0 = 1";
    }

    /// <summary>
    /// 构建清空表数据脚本
    /// </summary>
    /// <param name="tdb"></param>
    /// <param name="sntn">模式名.表名</param>
    /// <returns></returns>
    public static string SqlClearTable(EnumTo.TypeDB tdb, string sntn)
    {
        var fullTableName = SqlQuote(tdb, sntn);

        if (tdb == EnumTo.TypeDB.SQLite)
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
    public static string PreCheckConn(EnumTo.TypeDB? tdb, string connectionString)
    {
        var citem = new Dictionary<string, string>();

        switch (tdb)
        {
            case EnumTo.TypeDB.MySQL:
            case EnumTo.TypeDB.MariaDB:
                citem.Add("AllowLoadLocalInfile", "true");
                break;
            case EnumTo.TypeDB.SQLServer:
                citem.Add("TrustServerCertificate", "true");
                citem.Add("MultipleActiveResultSets", "true");
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
            var ckey = $"Conns-{Math.Abs(conn.GetHashCode())}";
            var cval = CacheTo.Get<string>(ckey);
            if (cval == null)
            {
                var clow = conn.ToLower();
                var pts = new List<string> { "database", "server", "filename", "source", "user" };
                if (!pts.Any(clow.Contains))
                {
                    conn = CalcTo.AESDecrypt(conn, pwd);
                }
                CacheTo.Set(ckey, conn);
                cval = conn;
            }
            return cval;
        }
        else
        {
            return conn = CalcTo.AESEncrypt(conn, pwd);
        }
    }

    /// <summary>
    /// URL 连接字符串转换
    /// 遵循规则：postgres://{username}:{password}@{host}:{port}/{dbname}
    /// </summary>
    /// <param name="ev"></param>
    /// <returns></returns>
    public static string SqlConnFromURL(string ev)
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
            Console.WriteLine($"解析 URL 连接字符串失败，环境变量：{ev}，值：{val}");
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

    #endregion

    /// <summary>
    /// 读取空表结构、元数据、主键列（先填充数据再设置主键列，考虑复合主键列只查询一列时的情况）
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<DbKitSchemaResult> ReaderSchemaAsync(this DbDataReader reader)
    {
        DbKitSchemaResult model = null;

        if (reader.FieldCount > 0)
        {
            model = new DbKitSchemaResult
            {
                Schema = await reader.GetSchemaTableAsync(),
                Table = new DataTable()
            };

            if (model.Schema.Columns.Contains("BaseSchemaName"))
            {
                model.Table.Namespace = model.Schema.Rows[0]["BaseSchemaName"].ToString();
                model.Schema.Namespace = model.Table.Namespace;
            }
            if (model.Schema.Columns.Contains("BaseTableName"))
            {
                model.Table.TableName = model.Schema.Rows[0]["BaseTableName"].ToString();
                model.Schema.TableName = model.Table.TableName;
            }

            foreach (DataRow dr in model.Schema.Rows)
            {
                //跳过隐藏列（针对配置 CommandBehavior.KeyInfo 添加了额外的列）
                if (model.Schema.Columns.Contains("IsHidden") && Convert.ToBoolean(dr["IsHidden"] == DBNull.Value ? false : dr["IsHidden"]))
                {
                    continue;
                }

                var columnName = dr["ColumnName"].ToString();
                var dataType = dr["DataType"];

                //处理相同的列
                int num = 0;
                while (model.Table.Columns.Contains(columnName))
                {
                    columnName = $"{dr["ColumnName"]}{++num}";
                }

                var column = new DataColumn()
                {
                    ColumnName = columnName,
                    DataType = dataType is string ? Type.GetType(dataType.ToString()) : (Type)dataType,
                    Unique = Convert.ToBoolean(dr["IsUnique"] == DBNull.Value ? false : dr["IsUnique"]),
                    AllowDBNull = Convert.ToBoolean(dr["AllowDBNull"] == DBNull.Value ? true : dr["AllowDBNull"]),
                    AutoIncrement = Convert.ToBoolean(dr["IsAutoIncrement"] == DBNull.Value ? false : dr["IsAutoIncrement"])
                };
                if (dataType.ToString() == "System.DBNull")
                {
                    column.DataType = typeof(object);
                }

                //取消长度设置，MySQL 执行 show engine innodb status 出错
                //if (column.DataType == typeof(string))
                //{
                //    column.MaxLength = (int)dr["ColumnSize"];
                //}
                if (Convert.ToBoolean(dr["IsKey"] == DBNull.Value ? false : dr["IsKey"]))
                {
                    model.KeyColumns.Add(column);
                }

                model.Table.Columns.Add(column);
            }
        }

        return model;
    }

    /// <summary>
    /// 读取数据集
    /// </summary>
    /// <param name="dbCommand"></param>
    /// <param name="tableLoad">默认使用 Load 模式，False 则逐行读取（针对Load模式出错时用）</param>
    /// <returns>表数据、受影响行数、表结构</returns>
    public static async Task<DbKitDataSetResult> ReaderDataSetAsync(this DbCommand dbCommand, bool tableLoad = true)
    {
        using var reader = await dbCommand.ExecuteReaderAsync(CommandBehavior.KeyInfo);

        var model = new DbKitDataSetResult()
        {
            Datas = new DataSet(),
            RecordsAffected = reader.RecordsAffected,
            Schemas = new DataSet()
        };

        if (tableLoad)
        {
            do
            {
                var dt = new DataTable
                {
                    TableName = $"table{model.Datas.Tables.Count + 1}"
                };

                var hasField = reader.FieldCount > 0;
                if (hasField)
                {
                    var st = await reader.GetSchemaTableAsync();
                    st.TableName = dt.TableName;
                    model.Schemas.Tables.Add(st);
                }

                dt.Load(reader); // 就算没字段也需要执行，触发 Close

                if (hasField)
                {
                    model.Datas.Tables.Add(dt);
                }
            } while (!reader.IsClosed);
        }
        else
        {
            do
            {
                if (reader.FieldCount > 0)
                {
                    var schemaModel = await reader.ReaderSchemaAsync();
                    schemaModel.Table.TableName = $"table{model.Datas.Tables.Count + 1}";

                    schemaModel.Schema.TableName = schemaModel.Table.TableName;
                    model.Schemas.Tables.Add(schemaModel.Schema);

                    while (await reader.ReadAsync())
                    {
                        var dr = schemaModel.Table.NewRow();
                        for (int i = 0; i < schemaModel.Table.Columns.Count; i++)
                        {
                            var col = schemaModel.Table.Columns[i];
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

                        schemaModel.Table.Rows.Add(dr.ItemArray);
                    }

                    //填充数据后设置主键列
                    if (schemaModel.KeyColumns.Count > 0)
                    {
                        try
                        {
                            schemaModel.Table.PrimaryKey = schemaModel.KeyColumns.ToArray();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }

                    model.Datas.Tables.Add(schemaModel.Table);
                }
            } while (await reader.NextResultAsync());
        }

        return model;
    }

    /// <summary>
    /// 读取数据集（仅数据，不含元数据、表结构）
    /// </summary>
    /// <param name="dbCommand"></param>
    /// <returns></returns>
    public static async Task<DbKitDataOnlyResult> ReaderDataOnlyAsync(this DbCommand dbCommand)
    {
        using var reader = await dbCommand.ExecuteReaderAsync();

        var model = new DbKitDataOnlyResult
        {
            RecordsAffected = reader.RecordsAffected,
            Datas = new DataSet()
        };

        do
        {
            var hasField = reader.FieldCount > 0;
            if (hasField)
            {
                var dt = new DataTable
                {
                    TableName = $"table{model.Datas.Tables.Count + 1}"
                };

                while (await reader.ReadAsync())
                {
                    object[] row = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var val = reader[i];

                        //构建列
                        if (dt.Rows.Count == 0)
                        {
                            var fieldType = reader.GetFieldType(i);
                            dt.Columns.Add(new DataColumn(reader.GetName(i), fieldType));
                        }

                        row[i] = val == DBNull.Value ? null : val;
                    }
                    dt.Rows.Add(row);
                }
                model.Datas.Tables.Add(dt);
            }
        } while (await reader.NextResultAsync());

        return model;
    }

    /// <summary>
    /// 查询读取数据行
    /// </summary>
    /// <param name="dbCommand"></param>
    /// <param name="readRow">读取行</param>
    /// <param name="emptyTable">表结构（空表） dt.Namespace = SchemaName，dt.TableName = TableName</param>
    /// <returns></returns>
    public static async Task ReaderDataRowAsync(this DbCommand dbCommand, Func<object[], Task> readRow = null, Func<DataTable, Task> emptyTable = null)
    {
        using var reader = await dbCommand.ExecuteReaderAsync(CommandBehavior.KeyInfo);

        do
        {
            if (reader.FieldCount > 0)
            {
                var schemaModel = await reader.ReaderSchemaAsync();
                if (emptyTable != null)
                {
                    await emptyTable.Invoke(schemaModel.Table); //空表
                }

                while (await reader.ReadAsync())
                {
                    object[] row = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader[i];
                    }

                    if (readRow != null)
                    {
                        await readRow.Invoke(row);
                    }
                }
            }
        } while (await reader.NextResultAsync());
    }
}

#endif