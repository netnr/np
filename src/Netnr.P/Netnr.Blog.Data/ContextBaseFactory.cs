using Microsoft.EntityFrameworkCore;

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
            GlobalTo.TDB = GlobalTo.GetValue<EnumTo.TypeDB>("TypeDB");
            return CreateDbContextOptionsBuilder<ContextBase>(GlobalTo.TDB, GetConn(), builder);
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
