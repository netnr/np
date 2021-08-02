using Microsoft.EntityFrameworkCore;
using Netnr.SharedDbContext;
using Netnr.SharedFast;

namespace Netnr.Blog.Data
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public class ContextBaseFactory : FactoryTo
    {
        /// <summary>
        /// 创建 DbContextOptionsBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<ContextBase> CreateDbContextOptionsBuilder(DbContextOptionsBuilder builder = null)
        {
            System.Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out GlobalTo.TDB);
            var conn = GetConn().Replace("~", GlobalTo.ContentRootPath);
            return CreateDbContextOptionsBuilder<ContextBase>(GlobalTo.TDB, conn, builder);
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
