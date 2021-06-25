using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Netnr.SharedFast;

namespace Netnr.ResponseFramework.Data
{
    /// <summary>
    /// 扩展
    /// </summary>
    public partial class ContextBase : DbContext
    {
        /// <summary>
        /// 填充表
        /// </summary>
        /// <param name="args"></param>
        public static Dictionary<string, object> GetDicDbSet(ContextBase db)
        {
            var dic = GetDicDbSet(db.SysButton, db.SysDictionary, db.SysLog, db.SysMenu, db.SysRole, db.SysTableConfig, db.SysUser, db.TempExample, db.TempInvoiceDetail, db.TempInvoiceMain);
            return dic;
        }

        /// <summary>
        /// 填充表
        /// </summary>
        /// <param name="args"></param>
        public static Dictionary<string, object> GetDicDbSet(params object[] args)
        {
            var DicDbSet = new Dictionary<string, object>();
            foreach (var arg in args)
            {
                var name = arg.GetType().FullName.Split(',')[0].Split('.').Last();
                DicDbSet.Add(name, arg);
            }
            return DicDbSet;
        }

        /// <summary>
        /// 数据库导出
        /// </summary>
        /// <param name="fullPath">存储路径</param>
        /// <returns></returns>
        public static SharedResultVM ExportDataBase(string fullPath = null)
        {
            var vm = new SharedResultVM();

            try
            {
                using var db = ContextBaseFactory.CreateDbContext();
                var dicDbSet = GetDicDbSet(db);

                var rows = 0;
                var dicOut = new Dictionary<string, object> { };
                foreach (var key in dicDbSet.Keys)
                {
                    var dbset = dicDbSet[key] as IQueryable<object>;
                    vm.Log.Add($"查询表：{key}");
                    var list = dbset.ToList();
                    vm.Log.Add($"耗时：{vm.PartTime()} ，查询受影响行数：{list.Count}");
                    rows += list.Count;

                    dicOut.Add(key, list);
                }

                vm.Data = dicOut;

                vm.Log.Add($"时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                vm.Log.Add($"驱动：{db.Database.ProviderName}");

                var conn = db.Database.GetDbConnection();
                vm.Log.Add($"数据库：{conn.Database}");
                conn.Open();
                vm.Log.Add($"版本号：{conn.ServerVersion}");
                conn.Close();

                vm.Log.Add($"导出数据表 {dicOut.Keys.Count} 个，共 {rows} 行");

                //写入保存
                if (!string.IsNullOrWhiteSpace(fullPath))
                {
                    Core.FileTo.WriteText(vm.ToJson(), fullPath, false);
                    vm.Log.Add($"写入文件：{fullPath}");
                }

                vm.Data = null;
                vm.Set(SharedEnum.RTag.success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 数据库导入
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="isClear">导入前清空</param>
        /// <returns></returns>
        public static SharedResultVM ImportDataBase(string fullPath, bool isClear = true)
        {
            var vm = new SharedResultVM();

            try
            {
                var jodb = Core.FileTo.ReadText(fullPath).ToJObject()["data"];

                using var db = ContextBaseFactory.CreateDbContext();
                var dicDbSet = GetDicDbSet(db);

                foreach (var table in dicDbSet.Keys)
                {
                    if (GlobalTo.TDB != SharedEnum.TypeDB.InMemory && isClear)
                    {
                        db.Database.ExecuteSqlRaw($"DELETE FROM {table}");
                    }

                    var dbset = dicDbSet[table];
                    var gt = dbset.GetType();
                    var ttype = Type.GetType(gt.FullName.Split("[[")[1].TrimEnd(']'));

                    var jarows = jodb[table] as JArray;
                    for (int i = 0; i < jarows?.Count; i++)
                    {
                        var mo = jarows[i].ToJson().ToType(ttype);
                        gt.GetMethod("Add").Invoke(dbset, new object[] { mo });
                    }
                }

                var num = db.SaveChanges();
                vm.Set(num > 0);
                vm.Data = num;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                vm.Set(ex);
            }

            return vm;
        }
    }
}