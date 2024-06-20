#if Full || Ado || AdoAll

namespace Netnr;

/// <summary>
/// 连接配置
/// </summary>
public partial class DbKitConnectionOption
{
    /// <summary>
    /// 连接类型
    /// </summary>
    public DBTypes ConnectionType { get; set; }

    /// <summary>
    /// 连接对象
    /// </summary>
    public DbConnection Connection { get; set; }

    /// <summary>
    /// 自动关闭连接，改为 False 需要手动调用 Close() 方法，默认 True
    /// </summary>
    public bool AutoClose { get; set; } = true;

    /// <summary>
    /// 默认超时 300
    /// </summary>
    public int Timeout { get; set; } = 300;

    private string _connectionString;
    /// <summary>
    /// 连接字符串（自动预检）
    /// </summary>
    public string ConnectionString
    {
        get
        {
            var conn = DbKitExtensions.PreCheckConn(_connectionString, ConnectionType);
            return conn;
        }
        set
        {
            _connectionString = value;
        }
    }

    /// <summary>
    /// 连接备注
    /// </summary>
    public string ConnectionRemark { get; set; }

    private string _DatabaseName = null;
    /// <summary>
    /// 指定数据库名，构建连接对象时调用 SetConnDatabaseName，默认从连接字符串读取
    /// </summary>
    public string DatabaseName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_DatabaseName))
            {
                //默认数据库名
                return this.GetDefaultDatabaseName();
            }
            else
            {
                return _DatabaseName;
            }
        }
        set
        {
            _DatabaseName = value;
        }
    }

    /// <summary>
    /// 深拷贝新实例（避免影响），默认 False
    /// </summary>
    public bool DeepCopyNewInstance { get; set; } = false;
}

/// <summary>
/// 执行配置
/// </summary>
public partial class DbKitCommandOption
{
    /// <summary>
    /// 标识（可能重复）
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 执行对象
    /// </summary>
    public DbCommand Command { get; set; }

    /// <summary>
    /// 默认自动提交事务 True
    /// </summary>
    public bool AutoCommit { get; set; } = true;

    /// <summary>
    /// 开启事务（不支持并行事务）
    /// </summary>
    /// <param name="level"></param>
    public async Task OpenTransactionAsync(IsolationLevel? level = null)
    {
        if (Command != null)
        {
            Command.Transaction = await Command.Connection.BeginTransactionAsync(level ?? IsolationLevel.Unspecified);
        }
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    public async Task CommitAsync()
    {
        if (Command?.Transaction != null)
        {
            await Command.Transaction.CommitAsync();
        }
    }
}

/// <summary>
/// TableSchema
/// </summary>
public partial class DbKitSchemaResult
{
    /// <summary>
    /// 空表
    /// </summary>
    public DataTable Table { get; set; }
    /// <summary>
    /// 主键列（考虑复合主键列只查询一列时的情况，先填充数据，再设置主键，即使是出错）
    /// Table.PrimaryKey = KeyColumns.ToArray();
    /// </summary>
    public List<DataColumn> KeyColumns { get; set; } = [];
    /// <summary>
    /// 表结构（元数据）
    /// </summary>
    public DataTable Schema { get; set; }
}

/// <summary>
/// Reader Result
/// </summary>
public partial class DbKitDataSetResult
{
    /// <summary>
    /// 数据集
    /// </summary>
    public DataSet Datas { get; set; }
    /// <summary>
    /// 受影响的行数，默认 -1
    /// </summary>
    public int RecordsAffected { get; set; } = -1;
    /// <summary>
    /// 表结构（元数据）
    /// </summary>
    public DataSet Schemas { get; set; }
}

/// <summary>
/// Reader Result
/// </summary>
public partial class DbKitDataOnlyResult
{
    /// <summary>
    /// 数据集
    /// </summary>
    public DataSet Datas { get; set; }
    /// <summary>
    /// 受影响的行数，默认 -1
    /// </summary>
    public int RecordsAffected { get; set; } = -1;
}

#endif