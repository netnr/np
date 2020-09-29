using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Netnr.ResponseFramework.Data
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public class ContextBaseFactory
    {
        /// <summary>
        /// 应用程序不为每个上下文实例创建新的ILoggerFactory实例非常重要。这样做会导致内存泄漏和性能下降
        /// </summary>
        private static ILoggerFactory _loggerFactory = null;
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_loggerFactory == null)
                {
                    var sc = new ServiceCollection();
                    sc.AddLogging(builder => builder.AddConsole().AddFilter(level => level >= LogLevel.Information));
                    _loggerFactory = sc.BuildServiceProvider().GetService<ILoggerFactory>();
                }
                return _loggerFactory;
            }
        }

        /// <summary>
        /// 创建 DbContextOptionsBuilder
        /// </summary>
        /// <returns></returns>
        public static DbContextOptionsBuilder<ContextBase> CreateDbContextOptionsBuilder(DbContextOptionsBuilder builder = null)
        {
            if (builder == null)
            {
                builder = new DbContextOptionsBuilder<ContextBase>();
            }

            if (!builder.IsConfigured)
            {
                System.Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out GlobalTo.TDB);

                var conn = GlobalTo.GetConn();

                switch (GlobalTo.TDB)
                {
                    case TypeDB.MySQL:
                        builder.UseMySql(conn);
                        break;
                    case TypeDB.SQLite:
                        builder.UseSqlite(conn.Replace("~", GlobalTo.ContentRootPath));
                        break;
                    case TypeDB.SQLServer:
                        builder.UseSqlServer(conn, options =>
                        {
                            //启用 row_number 分页 （兼容2005、2008）
                            //options.UseRowNumberForPaging();
                        });
                        break;
                    case TypeDB.PostgreSQL:
                        {
                            builder.UseNpgsql(conn);
                        }
                        break;
                    case TypeDB.InMemory:
                        builder.UseInMemoryDatabase(conn);
                        break;
                }

                //注册日志（修改日志等级为Information，可查看执行的SQL语句）
                builder.EnableSensitiveDataLogging();
                builder.UseLoggerFactory(LoggerFactory);
            }

            return builder as DbContextOptionsBuilder<ContextBase>;
        }

        /// <summary>
        /// 创建 新的数据库上下文
        /// </summary>
        /// <returns></returns>
        public static ContextBase CreateDbContext()
        {
            return new ContextBase(CreateDbContextOptionsBuilder().Options);
        }
    }
}
