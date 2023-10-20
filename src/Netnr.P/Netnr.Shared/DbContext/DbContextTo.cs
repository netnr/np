#if Full || DbContext

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

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
        public static DbContextOptionsBuilder<T> CreateDbContextOptionsBuilder<T>(DBTypes tdb, DbContextOptionsBuilder options = null) where T : DbContext
        {
            options ??= new DbContextOptionsBuilder<T>();

            if (!options.IsConfigured)
            {
                var connnectionString = GetConn();
                switch (tdb)
                {
                    case DBTypes.InMemory:
#if DbContextInMemory
                        builder.UseInMemoryDatabase(connnectionString);
#endif
                        break;
                    case DBTypes.SQLite:
#if DbContextSQLite
                        options.UseSqlite(connnectionString);
#endif
                        break;
                    case DBTypes.MySQL:
                    case DBTypes.MariaDB:
#if DbContextMySQL
                        options.UseMySql(connnectionString, ServerVersion.AutoDetect(connnectionString));
#endif
                        break;
                    case DBTypes.Oracle:
#if DbContextOracle
                        builder.UseOracle(connnectionString);
#endif
                        break;
                    case DBTypes.SQLServer:
#if DbContextSQLServer
                        options.UseSqlServer(connnectionString);
#endif
                        break;
                    case DBTypes.PostgreSQL:
#if DbContextPostgreSQL
                        options.UseNpgsql(connnectionString);
#endif
                        break;
                    case DBTypes.Dm:
#if DbContextDm
                        options.UseDm(connnectionString);
#endif
                        break;
                }
            }
            options.UseLoggerFactory(LogFactory).EnableSensitiveDataLogging(BaseTo.IsDev).EnableDetailedErrors();

            return options as DbContextOptionsBuilder<T>;
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="dbTypes"></param>
        /// <returns></returns>
        public static string GetConn(DBTypes? dbTypes = null)
        {
            var dbt = dbTypes ?? AppTo.DBT;
            var conn = AppTo.Configuration.GetConnectionString(dbt.ToString());
            if (dbt != DBTypes.InMemory)
            {
                var pwd = AppTo.GetValue("ConnectionStrings:Password");
                conn = DbKitExtensions.SqlConnEncryptOrDecrypt(conn, pwd);

                if (dbt == DBTypes.SQLite)
                {
                    conn = conn.Replace("~", AppTo.ContentRootPath);
                }

                conn = DbKitExtensions.PreCheckConn(conn, dbt);
                return conn;
            }
            return null;
        }

        /// <summary>
        /// 获取实体映射的表名、列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ValueTuple<string, Dictionary<string, string>> GetEntityMap<T>(DbContext db)
        {
            var dict = new Dictionary<string, string>();

            var entityType = db.Model.FindEntityType(typeof(T));
            var tableName = entityType.GetTableName();

            var tableMaps = entityType.GetTableMappings();
            foreach (var tableMap in tableMaps)
            {
                foreach (var columnMap in tableMap.ColumnMappings)
                {
                    //别名:列名
                    dict.Add(columnMap.Property.Name, columnMap.Column.Name);
                }
            }

            return new(tableName, dict);
        }
    }
}

#endif