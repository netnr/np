using Microsoft.AspNetCore.Mvc;
using Netnr.Core;
using Netnr.SharedAdo;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.SQLite;
using System.Diagnostics;

namespace Netnr.Test.Controllers
{
    /// <summary>
    /// Netnr.Shared
    /// </summary>
    public class SharedController : Controller
    {
        public IActionResult Index()
        {
            return Ok();
        }

        public IActionResult Test1()
        {
            var ss = new SystemStatusTo();
            return Content(ss.ToView());
        }

        public IActionResult Test2()
        {
            var ss = new SystemStatusTo();
            return Content(ss.ToJson());
        }

        public SharedResultVM Test3()
        {
            var vm = new SharedResultVM();

            var conn = @"Data Source=E:\TEMP\netnrf.db";
            var db = new DbHelper(new SQLiteConnection(conn));
            var ds = db.SqlQuery("PRAGMA table_info('SysUser');PRAGMA table_info('SysRole')");
            vm.Data = ds;

            return vm;
        }

        public SharedResultVM Test4()
        {
            var vm = new SharedResultVM();
            
            Console.WriteLine("TimeZoneInfo.Local.DisplayName = {0}", TimeZoneInfo.Local.DisplayName);
            Console.WriteLine("TimeZoneInfo.Local.Id = {0}", TimeZoneInfo.Local.Id);
            Console.WriteLine("TimeZoneInfo.Local.StandardName = {0}", TimeZoneInfo.Local.StandardName);
            Console.WriteLine("TimeZoneInfo.Local.DaylightName = {0}", TimeZoneInfo.Local.DaylightName);
            Console.WriteLine(string.Empty);

            var str = new OracleConnectionStringBuilder
            {
                UserID = "<username>",
                Password = "<password>",
                DataSource = "<database name>"
            };
            using (var con = new OracleConnection(str.ConnectionString))
            {
                con.Open();
                Console.WriteLine("Oracle.DataAccess: OracleConnection -> SessionInfo.TimeZone = {0}", con.GetSessionInfo().TimeZone);
                Console.WriteLine("Oracle.DataAccess: Version = {0}", FileVersionInfo.GetVersionInfo(con.GetType().Assembly.Location).FileVersion.ToString());

                var tz = new OracleCommand("SELECT SESSIONTIMEZONE FROM dual", con).ExecuteScalar();
                Console.WriteLine("Oracle.DataAccess: SESSIONTIMEZONE = {0}", tz.ToString());
                con.Close();
            }
            Console.WriteLine(string.Empty);

            var strm = new OracleConnectionStringBuilder();
            str.UserID = "<username>";
            str.Password = "<password>";
            str.DataSource = "<database name>";
            using (var con = new OracleConnection(str.ConnectionString))
            {
                con.Open();
                Console.WriteLine("Oracle.ManagedDataAccess: OracleConnection -> SessionInfo.TimeZone = {0}", con.GetSessionInfo().TimeZone);
                Console.WriteLine("Oracle.ManagedDataAccess: Version = {0}", FileVersionInfo.GetVersionInfo(con.GetType().Assembly.Location).FileVersion.ToString());

                var tz = new OracleCommand("SELECT SESSIONTIMEZONE FROM dual", con).ExecuteScalar();
                Console.WriteLine("Oracle.ManagedDataAccess: SESSIONTIMEZONE = {0}", tz.ToString());
                con.Close();
            }

            return vm;
        }

    }
}
