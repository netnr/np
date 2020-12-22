using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Netnr.SharedDbContext;
using Netnr.SharedFast;

namespace Netnr.Blog.Data
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
            var conn = GlobalTo.Configuration.GetConnectionString(GlobalTo.TDB.ToString());
            if (GlobalTo.TDB != SharedEnum.TypeDB.InMemory)
            {
                var pwd = GlobalTo.GetValue("ConnectionStrings:Password");
                conn = FactoryTo.ConnnectionEncryptOrDecrypt(conn, pwd, 2);
            }
            return FactoryTo.CreateDbContextOptionsBuilder<ContextBase>(GlobalTo.TDB, conn.Replace("~", GlobalTo.ContentRootPath), builder);
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
