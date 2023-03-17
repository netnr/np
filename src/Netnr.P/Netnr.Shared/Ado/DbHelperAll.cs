#if Full || AdoAll

using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;

namespace Netnr;

/// <summary>
/// Db帮助类
/// </summary>
public partial class DbHelper
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="tdb"></param>
    /// <param name="conn"></param>
    /// <returns></returns>
    public static DbHelper Init(EnumTo.TypeDB tdb, string conn)
    {
        var dbconn = DbConn(tdb, conn);
        var db = new DbHelper(dbconn);
        return db;
    }

    /// <summary>
    /// 数据库连接
    /// </summary>
    /// <param name="tdb"></param>
    /// <param name="conn"></param>
    /// <returns></returns>
    public static DbConnection DbConn(EnumTo.TypeDB tdb, string conn)
    {
        return tdb switch
        {
            EnumTo.TypeDB.SQLite => new SqliteConnection(conn),
            EnumTo.TypeDB.MySQL or EnumTo.TypeDB.MariaDB => new MySqlConnection(conn),
            EnumTo.TypeDB.Oracle => new OracleConnection(conn),
            EnumTo.TypeDB.SQLServer => new SqlConnection(conn),
            EnumTo.TypeDB.PostgreSQL => new NpgsqlConnection(conn),
            _ => null,
        };
    }

    /// <summary>
    /// 设置连接的数据库名（MySQL、SQLServer、PostgreSQL）
    /// </summary>
    /// <param name="tdb"></param>
    /// <param name="conn"></param>
    /// <param name="databaseName"></param>
    /// <returns></returns>
    public static string SetConnDatabase(EnumTo.TypeDB tdb, string conn, string databaseName)
    {
        return tdb switch
        {
            EnumTo.TypeDB.MySQL or EnumTo.TypeDB.MariaDB => new MySqlConnectionStringBuilder(conn)
            {
                Database = databaseName
            }.ConnectionString,
            EnumTo.TypeDB.SQLServer => new SqlConnectionStringBuilder(conn)
            {
                InitialCatalog = databaseName
            }.ConnectionString,
            EnumTo.TypeDB.PostgreSQL => new NpgsqlConnectionStringBuilder(conn)
            {
                Database = databaseName
            }.ConnectionString,
            _ => conn,
        };
    }

    /// <summary>
    /// 预检（目前仅 MySQL）
    /// </summary>
    /// <returns></returns>
    public int PreCheck()
    {
        try
        {
            if (Connection.GetType().Name.ToLower().Contains("mysql"))
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
        catch (Exception ex)
        {
            Console.WriteLine($"\r\n预检出错\r\n{ex.Message}\r\n");
            return -1;
        }

        return 0;
    }

    /// <summary>
    /// 批量写入
    /// </summary>
    /// <param name="tdb"></param>
    /// <param name="dt"></param>
    /// <param name="isCopy">复制模式，false MySQL 多行模式</param>
    /// <returns></returns>
    public int BulkCopy(EnumTo.TypeDB tdb, DataTable dt, bool isCopy = true)
    {
        int num = 0;

        switch (tdb)
        {
            case EnumTo.TypeDB.SQLite:
                num = BulkBatchSQLite(dt);
                break;
            case EnumTo.TypeDB.MySQL:
            case EnumTo.TypeDB.MariaDB:
                num = isCopy ? BulkCopyMySQL(dt) : BulkBatchMySQL(dt);
                break;
            case EnumTo.TypeDB.Oracle:
                num = BulkCopyOracle(dt);
                break;
            case EnumTo.TypeDB.SQLServer:
                num = BulkCopySQLServer(dt);
                break;
            case EnumTo.TypeDB.PostgreSQL:
                num = BulkKeepIdentityPostgreSQL(dt);
                break;
        }

        return num;
    }

    /// <summary>
    /// 批量写入
    /// </summary>
    /// <param name="tdb"></param>
    /// <param name="dt"></param>
    /// <param name="sqlEmpty">查询空表脚本，默认*，可选列，会影响数据更新的列</param>
    /// <returns></returns>
    public int BulkBatch(EnumTo.TypeDB tdb, DataTable dt, string sqlEmpty = null)
    {
        int num = 0;

        switch (tdb)
        {
            case EnumTo.TypeDB.SQLite:
                num = BulkBatchSQLite(dt, sqlEmpty);
                break;
            case EnumTo.TypeDB.MySQL:
            case EnumTo.TypeDB.MariaDB:
                num = BulkBatchMySQL(dt, sqlEmpty);
                break;
            case EnumTo.TypeDB.Oracle:
                num = BulkBatchOracle(dt, sqlEmpty);
                break;
            case EnumTo.TypeDB.SQLServer:
                num = BulkBatchSQLServer(dt, sqlEmpty);
                break;
            case EnumTo.TypeDB.PostgreSQL:
                num = BulkBatchPostgreSQL(dt, sqlEmpty);
                break;
        }

        return num;
    }
}

#endif