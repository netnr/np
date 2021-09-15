﻿#if Full || Ado || AdoFull

using System.Data;
using System.Data.Common;

namespace Netnr.SharedAdo
{
    /// <summary>
    /// Db帮助类
    /// </summary>
    public partial class DbHelper
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
                SharedEnum.TypeDB.MySQL or SharedEnum.TypeDB.MariaDB => $"`{KeyWord}`",
                SharedEnum.TypeDB.Oracle or SharedEnum.TypeDB.PostgreSQL => $"\"{KeyWord}\"",
                _ => KeyWord,
            };
        }

        /// <summary>
        /// SQL连接字符串预检
        /// </summary>
        /// <param name="tdb">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static string SqlConnPreCheck(SharedEnum.TypeDB? tdb, string connectionString)
        {
            var citem = new Dictionary<string, string>();

            switch (tdb)
            {
                case SharedEnum.TypeDB.MySQL:
                case SharedEnum.TypeDB.MariaDB:
                    citem.Add("AllowLoadLocalInfile", "true");
                    break;
                case SharedEnum.TypeDB.SQLServer:
                    citem.Add("TrustServerCertificate", "true");
                    break;
            }

            if (citem.Count > 0)
            {
                foreach (var key in citem.Keys)
                {
                    if (!connectionString.ToLower().Contains(key.ToLower()))
                    {
                        connectionString = connectionString.TrimEnd(';') + $";{key}={citem[key]}";
                    }
                }
            }

            return connectionString;
        }

        /// <summary>
        /// SQL连接字符串加密/解密
        /// </summary>
        /// <param name="conn">连接字符串</param>
        /// <param name="pwd">密码</param>
        /// <param name="isDecrypt">是解密，加密 false</param>
        public static string SqlConnEncryptOrDecrypt(string conn, string pwd, bool isDecrypt = true)
        {
            if (isDecrypt)
            {
                var ckey = "CONNED" + conn.GetHashCode();
                if (Core.CacheTo.Get(ckey) is not string cval)
                {
                    var clow = conn.ToLower();
                    var pts = new List<string> { "database", "server", "filename", "source", "user" };
                    if (!pts.Any(x => clow.Contains(x)))
                    {
                        cval = Core.CalcTo.AESDecrypt(conn, pwd);
                    }
                    else
                    {
                        cval = conn;
                    }

                    Core.CacheTo.Set(ckey, cval);
                }
                return cval;
            }
            else
            {
                return conn = Core.CalcTo.AESEncrypt(conn, pwd);
            }
        }
    }

    /// <summary>
    /// 扩展
    /// </summary>
    public static class DbHelperExtend
    {
        /// <summary>
        /// 查询返回数据集
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(this DbCommand dbCommand)
        {
            var ds = new DataSet();

            var reader = dbCommand.ExecuteReader();

            do
            {
                var table = new DataTable
                {
                    TableName = "table" + (ds.Tables.Count + 1).ToString()
                };
                table.Load(reader);
                ds.Tables.Add(table);
            } while (!reader.IsClosed);

            return ds;
        }
    }
}

#endif