using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Xunit;

namespace Netnr.Test
{
    public class TestNetnrAdo
    {
        [Fact]
        public async Task DbKitTransaction()
        {
            var connOption = new DbKitConnectionOption
            {
                ConnectionType = DBTypes.SQLServer,
                ConnectionString = "Server=local.host,1433;uid=sa;pwd=Abc1230...;database=xops"
            };
            var dbKit = connOption.CreateDbInstance();
            dbKit.ConnOption.AutoClose = false;

            var sql1 = "update base_user set user_phone='123' where user_account='xops'";
            var sql2 = "update base_user set user_mark='456' where user_account='xops'";
            var sql3 = "select * from base_user";

            await dbKit.SafeConn(async () =>
            {
                var edo = await dbKit.SqlExecuteDataOnly(sql3);
                var rows = edo.Datas.Tables[0];
                Debug.WriteLine(rows.ToJson());

                var eds = await dbKit.SqlExecuteDataSet(sql3);
                Debug.WriteLine(eds.Datas.Tables[0].ToJson());

                var num = 0;

                var cmdOption = dbKit.CommandCreate();
                await cmdOption.OpenTransactionAsync();
                cmdOption.AutoCommit = false;

                cmdOption.Command.CommandText = sql1;
                num += await cmdOption.Command.ExecuteNonQueryAsync();
                Debug.WriteLine(num);

                cmdOption.Command.CommandText = sql2;
                num += await cmdOption.Command.ExecuteNonQueryAsync();
                Debug.WriteLine(num);

                await cmdOption.CommitAsync();
                return num;
            });
        }

        [Fact]
        public async Task SQLiteSchema()
        {
            var connOption = new DbKitConnectionOption
            {
                ConnectionType = DBTypes.SQLite,
                ConnectionString = @"Data Source=D:\tmp\res\tmp.db"
            };
            var dbKit = connOption.CreateDbInstance();

            var st = Stopwatch.StartNew();
            var edo = await dbKit.SqlExecuteDataOnly("select * from stats_zoning limit 100");
            Debug.WriteLine(st.Elapsed);
            st.Restart();

            //查询 Schema 极慢，耗时 7 秒
            _ = await dbKit.SqlExecuteDataSet("select * from stats_zoning limit 100");
            Debug.WriteLine(st.Elapsed);
        }

        [Fact]
        public async Task SQLiteAttach()
        {
            var c1 = @"Data Source=D:\tmp\res\tmp.db";
            var conn = new SqliteConnection(c1);
            conn.Open();
            var sql1 = "PRAGMA database_list";
            var cmd1 = new SqliteCommand(sql1, conn);
            Debug.WriteLine((await cmd1.ReaderDataOnlyAsync()).ToJson(true));

            var sql2 = @"ATTACH DATABASE 'D:\tmp\res\netnrf.db' AS nr";
            var cmd2 = new SqliteCommand(sql2, conn);
            Debug.WriteLine($"ATTACH DATABASE: {cmd2.ExecuteNonQuery()}");

            conn.Close();
            conn.Open();
            Debug.WriteLine((await cmd1.ReaderDataOnlyAsync()).ToJson(true));
        }

        [Fact]
        public async Task InfoMessage()
        {
            // SQLServer
            {
                var conn = DbKitExtensions.PreCheckConn("Server=local.host;uid=sa;pwd=Abc1230...;", DBTypes.SQLServer);
                var csb = new SqlConnectionStringBuilder(conn)
                {
                    InitialCatalog = "xops"
                };
                var dbConn = new SqlConnection(csb.ConnectionString);
                dbConn.InfoMessage += (s, e) => Debug.WriteLine($"{nameof(dbConn)} InfoMessage: {e.Message}");

                var connOption = new DbKitConnectionOption
                {
                    ConnectionType = DBTypes.SQLServer,
                    Connection = dbConn
                };
                var dbKit = new DbKit(connOption);

                var sql = "print newid()";
                var result = await dbKit.SqlExecuteDataOnly(sql);
            }

            // PostgreSQL
            {
                var conn = "Server=local.host;Port=5432;User Id=postgres;Password=Abc1230...;database=netnr;";
                var sql = @"DO $$
BEGIN
    RAISE NOTICE '当前日期时间：%', now();
    RAISE NOTICE '版本信息：%', version();
END
$$;";
                var dbConn = new Npgsql.NpgsqlConnection(conn);
                dbConn.Notice += (s, e) => Debug.WriteLine($"{nameof(dbConn)} InfoMessage: {e.Notice.MessageText}");

                var connOption = new DbKitConnectionOption
                {
                    ConnectionType = DBTypes.PostgreSQL,
                    Connection = dbConn,
                    AutoClose = false
                };
                var dbKit = new DbKit(connOption);

                var result = await dbKit.SqlExecuteDataOnly(sql);
            }

            {
                var conn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=local.host)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=helowin)));User Id=CQSME;Password=CQSME";
                var sql = @"SELECT SYS_GUID() FROM dual";
                var dbConn = new Oracle.ManagedDataAccess.Client.OracleConnection(conn);
                dbConn.InfoMessage += (s, e) => Debug.WriteLine($"{nameof(dbConn)} InfoMessage: {e.Message}");

                var connOption = new DbKitConnectionOption
                {
                    ConnectionType = DBTypes.Oracle,
                    Connection = dbConn,
                    AutoClose = false
                };
                var dbKit = new DbKit(connOption);

                var result = await dbKit.SqlExecuteDataOnly(sql);
            }
        }

        [Fact]
        public void OracleReader()
        {
            var connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=local.host)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=helowin)));User Id=CQSME;Password=CQSME";
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
                        Debug.WriteLine($"{rowCount} rows, memory: {ParsingTo.FormatByte(Environment.WorkingSet)}");
                        dt.Clear();
                    }
                }
            } while (reader.NextResult());

            Debug.WriteLine($"Done! memory: {ParsingTo.FormatByte(Environment.WorkingSet)}");
        }

        [Fact]
        public async Task Test()
        {
            var connectionString = "Server=192.168.100.3;Port=52171;User ID=polardbx_root;Password=jfOHgHWQ;Database=cqent;";
            var connOption = new DbKitConnectionOption
            {
                ConnectionString = connectionString,
                ConnectionType = DBTypes.MySQL
            };

            var dbKit = connOption.CreateDbInstance();
            var eds = await dbKit.SqlExecuteDataSet("select * from analyse_industry_avg");

            var dt = eds.Datas.Tables[0];
            dt.TableName = "analyse_industry_avg";

            for (int i = 0; i < 99; i++)
            {
                var dr = dt.NewRow();

                dr[0] = Guid.NewGuid().ToString();
                dr[1] = Guid.NewGuid().ToString();

                dr[2] = Snowflake53To.Id();
                dr[3] = Snowflake53To.Id();
                dr[4] = Snowflake53To.Id();

                dr[5] = DateTime.Now.Month.ToString();
                dr[6] = DateTime.Now.Year.ToString();
                dr[7] = DateTime.Now;
                dr[8] = DateTime.Now;

                dt.Rows.Add(dr.ItemArray);
            }

            var num = await dbKit.BulkCopyMySQL(dt, openTransaction: false);

            Debug.WriteLine(num);
        }
    }
}
