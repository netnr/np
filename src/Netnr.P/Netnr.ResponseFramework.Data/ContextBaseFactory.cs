using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Netnr.SharedFast;

namespace Netnr.ResponseFramework.Data
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public class ContextBaseFactory
    {
        /// <summary>
        /// 创建 DbContextOptionsBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<ContextBase> CreateDbContextOptionsBuilder(DbContextOptionsBuilder builder = null)
        {
            System.Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out GlobalTo.TDB);
            var conn = GlobalTo.Configuration.GetConnectionString(GlobalTo.TDB.ToString()).Replace("~", GlobalTo.ContentRootPath);
            return SharedDbContext.FactoryTo.CreateDbContextOptionsBuilder<ContextBase>(GlobalTo.TDB, conn, builder);
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
