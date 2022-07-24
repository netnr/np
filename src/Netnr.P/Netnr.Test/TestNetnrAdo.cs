using Microsoft.Data.Sqlite;
using Xunit;

namespace Netnr.Test
{
    public class TestNetnrAdo
    {
        [Fact]
        public void SQLiteAttach()
        {
            var c1 = @"Data Source=D:\tmp\res\tmp.db";
            var conn = new SqliteConnection(c1);
            conn.Open();
            var sql1 = "PRAGMA database_list";
            var cmd1 = new SqliteCommand(sql1, conn);
            Debug.WriteLine(cmd1.ExecuteDataOnly().ToJson(true));

            var sql2 = @"ATTACH DATABASE 'D:\tmp\res\netnrf.db' AS nr";
            var cmd2 = new SqliteCommand(sql2, conn);
            Debug.WriteLine($"ATTACH DATABASE: {cmd2.ExecuteNonQuery()}");

            conn.Close();
            conn.Open();
            Debug.WriteLine(cmd1.ExecuteDataOnly().ToJson(true));
        }

        [Fact]
        public void InfoMessage()
        {
            // SQLServer
            {
                var conn = "Server=local.host;uid=sa;pwd=Abc123....;database=netnr;TrustServerCertificate=True;";
                var sql = "print newid()";
                var dbc = new Microsoft.Data.SqlClient.SqlConnection(conn);
                dbc.InfoMessage += (s, e) => Debug.WriteLine($"{dbc.GetType().Name} InfoMessage: {e.Message}");

                var db = new DbHelper(dbc);
                var result = db.SqlExecuteReader(sql);
            }

            // PostgreSQL
            {
                var conn = "Server=local.host;Port=5432;User Id=postgres;Password=Abc123....;database=netnr;";
                var sql = @"DO $$
BEGIN
    RAISE NOTICE '当前日期时间：%', now();
    RAISE NOTICE '版本信息：%', version();
END
$$;";
                var dbc = new Npgsql.NpgsqlConnection(conn);
                dbc.Notice += (s, e) => Debug.WriteLine($"{dbc.GetType().Name} InfoMessage: {e.Notice.MessageText}");

                var db = new DbHelper(dbc);
                var result = db.SqlExecuteReader(sql);
            }

            {
                var conn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=local.host)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=EE.Oracle.Docker)));User Id=CQSME;Password=Abc123....";
                var sql = @"";
                var dbc = new Oracle.ManagedDataAccess.Client.OracleConnection(conn);
                dbc.InfoMessage += (s, e) => Debug.WriteLine($"{dbc.GetType().Name} InfoMessage: {e.Message}");

                var db = new DbHelper(dbc);
                var result = db.SqlExecuteReader(sql);
            }
        }
    }
}
