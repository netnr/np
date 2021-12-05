#if Full || DataKit

using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.IO.Compression;
using System.Collections.Generic;
using System.Collections.Specialized;
using MySqlConnector;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Npgsql;
using Netnr.Core;
using Netnr.SharedAdo;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// DataKit 辅助
    /// </summary>
    public partial class DataKitAidTo
    {
        /// <summary>
        /// 构建数据库连接
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static DbConnection DbConn(SharedEnum.TypeDB tdb, string conn)
        {
            return tdb switch
            {
                SharedEnum.TypeDB.SQLite => new SqliteConnection(conn),
                SharedEnum.TypeDB.MySQL or SharedEnum.TypeDB.MariaDB => new MySqlConnection(conn),
                SharedEnum.TypeDB.Oracle => new OracleConnection(conn),
                SharedEnum.TypeDB.SQLServer => new SqlConnection(conn),
                SharedEnum.TypeDB.PostgreSQL => new NpgsqlConnection(conn),
                _ => null,
            };
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="zipPath">导入源 ZIP 压缩包</param>
        /// <param name="clearTable">导入前清空表，默认否</param>
        /// <param name="le">日志事件</param>
        /// <returns></returns>
        public static SharedResultVM ImportDatabase(SharedEnum.TypeDB tdb, string conn, string zipPath, bool clearTable = false, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            var vm = new SharedResultVM();
            vm.LogEvent(le);

            vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 导入数据");

            var hsName = new HashSet<string>();

            var db = new DbHelper(DbConn(tdb, conn));

            vm.Log.Add($"开始读取 {zipPath}");

            using var zipRead = ZipFile.OpenRead(zipPath);
            var zipList = zipRead.Entries.OrderBy(x => x.Name).ToList();

            for (int i = 0; i < zipList.Count; i++)
            {
                var dt = new DataTable();

                var item = zipList[i];
                dt.ReadXml(item.Open());

                //清空表
                if (hsName.Add(dt.TableName) && clearTable)
                {
                    var clearSql = (tdb == SharedEnum.TypeDB.SQLite ? "DELETE FROM " : "TRUNCATE TABLE ") + DbHelper.SqlQuote(tdb, dt.TableName);
                    vm.Log.Add($"清空表（{dt.TableName}）数据，执行脚本：{clearSql}");
                    db.SqlExecuteNonQuery(clearSql);
                    vm.Log.Add($"已清空表（{dt.TableName}）数据，耗时：{vm.PartTime()}\n");
                }

                vm.Log.Add($"导入分片：{item.Name}，大小：{ParsingTo.FormatByteSize(item.Length)}，共 {dt.Rows.Count} 行，表：{dt.TableName}");

                switch (tdb)
                {
                    case SharedEnum.TypeDB.SQLite:
                        db.BulkBatchSQLite(dt, dt.TableName);
                        break;
                    case SharedEnum.TypeDB.MySQL:
                    case SharedEnum.TypeDB.MariaDB:
                        db.BulkCopyMySQL(dt, dt.TableName);
                        break;
                    case SharedEnum.TypeDB.Oracle:
                        db.BulkCopyOracle(dt, dt.TableName);
                        break;
                    case SharedEnum.TypeDB.SQLServer:
                        db.BulkCopySQLServer(dt, dt.TableName);
                        break;
                    case SharedEnum.TypeDB.PostgreSQL:
                        {
                            //自增列（使用SQL并指定自增列值写入）
                            var autoIncrCol = dt.Columns.Cast<DataColumn>().Where(x => x.AutoIncrement == true).ToList();
                            if (autoIncrCol.Count > 0)
                            {
                                db.BulkBatchPostgreSQL(dt, dt.TableName, dataAdapter =>
                                {
                                    var sqlquote = "\"";
                                    var ct = dataAdapter.InsertCommand.CommandText;
                                    autoIncrCol = autoIncrCol.Where(x => !ct.Contains($"{sqlquote + x.ColumnName + sqlquote}")).ToList();
                                    if (autoIncrCol.Count > 0)
                                    {
                                        var fields = string.Empty;
                                        var values = string.Empty;

                                        for (int i = 0; i < autoIncrCol.Count; i++)
                                        {
                                            var col = autoIncrCol[i];
                                            fields += $"{sqlquote + col.ColumnName + sqlquote}, ";
                                            values += $"@{col.ColumnName}, ";

                                            //新增参数
                                            var parameter = new NpgsqlParameter(col.ColumnName, col.DataType)
                                            {
                                                SourceColumn = col.ColumnName
                                            };
                                            dataAdapter.InsertCommand.Parameters.Add(parameter);
                                        }

                                        ct = ct.Replace("(\"", $"({fields}\"").Replace("(@", $"({values}@");
                                        dataAdapter.InsertCommand.CommandText = ct;
                                    }
                                });
                            }
                            else
                            {
                                db.BulkCopyPostgreSQL(dt, dt.TableName);
                            }
                        }
                        break;
                }

                vm.Log.Add($"导入分片成功，耗时：{vm.PartTimeFormat()}，进度：{i + 1}/{zipList.Count}\n");
            }

            vm.Log.Add($"共耗时：{vm.UseTimeFormat}");

            vm.Set(SharedEnum.RTag.success);

            return vm;
        }

        /// <summary>
        /// 导出数据库
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="zipPath">导出源目录</param>
        /// <param name="tables">指定表</param>
        /// <param name="le">日志事件</param>
        /// <returns></returns>
        public static SharedResultVM ExportDatabase(SharedEnum.TypeDB tdb, string conn, string zipPath, List<string> tables = null, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            if (tables == null || tables.Count == 0)
            {
                tables = (DataKitTo.GetTable(tdb, conn).Data as List<TableVM>).Select(x => x.TableName).ToList();
            }

            var sqls = new List<string>();
            foreach (var table in tables)
            {
                var sql = $"SELECT * FROM {DbHelper.SqlQuote(tdb, table)}";
                sqls.Add(sql);
            }

            return ExportDataTable(tdb, conn, zipPath, sqls, le);
        }

        /// <summary>
        /// 导出表
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="zipPath">导出源目录</param>
        /// <param name="sqls">查询表</param>
        /// <param name="le">日志事件</param>
        /// <returns></returns>
        public static SharedResultVM ExportDataTable(SharedEnum.TypeDB tdb, string conn, string zipPath, List<string> sqls, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            var vm = new SharedResultVM();
            vm.LogEvent(le);

            vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 导出数据");

            //数据库
            var db = new DbHelper(DbConn(tdb, conn));

            var partMaxRow = 50000; //分片最大数据行
            var gcMaxSize = GC.GetTotalMemory(true) + 1024 * 1024 * 500; //内存占用限制

            var tmpFolder = Path.Combine(Path.GetDirectoryName(zipPath), Path.GetFileNameWithoutExtension(zipPath));
            if (!Directory.Exists(tmpFolder))
            {
                Directory.CreateDirectory(tmpFolder);
            }

            for (int i = 0; i < sqls.Count; i++)
            {
                var sql = sqls[i];

                vm.Log.Add($"查询表，执行脚本：{sql}");

                var fi = 1;

                db.SafeConn(() =>
                {
                    var cmd = db.Connection.CreateCommand();
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = sql;

                    var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                    var dtSchema = reader.GetSchemaTable();
                    var table = dtSchema.Rows[0]["BaseTableName"].ToString();

                    var dt = new DataTable
                    {
                        TableName = table
                    };
                    var rowCount = 0;

                    foreach (DataRow dr in dtSchema.Rows)
                    {
                        dt.Columns.Add(new DataColumn(dr["ColumnName"].ToString(), (Type)(dr["DataType"]))
                        {
                            Unique = (bool)dr["IsUnique"],
                            AllowDBNull = dr["AllowDBNull"] == DBNull.Value || (bool)dr["AllowDBNull"],
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

                        if (dt.Rows.Count >= partMaxRow || (dt.Rows.Count % 100 == 0 && GC.GetTotalMemory(true) > gcMaxSize))
                        {
                            rowCount += dt.Rows.Count;

                            var outXml = Path.Combine(tmpFolder, $"{table}_{fi.ToString().PadLeft(7, '0')}.xml");
                            dt.WriteXml(outXml, XmlWriteMode.WriteSchema);
                            fi++;

                            vm.Log.Add($"写入分片：{outXml}，共 {dt.Rows.Count} 行，耗时：{vm.PartTimeFormat()}");

                            dt.Clear();
                        }
                    }

                    if (fi == 1 || dt.Rows.Count > 0)
                    {
                        rowCount += dt.Rows.Count;

                        var outXml = Path.Combine(tmpFolder, $"{table}_{fi.ToString().PadLeft(7, '0')}.xml");
                        dt.WriteXml(outXml, XmlWriteMode.WriteSchema);

                        vm.Log.Add($"写入分片：{outXml}，共 {dt.Rows.Count} 行，耗时：{vm.PartTimeFormat()}");
                    }

                    vm.Log.Add($"导出表 {table} 完成，共 {rowCount} 行，进度：{i + 1}/{sqls.Count}\n");
                });
            }

            vm.Log.Add($"导出完成，共耗时：{vm.TotalTimeFormat()}");

            vm.Log.Add($"开始打包：{zipPath}");
            ZipTo.Create(tmpFolder);
            vm.Log.Add($"打包完成，耗时：{vm.PartTimeFormat()}，清理临时目录：{tmpFolder}");
            Directory.Delete(tmpFolder, true);

            vm.Set(SharedEnum.RTag.success);

            return vm;
        }
    }
}

#endif