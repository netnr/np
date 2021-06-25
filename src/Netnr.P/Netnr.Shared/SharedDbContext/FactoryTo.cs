# if Full || DbContext

using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Netnr.SharedDbContext
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public static class FactoryTo
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
        public static DbContextOptionsBuilder<T> CreateDbContextOptionsBuilder<T>(SharedEnum.TypeDB tdb, string connnectionString, DbContextOptionsBuilder builder = null) where T : DbContext
        {
            if (builder == null)
            {
                builder = new DbContextOptionsBuilder<T>();
            }

            if (!builder.IsConfigured)
            {
                switch (tdb)
                {
                    case SharedEnum.TypeDB.InMemory:
#if DbContextInMemory
                        builder.UseInMemoryDatabase(connnectionString);
#endif
                        break;
                    case SharedEnum.TypeDB.SQLite:
#if DbContextSQLite
                        builder.UseSqlite(connnectionString);
#endif
                        break;
                    case SharedEnum.TypeDB.MySQL:
#if DbContextMySQL
                        builder.UseMySql(connnectionString, ServerVersion.AutoDetect(connnectionString));
#endif
                        break;
                    case SharedEnum.TypeDB.Oracle:
#if DbContextOracle
                        builder.UseOracle(connnectionString);
#endif
                        break;
                    case SharedEnum.TypeDB.SQLServer:
#if DbContextSQLServer
                        builder.UseSqlServer(connnectionString);
#endif
                        break;
                    case SharedEnum.TypeDB.PostgreSQL:
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
        /// 连接字符串加密/解密
        /// </summary>
        /// <param name="conn">连接字符串</param>
        /// <param name="pwd">密码</param>
        /// <param name="ed">1加密 2解密</param>
        public static string ConnnectionEncryptOrDecrypt(string conn, string pwd, int ed = 1)
        {
            switch (ed)
            {
                //解密
                case 2:
                    {
                        var ckey = "CONNED" + conn.GetHashCode();
                        if (Core.CacheTo.Get(ckey) is not string cval)
                        {
                            var clow = conn.ToLower();
                            var pts = new List<string> { "database", "server", "filename", "source", "user" };
                            if (!pts.Any(x => clow.Contains(x)))
                            {
                                cval = Core.CalcTo.AESDecrypt(conn, pwd);
                            }
                            else
                            {
                                cval = conn;
                            }
                            Core.CacheTo.Set(ckey, cval);
                        }
                        return cval;
                    }
                //加密
                default:
                    return conn = Core.CalcTo.AESEncrypt(conn, pwd);
            }
        }
    }
}

#endif