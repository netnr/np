using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;

namespace Netnr.ResponseFramework.Data
{
    /// <summary>
    /// ContextBase 连接
    /// </summary>
    public partial class ContextBase : DbContext
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public static TypeDB TDB;

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
                    sc.AddLogging(builder => builder.AddConsole().AddFilter(level => level >= LogLevel.Warning));
                    _loggerFactory = sc.BuildServiceProvider().GetService<ILoggerFactory>();
                }
                return _loggerFactory;
            }
        }

        public ContextBase(DbContextOptions<ContextBase> options) : base(options)
        {

        }

        /// <summary>
        /// 得到一个 DbContextOptionsBuilder 对象
        /// </summary>
        /// <param name="optionsBuilder">初始化该对象</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<ContextBase> DCOB(DbContextOptionsBuilder optionsBuilder = null)
        {
            DbContextOptionsBuilder<ContextBase> cb = null;

            if (optionsBuilder == null)
            {
                cb = new DbContextOptionsBuilder<ContextBase>();
                optionsBuilder = cb;
            }

            if (!optionsBuilder.IsConfigured)
            {
                System.Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out TDB);

                switch (TDB)
                {
                    case TypeDB.MySQL:
                        optionsBuilder.UseMySql(GlobalTo.Configuration.GetConnectionString(TDB.ToString()));
                        break;
                    case TypeDB.SQLite:
                        optionsBuilder.UseSqlite(GlobalTo.Configuration.GetConnectionString(TDB.ToString()).Replace("~", GlobalTo.ContentRootPath));
                        break;
                    case TypeDB.InMemory:
                        optionsBuilder.UseInMemoryDatabase(GlobalTo.Configuration.GetConnectionString(TDB.ToString()));
                        break;
                    case TypeDB.SQLServer:
                        optionsBuilder.UseSqlServer(GlobalTo.Configuration.GetConnectionString(TDB.ToString()), options =>
                        {
                            //启用 row_number 分页 （兼容2005、2008）
                            //options.UseRowNumberForPaging();
                        });
                        break;
                    case TypeDB.PostgreSQL:
                        {
                            optionsBuilder.UseNpgsql(GlobalTo.Configuration.GetConnectionString(TDB.ToString()));
                        }

                        break;
                }

                //注册日志（修改日志等级为Information，可查看执行的SQL语句）
                optionsBuilder.UseLoggerFactory(LoggerFactory);
            }

            return cb;
        }
    }
}