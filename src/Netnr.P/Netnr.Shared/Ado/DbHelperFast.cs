#if Full || AdoFull

using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;

namespace Netnr
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
        /// 批量写入
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int BulkCopy(EnumTo.TypeDB tdb, DataTable dt)
        {
            int num = 0;

            switch (tdb)
            {
                case EnumTo.TypeDB.SQLite: num = BulkBatchSQLite(dt); break;
                case EnumTo.TypeDB.MySQL:
                case EnumTo.TypeDB.MariaDB: num = BulkCopyMySQL(dt); break;
                case EnumTo.TypeDB.Oracle: num = BulkCopyOracle(dt); break;
                case EnumTo.TypeDB.SQLServer: num = BulkCopySQLServer(dt); break;
                case EnumTo.TypeDB.PostgreSQL: num = BulkKeepIdentityPostgreSQL(dt); break;
            }

            return num;
        }
    }
}

#endif