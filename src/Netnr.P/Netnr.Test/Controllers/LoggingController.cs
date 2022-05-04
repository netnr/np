using Netnr.SharedLogging;
using System.Collections.Concurrent;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// 日志
    /// </summary>
    [Route("[controller]/[action]")]
    public class LoggingController : Controller
    {
        public LoggingController()
        {
            LoggingTo.OptionsDbPart = LoggingTo.DBPartType.Year;
        }

        [HttpGet]
        public SharedResultVM Query()
        {
            var vm = new SharedResultVM();

            try
            {
                var now = DateTime.Now;
                vm.Data = LoggingTo.Query(now, now.AddYears(30));
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        [HttpGet]
        public SharedResultVM Query2()
        {
            var vm = new SharedResultVM();

            var connString = @"Data Source=D:\tmp\log.db";
            var sqlTotal = "select count(*) from LoggingModel";
            var sqlList = "select * from LoggingModel limit 30 offset 0";

            {
                var conn = new System.Data.SQLite.SQLiteConnection(connString);
                conn.Open();
                vm.Log.Add($"{conn.GetType().Assembly.FullName}");

                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlTotal;
                var val = cmd.ExecuteScalar();
                vm.Log.Add($"count: {val},use: {vm.PartTime()}ms");

                cmd.CommandText = sqlList;
                var reader = cmd.ExecuteReader();
                var schema = reader.GetSchemaTable();
                vm.Log.Add($"schema: {schema.Rows.Count},use: {vm.PartTime()}ms");

                var table = new DataTable();
                table.Load(reader);

                vm.Log.Add($"table: {table.Rows.Count},use: {vm.PartTime()}ms");
            }

            vm.Log.Add(string.Empty);

            {
                var conn = new Microsoft.Data.Sqlite.SqliteConnection(connString);
                conn.Open();
                vm.Log.Add($"{conn.GetType().Assembly.FullName}");

                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlTotal;
                var val = cmd.ExecuteScalar();
                vm.Log.Add($"count: {val},use: {vm.PartTime()}ms");

                cmd.CommandText = sqlList;
                var reader = cmd.ExecuteReader();

                var table = new List<object>();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    table.Add(row);
                }

                vm.Log.Add($"sum: {table.ToJson()},use: {vm.PartTime()}ms");
            }

            return vm;
        }

        [HttpGet]
        public SharedResultVM Add()
        {
            var vm = new SharedResultVM();

            var now = DateTime.Now;
            var ct = new SharedApp.ClientTo(HttpContext);

            var listLog = new ConcurrentBag<LoggingModel>();
            Parallel.For(0, 999, i =>
            {
                var lm = new LoggingModel()
                {
                    LogAction = "/",
                    LogCreateTime = now.AddHours(i),
                    LogContent = $"日志内容{i},{now}",
                    LogUserAgent = ct.UserAgent,
                    LogIp = ct.IPv4,
                    LogReferer = ct.Referer
                };
                listLog.Add(lm);
            });

            LoggingTo.Add(listLog);

            vm.Data = listLog.Count;

            return vm;
        }

        /// <summary>
        /// 删除存储目录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM DeleteStorageDir()
        {
            var vm = new SharedResultVM();

            try
            {
                vm.Log.Add(LoggingTo.OptionsDbRoot);
                System.IO.Directory.Delete(LoggingTo.OptionsDbRoot, true);

                vm.Set(SharedEnum.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
