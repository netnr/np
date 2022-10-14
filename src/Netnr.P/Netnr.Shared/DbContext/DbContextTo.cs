# if Full || DbContext

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Netnr
{
    /// <summary>
    /// 数据库
    /// </summary>
    public class DbContextTo
    {
        /// <summary>
        /// 应用程序不为每个上下文实例创建新的ILoggerFactory实例非常重要。这样做会导致内存泄漏和性能下降
        /// </summary>
        private static ILoggerFactory logFactory = null;
        /// <summary>
        /// 日志
        /// </summary>
        public static ILoggerFactory LogFactory
        {
            get
            {
                logFactory ??= LoggerFactory.Create(logging => logging.AddConsole().AddFilter(level => level >= LogLevel.Information));
                return logFactory;
            }
        }

        /// <summary>
        /// 创建 DbContextOptionsBuilder
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<T> CreateDbContextOptionsBuilder<T>(EnumTo.TypeDB tdb, DbContextOptionsBuilder options = null) where T : DbContext
        {
            options ??= new DbContextOptionsBuilder<T>();

            if (!options.IsConfigured)
            {
                var connnectionString = GetConn();
                switch (tdb)
                {
                    case EnumTo.TypeDB.InMemory:
#if DbContextInMemory
                        builder.UseInMemoryDatabase(connnectionString);
#endif
                        break;
                    case EnumTo.TypeDB.SQLite:
#if DbContextSQLite
                        options.UseSqlite(connnectionString);
#endif
                        break;
                    case EnumTo.TypeDB.MySQL:
                    case EnumTo.TypeDB.MariaDB:
#if DbContextMySQL
                        options.UseMySql(connnectionString, ServerVersion.AutoDetect(connnectionString));
#endif
                        break;
                    case EnumTo.TypeDB.Oracle:
#if DbContextOracle
                        builder.UseOracle(connnectionString);
#endif
                        break;
                    case EnumTo.TypeDB.SQLServer:
#if DbContextSQLServer
                        options.UseSqlServer(connnectionString);
#endif
                        break;
                    case EnumTo.TypeDB.PostgreSQL:
#if DbContextPostgreSQL
                        options.UseNpgsql(connnectionString);
#endif
                        break;
                }
            }
            options.UseLoggerFactory(LogFactory).EnableSensitiveDataLogging().EnableDetailedErrors();

            return options as DbContextOptionsBuilder<T>;
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="typeDB"></param>
        /// <returns></returns>
        public static string GetConn(EnumTo.TypeDB? typeDB = null)
        {
            var tdb = typeDB ?? AppTo.TDB;
            var conn = AppTo.Configuration.GetConnectionString(tdb.ToString());
            if (tdb != EnumTo.TypeDB.InMemory)
            {
                var pwd = AppTo.GetValue("ConnectionStrings:Password");
                conn = DbHelper.SqlConnEncryptOrDecrypt(conn, pwd);

                if (tdb == EnumTo.TypeDB.SQLite)
                {
                    conn = conn.Replace("~", AppTo.ContentRootPath);
                }

                conn = DbHelper.SqlConnPreCheck(tdb, conn);
                return conn;
            }
            return null;
        }
    }
}

#endif