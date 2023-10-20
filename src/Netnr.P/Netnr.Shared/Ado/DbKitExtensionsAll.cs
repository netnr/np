#if Full || AdoAll

using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using ClickHouse.Client.ADO;
using MySqlConnector;
using Npgsql;

namespace Netnr;

/// <summary>
/// 静态扩展
/// </summary>
public static partial class DbKitExtensions
{
    /// <summary>
    /// 创建实例
    /// </summary>
    /// <param name="connOption"></param>
    /// <returns></returns>
    public static DbKit CreateDbInstance(this DbKitConnectionOption connOption)
    {
        DbKitConnectionOption newOption;

        //深拷贝构建新实例
        if (connOption.DeepCopyNewInstance)
        {
            newOption = new DbKitConnectionOption();
            newOption.ToDeepCopy(connOption);
            newOption.Connection = null;
        }
        else
        {
            newOption = connOption;
        }

        newOption.CreateDbConn();
        var dbKit = new DbKit(newOption);

        return dbKit;
    }

    /// <summary>
    /// 创建连接对象
    /// </summary>
    /// <param name="connOption"></param>
    /// <param name="setDatabase">尝试根据数据库名设置连接，默认 True</param>
    /// <returns></returns>
    public static void CreateDbConn(this DbKitConnectionOption connOption, bool setDatabase = true)
    {
        if (connOption.Connection == null)
        {
            //更新连接数据库名
            if (setDatabase && !string.IsNullOrWhiteSpace(connOption.DatabaseName))
            {
                connOption.SetConnDatabaseName(connOption.DatabaseName);
            }

            switch (connOption.ConnectionType)
            {
                case DBTypes.SQLite:
                    connOption.Connection = new SqliteConnection(connOption.ConnectionString);
                    break;
                case DBTypes.MySQL:
                case DBTypes.MariaDB:
                    connOption.Connection = new MySqlConnection(connOption.ConnectionString);
                    break;
                case DBTypes.Oracle:
                    connOption.Connection = new OracleConnection(connOption.ConnectionString);
                    break;
                case DBTypes.SQLServer:
                    connOption.Connection = new SqlConnection(connOption.ConnectionString);
                    break;
                case DBTypes.PostgreSQL:
                    connOption.Connection = new NpgsqlConnection(connOption.ConnectionString);
                    break;
                case DBTypes.ClickHouse:
                    connOption.Connection = new ClickHouseConnection(connOption.ConnectionString);
                    break;
            }
        }
    }

    /// <summary>
    /// 获取连接的数据库名
    /// </summary>
    /// <param name="connOption"></param>
    /// <returns></returns>
    public static string GetDefaultDatabaseName(this DbKitConnectionOption connOption)
    {
        string databaseName;
        if (connOption.ConnectionType == DBTypes.Oracle)
        {
            var csb = new OracleConnectionStringBuilder(connOption.ConnectionString);
            databaseName = csb.UserID;
        }
        else
        {
            connOption.CreateDbConn(setDatabase: false);
            databaseName = connOption.Connection.Database;
        }

        return databaseName;
    }

