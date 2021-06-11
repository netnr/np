#if Full || DataKit

using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using MySqlConnector;
using System.Data.SQLite;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using Npgsql;

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
            var cb = SqlCmdBuild(tdb);
            return $"{cb.QuotePrefix}{KeyWord}{cb.QuoteSuffix}";
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="tdb"></param>
        /// <returns></returns>
        public static DbCommandBuilder SqlCmdBuild(SharedEnum.TypeDB? tdb)
        {
            return tdb switch
            {
                SharedEnum.TypeDB.SQLite => new SQLiteCommandBuilder(),
                SharedEnum.TypeDB.MySQL => new MySqlCommandBuilder(),
                SharedEnum.TypeDB.Oracle => new OracleCommandBuilder(),
                SharedEnum.TypeDB.SQLServer => new SqlCommandBuilder(),
                SharedEnum.TypeDB.PostgreSQL => new NpgsqlCommandBuilder(),
                _ => null,
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
                SharedEnum.TypeDB.SQLite => new SQLiteConnection(conn),
                SharedEnum.TypeDB.MySQL => new MySqlConnection(conn),
                SharedEnum.TypeDB.Oracle => new OracleConnection(conn),
                SharedEnum.TypeDB.SQLServer => new SqlConnection(conn),
                SharedEnum.TypeDB.PostgreSQL => new NpgsqlConnection(conn),
                _ => null,
            };
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
                case SharedEnum.TypeDB.SQLite:
                    {
                        var cb = new SQLiteCommandBuilder();
                        var dbConn = new SQLiteConnection(conn);
                        //引用表名
                        var quoteTable = $"{cb.QuotePrefix}{table}{cb.QuoteSuffix}";
                        cb.DataAdapter = new SQLiteDataAdapter
                        {
                            SelectCommand = new SQLiteCommand($"select * from {quoteTable} where 0=1", dbConn)
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

                            foreach (SQLiteParameter par in pars)
                            {
                                if (par.SourceColumn == dc.ColumnName)
                                {
                                    vm.DbType = par.DbType;
                                    break;
                                }
                            }

                            vms.Add(vm);
                        }
                    }
                    break;
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

        /// <summary>
        /// 执行脚本历史记录
        /// </summary>
        /// <param name="tdb"></param>
        /// <param name="conn"></param>
        /// <param name="search"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        public static void SqlScriptHistory(SharedEnum.TypeDB tdb, string conn, string search, DateTime? beginTime, DateTime? endTime)
        {
            switch (tdb)
            {
                case SharedEnum.TypeDB.SQLite:
                    break;
                case SharedEnum.TypeDB.MySQL:
                    break;
                case SharedEnum.TypeDB.Oracle:
                    break;
                case SharedEnum.TypeDB.SQLServer:
                    {
                    }
                    break;
                case SharedEnum.TypeDB.PostgreSQL:
                    break;
            }
        }
    }
}

#endif