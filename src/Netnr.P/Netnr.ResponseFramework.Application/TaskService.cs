using System;
using System.IO;
using System.Linq;
using FluentScheduler;
using Newtonsoft.Json.Linq;
using Netnr.SharedDbContext;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Netnr.ResponseFramework.Data;
using Microsoft.EntityFrameworkCore;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.ResponseFramework.Application
{
    /// <summary>
    /// 任务
    /// 帮助文档：https://github.com/fluentscheduler/FluentScheduler
    /// </summary>
    public class TaskService
    {
        /// <summary>
        /// 任务注册
        /// </summary>
        public class Reg : Registry
        {
            /// <summary>
            /// 构造
            /// </summary>
            public Reg()
            {
                //每间隔一天在4:4 重置数据库
                _ = Schedule<ResetDataBaseJob>().ToRunEvery(1).Days().At(4, 4);
                //每间隔两天在3:3 清理临时目录
                _ = Schedule<ClearTmpJob>().ToRunEvery(2).Days().At(3, 3);
            }
        }

        /// <summary>
        /// 重置数据库
        /// </summary>
        public class ResetDataBaseJob : IJob
        {
            void IJob.Execute()
            {
                var vm = DatabaseAsJson.ReadToJson();
                ConsoleTo.Log(vm);
            }
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        public class ClearTmpJob : IJob
        {
            void IJob.Execute()
            {
                var vm = ClearTmp();
                ConsoleTo.Log(vm);
            }
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        /// <returns></returns>
        public static SharedResultVM ClearTmp()
        {
            var vm = new SharedResultVM();

            try
            {
                string directoryPath = PathTo.Combine(GlobalTo.WebRootPath, GlobalTo.GetValue("StaticResource:TmpDir"));

                vm.Log.Add($"清理临时目录：{directoryPath}");
                if (!Directory.Exists(directoryPath))
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "文件路径不存在";
                    return vm;
                }

                int delFileCount = 0;
                int delFolderCount = 0;

                //删除文件
                var listFile = Directory.GetFiles(directoryPath).ToList();
                foreach (var path in listFile)
                {
                    if (!path.Contains("README"))
                    {
                        try
                        {
                            File.Delete(path);
                            delFileCount++;
                        }
                        catch (Exception ex)
                        {
                            vm.Log.Add($"删除文件异常：{ex.Message}");
                        }
                    }
                }

                //删除文件夹
                var listFolder = Directory.GetDirectories(directoryPath).ToList();
                foreach (var path in listFolder)
                {
                    try
                    {
                        Directory.Delete(path, true);
                        delFolderCount++;
                    }
                    catch (Exception ex)
                    {
                        vm.Log.Add($"删除文件夹异常：{ex.Message}");
                    }
                }

                vm.Log.Insert(0, $"删除文件{delFileCount}个，删除{delFolderCount}个文件夹");
                vm.Set(SharedEnum.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// - 数据库数据存储为JSON（备份数据）
        /// - 根据JSON反序列化导入（重置数据）
        /// </summary>
        public class DatabaseAsJson
        {
            /// <summary>
            /// JSON存储路径
            /// </summary>
            private static readonly string fullJsonData = PathTo.Combine(GlobalTo.WebRootPath, "/scripts/table-json/data.json");

            /// <summary>
            /// 获取所有表
            /// </summary>
            /// <param name="db"></param>
            /// <returns></returns>
            public static List<string> Tables(DbContext db)
            {
                var list = new List<string>();

                var pis = db.GetType().GetProperties();
                foreach (var pi in pis)
                {
                    if (pi.PropertyType.Name.Contains("DbSet"))
                    {
                        list.Add(pi.Name);
                    }
                }
                _ = list.Remove("SysLog");

                return list;
            }

            /// <summary>
            /// 写入JSON文件
            /// </summary>
            /// <param name="CoverJson">写入JSON文件，默认 false</param>
            /// <returns></returns>
            public static SharedResultVM WriteToJson(bool CoverJson = false)
            {
                var vm = new SharedResultVM();

                var dicKey = new Dictionary<string, object>();

                using var db = ContextBaseFactory.CreateDbContext();

                vm.Log.Add($"时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                vm.Log.Add($"驱动：{db.Database.ProviderName}");

                var conn = db.Database.GetDbConnection();
                vm.Log.Add($"数据库：{conn.Database}");
                conn.Open();
                vm.Log.Add($"版本号：{conn.ServerVersion}");
                conn.Close();

                var tables = Tables(db);
                var listSql = new List<string>();
                foreach (var table in tables)
                {
                    listSql.Add($"select * from {db.Ros()}{table}{db.Ros()}");
                }
                vm.Log.AddRange(listSql);
                var ds = db.SqlQuery(string.Join(";", listSql));
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    var dt = ds.Tables[i];
                    dicKey.Add(tables[i], dt);
                }
                vm.Data = dicKey;

                vm.Log.Add($"写入JSON：{CoverJson}");
                if (CoverJson)
                {
                    FileTo.WriteText(vm.ToJson(), fullJsonData, false);
                }

                vm.Set(SharedEnum.RTag.success);

                return vm;
            }

            /// <summary>
            /// 读取JSON文件
            /// </summary>
            /// <param name="isClear">是否清理表，默认清理</param>
            /// <returns></returns>
            public static SharedResultVM ReadToJson(bool isClear = true)
            {
                var vm = new SharedResultVM();

                var listSql = new List<string>();
                try
                {
                    var jdata = FileTo.ReadText(fullJsonData).ToJObject()["data"];

                    using var db = ContextBaseFactory.CreateDbContext();
                    vm.Log.Add($"数据库驱动：{db.Database.ProviderName}");

                    var tables = Tables(db);

                    if (isClear)
                    {
                        vm.Log.Add("开始清空表");
                        tables.ForEach(table =>
                        {
                            listSql.Add($"delete from {db.Ros()}{table}{db.Ros()}");
                        });
                        vm.Log.AddRange(listSql);
                    }

                    var listNq = new List<JTokenType>() { JTokenType.Boolean, JTokenType.Float, JTokenType.Integer };

                    vm.Log.Add("开始插入数据");

                    foreach (string table in tables)
                    {
                        var ja = jdata[table];
                        if (ja == null)
                        {
                            continue;
                        }

                        var len = ja.Count();

                        for (int i = 0; i < len; i++)
                        {
                            var jo = ja[i] as JObject;
                            var prs = jo.Properties();
                            var fields = prs.Select(x => x.Name).ToList();
                            var values = new List<string>();

                            for (int j = 0; j < fields.Count; j++)
                            {
                                var jv = jo[fields[j]];
                                if (jv.Type == JTokenType.Null)
                                {
                                    values.Add("NULL");
                                }
                                else if (listNq.Contains(jv.Type))
                                {
                                    values.Add(jv.ToString());
                                }
                                else
                                {
                                    values.Add($"'{jv.ToString().OfSql()}'");
                                }
                            }

                            var sql = $"INSERT INTO {db.Ros()}{table}{db.Ros()}({db.Ros()}{string.Join($"{db.Ros()},{db.Ros()}", fields)}{db.Ros()}) values({string.Join(",", values)})";
                            listSql.Add(sql);
                        }
                    }

                    var num = db.SqlExecute(listSql);
                    vm.Log.Add($"脚本内容长度：{listSql.Sum(x => x.Length)}");
                    vm.Log.Add($"完成，耗时：{vm.PartTime()}，受影响行数：{num}");

                    vm.Data = num;
                    vm.Set(SharedEnum.RTag.success);
                }
                catch (Exception ex)
                {
                    vm.Set(ex);
                }

                return vm;
            }
        }

    }
}
