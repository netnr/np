# if Full || DbContext

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Netnr
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public class FactoryTo
    {
        /// <summary>
        /// 应用程序不为每个上下文实例创建新的ILoggerFactory实例非常重要。这样做会导致内存泄漏和性能下降
        /// </summary>
        private static ILoggerFactory logFactory = null;
        public static ILoggerFactory LogFactory
        {
            get
            {
                if (logFactory == null)
                {
                    logFactory = LoggerFactory.Create(logging => logging.AddConsole().AddFilter(level => level >= LogLevel.Information));
                }
                return logFactory;
            }
        }

        /// <summary>
        /// 创建 DbContextOptionsBuilder
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="connnectionString">连接字符串</param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<T> CreateDbContextOptionsBuilder<T>(EnumTo.TypeDB tdb, string connnectionString, DbContextOptionsBuilder builder = null) where T : DbContext
        {
            if (builder == null)
            {
                builder = new DbContextOptionsBuilder<T>();
            }

            if (!builder.IsConfigured)
            {
                switch (tdb)
                {
                    case EnumTo.TypeDB.InMemory:
#if DbContextInMemory
                        builder.UseInMemoryDatabase(connnectionString);
#endif
                        break;
                    case EnumTo.TypeDB.SQLite:
#if DbContextSQLite
                        builder.UseSqlite(connnectionString);
#endif
                        break;
                    case EnumTo.TypeDB.MySQL:
                    case EnumTo.TypeDB.MariaDB:
#if DbContextMySQL
                        builder.UseMySql(connnectionString, ServerVersion.AutoDetect(connnectionString));
#endif
                        break;
                    case EnumTo.TypeDB.Oracle:
#if DbContextOracle
                        builder.UseOracle(connnectionString);
#endif
                        break;
                    case EnumTo.TypeDB.SQLServer:
#if DbContextSQLServer
                        builder.UseSqlServer(connnectionString);
#endif
                        break;
                    case EnumTo.TypeDB.PostgreSQL:
#if DbContextPostgreSQL
                        builder.UseNpgsql(connnectionString);
#endif
                        break;
                }
            }
            builder.UseLoggerFactory(LogFactory).EnableSensitiveDataLogging().EnableDetailedErrors();

            return builder as DbContextOptionsBuilder<T>;
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="typeDB"></param>
        /// <returns></returns>
        public static string GetConn(EnumTo.TypeDB? typeDB = null)
        {
            var tdb = typeDB ?? GlobalTo.TDB;
            var conn = GlobalTo.Configuration.GetConnectionString(tdb.ToString());
            if (tdb != EnumTo.TypeDB.InMemory)
            {
                var pwd = GlobalTo.GetValue("ConnectionStrings:Password");
                conn = DbHelper.SqlConnEncryptOrDecrypt(conn, pwd);

                if (tdb == EnumTo.TypeDB.SQLite)
                {
                    conn = conn.Replace("~", GlobalTo.ContentRootPath);
                }

                conn = DbHelper.SqlConnPreCheck(tdb, conn);
                return conn;
            }
            return null;
        }
    }
}

#endif