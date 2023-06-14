#if Full || DataKit

namespace Netnr;

/// <summary>
/// 表列信息
/// </summary>
public partial class DataKitColumnResult
{
    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; }
    /// <summary>
    /// 模式名
    /// </summary>
    public string SchemaName { get; set; }
    /// <summary>
    /// 表注释
    /// </summary>
    public string TableComment { get; set; }
    /// <summary>
    /// 列名
    /// </summary>
    public string ColumnName { get; set; }
    /// <summary>
    /// 数据类型及长度
    /// </summary>
    public string ColumnType { get; set; }
    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; }
    /// <summary>
    /// 数据长度
    /// </summary>
    public string DataLength { get; set; }
    /// <summary>
    /// 数据精度
    /// </summary>
    public string DataScale { get; set; }
    /// <summary>
    /// 排序
    /// </summary>
    public int ColumnOrder { get; set; }
    /// <summary>
    /// 主键（大于等于1）
    /// </summary>
    public int PrimaryKey { get; set; }
    /// <summary>
    /// 自增（1：是）
    /// </summary>
    public int AutoIncr { get; set; }
    /// <summary>
    /// 可为空（1：空；0：非空）
    /// </summary>
    public int IsNullable { get; set; }
    /// <summary>
    /// 默认值
    /// </summary>
    public string ColumnDefault { get; set; }
    /// <summary>
    /// 列注释
    /// </summary>
    public string ColumnComment { get; set; }
}

