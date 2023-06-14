#if Full || AdoAll

using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
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
    public static DbKit CreateInstance(this DbKitConnectionOption connOption)
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
        var dbk = new DbKit(newOption);

        return dbk;
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
                case EnumTo.TypeDB.SQLite:
                    connOption.Connection = new SqliteConnection(connOption.ConnectionString);
                    break;
                case EnumTo.TypeDB.MySQL:
                case EnumTo.TypeDB.MariaDB:
                    connOption.Connection = new MySqlConnection(connOption.ConnectionString);
                    break;
                case EnumTo.TypeDB.Oracle:
                    connOption.Connection = new OracleConnection(connOption.ConnectionString);
                    break;
                case EnumTo.TypeDB.SQLServer:
                    connOption.Connection = new SqlConnection(connOption.ConnectionString);
                    break;
                case EnumTo.TypeDB.PostgreSQL:
                    connOption.Connection = new NpgsqlConnection(connOption.ConnectionString);
                    break;
            }
        }
    }

    /// <summary>
    /// 手动开启连接
    /// </summary>
    /// <param name="connOption"></param>
    public static async Task Open(this DbKitConnectionOption connOption)
    {
        if (connOption.Connection?.State == ConnectionState.Closed)
        {
            await connOption.Connection.OpenAsync();
        }
    }

    /// <summary>
    /// 手动关闭连接
    /// </summary>
    /// <param name="connOption"></param>
    public static async Task Close(this DbKitConnectionOption connOption)
    {
        if (connOption.Connection?.State == ConnectionState.Open)
        {
            await connOption.Connection.CloseAsync();
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
        if (connOption.ConnectionType == EnumTo.TypeDB.Oracle)
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
                EnumTo.TypeDB.MySQL or EnumTo.TypeDB.MariaDB => new MySqlConnectionStringBuilder(connOption.ConnectionString)
                {
                    Database = databaseName
                }.ConnectionString,
                EnumTo.TypeDB.SQLServer => new SqlConnectionStringBuilder(connOption.ConnectionString)
                {
                    InitialCatalog = databaseName
                }.ConnectionString,
                EnumTo.TypeDB.PostgreSQL => new NpgsqlConnectionStringBuilder(connOption.ConnectionString)
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
                case EnumTo.TypeDB.MySQL:
                case EnumTo.TypeDB.MariaDB:
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
            Console.WriteLine($"\r\n预检出错\r\n{ex.Message}\r\n");
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
            case EnumTo.TypeDB.SQLite:
                num = await db.BulkBatchSQLite(dt);
                break;
            case EnumTo.TypeDB.MySQL:
            case EnumTo.TypeDB.MariaDB:
                num = isCopy ? await db.BulkCopyMySQL(dt) : await db.BulkBatchMySQL(dt);
                break;
            case EnumTo.TypeDB.Oracle:
                num = await db.BulkCopyOracle(dt);
                break;
            case EnumTo.TypeDB.SQLServer:
                num = await db.BulkCopySQLServer(dt);
                break;
            case EnumTo.TypeDB.PostgreSQL:
                num = await db.BulkKeepIdentityPostgreSQL(dt);
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
            case EnumTo.TypeDB.SQLite:
                num = await db.BulkBatchSQLite(dt, sqlEmpty);
                break;
            case EnumTo.TypeDB.MySQL:
            case EnumTo.TypeDB.MariaDB:
                num = await db.BulkBatchMySQL(dt, sqlEmpty);
                break;
            case EnumTo.TypeDB.Oracle:
                num = await db.BulkBatchOracle(dt, sqlEmpty);
                break;
            case EnumTo.TypeDB.SQLServer:
                num = await db.BulkBatchSQLServer(dt, sqlEmpty);
                break;
            case EnumTo.TypeDB.PostgreSQL:
                num = await db.BulkBatchPostgreSQL(dt, sqlEmpty);
                break;
        }

        return num;
    }
}

#endif