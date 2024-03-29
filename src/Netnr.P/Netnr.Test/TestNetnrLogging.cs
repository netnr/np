﻿using System.Collections.Concurrent;
using Xunit;

namespace Netnr.Test
{
    public class TestNetnrLogging
    {
        [Fact]
        public async Task DCQ()
        {
            var now = DateTime.Now;

            //删除
            Debug.WriteLine($"删除数据库根目录: {LoggingTo.OptionsDbRoot}");
            if (Directory.Exists(LoggingTo.OptionsDbRoot))
            {
                Directory.Delete(LoggingTo.OptionsDbRoot, true);
            }

            //并行写入日志记录
            var listLog = new ConcurrentBag<LoggingModel>();
            Parallel.For(0, 999, i =>
            {
                var lm = new LoggingModel()
                {
                    LogAction = "/",
                    LogCreateTime = now.AddHours(i),
                    LogContent = $"日志内容{i},{now}",
                    LogUserAgent = "Chrome/54.0 (Windows NT 10.0)",
                    LogIp = "127.0.0.1",
                    LogReferer = "/"
                };
                listLog.Add(lm);
            });
            LoggingTo.Add(listLog);

            //等待写入完成（初始化 UAParser 需要约 5 秒）
            Thread.Sleep(10000);
            Assert.True(LoggingTo.CurrentCacheLog.IsEmpty);

            //查询1
            var result1 = await LoggingTo.Query(now.AddHours(-1), now.AddHours(1000), 1, 50);
            Assert.True((result1.RowData as DataTable).Rows.Count == 50);
            Assert.True(result1.RowCount == 999);

            //查询2
            var result2 = await LoggingTo.Query($"select * from (TABLE)", "select * from (TABLE) limit 50 offset 0");
            Assert.True((result2.RowData as DataTable).Rows.Count == 50);
            Assert.True(result2.RowCount == 999);
        }
    }
}