/// <summary>
/// 数据库信息
/// </summary>
public partial class DataKitDatabaseResult
{
    /// <summary>
    /// 库名
    /// </summary>
    public string DatabaseName { get; set; }
    /// <summary>
    /// 所属者
    /// </summary>
    public string DatabaseOwner { get; set; }
    /// <summary>
    /// 表空间
    /// </summary>
    public string DatabaseSpace { get; set; }
    /// <summary>
    /// 字符集
    /// </summary>
    public string DatabaseCharset { get; set; }
    /// <summary>
    /// 排序规则
    /// </summary>
    public string DatabaseCollation { get; set; }
    /// <summary>
    /// 数据路径
    /// </summary>
    public string DatabasePath { get; set; }
    /// <summary>
    /// 日志路径
    /// </summary>
    public string DatabaseLogPath { get; set; }
    /// <summary>
    /// 数据大小
    /// </summary>
    public long DatabaseDataLength { get; set; }
    /// <summary>
    /// 日志大小
    /// </summary>
    public long DatabaseLogLength { get; set; }
    /// <summary>
    /// 索引大小
    /// </summary>
    public long DatabaseIndexLength { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? DatabaseCreateTime { get; set; }
}

/// <summary>
/// 表信息
/// </summary>
public partial class DataKitTablResult
{
    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; }
    /// <summary>
    /// 模式名
    /// </summary>
    public string SchemaName { get; set; }
    /// <summary>
    /// 所属
    /// </summary>
    public string TableOwner { get; set; }
    /// <summary>
    /// 空间
    /// </summary>
    public string TableSpace { get; set; }
    /// <summary>
    /// 表类型（表、视图）
    /// </summary>
    public string TableType { get; set; }
    /// <summary>
    /// 引擎
    /// </summary>
    public string TableEngine { get; set; }
    /// <summary>
    /// 总行数
    /// </summary>
    public long TableRows { get; set; }
    /// <summary>
    /// 数据大小
    /// </summary>
    public long TableDataLength { get; set; }
    /// <summary>
    /// 索引大小
    /// </summary>
    public long TableIndexLength { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? TableCreateTime { get; set; }
    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime? TableModifyTime { get; set; }
    /// <summary>
    /// 排序规则
    /// </summary>
    public string TableCollation { get; set; }
    /// <summary>
    /// 表注释
    /// </summary>
    public string TableComment { get; set; }
}

/// <summary>
/// 传递参数
/// </summary>
public partial class DataKitTransfer
{
    /// <summary>
    /// 读写项
    /// </summary>
    public class ReadWriteItem
    {
        /// <summary>
        /// 读取表数据SQL（可带模式名 SchemaName）
        /// </summary>
        public string ReadDataSQL { get; set; }
        /// <summary>
        /// 读取表名（可带模式名 SchemaName，用于生成列映射）
        /// </summary>
        public string ReadTableName { get; set; }
        /// <summary>
        /// 写入表名（可带模式名 SchemaName）
        /// </summary>
        public string WriteTableName { get; set; }
        /// <summary>
        /// 清空写入表SQL（可带模式名 SchemaName）
        /// </summary>
        public string WriteDeleteSQL { get; set; }
        /// <summary>
        /// 读取列 映射 写入列
        /// </summary>
        public Dictionary<string, string> ReadWriteColumnMap { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// 迁移
    /// </summary>
    public class MigrateBase
    {
        /// <summary>
        /// 读取连接信息
        /// </summary>
        public DbKitConnectionOption ReadConnectionInfo { get; set; }
        /// <summary>
        /// 读取连接信息（引用）
        /// </summary>
        public string RefReadConnectionInfo { get; set; }
        /// <summary>
        /// 读取数据库名，回填 ReadConnectionInfo
        /// </summary>
        public string ReadDatabaseName { get; set; }
        /// <summary>
        /// 写入连接信息
        /// </summary>
        public DbKitConnectionOption WriteConnectionInfo { get; set; }
        /// <summary>
        /// 写入连接信息（引用）
        /// </summary>
        public string RefWriteConnectionInfo { get; set; }
        /// <summary>
        /// 写入数据库名，回填 WriteConnectionInfo
        /// </summary>
        public string WriteDatabaseName { get; set; }
    }

    /// <summary>
    /// 迁移数据表
    /// </summary>
    public class MigrateDataTable : MigrateBase
    {
        /// <summary>
        /// 读写表集合
        /// </summary>
        public List<ReadWriteItem> ListReadWrite { get; set; } = new List<ReadWriteItem>();
    }

    /// <summary>
    /// 迁移数据库
    /// </summary>
    public class MigrateDatabase : MigrateBase
    {
        /// <summary>
        /// 写入前删除表数据
        /// </summary>
        public bool WriteDeleteData { get; set; }
        /// <summary>
        /// 转换为
        /// </summary>
        /// <param name="isSimilar">相似匹配，默认是</param>
        /// <returns></returns>
        public async Task<MigrateDataTable> AsMigrateDataTable(bool isSimilar = true)
        {
            var mdb = this;
            var mdt = new MigrateDataTable().ToDeepCopy(mdb);

            var readTables = await DataKitTo.CreateDkInstance(mdt.ReadConnectionInfo).GetTable();
            var writeTables = await DataKitTo.CreateDkInstance(mdt.WriteConnectionInfo).GetTable();

            if (readTables?.Count > 0 && writeTables?.Count > 0)
            {
                readTables.ForEach(readTable =>
                {
                    //读取库的表名 在 写入库
                    List<DataKitTablResult> listWriteTable = null;

                    //相似匹配
                    if (isSimilar)
                    {
                        listWriteTable = writeTables.Where(x => DataKitTo.SimilarMatch(x.TableName, readTable.TableName)).ToList();
                    }
                    else
                    {
                        listWriteTable = writeTables.Where(x => readTable.TableName == x.TableName).ToList();
                    }
                    if (listWriteTable.Count > 0)
                    {
                        //尝试匹配模式名 或 取第一条
                        var writeTable = listWriteTable.FirstOrDefault(x => x.SchemaName == readTable.SchemaName);
                        writeTable ??= listWriteTable.First();

                        var readSNTN = DbKitExtensions.SqlSNTN(readTable.TableName, readTable.SchemaName, mdt.ReadConnectionInfo.ConnectionType);
                        var writeSNTN = DbKitExtensions.SqlSNTN(writeTable.TableName, writeTable.SchemaName, mdt.WriteConnectionInfo.ConnectionType);

                        var clearTableSql = $"{(mdt.WriteConnectionInfo.ConnectionType == EnumTo.TypeDB.SQLite ? "DELETE FROM" : "TRUNCATE TABLE")} {writeSNTN}";

                        mdt.ListReadWrite.Add(new ReadWriteItem
                        {
                            ReadDataSQL = $"SELECT * FROM {readSNTN}",
                            WriteTableName = DbKitExtensions.SqlSNTN(writeTable.TableName, writeTable.SchemaName),
                            WriteDeleteSQL = mdb.WriteDeleteData ? clearTableSql : null
                        });
                    }
                });
            }

            return mdt;
        }
    }

    /// <summary>
    /// 导出
    /// </summary>
    public class ExportBase
    {
        /// <summary>
        /// 连接信息
        /// </summary>
        public DbKitConnectionOption ReadConnectionInfo { get; set; }
        /// <summary>
        /// 读取连接信息（引用）
        /// </summary>
        public string RefReadConnectionInfo { get; set; }
        /// <summary>
        /// 读取数据库名，回填 ReadConnectionInfo
        /// </summary>
        public string ReadDatabaseName { get; set; }
        /// <summary>
        /// 导出类型（仅数据：dataOnly；结构及数据：all）
        /// </summary>
        public string ExportType { get; set; } = "all";
        /// <summary>
        /// 导出包完整路径
        /// </summary>
        public string PackagePath { get; set; }
    }

    /// <summary>
    /// 导出表
    /// </summary>
    public class ExportDataTable : ExportBase
    {
        /// <summary>
        /// 读取数据表（可带模式名 SchemaName）
        /// </summary>
        public List<string> ListReadDataSQL { get; set; } = new List<string>();
    }

    /// <summary>
    /// 导出库
    /// </summary>
    public class ExportDatabase : ExportBase
    {
        /// <summary>
        /// 读取表名（可带模式名 SchemaName）
        /// </summary>
        public List<string> ListReadTableName { get; set; } = new List<string>();
    }

    /// <summary>
    /// 导入数据库
    /// </summary>
    public class ImportDatabase
    {
        /// <summary>
        /// 连接信息
        /// </summary>
        public DbKitConnectionOption WriteConnectionInfo { get; set; }
        /// <summary>
        /// 写入连接信息（引用）
        /// </summary>
        public string RefWriteConnectionInfo { get; set; }
        /// <summary>
        /// 写入数据库名，回填 WriteConnectionInfo
        /// </summary>
        public string WriteDatabaseName { get; set; }
        /// <summary>
        /// 导入包完整路径
        /// </summary>
        public string PackagePath { get; set; }
        /// <summary>
        /// 写入前删除表数据
        /// </summary>
        public bool WriteDeleteData { get; set; }
        /// <summary>
        /// 读取表 映射 写入表（可带模式名 SchemaName）
        /// </summary>
        public Dictionary<string, string> ReadWriteTableMap { get; set; } = new Dictionary<string, string>();
    }
}

#endif