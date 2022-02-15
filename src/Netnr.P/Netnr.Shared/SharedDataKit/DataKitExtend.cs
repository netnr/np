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
    /// DataKit 拓展
    /// </summary>
    public partial class DataKit
    {
        /// <summary>
        /// 执行结果统一输出
        /// </summary>
        /// <param name="er"></param>
        /// <param name="listInfo"></param>
        /// <param name="st"></param>
        /// <returns></returns>
        public static Tuple<DataSet, DataSet, object> ExecuteUnity(Tuple<DataSet, int, DataSet> er, List<string> listInfo, SharedTimingVM st)
        {
            st.sw.Stop();
            listInfo.Add($"耗时: {st.PartTimeFormat()}");

            if (er.Item2 != -1)
            {
                listInfo.Insert(0, $"({er.Item2} 行受影响)");
            }

            var dtInfo = new DataTable();
            dtInfo.Columns.Add(new DataColumn("message"));
            listInfo.ForEach(info =>
            {
                var drInfo = dtInfo.NewRow();
                drInfo[0] = info;
                dtInfo.Rows.Add(drInfo.ItemArray);
            });

            return new Tuple<DataSet, DataSet, object>(er.Item1, er.Item3, new { info = dtInfo });
        }

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

        //分批最大行数
        public const int BatchMaxRows = 10000;

        /// <summary>
        /// 导出数据库
        /// </summary>
        /// <param name="edb"></param>
        /// <param name="le">日志事件</param>
        /// <returns></returns>
        public static SharedResultVM ExportDatabase(TransferVM.ExportDatabase edb, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            if (edb.ListReadTableName.Count == 0)
            {
                edb.ListReadTableName = (GetTable(edb.ReadConnectionInfo.ConnectionType, edb.ReadConnectionInfo.ConnectionString).Data as List<TableVM>).Select(x => x.TableName).ToList();
            }

            var edt = new TransferVM.ExportDataTable().ToRead(edb);
            edt.ListReadSQL = new List<string>();

            foreach (var table in edb.ListReadTableName)
            {
                var sql = $"SELECT * FROM {DbHelper.SqlQuote(edb.ReadConnectionInfo.ConnectionType, table)}";
                edt.ListReadSQL.Add(sql);
            }

            return ExportDataTable(edt, le);
        }

        /// <summary>
        /// 导出表
        /// </summary>
        /// <param name="edt"></param>
        /// <param name="le">日志事件</param>
        /// <returns></returns>
        public static SharedResultVM ExportDataTable(TransferVM.ExportDataTable edt, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            var vm = new SharedResultVM();
            vm.LogEvent(le);

            vm.Log.Add($"\n{DateTime.Now:yyyy-MM-dd HH:mm:ss} 导出数据\n");

            //数据库
            var db = edt.ReadConnectionInfo.NewDbHelper();

            //打包
            var zipFolder = Path.GetDirectoryName(edt.ZipPath);
            if (!Directory.Exists(zipFolder))
            {
                Directory.CreateDirectory(zipFolder);
            }
            var isOldZip = File.Exists(edt.ZipPath);
            using ZipArchive zip = ZipFile.Open(edt.ZipPath, isOldZip ? ZipArchiveMode.Update : ZipArchiveMode.Create);

            for (int i = 0; i < edt.ListReadSQL.Count; i++)
            {
                var sql = edt.ListReadSQL[i];

                vm.Log.Add($"读取表：{sql}");

                var rowCount = 0;
                var batchNo = 0;

                var dt = new DataTable();

                var sw = new Stopwatch();
                sw.Start();

                //读取行
                db.SqlExecuteDataRow(sql, dr =>
                {
                    rowCount++;

                    if (rowCount == 1)
                    {
                        dt = dr.Table.Clone();
                    }

                    dt.Rows.Add(dr.ItemArray);

                    if (sw.Elapsed.TotalMilliseconds > 5000)
                    {
                        vm.Log.Add($"当前读取行：{dt.Rows.Count}/{rowCount}");
                        sw.Restart();
                    }

                    if (dt.Rows.Count >= BatchMaxRows)
                    {
                        batchNo++;

                        var xmlName = $"{dt.TableName}_{batchNo.ToString().PadLeft(7, '0')}.xml";

                        //xml 写入 zip
                        var zae = isOldZip
                        ? zip.GetEntry(xmlName) ?? zip.CreateEntry(xmlName)
                        : zip.CreateEntry(xmlName);

                        var ceStream = zae.Open();
                        dt.WriteXml(ceStream, XmlWriteMode.WriteSchema);
                        ceStream.Close();

                        vm.Log.Add($"导出表（{dt.TableName}）第 {batchNo} 批（行：{dt.Rows.Count}/{rowCount}），耗时：{vm.PartTimeFormat()}");
                        sw.Restart();

                        dt.Clear();
                    }
                });

                if (batchNo == 0 && dt.Rows.Count == 0)
                {
                    vm.Log.Add($"跳过空表，导表进度：{i + 1}/{edt.ListReadSQL.Count}\n");
                }
                else
                {
                    //最后一批
                    if (dt.Rows.Count > 0)
                    {
                        batchNo++;

                        var xmlName = $"{dt.TableName}_{batchNo.ToString().PadLeft(7, '0')}.xml";

                        //xml 写入 zip
                        var zae = isOldZip
                        ? zip.GetEntry(xmlName) ?? zip.CreateEntry(xmlName)
                        : zip.CreateEntry(xmlName);

                        var ceStream = zae.Open();
                        dt.WriteXml(ceStream, XmlWriteMode.WriteSchema);
                        ceStream.Close();

                        vm.Log.Add($"导出表（{dt.TableName}）第 {batchNo} 批（行：{dt.Rows.Count}/{rowCount}），耗时：{vm.PartTimeFormat()}");
                    }

                    vm.Log.Add($"导出表（{dt.TableName}）完成（行：{rowCount}），导表进度：{i + 1}/{edt.ListReadSQL.Count}\n");
                }

                //清理包历史分片
                if (isOldZip)
                {
                    var hasOldData = false;
                    do
                    {
                        var xmlName = $"{dt.TableName}_{(++batchNo).ToString().PadLeft(7, '0')}.xml";
                        var zae = zip.GetEntry(xmlName);
                        if (hasOldData = (zae != null))
                        {
                            zae.Delete();
                        }
                    } while (hasOldData);
                }
            }

            vm.Log.Add($"导出完成：{edt.ZipPath}，共耗时：{vm.TotalTimeFormat()}\n");
            GC.Collect();

            vm.Set(SharedEnum.RTag.success);
            return vm;
        }

        /// <summary>
        /// 迁移数据表
        /// </summary>
        /// <param name="mdt"></param>
        /// <param name="le">实时日志</param>
        /// <returns></returns>
        public static SharedResultVM MigrateDataTable(TransferVM.MigrateDataTable mdt, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            var vm = new SharedResultVM();
            vm.LogEvent(le);

            vm.Log.Add($"\n{DateTime.Now:yyyy-MM-dd HH:mm:ss} 迁移表数据\n");

            //读取数据库
            var dbRead = mdt.ReadConnectionInfo.NewDbHelper();
            //写入数据库
            var dbWrite = mdt.WriteConnectionInfo.NewDbHelper();

            //遍历
            for (int i = 0; i < mdt.ListReadWrite.Count; i++)
            {
                var rw = mdt.ListReadWrite[i];

                vm.Log.Add($"读取表（{rw.WriteTableName}）结构");
                var dtWrite = dbWrite.SqlExecuteReader(DbHelper.SqlEmpty(rw.WriteTableName, tdb: mdt.WriteConnectionInfo.ConnectionType)).Item1.Tables[0];
                dtWrite.TableName = rw.WriteTableName;
                var dtWriteColumnName = dtWrite.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

                //读取表的列 => 写入表的列
                var rwMap = new Dictionary<string, string>();

                vm.Log.Add($"读取表数据：{rw.ReadSQL}");

                var rowCount = 0;
                var batchNo = 0;
                var catchCount = 0;

                //读取行
                dbRead.SqlExecuteDataRow(rw.ReadSQL, drRead =>
                {
                    rowCount++;

                    //构建列映射
                    if (rowCount == 1)
                    {
                        foreach (DataColumn dcRead in drRead.Table.Columns)
                        {
                            //指定映射
                            if (rw.ReadWriteColumnMap.ContainsKey(dcRead.ColumnName))
                            {
                                rwMap.Add(dcRead.ColumnName, rw.ReadWriteColumnMap[dcRead.ColumnName]);
                            }
                            else
                            {
                                //自动映射
                                var columnNameWrite = dtWriteColumnName.FirstOrDefault(x => x.ToLower() == dcRead.ColumnName.ToLower());
                                if (columnNameWrite != null)
                                {
                                    rwMap.Add(dcRead.ColumnName, columnNameWrite);
                                }
                            }
                        }
                    }

                    //构建一行 写入表
                    var drWriteNew = dtWrite.NewRow();
                    //根据读取表列映射写入表列填充单元格数据
                    foreach (var columnNameRead in rwMap.Keys)
                    {
                        //读取表列值
                        var valueRead = drRead[columnNameRead];
                        if (valueRead is not DBNull)
                        {
                            //读取表列值 转换类型为 写入表列值
                            try
                            {
                                //写入表列
                                var columnNameWrite = rwMap[columnNameRead];
                                //写入表列类型
                                var typeWrite = dtWrite.Columns[columnNameWrite].DataType;

                                //读取表列值 赋予 写入表列
                                drWriteNew[columnNameWrite] = Convert.ChangeType(valueRead, typeWrite);
                            }
                            catch (Exception ex)
                            {
                                catchCount++;
                                vm.Log.Add($"列值转换失败：");
                                vm.Log.Add(ex.ToJson());
                            }
                        }
                    }
                    //添加新行
                    dtWrite.Rows.Add(drWriteNew.ItemArray);

                    if (dtWrite.Rows.Count >= BatchMaxRows)
                    {
                        batchNo++;

                        if (batchNo == 1)
                        {
                            //第一批写入前清理表
                            if (!string.IsNullOrWhiteSpace(rw.WriteDeleteSQL))
                            {
                                vm.Log.Add($"清理写入表：{rw.WriteDeleteSQL}");
                                var num = dbWrite.SqlExecuteReader(rw.WriteDeleteSQL).Item2;
                                vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                            }
                        }

                        //分批写入
                        vm.Log.Add($"写入表（{rw.WriteTableName}）第 {batchNo} 批（行：{dtWrite.Rows.Count}/{rowCount}）");

                        switch (mdt.WriteConnectionInfo.ConnectionType)
                        {
                            case SharedEnum.TypeDB.SQLite:
                                dbWrite.BulkBatchSQLite(dtWrite);
                                break;
                            case SharedEnum.TypeDB.MySQL:
                            case SharedEnum.TypeDB.MariaDB:
                                dbWrite.BulkCopyMySQL(dtWrite);
                                break;
                            case SharedEnum.TypeDB.Oracle:
                                dbWrite.BulkCopyOracle(dtWrite);
                                break;
                            case SharedEnum.TypeDB.SQLServer:
                                dbWrite.BulkCopySQLServer(dtWrite);
                                break;
                            case SharedEnum.TypeDB.PostgreSQL:
                                dbWrite.BulkKeepIdentityPostgreSQL(dtWrite);
                                break;
                        }

                        //清理
                        dtWrite.Clear();
                    }
                });

                //最后一批
                if (dtWrite.Rows.Count > 0)
                {
                    batchNo++;

                    if (batchNo == 1)
                    {
                        //第一批写入前清理表
                        if (!string.IsNullOrWhiteSpace(rw.WriteDeleteSQL))
                        {
                            vm.Log.Add($"清理写入表：{rw.WriteDeleteSQL}");
                            var num = dbWrite.SqlExecuteReader(rw.WriteDeleteSQL).Item2;
                            vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                        }
                    }

                    //分批写入
                    vm.Log.Add($"写入表（{rw.WriteTableName}）第 {batchNo} 批（行：{dtWrite.Rows.Count}/{rowCount}）");

                    switch (mdt.WriteConnectionInfo.ConnectionType)
                    {
                        case SharedEnum.TypeDB.SQLite:
                            dbWrite.BulkBatchSQLite(dtWrite);
                            break;
                        case SharedEnum.TypeDB.MySQL:
                        case SharedEnum.TypeDB.MariaDB:
                            dbWrite.BulkCopyMySQL(dtWrite);
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            dbWrite.BulkCopyOracle(dtWrite);
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            dbWrite.BulkCopySQLServer(dtWrite);
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            dbWrite.BulkKeepIdentityPostgreSQL(dtWrite);
                            break;
                    }

                    //清理
                    dtWrite.Clear();
                }

                if (catchCount > 0)
                {
                    vm.Log.Add($"列值转换失败：{catchCount} 次");
                }
                vm.Log.Add($"写入表（{rw.WriteTableName}）完成，耗时：{vm.PartTimeFormat()}，写表进度：{i + 1}/{mdt.ListReadWrite.Count}\n");
            }

            vm.Log.Add($"总共耗时：{vm.TotalTimeFormat()}\n");
            GC.Collect();

            vm.Set(SharedEnum.RTag.success);
            return vm;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="idb"></param>
        /// <param name="le">日志事件</param>
        /// <returns></returns>
        public static SharedResultVM ImportDatabase(TransferVM.ImportDatabase idb, Action<NotifyCollectionChangedEventArgs> le = null)
        {
            var vm = new SharedResultVM();
            vm.LogEvent(le);

            vm.Log.Add($"\n{DateTime.Now:yyyy-MM-dd HH:mm:ss} 导入数据\n");

            var db = idb.WriteConnectionInfo.NewDbHelper();

            var hsName = new HashSet<string>();

            vm.Log.Add($"读取数据源：{idb.ZipPath}\n");

            using var zipRead = ZipFile.OpenRead(idb.ZipPath);
            var zipList = zipRead.Entries.OrderBy(x => x.Name).ToList();

            for (int i = 0; i < zipList.Count; i++)
            {
                var dt = new DataTable();

                var item = zipList[i];
                dt.ReadXml(item.Open());

                //指定导入表名
                if (idb.ReadWriteTableMap.ContainsKey(dt.TableName))
                {
                    dt.TableName = idb.ReadWriteTableMap[dt.TableName];
                }

                //清空表
                if (hsName.Add(dt.TableName) && idb.WriteDeleteData)
                {
                    var clearTableSql = DbHelper.SqlClearTable(idb.WriteConnectionInfo.ConnectionType, dt.TableName);

                    vm.Log.Add($"清理写入表：{clearTableSql}");
                    var num = db.SqlExecuteReader(clearTableSql).Item2;
                    vm.Log.Add($"返回受影响行数：{num}，耗时：{vm.PartTimeFormat()}");
                }

                vm.Log.Add($"导入表（{dt.TableName}）分片：{item.Name}（大小：{ParsingTo.FormatByteSize(item.Length)}，行：{dt.Rows.Count}）");

                switch (idb.WriteConnectionInfo.ConnectionType)
                {
                    case SharedEnum.TypeDB.SQLite:
                        db.BulkBatchSQLite(dt);
                        break;
                    case SharedEnum.TypeDB.MySQL:
                    case SharedEnum.TypeDB.MariaDB:
                        db.BulkCopyMySQL(dt);
                        break;
                    case SharedEnum.TypeDB.Oracle:
                        db.BulkCopyOracle(dt);
                        break;
                    case SharedEnum.TypeDB.SQLServer:
                        db.BulkCopySQLServer(dt);
                        break;
                    case SharedEnum.TypeDB.PostgreSQL:
                        db.BulkKeepIdentityPostgreSQL(dt);
                        break;
                }

                vm.Log.Add($"导入表（{dt.TableName}）分片成功，耗时：{vm.PartTimeFormat()}，导入进度：{i + 1}/{zipList.Count}\n");
            }

            vm.Log.Add($"导入完成，共耗时：{vm.UseTimeFormat}\n");
            GC.Collect();

            vm.Set(SharedEnum.RTag.success);
            return vm;
        }

    }
}

#endif