using Microsoft.AspNetCore.Mvc;
using Netnr.Core;
using Netnr.SharedAdo;
using Netnr.SharedCompile;
using Netnr.SharedDataKit;
using System;
using System.Threading.Tasks;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// Netnr.Shared
    /// </summary>
    [Route("[controller]/[action]")]
    public class SharedController : Controller
    {
        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="conn">连接字符串</param>
        /// <param name="sql"></param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM To_Ado_SQLite(string conn = @"Data Source=https://s1.netnr.eu.org/2020/05/22/2037505934.db", string sql = "select sqlite_version();PRAGMA encoding;select datetime();")
        {
            var vm = new SharedResultVM();

            if (conn.Contains("://"))
            {
                var dbname = Math.Abs(conn.GetHashCode()).ToString() + ".db";
                var fileName = PathTo.Combine(System.IO.Path.GetTempPath(), dbname);
                if (!System.IO.File.Exists(fileName))
                {
                    HttpTo.DownloadSave(HttpTo.HWRequest(conn.Split('=')[1]), fileName);
                }
                conn = @$"Data Source={fileName}";
            }

            var db = new DbHelper(new System.Data.SQLite.SQLiteConnection(conn));
            var ds = db.SqlQuery(sql);

            vm.Log.Add(conn);
            vm.Data = ds;

            return vm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM To_DataKit_Aid()
        {
            return SharedResultVM.Try(vm =>
            {
                var conn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.100.115)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=EE.Oracle.Docker)));User Id=CQSME;Password=SMECQ;";
                vm.Data = DataKitAidTo.SqlMappingCsharp(SharedEnum.TypeDB.Oracle, "PLTF_ACTIVITY", conn);
                vm.Set(SharedEnum.RTag.success);

                return vm;
            });
        }

        /// <summary>
        /// User-Agent
        /// </summary>
        /// <param name="ua">User-Agent</param>
        /// <param name="loop">循环</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM To_UserAgent(string ua = "", int loop = 1)
        {
            var vm = new SharedResultVM();

            if (string.IsNullOrWhiteSpace(ua))
            {
                ua = Request.HttpContext.Request.Headers["User-Agent"].ToString();
            }

            Parallel.For(0, loop, i =>
            {
                _ = new SharedUserAgent.UserAgentTo(ua);
            });

            var uainfo = new SharedUserAgent.UserAgentTo(ua);
            vm.Data = uainfo;

            return vm;
        }

        /// <summary>
        /// 动态编译并执行
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM To_Compile()
        {
            return SharedResultVM.Try(vm =>
            {
                var code = @"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class DynamicCompile
{
    public void Main()
    {
        Console.WriteLine(DateTime.Now);
        Console.WriteLine(Environment.OSVersion);
        Console.WriteLine(Environment.SystemDirectory);
        Console.WriteLine(Environment.Version);
        Console.WriteLine(RuntimeInformation.FrameworkDescription);
        Console.WriteLine(RuntimeInformation.OSDescription);
    }
}
";
                vm.Msg = code;
                var ce = CompileTo.Executing(code, "System.Runtime.InteropServices.RuntimeInformation.dll".Split(",")).Split(Environment.NewLine);
                foreach (var item in ce)
                {
                    vm.Log.Add(item);
                }

                return vm;
            });
        }

    }
}
