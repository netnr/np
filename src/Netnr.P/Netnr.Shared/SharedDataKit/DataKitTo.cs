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
    public partial class DataKitTo
    {
        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static SharedResultVM Entry(SharedEnum.TypeDB? tdb, string conn, Func<SharedResultVM, IDataKitTo, SharedResultVM> func)
        {
            var vm = new SharedResultVM();

            try
            {
                //内部上下文
                Tuple<SharedEnum.TypeDB, string> cdb = Core.CacheTo.Get("CDB") as Tuple<SharedEnum.TypeDB, string>;
                if (cdb != null)
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

                IDataKitTo dkto = null;

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

                    switch (tdb.Value)
                    {
                        case SharedEnum.TypeDB.SQLite:
                            dkto = new DataKitSQLiteTo(new SqliteConnection(conn));
                            break;
                        case SharedEnum.TypeDB.MySQL:
                        case SharedEnum.TypeDB.MariaDB:
                            dkto = new DataKitMySQLTo(new MySqlConnection(conn));
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            dkto = new DataKitOracleTo(new OracleConnection(conn));
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            dkto = new DataKitSQLServerTo(new SqlConnection(conn));
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            dkto = new DataKitPostgreSQLTo(new NpgsqlConnection(conn));
                            break;
                    }
                }

                vm = func(vm, dkto);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取库
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        public static SharedResultVM GetDatabase(SharedEnum.TypeDB? tdb, string conn)
        {
            return Entry(tdb, conn, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetDatabase();
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
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM GetTable(SharedEnum.TypeDB? tdb, string conn, string DatabaseName = null)
        {
            return Entry(tdb, conn, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetTable(DatabaseName);
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
        /// <param name="filterTableName"></param>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public static SharedResultVM GetTableDDL(SharedEnum.TypeDB? tdb, string conn, string filterTableName = null, string DatabaseName = null)
        {
            return Entry(tdb, conn, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetTableDDL(filterTableName, DatabaseName);
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
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM GetColumn(SharedEnum.TypeDB? tdb, string conn, string filterTableName = null, string DatabaseName = null)
        {
            return Entry(tdb, conn, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.GetColumn(filterTableName, DatabaseName);
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
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM SetTableComment(SharedEnum.TypeDB? tdb, string conn, string TableName, string TableComment, string DatabaseName = null)
        {
            return Entry(tdb, conn, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.SetTableComment(TableName, TableComment, DatabaseName);
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
        /// <param name="TableName">表名</param>
        /// <param name="ColumnName">列名</param>
        /// <param name="ColumnComment">列注释</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM SetColumnComment(SharedEnum.TypeDB? tdb, string conn, string TableName, string ColumnName, string ColumnComment, string DatabaseName = null)
        {
            return Entry(tdb, conn, (vm, dk) =>
            {
                if (dk != null)
                {
                    vm.Data = dk.SetColumnComment(TableName, ColumnName, ColumnComment, DatabaseName);
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
        /// <param name="TableName">表名</param>
        /// <param name="page">页码</param>
        /// <param name="rows">页量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序方式</param>
        /// <param name="listFieldName">查询列，默认为 *</param>
        /// <param name="whereSql">条件</param>
        /// <param name="DatabaseName">数据库名</param>
        /// <returns></returns>
        public static SharedResultVM GetData(SharedEnum.TypeDB? tdb, string conn, string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql, string DatabaseName = null)
        {
            return Entry(tdb, conn, (vm, dk) =>
            {
                if (dk != null)
                {
                    var gd = dk.GetData(TableName, page, rows, sort, order, listFieldName, whereSql, DatabaseName);
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
            return Entry(tdb, conn, (vm, dk) =>
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
                        catch (Exception)
                        {

                        }
                    }
                }
            });

            return mo;
        }
    }
}

#endif