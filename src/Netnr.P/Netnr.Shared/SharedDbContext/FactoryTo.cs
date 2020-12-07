# if Full || DbContext

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
                        builder.UseInMemoryDatabase(connnectionString);
                        break;
                    case SharedEnum.TypeDB.SQLite:
                        builder.UseSqlite(connnectionString);
                        break;
                    case SharedEnum.TypeDB.MySQL:
                        builder.UseMySql(connnectionString, ServerVersion.AutoDetect(connnectionString));
                        break;
                    case SharedEnum.TypeDB.Oracle:
                        builder.UseOracle(connnectionString);
                        break;
                    case SharedEnum.TypeDB.SQLServer:
                        builder.UseSqlServer(connnectionString);
                        break;
                    case SharedEnum.TypeDB.PostgreSQL:
                        builder.UseNpgsql(connnectionString);
                        break;
                }
            }
            builder.UseLoggerFactory(LogFactory).EnableSensitiveDataLogging().EnableDetailedErrors();

            return builder as DbContextOptionsBuilder<T>;
        }
    }
}

#endif