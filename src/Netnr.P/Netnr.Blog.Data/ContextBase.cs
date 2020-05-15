using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Netnr.Blog.Data
{
    public partial class ContextBase : DbContext
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public readonly TypeDB TDB;

        /// <summary>
        /// 构造
        /// </summary>
        public ContextBase()
        {
            Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out TDB);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ContextBase(DbContextOptions<ContextBase> options) : base(options)
        {

        }

        private static ILoggerFactory _loggerFactory = null;

        /// <summary>
        /// 日志
        /// </summary>
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

        /// <summary>
        /// 配置连接字符串
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
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
                        optionsBuilder.UseNpgsql(GlobalTo.Configuration.GetConnectionString(TDB.ToString()));
                        break;
                }

                //注册日志（修改日志等级为Information，可查看执行的SQL语句）
                optionsBuilder.UseLoggerFactory(LoggerFactory);
            }
        }
    }
}