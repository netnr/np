#if Full || AdoFull || AdoPostgreSQL

using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// PostgreSQL操作类
    /// </summary>
    public partial class DbHelper
    {
        /// <summary>
        /// 表批量写入（排除自增列）
        /// https://www.npgsql.org/doc/copy.html
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <returns></returns>
        public int BulkCopyPostgreSQL(DataTable dt, string table)
        {
            return SafeConn(() =>
            {
                var connection = (NpgsqlConnection)Connection;

                //提取表列类型与数据库类型
                var cb = new NpgsqlCommandBuilder();
                var quoteTable = $"{cb.QuotePrefix}{table}{cb.QuoteSuffix}";
                cb.DataAdapter = new NpgsqlDataAdapter
                {
                    SelectCommand = new NpgsqlCommand($"select * from {quoteTable} where 0=1", connection)
                };

                //获取列类型
                var pars = cb.GetInsertCommand(true).Parameters;
                var colDbType = new Dictionary<string, NpgsqlDbType>();
                foreach (NpgsqlParameter par in pars)
                {
                    colDbType.Add(par.SourceColumn, par.NpgsqlDbType);
                }

                //获取自增
                var dtSchema = new DataTable();
                cb.DataAdapter.FillSchema(dtSchema, SchemaType.Source);
                var autoIncrCol = dtSchema.Columns.Cast<DataColumn>().Where(x => x.AutoIncrement == true).Select(x => x.ColumnName).ToList();

                //排除自增
                var columns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                columns = columns.Except(autoIncrCol).ToList();
                string copyString = $"COPY {quoteTable}(\"" + string.Join("\",\"", columns) + "\") FROM STDIN (FORMAT BINARY)";
                autoIncrCol.ForEach(x => dt.Columns.Remove(x));

                var num = 0;
                using (var writer = connection.BeginBinaryImport(copyString))
                {
                    var now = DateTime.Now;
                    writer.Timeout = now.AddSeconds(3600) - now;

                    foreach (DataRow dr in dt.Rows)
                    {
                        writer.StartRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            var val = dr[dc.ColumnName];
                            if (val is not DBNull)
                            {
                                //列对应数据库类型
                                var dbType = colDbType[dc.ColumnName];
                                if (dc.DataType.FullName == "System.String")
                                {
                                    writer.Write(val.ToString().Replace("\0", ""), dbType);
                                }
                                else
                                {
                                    writer.Write(val, dbType);
                                }
                            }
                            else
                            {
                                writer.WriteNull();
                            }
                        }
                    }

                    num = (int)writer.Complete();
                }

                return num;
            });
        }

        /// <summary>
        /// 表批量写入（须手动处理自增列SQL）
        /// 根据行数据 RowState 状态新增、修改
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="table">数据库表名</param>
        /// <param name="dataAdapter">执行前修改（命令行脚本、超时等信息）</param>
        /// <param name="openTransaction">开启事务，默认开启</param>
        /// <returns></returns>
        public int BulkBatchPostgreSQL(DataTable dt, string table, Action<NpgsqlDataAdapter> dataAdapter = null, bool openTransaction = true)
        {
            return SafeConn(() =>
            {
                dt.TableName = table;

                var connection = (NpgsqlConnection)Connection;
                NpgsqlTransaction transaction = openTransaction ? (NpgsqlTransaction)(Transaction = connection.BeginTransaction()) : null;

                var cb = new NpgsqlCommandBuilder();
                cb.DataAdapter = new NpgsqlDataAdapter
                {
                    SelectCommand = new NpgsqlCommand($"select * from {cb.QuotePrefix}{table}{cb.QuoteSuffix}", connection, transaction)
                };
                cb.ConflictOption = ConflictOption.OverwriteChanges;

                var da = new NpgsqlDataAdapter
                {
                    InsertCommand = cb.GetInsertCommand(true),
                    UpdateCommand = cb.GetUpdateCommand(true)
                };
                da.InsertCommand.CommandTimeout = 300;
                da.UpdateCommand.CommandTimeout = 300;

                //处理：无效的 "UTF8" 编码字节顺序: 0x00
                var listColName = dt.Columns.Cast<DataColumn>().Where(x => x.DataType == typeof(string)).Select(x => x.ColumnName).ToList();
                foreach (DataRow dr in dt.Rows)
                {
                    listColName.ForEach(colName =>
                    {
                        var val = dr[colName];
                        if (val is not DBNull)
                        {
                            dr[colName] = val.ToString().Replace("\0", "");
                        }
                    });
                }

                //执行前修改
                dataAdapter?.Invoke(da);

                var num = da.Update(dt);

                transaction?.Commit();

                return num;
            });
        }
    }
}

#endif