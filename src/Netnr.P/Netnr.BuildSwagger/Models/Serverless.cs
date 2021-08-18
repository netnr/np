namespace Netnr.BuildSwagger.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
    public class Serverless
    {
        public class PublicResult
        {
            public int code { get; set; }
            public string msg { get; set; }
            public object data { get; set; }
        }

        public class ClockResult
        {
            public int week_number { get; set; }
            public DateTime utc_datetime { get; set; }
            public long unixtime { get; set; }
            public int day_of_year { get; set; }
            public int day_of_week { get; set; }
            public DateTime datetime { get; set; }
            public int time_zone { get; set; }
            public string client_ip { get; set; }
        }

        public class CaptchaResult
        {
            public string text { get; set; }
            public string data { get; set; }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public enum TypeDB
        {
            /// <summary>
            /// Memory
            /// </summary>
            InMemory = 0,
            /// <summary>
            /// SQLite
            /// </summary>
            SQLite = 1,
            /// <summary>
            /// MySQL
            /// </summary>
            MySQL = 2,
            /// <summary>
            /// Oracle
            /// </summary>
            Oracle = 3,
            /// <summary>
            /// SQLServer
            /// </summary>
            SQLServer = 4,
            /// <summary>
            /// PostgreSQL
            /// </summary>
            PostgreSQL = 5
        }
    }
}
