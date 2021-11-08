using DocumentFormat.OpenXml.Packaging;
using Microsoft.Data.Sqlite;
using Netnr.Core;
using Netnr.SharedAdo;
using Netnr.SharedCompile;
using Netnr.SharedDataKit;
using System.Text.Json;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// Netnr.Shared
    /// </summary>
    [Route("[controller]/[action]")]
    public class SharedController : Controller
    {
        /// <summary>
        /// 执行SQL获取打印消息
        /// </summary>
        /// <param name="typeDB"></param>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM To_AdoInfo(SharedEnum.TypeDB typeDB, string conn, string sql)
        {
            var vm = new SharedResultVM();

            DbConnection dbConn = null;

            switch (typeDB)
            {
                case SharedEnum.TypeDB.MySQL:
                case SharedEnum.TypeDB.MariaDB:
                    {
                        var dbc = new MySqlConnector.MySqlConnection(conn);
                        dbc.InfoMessage += (s, e) => vm.Log.Add(e.Errors.FirstOrDefault()?.Message);
                        dbConn = dbc;
                    }
                    break;
                case SharedEnum.TypeDB.Oracle:
                    {
                        var dbc = new Oracle.ManagedDataAccess.Client.OracleConnection(conn);
                        dbc.InfoMessage += (s, e) => vm.Log.Add(e.Message);
                        dbConn = dbc;
                    }
                    break;
                case SharedEnum.TypeDB.SQLServer:
                    {
                        var dbc = new Microsoft.Data.SqlClient.SqlConnection(conn);
                        dbc.InfoMessage += (s, e) => vm.Log.Add(e.Message);
                        dbConn = dbc;
                    }
                    break;
            }

            var db = new DbHelper(dbConn);
            db.SqlExecuteReader(sql);

            return vm;
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
