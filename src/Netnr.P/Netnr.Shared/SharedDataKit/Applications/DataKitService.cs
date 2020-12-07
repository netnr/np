#if Full || DataKit

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Netnr.SharedDataKit.Applications
{
    /// <summary>
    /// DataKit调用
    /// </summary>
    public class DataKitService
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
                var dsname = DateTime.Now.ToString("yyyyMMdd") + "_" + SHA128ToLong(ds) + ".db";

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
                        new System.Net.WebClient().DownloadFile(ds, dspath + dsname);
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
                            vm.Data = new DataKitMySQLService(conn).GetTable();
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteService(conn).GetTable();
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleService(conn).GetTable();
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerService(conn).GetTable();
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLService(conn).GetTable();
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
                            vm.Data = new DataKitMySQLService(conn).GetColumn(listTableName);
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteService(conn).GetColumn(listTableName);
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleService(conn).GetColumn(listTableName);
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerService(conn).GetColumn(listTableName);
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLService(conn).GetColumn(listTableName);
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
                            vm.Data = new DataKitMySQLService(conn).SetTableComment(TableName, TableComment);
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteService(conn).SetTableComment(TableName, TableComment);
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleService(conn).SetTableComment(TableName, TableComment);
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerService(conn).SetTableComment(TableName, TableComment);
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLService(conn).SetTableComment(TableName, TableComment);
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
                            vm.Data = new DataKitMySQLService(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteService(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleService(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerService(conn).SetColumnComment(TableName, FieldName, FieldComment);
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLService(conn).SetColumnComment(TableName, FieldName, FieldComment);
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
                                    data = new DataKitMySQLService(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitSQLiteService(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitOracleService(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitSQLServerService(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
                                    total
                                };
                            }
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            {
                                vm.Data = new
                                {
                                    data = new DataKitPostgreSQLService(conn).GetData(TableName, page, rows, sort, order, listFieldName, whereSql, out int total),
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
                            vm.Data = new DataKitMySQLService(conn).GetDEI();
                            break;
                        case SharedEnum.TypeDB.SQLite:
                            vm.Data = new DataKitSQLiteService(conn).GetDEI();
                            break;
                        case SharedEnum.TypeDB.Oracle:
                            vm.Data = new DataKitOracleService(conn).GetDEI();
                            break;
                        case SharedEnum.TypeDB.SQLServer:
                            vm.Data = new DataKitSQLServerService(conn).GetDEI();
                            break;
                        case SharedEnum.TypeDB.PostgreSQL:
                            vm.Data = new DataKitPostgreSQLService(conn).GetDEI();
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
        /// SHA1 加密 为 long
        /// </summary>
        /// <param name="str">内容</param>
        /// <returns></returns>
        private static long SHA128ToLong(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            using SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider();
            byte[] byteArr = SHA1.ComputeHash(buffer);
            return BitConverter.ToInt64(byteArr, 0);
        }
    }
}

#endif