    /// <summary>
    /// 获取连接信息（去密码）
    /// </summary>
    /// <param name="connOption"></param>
    /// <param name="isReplace">替换，默认删除</param>
    /// <returns></returns>
    public static string GetSafeConnectionString(this DbKitConnectionOption connOption, bool isReplace = false)
    {
        var conn = connOption.ConnectionString;
        if (string.IsNullOrWhiteSpace(conn) && connOption.Connection != null)
        {
            conn = connOption.Connection.ConnectionString;
        }

        switch (connOption.ConnectionType)
        {
            case DBTypes.SQLite:
                {
                    var csb = new SqliteConnectionStringBuilder(conn);
                    if (isReplace)
                    {
                        csb.Password = "***";
                    }
                    else
                    {
                        csb.Remove(nameof(csb.Password));
                    }
                    conn = csb.ToString();
                }
                break;
            case DBTypes.MySQL:
            case DBTypes.MariaDB:
                {
                    var csb = new MySqlConnectionStringBuilder(conn);
                    if (isReplace)
                    {
                        csb.Password = "***";
                    }
                    else
                    {
                        csb.Remove(nameof(csb.Password));
                    }
                    conn = csb.ToString();
                }
                break;
            case DBTypes.Oracle:
                {
                    var csb = new OracleConnectionStringBuilder(conn);
                    if (isReplace)
                    {
                        csb.Password = "***";
                    }
                    else
                    {
                        csb.Remove(nameof(csb.Password));
                    }
                    conn = csb.ToString();
                }
                break;
            case DBTypes.SQLServer:
                {
                    var csb = new SqlConnectionStringBuilder(conn);
                    if (isReplace)
                    {
                        csb.Password = "***";
                    }
                    else
                    {
                        csb.Remove(nameof(csb.Password));
                    }
                    conn = csb.ToString();
                }
                break;
            case DBTypes.PostgreSQL:
                {
                    var csb = new NpgsqlConnectionStringBuilder(conn);
                    if (isReplace)
                    {
                        csb.Password = "***";
                    }
                    else
                    {
                        csb.Remove(nameof(csb.Password));
                    }
                    conn = csb.ToString();
                }
                break;
            case DBTypes.ClickHouse:
                {
                    var csb = new ClickHouseConnectionStringBuilder(conn);
                    if (isReplace)
                    {
                        csb.Password = "***";
                    }
                    else
                    {
                        csb.Remove(nameof(csb.Password));
                    }
                    conn = csb.ToString();
                }
                break;
        }

        return conn;
    }

    /// <summary>
    /// 获取连接信息项
    /// </summary>
    /// <param name="connOption"></param>
    /// <returns></returns>
    public static string GetSafeConnectionOption(this DbKitConnectionOption connOption)
    {
        return $"[{connOption.ConnectionRemark}]({connOption.ConnectionType}://{connOption.GetSafeConnectionString()})";
    }

    /// <summary>
    /// 设置连接字符串数据库名（MySQL、SQLServer、PostgreSQL）
    /// </summary>
    /// <param name="connOption"></param>
    /// <param name="databaseName">要设置的数据库名称</param>
    /// <returns></returns>
    public static DbKitConnectionOption SetConnDatabaseName(this DbKitConnectionOption connOption, string databaseName)
    {
        if (databaseName != null)
        {
            connOption.ConnectionString = connOption.ConnectionType switch
            {
                DBTypes.MySQL or DBTypes.MariaDB => new MySqlConnectionStringBuilder(connOption.ConnectionString)
                {
                    Database = databaseName
                }.ConnectionString,
                DBTypes.SQLServer => new SqlConnectionStringBuilder(connOption.ConnectionString)
                {
                    InitialCatalog = databaseName
                }.ConnectionString,
                DBTypes.PostgreSQL => new NpgsqlConnectionStringBuilder(connOption.ConnectionString)
                {
                    Database = databaseName
                }.ConnectionString,
                DBTypes.ClickHouse => new ClickHouseConnectionStringBuilder(connOption.ConnectionString)
                {
                    Database = databaseName
                }.ConnectionString,
                _ => connOption.ConnectionString,
            };
        }

        return connOption;
    }

