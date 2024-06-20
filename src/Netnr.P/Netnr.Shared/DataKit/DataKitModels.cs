#if Full || DataKit

namespace Netnr;

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
public partial class DataKitTableResult
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
        public Dictionary<string, string> ReadWriteColumnMap { get; set; } = [];
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
        public List<ReadWriteItem> ListReadWrite { get; set; } = [];
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
        /// 忽略读取源表名（可带模式名 SchemaName）
        /// </summary>
        public List<string> ListIgnoreTableName { get; set; } = [];
        /// <summary>
        /// 指定读取源开始表名（可带模式名 SchemaName），针对失败时可跳过成功的表
        /// </summary>
        public string StartTableName { get; set; }

        /// <summary>
        /// 转换为
        /// </summary>
        /// <param name="isSimilar">相似匹配，默认是</param>
        /// <returns></returns>
        public async Task<MigrateDataTable> AsMigrateDataTable(bool isSimilar = true)
        {
            var mdb = this;
            var mdt = new MigrateDataTable().ToDeepCopy(mdb);

            var readTables = await DataKitTo.CreateDataKitInstance(mdt.ReadConnectionInfo).GetTable();
            var writeTables = await DataKitTo.CreateDataKitInstance(mdt.WriteConnectionInfo).GetTable();

            if (readTables?.Count > 0 && writeTables?.Count > 0)
            {
                //开始表名
                if (!string.IsNullOrWhiteSpace(StartTableName))
                {
                    for (int i = 0; i < readTables.Count; i++)
                    {
                        var readTable = readTables[i];
                        var readSNTN = DbKitExtensions.SqlSNTN(readTable.TableName, readTable.SchemaName, mdt.ReadConnectionInfo.ConnectionType);
                        if (DbKitExtensions.SqlEqualSNTN(StartTableName, readSNTN))
                        {
                            readTables = readTables.Skip(i).ToList();
                            break;
                        }
                    }
                }

                readTables.ForEach(readTable =>
                {
                    var readSNTN = DbKitExtensions.SqlSNTN(readTable.TableName, readTable.SchemaName, mdt.ReadConnectionInfo.ConnectionType);

                    //忽略表名
                    if (!mdb.ListIgnoreTableName.Any(x => DbKitExtensions.SqlEqualSNTN(x, readSNTN)))
                    {
                        //读取库的表名 在 写入库
                        List<DataKitTableResult> listWriteTable = null;

                        //相似匹配
                        if (isSimilar)
                        {
                            listWriteTable = writeTables.Where(x => DataKitTo.SimilarMatch(x.TableName, readTable.TableName)).ToList();
                        }
                        else
                        {
                            listWriteTable = writeTables.Where(x => x.TableName == readTable.TableName).ToList();
                        }
                        if (listWriteTable.Count > 0)
                        {
                            //模式名.表名
                            var writeTable = listWriteTable.FirstOrDefault(x => x.SchemaName == readTable.SchemaName && x.TableName == readTable.TableName);
                            //表名相同
                            writeTable ??= listWriteTable.FirstOrDefault(x => x.TableName == readTable.TableName);
                            //表名相似
                            writeTable ??= listWriteTable.First();

                            var writeSNTN = DbKitExtensions.SqlSNTN(writeTable.TableName, writeTable.SchemaName, mdt.WriteConnectionInfo.ConnectionType);

                            var clearTableSql = $"{(mdt.WriteConnectionInfo.ConnectionType == DBTypes.SQLite ? "DELETE FROM" : "TRUNCATE TABLE")} {writeSNTN}";

                            mdt.ListReadWrite.Add(new ReadWriteItem
                            {
                                ReadDataSQL = $"SELECT * FROM {readSNTN}",
                                WriteTableName = DbKitExtensions.SqlSNTN(writeTable.TableName, writeTable.SchemaName),
                                WriteDeleteSQL = mdb.WriteDeleteData ? clearTableSql : null
                            });
                        }
                    }
                });
            }

            return mdt;
        }
    }

    /// <summary>
    /// 同步
    /// </summary>
    public class SyncBase : MigrateBase
    {
        /// <summary>
        /// DDL 创建，默认auto不存在则创建，或cover始终重建覆盖
        /// </summary>
        public string DDLCreate { get; set; } = "auto";
        /// <summary>
        /// 默认转小写，或相同Same
        /// </summary>
        public string DDLLowerCase { get; set; } = "LowerCase";
        /// <summary>
        /// 允许列为 null，默认否，不推荐开启
        /// </summary>
        public bool AllowDBNull { get; set; }
        /// <summary>
        /// 表名 映射前缀
        /// </summary>
        public string TableNameMappingPrefix { get; set; }
        /// <summary>
        /// 表名 同步后缀
        /// </summary>
        public string TableNameSyncSuffix { get; set; } = "___sync___";
        /// <summary>
        /// 默认时间
        /// </summary>
        public DateTime DefaultDateTime { get; set; } = new DateTime(1970, 1, 1);
        /// <summary>
        /// 默认数值
        /// </summary>
        public int DefaultNumeric { get; set; } = 0;
        /// <summary>
        /// 默认字符
        /// </summary>
        public string DefaultString { get; set; } = "";
    }

    /// <summary>
    /// 同步表
    /// </summary>
    public class SyncDataTable : SyncBase
    {
        /// <summary>
        /// 读取数据表（可带模式名 SchemaName）
        /// </summary>
        public List<string> ListReadDataSQL { get; set; } = [];
    }

    /// <summary>
    /// 同步库
    /// </summary>
    public class SyncDatabase : SyncBase
    {
        /// <summary>
        /// 忽略读取源表名（可带模式名 SchemaName）
        /// </summary>
        public List<string> ListIgnoreTableName { get; set; } = [];
        /// <summary>
        /// 指定读取源开始表名（可带模式名 SchemaName），针对失败时可跳过成功的表
        /// </summary>
        public string StartTableName { get; set; }
        /// <summary>
        /// 转换为
        /// </summary>
        /// <returns></returns>
        public async Task<SyncDataTable> AsSyncDataTable()
        {
            var sdb = this;
            var sdt = new SyncDataTable().ToDeepCopy(sdb);

            var readTables = await DataKitTo.CreateDataKitInstance(sdt.ReadConnectionInfo).GetTable();
            if (readTables?.Count > 0)
            {
                //开始表名
                if (!string.IsNullOrWhiteSpace(StartTableName))
                {
                    for (int i = 0; i < readTables.Count; i++)
                    {
                        var readTable = readTables[i];
                        var readSNTN = DbKitExtensions.SqlSNTN(readTable.TableName, readTable.SchemaName, sdt.ReadConnectionInfo.ConnectionType);
                        if (DbKitExtensions.SqlEqualSNTN(StartTableName, readSNTN))
                        {
                            readTables = readTables.Skip(i).ToList();
                            break;
                        }
                    }
                }

                sdt.ListReadDataSQL = [];
                readTables.ForEach(readTable =>
                {
                    var readSNTN = DbKitExtensions.SqlSNTN(readTable.TableName, readTable.SchemaName, sdt.ReadConnectionInfo.ConnectionType);

                    //忽略表名
                    if (!sdb.ListIgnoreTableName.Any(x => DbKitExtensions.SqlEqualSNTN(x, readSNTN)))
                    {
                        sdt.ListReadDataSQL.Add($"SELECT * FROM {readSNTN}");
                    }
                });
            }

            return sdt;
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
        /// 导出包完整路径（自动创建目录）
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
        public List<string> ListReadDataSQL { get; set; } = [];
    }

    /// <summary>
    /// 导出库
    /// </summary>
    public class ExportDatabase : ExportBase
    {
        /// <summary>
        /// 读取表名（可带模式名 SchemaName）
        /// </summary>
        public List<string> ListReadTableName { get; set; } = [];
        /// <summary>
        /// 忽略表名（可带模式名 SchemaName）
        /// </summary>
        public List<string> ListIgnoreTableName { get; set; } = [];

        /// <summary>
        /// 转换，导出数据库对象转换为导出数据表对象
        /// </summary>
        /// <returns></returns>
        public async Task<ExportDataTable> AsExportDataTable()
        {
            var edb = this;

            // 未指定表名，则获取所有表名
            if (edb.ListReadTableName.Count == 0)
            {
                var dataKit = DataKitTo.CreateDataKitInstance(edb.ReadConnectionInfo);
                var listTable = await dataKit.GetTable();
                edb.ListReadTableName = listTable.Select(x => DbKitExtensions.SqlSNTN(x.TableName, x.SchemaName, edb.ReadConnectionInfo.ConnectionType)).ToList();
            }

            var edt = new ExportDataTable().ToDeepCopy(edb);

            // 构建读取数据的 SQL
            edt.ListReadDataSQL = [];
            foreach (var table in edb.ListReadTableName)
            {
                //忽略表名
                if (edb.ListIgnoreTableName.Any(x => DbKitExtensions.SqlEqualSNTN(x, table)))
                {
                    continue;
                }

                edt.ListReadDataSQL.Add($"SELECT * FROM {table}");
            }

            return edt;
        }
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
        public Dictionary<string, string> ReadWriteTableMap { get; set; } = [];
    }

}

#endif