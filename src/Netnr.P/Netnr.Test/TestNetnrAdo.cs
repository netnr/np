using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.Data;
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
                var dbc = new SqlConnection(conn);
                dbc.InfoMessage += (s, e) => Debug.WriteLine($"{nameof(dbc)} InfoMessage: {e.Message}");

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
                dbc.Notice += (s, e) => Debug.WriteLine($"{nameof(dbc)} InfoMessage: {e.Notice.MessageText}");

                var db = new DbHelper(dbc);
                var result = db.SqlExecuteReader(sql);
            }

            {
                var conn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=local.host)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=EE.Oracle.Docker)));User Id=CQSME;Password=Abc123....";
                var sql = @"";
                var dbc = new Oracle.ManagedDataAccess.Client.OracleConnection(conn);
                dbc.InfoMessage += (s, e) => Debug.WriteLine($"{nameof(dbc)} InfoMessage: {e.Message}");

                var db = new DbHelper(dbc);
                var result = db.SqlExecuteReader(sql);
            }
        }

        [Fact]
        public void OracleReader()
        {
            var connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=local.host)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=EE.Oracle.Docker)));User Id=CQSME;Password=123";

            var conn = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM PT_SYS_LOG";
            var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            do
            {
                var dt = new DataTable();
                var dtSchema = reader.GetSchemaTable();
                foreach (DataRow dr in dtSchema.Rows)
                {
                    //跳过隐藏列（针对配置 CommandBehavior.KeyInfo 添加了额外的列）
                    if (dt.Columns.Contains("IsHidden") && Convert.ToBoolean(dr["IsHidden"] == DBNull.Value ? false : dr["IsHidden"]))
                    {
                        continue;
                    }

                    var columnName = dr["ColumnName"].ToString();
                    var dataType = dr["DataType"];

                    var column = new DataColumn()
                    {
                        ColumnName = columnName,
                        DataType = (Type)dataType,
                        Unique = Convert.ToBoolean(dr["IsUnique"] == DBNull.Value ? false : dr["IsUnique"]),
                        AllowDBNull = Convert.ToBoolean(dr["AllowDBNull"] == DBNull.Value ? true : dr["AllowDBNull"]),
                        AutoIncrement = Convert.ToBoolean(dr["IsAutoIncrement"] == DBNull.Value ? false : dr["IsAutoIncrement"])
                    };

                    if (column.DataType == typeof(string))
                    {
                        column.MaxLength = (int)dr["ColumnSize"];
                    }
                    dt.Columns.Add(column);
                }

                var rowCount = 0;
                while (reader.Read())
                {
                    //var dr = dt.NewRow();                    
                    //for (int i = 0; i < reader.FieldCount; i++)
                    //{
                    //    dr[i] = reader[i];
                    //}
                    //dt.Rows.Add(dr.ItemArray);

                    object[] row = new object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader[i];
                    }
                    dt.Rows.Add(row);

                    if (++rowCount % 10000 == 0)
                    {
                        Debug.WriteLine($"{rowCount} rows, memory: {ParsingTo.FormatByteSize(Environment.WorkingSet)}");
                        dt.Clear();
                    }
                }
            } while (reader.NextResult());

            Debug.WriteLine($"Done! memory: {ParsingTo.FormatByteSize(Environment.WorkingSet)}");
        }
    }
}
