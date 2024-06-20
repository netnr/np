using DuckDB.NET.Data;
using DuckDbSharp;
using Xunit;

namespace Netnr.Test
{
    /// <summary>
    /// https://github.com/Giorgi/DuckDB.NET
    /// </summary>
    public class TestDatabaseDuckDB
    {
        /*
            INTEGER：有符号整数类型，通常表示整数值。
            BIGINT：大整数类型，用于存储较大范围的整数值。
            SMALLINT：小整数类型，用于存储较小范围的整数值。

            DECIMAL：十进制数类型，用于存储精确的小数值。
            DOUBLE：双精度浮点数类型，用于存储更大范围的浮点数值。
            FLOAT：单精度浮点数类型，用于存储较小范围的浮点数值。

            VARCHAR：可变长度字符串类型，存储可变长度的字符序列。
            CHAR：固定长度字符串类型，存储固定长度的字符序列。

            BOOLEAN：布尔类型，用于存储真值（true/false）。

            DATE：日期类型，用于存储日期值。
            TIME：时间类型，用于存储时间值。
            TIMESTAMP：时间戳类型，用于存储日期和时间值。

            BLOB：二进制大对象类型，用于存储大块二进制数据。
            ARRAY：数组类型，用于存储多个相同类型的值。
         */

        [Fact]
        public async Task DuckDB_1()
        {
            Debug.WriteLine("tmp");

            using var connection = new DuckDBConnection(@"Data Source=E:\package\res\duck_test_09.db");
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT current_setting('access_mode');";
            var executeScalar = command.ExecuteScalar();

            //command.CommandText = "set access_mode=READ_ONLY";
            //var executeNonQuery = command.ExecuteNonQuery();

            command.CommandText = "from duckdb_extensions();";
            var duckdb_extensions = await command.ReaderDataSetAsync();

            using (var duckDbCommand = connection.CreateCommand())
            {
                var table = "CREATE TABLE large_table(c1 INTEGER, c2 BIGINT, c3 DECIMAL, c4 TIMESTAMP, c5 VARCHAR);";
                duckDbCommand.CommandText = table;
                duckDbCommand.ExecuteNonQuery();
            }

            var st = Stopwatch.StartNew();

            using var appender = connection.CreateAppender("large_table");
            for (var i = 0; i < 9_999_999; i++)
            {
                var row = appender.CreateRow();
                row.AppendValue(i)
                    .AppendValue(Snowflake53To.Id())
                    .AppendValue(i * 1.0)
                    .AppendValue(DateTime.Now)
                    .AppendValue(Guid.NewGuid().ToString("N"))
                    .EndRow();
            }

            Debug.WriteLine(st.Elapsed);
        }

        [Fact]
        public void DuckDB_2()
        {
            Debug.WriteLine("tmp");

            var db = ThreadSafeTypedDuckDbConnection.Create(@"D:\tmp\res\duck_test.db");
            var executeScalar = db.ExecuteScalar("SELECT current_setting('access_mode');", null);
            Debug.WriteLine(executeScalar);

            db.CreateTable<TestTable>("test_table", true);

            var st = Stopwatch.StartNew();
            var items = new List<TestTable>(1_999_999);
            for (var i = 0; i < 1_999_999; i++)
            {
                items.Add(new TestTable
                {
                    Col1 = Snowflake53To.Id(),
                    Col2 = i * 1.0,
                    Col4 = i * 1.0m,
                    Col5 = Guid.NewGuid().ToString("N"),
                    Col6 = DateTime.Now,
                    Col7 = i % 2 == 0
                });
            }
            Debug.WriteLine(st.Elapsed);
            st.Restart();

            db.InsertRange("test_table", items);

            Debug.WriteLine(st.Elapsed);
        }

        [Fact]
        public async Task DuckDB_4()
        {
            using var connection = new DuckDBConnection(@"Data Source=D:/package/res/duck_test_10.db");
            await connection.OpenAsync();

            Debug.WriteLine($"建表 files(id,name)");
            var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE files(id INTEGER, name VARCHAR)";
            await command.ExecuteNonQueryAsync();

            var st = Stopwatch.StartNew();
            using var appender = connection.CreateAppender("files");
            for (var i = 1; i <= 2_000_000; i++)
            {
                var name = i % 5 == 0 ? $"{Guid.NewGuid()}.py" : Guid.NewGuid().ToString();
                appender.CreateRow().AppendValue(i).AppendValue(name).EndRow();
            }
            Debug.WriteLine($"写入 200 万条数据耗时：{st.Elapsed}");
            st.Restart();

            command.CommandText = "SELECT name FROM files WHERE name like '%.py'";
            var list = await command.ReaderDataOnlyAsync();
            Debug.WriteLine($"查找 .py 结尾的文件共 {list.Datas.Tables[0].Rows.Count} 条，耗时：{st.Elapsed}");
        }

        class TestTable
        {
            public long Col1;
            public double Col2;
            public decimal Col4;
            public string Col5;
            public DateTime Col6;
            public bool Col7;
        }
    }
}