namespace Netnr.ResponseFramework.Application.Datas;

/// <summary>
/// 数据库工厂
/// </summary>
public class ContextBaseFactory : DbContextTo
{
    /// <summary>
    /// 创建 新的数据库上下文
    /// </summary>
    /// <returns></returns>
    public static ContextBase CreateDbContext()
    {
        var cob = CreateDbContextOptionsBuilder<ContextBase>(AppTo.TDB);
        return new ContextBase(cob.Options);
    }
}