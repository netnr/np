#if Full || DataKit

using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace Netnr.SharedDataKit
{
    /// <summary>
    /// DataKit调用
    /// </summary>
    public partial class DataKitTo
    {
        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        private static SharedResultVM ForTypeDBSetConn(SharedEnum.TypeDB? tdb, string conn)
        {
            var vm = new SharedResultVM();

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
            else if (tdb == SharedEnum.TypeDB.SQLite)
            {
                //下载 SQLite 文件
                var ds = conn[12..].TrimEnd(';');
                //路径
                var dspath = AppContext.BaseDirectory + "tmp/";
                if (!Directory.Exists(dspath))
                {
                    _ = Directory.CreateDirectory(dspath);
                }
                //文件名
                var dsname = $"{DateTime.Now:yyyyMMdd}_{Math.Abs(ds.GetHashCode())}.db";

                //网络路径
                if (ds.ToLower().StartsWith("http"))
                {
                    //不存在则下载
                    if (!File.Exists(dspath + dsname))
                    {
                        //删除超过1天的下载文件
                        var files = Directory.GetFiles(dspath);
                        foreach (var file in files)
                        {
                            var ct = Path.GetFileName(file).Split('_')[0];
                            if (ct.Length == 8)
                            {
                                if ((DateTime.Now - DateTime.Parse(ct.Insert(4, "-").Insert(7, "-"))).TotalDays >= 1)
                                {
                                    File.Delete(file);
                                }
                            }
                        }

                        //下载
                        //new System.Net.WebClient().DownloadFile(ds, dspath + dsname);
                    }

                    vm.Data = "Data Source=" + dspath + dsname;
                }
                else
                {
                    vm.Data = "Data Source=" + ds;
                }

                vm.Set(SharedEnum.RTag.success);
            }
            else
            {
                vm.Data = conn;
                vm.Set(SharedEnum.RTag.success);
            }

            return vm;
        }

        /// <summary>
        /// 获取所有表名及注释
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        public static SharedResultVM GetTable(SharedEnum.TypeDB? tdb, string conn)
        {
            var vm = new SharedResultVM();

            try
            {
                var tvm = ForTypeDBSetConn(tdb, conn);

                if (tvm.Code == 200)
                {
                    conn = tvm.Data.ToString();
                    switch (tdb)
                    {
                        case SharedEnum.TypeDB.MySQL:
                            vm.Data = new DataKitMySQLTo(conn).GetTable();
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteTo(conn).GetTable();
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleTo(conn).GetTable();
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerTo(conn).GetTable();
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLTo(conn).GetTable();
                            break;
                    }

                    vm.Set(SharedEnum.RTag.success);
                }
                else
                {
                    vm = tvm;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="filterTableName">过滤表名，英文逗号分隔，为空时默认所有表</param>
        /// <returns></returns>
        public static SharedResultVM GetColumn(SharedEnum.TypeDB? tdb, string conn, string filterTableName = "")
        {
            var vm = new SharedResultVM();

            try
            {
                var listTableName = new List<string>();
                if (!string.IsNullOrWhiteSpace(filterTableName))
                {
                    listTableName = filterTableName.Split(',').ToList();
                }
                else
                {
                    listTableName = null;
                }

                var tvm = ForTypeDBSetConn(tdb, conn);
                if (tvm.Code == 200)
                {
                    conn = tvm.Data.ToString();
                    switch (tdb)
                    {
                        case SharedEnum.TypeDB.MySQL:
                            vm.Data = new DataKitMySQLTo(conn).GetColumn(listTableName);
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteTo(conn).GetColumn(listTableName);
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleTo(conn).GetColumn(listTableName);
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerTo(conn).GetColumn(listTableName);
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLTo(conn).GetColumn(listTableName);
                            break;
                    }

                    vm.Set(SharedEnum.RTag.success);
                }
                else
                {
                    vm = tvm;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="TableName">表名</param>
        /// <param name="TableComment">表注释</param>
        /// <returns></returns>
        public static SharedResultVM SetTableComment(SharedEnum.TypeDB? tdb, string conn, string TableName, string TableComment)
        {
            var vm = new SharedResultVM();

            try
            {
                var tvm = ForTypeDBSetConn(tdb, conn);

                if (tvm.Code == 200)
                {
                    conn = tvm.Data.ToString();
                    switch (tdb)
                    {
                        case SharedEnum.TypeDB.MySQL:
                            vm.Data = new DataKitMySQLTo(conn).SetTableComment(TableName, TableComment);
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteTo(conn).SetTableComment(TableName, TableComment);
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleTo(conn).SetTableComment(TableName, TableComment);
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerTo(conn).SetTableComment(TableName, TableComment);
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLTo(conn).SetTableComment(TableName, TableComment);
                            break;
                    }

                    vm.Set(SharedEnum.RTag.success);
                }
                else
                {
                    vm = tvm;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 设置列注释
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="TableName">表名</param>
        /// <param name="FieldName">列名</param>
        /// <param name="FieldComment">列注释</param>
        /// <returns></returns>
        public static SharedResultVM SetColumnComment(SharedEnum.TypeDB? tdb, string conn, string TableName, string FieldName, string FieldComment)
        {
            var vm = new SharedResultVM();

            try
            {
                var tvm = ForTypeDBSetConn(tdb, conn);

                if (tvm.Code == 200)
                {
                    conn = tvm.Data.ToString();
                    switch (tdb)
                    {
                        case SharedEnum.TypeDB.MySQL:
                            vm.Data = new DataKitMySQLTo(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteTo(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleTo(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerTo(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLTo(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                    }

                    vm.Set(SharedEnum.RTag.success);
                }
                else
                {
                    vm = tvm;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <param name="TableName">表名</param>
        /// <param name="page">页码</param>
        /// <param name="rows">页量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序方式</param>
        /// <param name="listFieldName">查询列，默认为 *</param>
        /// <param name="whereSql">条件</param>
        /// <returns></returns>
        public static SharedResultVM GetData(SharedEnum.TypeDB? tdb, string conn, string TableName, int page, int rows, string sort, string order, string listFieldName, string whereSql)
        {
            var vm = new SharedResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(listFieldName))
                {
                    listFieldName = "*";
                }

                var tvm = ForTypeDBSetConn(tdb, conn);

                if (tvm.Code == 200)
                {
                    conn = tvm.Data.ToString();
                    switch (tdb)
                    {
                        case SharedEnum.TypeDB.MySQL:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitMySQLTo(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitSQLiteTo(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitOracleTo(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitSQLServerTo(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitPostgreSQLTo(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                    }

                    vm.Set(SharedEnum.RTag.success);
                }
                else
                {
                    vm = tvm;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 查询数据库环境信息
        /// </summary>
        /// <param name="tdb">数据库类型（0：MySQL，1：SQLite，2：Oracle，3：SQLServer，4：PostgreSQL）</param>
        /// <param name="conn">连接字符串</param>
        /// <returns></returns>
        public static SharedResultVM GetDEI(SharedEnum.TypeDB? tdb, string conn)
        {
            var vm = new SharedResultVM();

            try
            {
                var tvm = ForTypeDBSetConn(tdb, conn);

                if (tvm.Code == 200)
                {
                    conn = tvm.Data.ToString();
                    switch (tdb)
                    {
                        case SharedEnum.TypeDB.MySQL:
                            vm.Data = new DataKitMySQLTo(conn).GetDEI();
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteTo(conn).GetDEI();
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleTo(conn).GetDEI();
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerTo(conn).GetDEI();
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLTo(conn).GetDEI();
                            break;
                    }

                    vm.Set(SharedEnum.RTag.success);
                }
                else
                {
                    vm = tvm;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
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