using Microsoft.AspNetCore.Mvc;
using Netnr.Core;
using Netnr.SharedAdo;
using System;
using System.Data.SQLite;
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
        public SharedResultVM Ado_SQLite(string conn = @"Data Source=https://cdn.jsdelivr.net/gh/netnr/static/2020/05/22/2037505934.db", string sql = "select sqlite_version();PRAGMA encoding;select datetime();")
        {
            var vm = new SharedResultVM();

            if (conn.Contains("://"))
            {
                var dbname = Math.Abs(conn.GetHashCode()).ToString() + ".db";
                var fileName = PathTo.Combine(System.IO.Path.GetTempPath(), dbname);
                if (!System.IO.File.Exists(fileName))
                {
                    using var wc = new System.Net.WebClient();
                    wc.DownloadFile(conn.Split('=')[1], fileName);
                }
                conn = @$"Data Source={fileName}";
            }

            var db = new DbHelper(new SQLiteConnection(conn));
            var ds = db.SqlQuery(sql);

            vm.Log.Add(conn);
            vm.Data = ds;

            return vm;
        }


        /// <summary>
        /// User-Agent
        /// </summary>
        /// <param name="ua">User-Agent</param>
        /// <param name="loop">循环</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM UserAgentTo(string ua = "", int loop = 1)
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

    }
}
