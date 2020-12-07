using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Netnr.SharedFast;

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
            System.Enum.TryParse(GlobalTo.GetValue("TypeDB"), true, out SharedEnum.TypeDB tdb);
            var conn = GlobalTo.Configuration.GetConnectionString(tdb.ToString()).Replace("~", GlobalTo.ContentRootPath);
            return SharedDbContext.FactoryTo.CreateDbContextOptionsBuilder<ContextBase>(tdb, conn, builder);
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