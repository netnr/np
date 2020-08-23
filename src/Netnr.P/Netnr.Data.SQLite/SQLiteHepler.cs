using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Netnr.Data.SQLite
{
    /// <summary>
    /// SQLite操作类
    /// </summary>
    public class SQLiteHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString = string.Empty;

        /// <summary>
        /// 构造，初始化字符串
        /// </summary>
        /// <param name="conn"></param>
        public SQLiteHelper(string conn)
        {
            connectionString = conn;
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql">查询SQL</param>
        /// <param name="listPreSql">前置执行SQL</param>
        /// <returns></returns>
        public DataSet Query(string sql, List<string> listPreSql = null)
        {
            using SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            //前置SQL
            if (listPreSql != null)
            {
                var cmd = connection.CreateCommand();
                using var transaction = connection.BeginTransaction();
                foreach (var preSql in listPreSql)
                {
                    cmd.CommandText = preSql;
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
            }

            SQLiteDataAdapter sda = new SQLiteDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            sda.Fill(ds, "ds");

            return ds;
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string sql, SQLiteParameter[] parameters = null)
        {
            using SQLiteConnection connection = new SQLiteConnection(connectionString);
            using var cmd = connection.CreateCommand();
            connection.Open();

            cmd.CommandText = sql;
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            int rows = cmd.ExecuteNonQuery();

            return rows;
        }

        /// <summary>
        /// 批量执行SQL
        /// </summary>
        /// <param name="listSql"></param>
        /// <returns></returns>
        public bool ExecuteNonQuery(List<string> listSql)
        {
            using var connection = new SQLiteConnection(connectionString);
            using var cmd = connection.CreateCommand();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var result = false;
            try
            {
                foreach (var sql in listSql)
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();

                result = true;
            }
            catch (System.Exception)
            {
                transaction.Rollback();
            }

            return result;
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="listPreSql">前置执行SQL</param>
        /// <returns>DataTable</returns>
        public object ExecuteScalar(string sql, List<string> listPreSql = null)
        {
            using SQLiteConnection connection = new SQLiteConnection(connectionString);
            using var cmd = connection.CreateCommand();
            connection.Open();

            if (listPreSql != null)
            {
                using var transaction = connection.BeginTransaction();
                foreach (var preSql in listPreSql)
                {
                    cmd.CommandText = preSql;
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
            }

            cmd.CommandText = sql;
            var result = cmd.ExecuteScalar();

            return result;
        }
    }
}