    /// <summary>
    /// 预执行（目前仅针对 MySQL local_infile）
    /// </summary>
    /// <returns></returns>
    public static async Task<int> PreExecute(this DbKit db)
    {
        var num = 0;
        try
        {
            switch (db.ConnOption.ConnectionType)
            {
                case DBTypes.MySQL:
                case DBTypes.MariaDB:
                    {
                        var edo = await db.SqlExecuteDataOnly("SHOW VARIABLES");
                        var dt = edo.Datas.Tables[0];

                        var dictVar = new Dictionary<string, string>
                        {
                            { "local_infile","是否允许加载本地数据，BulkCopy 需要开启"},
                            { "max_allowed_packet","传输的 packet 大小限制，最大 1G，单位：B"},
                            { "innodb_lock_wait_timeout","innodb 的 dml 操作的行级锁的等待时间，事务等待获取资源等待的最长时间，BulkCopy 量大超时设置，单位：秒"}
                        };

                        var listBetterSql = new List<string>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            var val = dr[1];
                            switch (dr[0].ToString())
                            {
                                case "local_infile":
                                    if (val.ToString() != "ON")
                                    {
                                        //ON 开启，OFF 关闭
                                        listBetterSql.Add("SET GLOBAL local_infile = ON");
                                    }
                                    break;
                                case "max_allowed_packet":
                                    //小于 512M
                                    if (Convert.ToInt32(val) < 536870912)
                                    {
                                        //传输的 packet 大小 1G
                                        listBetterSql.Add("SET GLOBAL max_allowed_packet = 1073741824");
                                    }
                                    break;
                                case "innodb_lock_wait_timeout":
                                    if (Convert.ToInt32(val) < 600)
                                    {
                                        //10 分钟超时
                                        listBetterSql.Add("SET GLOBAL innodb_lock_wait_timeout = 600");
                                    }
                                    break;
                            }
                        }

                        //有优化
                        if (listBetterSql.Count > 0)
                        {
                            Console.WriteLine($"\n执行优化脚本：\n{string.Join(Environment.NewLine, listBetterSql)}\n");
                            await db.SqlExecuteNonQuery(listBetterSql);
                        }

                        num = listBetterSql.Count;
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\r\n预执行出错\r\n{ex.Message}\r\n");
            num = -1;
        }

        return num;
    }

    /// <summary>
    /// 批量写入
    /// </summary>
    /// <param name="db"></param>
    /// <param name="dt"></param>
    /// <param name="isCopy">复制模式，false MySQL 多行模式</param>
    /// <returns></returns>
    public static async Task<int> BulkCopy(this DbKit db, DataTable dt, bool isCopy = true)
    {
        int num = 0;

        switch (db.ConnOption.ConnectionType)
        {
            case DBTypes.SQLite:
                num = await db.BulkBatchSQLite(dt);
                break;
            case DBTypes.MySQL:
            case DBTypes.MariaDB:
                num = isCopy ? await db.BulkCopyMySQL(dt) : await db.BulkBatchMySQL(dt);
                break;
            case DBTypes.Oracle:
                num = await db.BulkCopyOracle(dt);
                break;
            case DBTypes.SQLServer:
                num = await db.BulkCopySQLServer(dt);
                break;
            case DBTypes.PostgreSQL:
                num = await db.BulkKeepIdentityPostgreSQL(dt);
                break;
            case DBTypes.ClickHouse:
                num = await db.BulkCopyClickHouse(dt);
                break;
        }

        return num;
    }

    /// <summary>
    /// 批量写入
    /// </summary>
    /// <param name="db"></param>
    /// <param name="dt"></param>
    /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
    /// <returns></returns>
    public static async Task<int> BulkBatch(this DbKit db, DataTable dt, string sqlEmpty = null)
    {
        int num = 0;

        switch (db.ConnOption.ConnectionType)
        {
            case DBTypes.SQLite:
                num = await db.BulkBatchSQLite(dt, sqlEmpty);
                break;
            case DBTypes.MySQL:
            case DBTypes.MariaDB:
                num = await db.BulkBatchMySQL(dt, sqlEmpty);
                break;
            case DBTypes.Oracle:
                num = await db.BulkBatchOracle(dt, sqlEmpty);
                break;
            case DBTypes.SQLServer:
                num = await db.BulkBatchSQLServer(dt, sqlEmpty);
                break;
            case DBTypes.PostgreSQL:
                num = await db.BulkBatchPostgreSQL(dt, sqlEmpty);
                break;
        }

        return num;
    }
}

#endif