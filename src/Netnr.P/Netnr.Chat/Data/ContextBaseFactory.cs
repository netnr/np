using Microsoft.EntityFrameworkCore;

namespace Netnr.Chat.Data
{
    /// <summary>
    /// ContextBase 连接
    /// </summary>
    public partial class ContextBaseFactory : DbContext
    {
        /// <summary>
        /// 创建 DbContextOptionsBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<ContextBase> CreateDbContextOptionsBuilder(DbContextOptionsBuilder builder = null)
        {
            Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out EnumTo.TypeDB tdb);
            var conn = GlobalTo.Configuration.GetConnectionString(tdb.ToString()).Replace("~", GlobalTo.ContentRootPath);
            return FactoryTo.CreateDbContextOptionsBuilder<ContextBase>(tdb, conn, builder);
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