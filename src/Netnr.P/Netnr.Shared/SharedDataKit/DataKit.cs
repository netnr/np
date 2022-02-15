#if Full || DataKit

using System.Data;
using MySqlConnector;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// DataKit调用
    /// </summary>
    public partial class DataKit
    {
        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="database"></param>
        /// <param name="vmdk"></param>
        /// <returns></returns>
        private static SharedResultVM Entry(SharedEnum.TypeDB? tdb, string conn, string database, Func<SharedResultVM, IDataKit, SharedResultVM> vmdk)
        {
            var vm = new SharedResultVM();

            try
            {
                //内部上下文
                if (Core.CacheTo.Get("CDB") is Tuple<SharedEnum.TypeDB, string> cdb)
                {
                    tdb = cdb.Item1;
                    conn = cdb.Item2;
                }

                if (tdb == null)
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "数据库类型不能为空";
                }
                else if (string.IsNullOrWhiteSpace(conn))
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "连接字符串不能为空";
                }

                IDataKit dkto = null;

                if (string.IsNullOrWhiteSpace(vm.Msg))
                {
                    //额外处理 SQLite
                    if (tdb == SharedEnum.TypeDB.SQLite)
                    {
                        //下载 SQLite 文件
                        var ds = conn[12..].TrimEnd(';');
                        //路径
                        var dspath = Path.GetTempPath();
                        //文件名
                        var dsname = Path.GetFileName(ds);
                        var fullPath = Path.Combine(dspath, dsname);

                        //网络路径
                        if (ds.ToLower().StartsWith("http"))
                        {
                            //不存在则下载
                            if (!File.Exists(fullPath))
                            {
                                //下载
                                Core.HttpTo.DownloadSave(Core.HttpTo.HWRequest(ds), fullPath);
                            }

                            conn = "Data Source=" + fullPath;
                        }
                        else
                        {
                            conn = "Data Source=" + ds;
                        }
                    }

                    conn = SharedAdo.DbHelper.SqlConnPreCheck(tdb, conn);

                    switch (tdb.Value)
                    {
                        case SharedEnum.TypeDB.SQLite:
                            dkto = new DataKitSQLite(new SqliteConnection(conn));
                            break;
                        case SharedEnum.TypeDB.MySQL:
                        case SharedEnum.TypeDB.MariaDB:
                            {
                                var csb = new MySqlConnectionStringBuilder(conn);
                                if (!string.IsNullOrWhiteSpace(database))
                                {
                                    csb.Database = database;
                                }
                                dkto = new DataKitMySQL(new MySqlConnection(csb.ConnectionString));
                            }
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            {
                                dkto = new DataKitOracle(new OracleConnection(conn));
                            }
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            {
                                var csb = new SqlConnectionStringBuilder(conn);
                                if (!string.IsNullOrWhiteSpace(database))
                                {
                                    csb.InitialCatalog = database;
                                }
                                dkto = new DataKitSQLServer(new SqlConnection(csb.ConnectionString));
                            }
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            {
                                var csb = new NpgsqlConnectionStringBuilder(conn);
                                if (!string.IsNullOrWhiteSpace(database))
                                {
                                    csb.Database = database;
                                }
                                dkto = new DataKitPostgreSQL(new NpgsqlConnection(csb.ConnectionString));
                            }
                            break;
                    }
                }

                vm = vmdk(vm, dkto);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取库名
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        public static SharedResultVM GetDatabaseName(SharedEnum.TypeDB? tdb, string conn)
        {
            return Entry(tdb, conn, null, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetDatabaseName();
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="filterDatabaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM GetDatabase(SharedEnum.TypeDB? tdb, string conn, string filterDatabaseName = null)
        {
            return Entry(tdb, conn, null, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetDatabase(filterDatabaseName);
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="databaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM GetTable(SharedEnum.TypeDB? tdb, string conn, string databaseName = null)
        {
            return Entry(tdb, conn, databaseName, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetTable(databaseName);
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 表DDL
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static SharedResultVM GetTableDDL(SharedEnum.TypeDB? tdb, string conn, string tableName, string tableSchema, string databaseName = null)
        {
            return Entry(tdb, conn, databaseName, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetTableDDL(tableName, tableSchema, databaseName);
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="filterTableName">过滤表名，英文逗号分隔，为空时默认所有表</param>
        /// <param name="databaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM GetColumn(SharedEnum.TypeDB? tdb, string conn, string filterTableName = null, string databaseName = null)
        {
            return Entry(tdb, conn, databaseName, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetColumn(filterTableName, databaseName);
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableComment">表注释</param>
        /// <param name="databaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM SetTableComment(SharedEnum.TypeDB? tdb, string conn, string tableName, string tableComment, string databaseName = null)
        {
            return Entry(tdb, conn, databaseName, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.SetTableComment(tableName, tableComment, databaseName);
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <param name="columnComment">列注释</param>
        /// <param name="databaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM SetColumnComment(SharedEnum.TypeDB? tdb, string conn, string tableName, string columnName, string columnComment, string databaseName = null)
        {
            return Entry(tdb, conn, databaseName, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.SetColumnComment(tableName, columnName, columnComment, databaseName);
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="sql">脚本</param>
        /// <param name="databaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM ExecuteSql(SharedEnum.TypeDB? tdb, string conn, string sql, string databaseName = null)
        {
            return Entry(tdb, conn, databaseName, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.ExecuteSql(sql.Trim(), databaseName);
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="page">页码</param>
        /// <param name="rows">页量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序方式</param>
        /// <param name="listFieldName">查询列，默认为 *</param>
        /// <param name="whereSql">条件</param>
        /// <param name="databaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM GetData(SharedEnum.TypeDB? tdb, string conn, string tableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, string databaseName = null)
        {
            return Entry(tdb, conn, databaseName, (vm, dk) =>
            {
                if (dk != null)
                {
                    var gd = dk.GetData(tableName, page, rows, sort, order, listFieldName, whereSql, databaseName);
                    vm.Data = new
                    {
                        data = gd.Item1,
                        total = gd.Item2
                    };
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 查询数据库环境信息
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        public static SharedResultVM GetDEI(SharedEnum.TypeDB? tdb, string conn)
        {
            return Entry(tdb, conn, null, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetDEI();
                    vm.Set(SharedEnum.RTag.success);
                }
                return vm;
            });
        }

        /// <summary>
        /// 表转DEI实体
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DEIVM TableToDEI(DataTable dt)
        {
            var mo = new DEIVM();

            var dts = dt.Select();
            var mogt = mo.GetType();
            mogt.GetProperties().ToList().ForEach(pi =>
            {
                var dr = dts.FirstOrDefault(x => x[0].ToString().ToLower() == pi.Name.ToLower());
                if (dr != null)
                {
                    var drValue = dr[1];

                    Type type = pi.PropertyType;
                    if (pi.PropertyType.FullName.Contains("System.Nullable"))
                    {
                        type = Type.GetType("System." + pi.PropertyType.FullName.Split(',')[0].Split('.')[2]);
                    }

                    if (pi != null && pi.CanWrite && (drValue != null && drValue is not DBNull))
                    {
                        try
                        {
                            drValue = Convert.ChangeType(drValue, type);
                            pi.SetValue(mo, drValue, null);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            });

            return mo;
        }
    }
}

#endif