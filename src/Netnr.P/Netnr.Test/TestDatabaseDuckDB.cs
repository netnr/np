using DuckDB.NET.Data;
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
        public void DuckDB_1()
        {
            Debug.WriteLine("tmp");

            using var connection = new DuckDBConnection("Data Source=D:/tmp/res/duck.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "set access_mode=READ_ONLY";
            var executeNonQuery = command.ExecuteNonQuery();

            command.CommandText = "SELECT current_setting('access_mode');";
            var executeScalar = command.ExecuteScalar();

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
    }
}