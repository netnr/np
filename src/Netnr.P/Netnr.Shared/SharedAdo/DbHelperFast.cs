#if Full || AdoFull

using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;

namespace Netnr.SharedAdo
{
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
        public static DbHelper Init(SharedEnum.TypeDB tdb, string conn)
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
        public static DbConnection DbConn(SharedEnum.TypeDB tdb, string conn)
        {
            return tdb switch
            {
                SharedEnum.TypeDB.SQLite => new SqliteConnection(conn),
                SharedEnum.TypeDB.MySQL or SharedEnum.TypeDB.MariaDB => new MySqlConnection(conn),
                SharedEnum.TypeDB.Oracle => new OracleConnection(conn),
                SharedEnum.TypeDB.SQLServer => new SqlConnection(conn),
                SharedEnum.TypeDB.PostgreSQL => new NpgsqlConnection(conn),
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
        public static string SetConnDatabase(SharedEnum.TypeDB tdb, string conn, string databaseName)
        {
            return tdb switch
            {
                SharedEnum.TypeDB.MySQL or SharedEnum.TypeDB.MariaDB => new MySqlConnectionStringBuilder(conn)
                {
                    Database = databaseName
                }.ConnectionString,
                SharedEnum.TypeDB.SQLServer => new SqlConnectionStringBuilder(conn)
                {
                    InitialCatalog = databaseName
                }.ConnectionString,
                SharedEnum.TypeDB.PostgreSQL => new NpgsqlConnectionStringBuilder(conn)
                {
                    Database = databaseName
                }.ConnectionString,
                _ => conn,
            };
        }

        /// <summary>
        /// 批量写入
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int BulkCopy(SharedEnum.TypeDB tdb, DataTable dt)
        {
            int num = 0;

            switch (tdb)
            {
                case SharedEnum.TypeDB.SQLite: num = BulkBatchSQLite(dt); break;
                case SharedEnum.TypeDB.MySQL:
                case SharedEnum.TypeDB.MariaDB: num = BulkCopyMySQL(dt); break;
                case SharedEnum.TypeDB.Oracle: num = BulkCopyOracle(dt); break;
                case SharedEnum.TypeDB.SQLServer: num = BulkCopySQLServer(dt); break;
                case SharedEnum.TypeDB.PostgreSQL: num = BulkKeepIdentityPostgreSQL(dt); break;
            }

            return num;
        }
    }
}

#endif