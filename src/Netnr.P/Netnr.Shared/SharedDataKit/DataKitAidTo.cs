#if Full || DataKit

using System.Data;
using System.Data.Common;
using System.Collections.Specialized;
using MySqlConnector;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Npgsql;
using Netnr.Core;
using Netnr.SharedAdo;
using System.IO.Compression;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// DataKit 辅助
    /// </summary>
    public partial class DataKitAidTo
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="typeDB"></param>
        /// <returns></returns>
        public static SharedEnum.TypeDB GetTypeDB(string typeDB)
        {
            Enum.TryParse(typeDB, true, out SharedEnum.TypeDB tdb);
            return tdb;
        }

        /// <summary>
        /// SQL引用符号
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="KeyWord">关键字</param>
        /// <returns></returns>
        public static string SqlQuote(SharedEnum.TypeDB? tdb, string KeyWord)
        {
            return tdb switch
            {
                SharedEnum.TypeDB.SQLite or SharedEnum.TypeDB.SQLServer => $"[{KeyWord}]",
                SharedEnum.TypeDB.MySQL => $"`{KeyWord}`",
                SharedEnum.TypeDB.Oracle or SharedEnum.TypeDB.PostgreSQL => $"\"{KeyWord}\"",
                _ => KeyWord,
            };
        }

        /// <summary>
        /// 构建数据库连接
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static DbConnection SqlConn(SharedEnum.TypeDB tdb, string conn)
        {
            return tdb switch
            {
                SharedEnum.TypeDB.SQLite => new SqliteConnection(conn),
                SharedEnum.TypeDB.MySQL => new MySqlConnection(conn),
                SharedEnum.TypeDB.Oracle => new OracleConnection(conn),
                SharedEnum.TypeDB.SQLServer => new SqlConnection(conn),
                SharedEnum.TypeDB.PostgreSQL => new NpgsqlConnection(conn),
                _ => null,
            };
        }

        /// <summary>
        /// 数据库导出
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="zipPath">导出源目录</param>
        /// <param name="tables">指定表</param>
        /// <param name="le">日志事件</param>
        /// <returns></returns>
        public static SharedResultVM DatabaseExport(SharedEnum.TypeDB tdb, string conn, string zipPath, List<string> tables = null, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            var vm = new SharedResultVM();
            vm.LogEvent(le);

            vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 数据库导出");

            if (tables == null || tables.Count == 0)
            {
                tables = (DataKitTo.GetTable(tdb, conn).Data as List<TableVM>).Select(x => x.TableName).ToList();
            }
            vm.Log.Add($"导出表（{tables.Count}）：{string.Join(",", tables)}");

            //数据库
            var db = new DbHelper(SqlConn(tdb, conn));

            var partMaxRow = 10000;
            var partMaxSize = 1024 * 1024 * 25;
            var tmpFolder = Path.Combine(Path.GetDirectoryName(zipPath), Path.GetFileNameWithoutExtension(zipPath));
            if (!Directory.Exists(tmpFolder))
            {
                Directory.CreateDirectory(tmpFolder);
            }

            for (int i = 0; i < tables.Count; i++)
            {
                var table = tables[i];

                var sql = $"SELECT * FROM {SqlQuote(tdb, table)}";
                vm.Log.Add($"查询表 {table}，执行脚本：{sql}");

                var fi = 1;

                db.SafeConn(() =>
                {
                    var cmd = db.Connection.CreateCommand();
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = sql;

                    var reader = cmd.ExecuteReader();

                    var dt = new DataTable
                    {
                        TableName = table
                    };

                    var dtSchema = reader.GetSchemaTable();
                    foreach (DataRow dr in dtSchema.Rows)
                    {
                        dt.Columns.Add(new DataColumn(dr["ColumnName"].ToString(), (Type)(dr["DataType"]))
                        {
                            Unique = (bool)dr["IsUnique"],
                            AllowDBNull = (bool)dr["AllowDBNull"],
                            AutoIncrement = (bool)dr["IsAutoIncrement"]
                        });
                    }

                    while (reader.Read())
                    {
                        var dr = dt.NewRow();
                        for (int f = 0; f < reader.FieldCount; f++)
                        {
                            dr[f] = reader[f];
                        }
                        dt.Rows.Add(dr.ItemArray);

                        if (dt.Rows.Count >= partMaxRow || (dt.Rows.Count % 100 == 0 && GC.GetTotalMemory(true) > partMaxSize))
                        {
                            var outXml = Path.Combine(tmpFolder, $"{table}_{fi.ToString().PadLeft(7, '0')}.xml");
                            dt.WriteXml(outXml, XmlWriteMode.WriteSchema);
                            fi++;

                            vm.Log.Add($"写入分片：{outXml}，耗时：{vm.PartTimeFormat()}");

                            dt.Clear();
                        }
                    }

                    if (fi == 1 || dt.Rows.Count > 0)
                    {
                        var outXml = Path.Combine(tmpFolder, $"{table}_{fi.ToString().PadLeft(7, '0')}.xml");
                        dt.WriteXml(outXml, XmlWriteMode.WriteSchema);
                    }

                    vm.Log.Add($"导出表 {table} 完成，进度：{i + 1}/{tables.Count}\n");
                });
            }

            vm.Log.Add($"导出完成，共耗时：{vm.UseTimeFormat}");

            vm.Log.Add($"开始打包：{zipPath}");
            ZipTo.Create(tmpFolder);
            vm.Log.Add($"打包完成，耗时：{vm.PartTimeFormat()}，清理临时目录：{tmpFolder}");
            Directory.Delete(tmpFolder, true);

            vm.Set(SharedEnum.RTag.success);

            return vm;
        }

        /// <summary>
        /// 数据库导入
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="zipPath">导入源 ZIP 压缩包</param>
        /// <param name="clearTable">导入前清空表，默认否</param>
        /// <param name="le">日志事件</param>
        /// <returns></returns>
        public static SharedResultVM DatabaseImport(SharedEnum.TypeDB tdb, string conn, string zipPath, bool clearTable = false, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            var vm = new SharedResultVM();
            vm.LogEvent(le);

            vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 数据库导入");

            var hsName = new HashSet<string>();

            var db = new DbHelper(SqlConn(tdb, conn));

            vm.Log.Add($"开始读取 {zipPath}");

            using var zipRead = ZipFile.OpenRead(zipPath);
            var zipList = zipRead.Entries.ToList();

            for (int i = 0; i < zipList.Count; i++)
            {
                var dt = new DataTable();

                var item = zipList[i];
                dt.ReadXml(item.Open());

                //清空表
                if (hsName.Add(dt.TableName) && clearTable)
                {
                    var clearSql = (tdb == SharedEnum.TypeDB.SQLite ? "DELETE FROM " : "TRUNCATE TABLE ") + SqlQuote(tdb, dt.TableName);
                    vm.Log.Add($"清空表（{dt.TableName}）数据，执行脚本：{clearSql}");
                    db.SqlExecute(clearSql);
                    vm.Log.Add($"已清空表（{dt.TableName}）数据，耗时：{vm.PartTime()}\n");
                }

                vm.Log.Add($"导入分片：{item.Name}，大小：{ParsingTo.FormatByteSize(item.Length)}，共 {dt.Rows.Count} 行，表：{dt.TableName}");

                switch (tdb)
                {
                    case SharedEnum.TypeDB.SQLite:
                        db.BulkBatchSQLite(dt, dt.TableName);
                        break;
                    case SharedEnum.TypeDB.MySQL:
                        db.BulkCopyMySQL(dt, dt.TableName);
                        break;
                    case SharedEnum.TypeDB.Oracle:
                        db.BulkCopyOracle(dt, dt.TableName);
                        break;
                    case SharedEnum.TypeDB.SQLServer:
                        db.BulkCopySQLServer(dt, dt.TableName);
                        break;
                    case SharedEnum.TypeDB.PostgreSQL:
                        db.BulkCopyPostgreSQL(dt, dt.TableName);
                        break;
                }

                vm.Log.Add($"导入分片成功，耗时：{vm.PartTimeFormat()}，进度：{i + 1}/{zipList.Count}\n");
            }

            vm.Log.Add($"共耗时：{vm.UseTimeFormat}");

            vm.Set(SharedEnum.RTag.success);

            return vm;
        }

        /// <summary>
        /// 数据库表映射C#
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="table"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<SqlMappingCsharpVM> SqlMappingCsharp(SharedEnum.TypeDB tdb, string table, string conn)
        {
            var vms = new List<SqlMappingCsharpVM>();

            switch (tdb)
            {
                case SharedEnum.TypeDB.MySQL:
                    {
                        var cb = new MySqlCommandBuilder();
                        var dbConn = new MySqlConnection(conn);
                        //引用表名
                        var quoteTable = $"{cb.QuotePrefix}{table}{cb.QuoteSuffix}";
                        cb.DataAdapter = new MySqlDataAdapter
                        {
                            SelectCommand = new MySqlCommand($"select * from {quoteTable} where 0=1", dbConn)
                        };

                        //参数化
                        var pars = cb.GetInsertCommand(true).Parameters;

                        //空表
                        if (dbConn.State != ConnectionState.Open)
                        {
                            dbConn.Open();
                        }
                        var dt = new DataTable();
                        cb.DataAdapter.Fill(dt);
                        dbConn.Close();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            var vm = new SqlMappingCsharpVM()
                            {
                                ColumnName = dc.ColumnName,
                                DataTypeName = dc.DataType.FullName,
                                MaxLength = dc.MaxLength,
                                AllowDBNull = dc.AllowDBNull,
                                Ordinal = dc.Ordinal
                            };

                            foreach (MySqlParameter par in pars)
                            {
                                if (par.SourceColumn == dc.ColumnName)
                                {
                                    vm.DbType = par.MySqlDbType;
                                    break;
                                }
                            }

                            vms.Add(vm);
                        }
                    }
                    break;
                case SharedEnum.TypeDB.Oracle:
                    {
                        var cb = new OracleCommandBuilder();
                        var dbConn = new OracleConnection(conn);
                        //引用表名
                        var quoteTable = $"{cb.QuotePrefix}{table}{cb.QuoteSuffix}";
                        cb.DataAdapter = new OracleDataAdapter
                        {
                            SelectCommand = new OracleCommand($"select * from {quoteTable} where 0=1", dbConn)
                        };

                        //参数化
                        var pars = cb.GetInsertCommand(true).Parameters;

                        //空表
                        if (dbConn.State != ConnectionState.Open)
                        {
                            dbConn.Open();
                        }
                        var dt = new DataTable();
                        cb.DataAdapter.Fill(dt);
                        dbConn.Close();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            var vm = new SqlMappingCsharpVM()
                            {
                                ColumnName = dc.ColumnName,
                                DataTypeName = dc.DataType.FullName,
                                MaxLength = dc.MaxLength,
                                AllowDBNull = dc.AllowDBNull,
                                Ordinal = dc.Ordinal
                            };

                            foreach (OracleParameter par in pars)
                            {
                                if (par.SourceColumn == dc.ColumnName)
                                {
                                    vm.DbType = par.OracleDbType;
                                    break;
                                }
                            }

                            vms.Add(vm);
                        }
                    }
                    break;
                case SharedEnum.TypeDB.SQLServer:
                    {
                        var cb = new SqlCommandBuilder();
                        var dbConn = new SqlConnection(conn);
                        //引用表名
                        var quoteTable = $"{cb.QuotePrefix}{table}{cb.QuoteSuffix}";
                        cb.DataAdapter = new SqlDataAdapter
                        {
                            SelectCommand = new SqlCommand($"select * from {quoteTable} where 0=1", dbConn)
                        };

                        //参数化
                        var pars = cb.GetInsertCommand(true).Parameters;

                        //空表
                        if (dbConn.State != ConnectionState.Open)
                        {
                            dbConn.Open();
                        }
                        var dt = new DataTable();
                        cb.DataAdapter.Fill(dt);
                        dbConn.Close();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            var vm = new SqlMappingCsharpVM()
                            {
                                ColumnName = dc.ColumnName,
                                DataTypeName = dc.DataType.FullName,
                                MaxLength = dc.MaxLength,
                                AllowDBNull = dc.AllowDBNull,
                                Ordinal = dc.Ordinal
                            };

                            foreach (SqlParameter par in pars)
                            {
                                if (par.SourceColumn == dc.ColumnName)
                                {
                                    vm.DbType = par.SqlDbType;
                                    break;
                                }
                            }

                            vms.Add(vm);
                        }
                    }
                    break;
                case SharedEnum.TypeDB.PostgreSQL:
                    {
                        var cb = new NpgsqlCommandBuilder();
                        var dbConn = new NpgsqlConnection(conn);
                        //引用表名
                        var quoteTable = $"{cb.QuotePrefix}{table}{cb.QuoteSuffix}";
                        cb.DataAdapter = new NpgsqlDataAdapter
                        {
                            SelectCommand = new NpgsqlCommand($"select * from {quoteTable} where 0=1", dbConn)
                        };

                        //参数化
                        var pars = cb.GetInsertCommand(true).Parameters;

                        //空表
                        if (dbConn.State != ConnectionState.Open)
                        {
                            dbConn.Open();
                        }
                        var dt = new DataTable();
                        cb.DataAdapter.Fill(dt);
                        dbConn.Close();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            var vm = new SqlMappingCsharpVM()
                            {
                                ColumnName = dc.ColumnName,
                                DataTypeName = dc.DataType.FullName,
                                MaxLength = dc.MaxLength,
                                AllowDBNull = dc.AllowDBNull,
                                Ordinal = dc.Ordinal
                            };

                            foreach (NpgsqlParameter par in pars)
                            {
                                if (par.SourceColumn == dc.ColumnName)
                                {
                                    vm.DbType = par.NpgsqlDbType;
                                    break;
                                }
                            }

                            vms.Add(vm);
                        }
                    }
                    break;
            }

            return vms;
        }
    }
}

#endif