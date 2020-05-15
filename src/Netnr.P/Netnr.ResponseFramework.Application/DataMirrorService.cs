using Microsoft.EntityFrameworkCore;
using Netnr.ResponseFramework.Data;
using Netnr.ResponseFramework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Netnr.ResponseFramework.Application
{
    /// <summary>
    /// - 数据库数据存储为JSON（备份数据）
    /// - 根据JSON反序列化导入（重置数据）
    /// 
    /// 说明：从执行SQL脚本重置 改为 EF清理表插入数据的模式，更好维护，而且兼容了内存数据库
    /// </summary>
    public class DataMirrorService
    {
        public ContextBase db;

        /// <summary>
        /// JSON存储路径
        /// </summary>
        public string JsonPath = GlobalTo.WebRootPath + "/scripts/table-json/";
        public string JsonName = "data.json";

        public virtual DbSet<SysButton> Tsb { get; set; }
        public virtual DbSet<SysDictionary> Tsd { get; set; }
        public virtual DbSet<SysMenu> Tsm { get; set; }
        public virtual DbSet<SysRole> Tsr { get; set; }
        public virtual DbSet<SysTableConfig> Tstc { get; set; }
        public virtual DbSet<SysUser> Tsu { get; set; }
        public virtual DbSet<TempExample> Tte { get; set; }
        public virtual DbSet<TempInvoiceDetail> Ttid { get; set; }
        public virtual DbSet<TempInvoiceMain> Ttim { get; set; }

        public DataMirrorService()
        {
            db = new ContextBase(ContextBase.DCOB().Options);

            Tsb = db.SysButton;
            Tsd = db.SysDictionary;
            Tsm = db.SysMenu;
            Tsr = db.SysRole;
            Tstc = db.SysTableConfig;
            Tsu = db.SysUser;
            Tte = db.TempExample;
            Ttid = db.TempInvoiceDetail;
            Ttim = db.TempInvoiceMain;
        }

        /// <summary>
        /// 保存为JSON文件
        /// </summary>
        /// <param name="CoverJson">写入JSON文件，默认 false</param>
        /// <returns></returns>
        public ActionResultVM SaveAsJson(bool CoverJson = false)
        {
            var vm = new ActionResultVM();

            var dicKey = new Dictionary<string, object>
            {
                { "SysButton", Tsb.ToList() },
                { "SysDictionary", Tsd.ToList() },
                { "SysMenu", Tsm.ToList() },
                { "SysRole", Tsr.ToList() },
                { "SysTableConfig", Tstc.ToList() },
                { "SysUser", Tsu.ToList() },

                { "TempExample", Tte.ToList() },
                { "TempInvoiceDetail", Ttid.ToList() },
                { "TempInvoiceMain", Ttim.ToList() }
            };

            vm.Data = dicKey;

            if (CoverJson)
            {
                Core.FileTo.WriteText(vm.ToJson(), JsonPath, JsonName, false);
            }

            vm.Set(ARTag.success);

            return vm;
        }

        /// <summary>
        /// 根据JSON写入数据
        /// </summary>
        /// <param name="isClear">是否清理表，默认清理</param>
        /// <returns></returns>
        public ActionResultVM AddForJson(bool isClear = true)
        {
            var vm = new ActionResultVM();

            try
            {
                var json = Core.FileTo.ReadText(JsonPath, JsonName);

                var objs = json.ToJObject()["data"];

                var jsb = objs["SysButton"].ToString().ToEntitys<SysButton>();
                var jsd = objs["SysDictionary"].ToString().ToEntitys<SysDictionary>();
                var jsm = objs["SysMenu"].ToString().ToEntitys<SysMenu>();
                var jsr = objs["SysRole"].ToString().ToEntitys<SysRole>();
                var jstc = objs["SysTableConfig"].ToString().ToEntitys<SysTableConfig>();
                var jsu = objs["SysUser"].ToString().ToEntitys<SysUser>();
                var jte = objs["TempExample"].ToString().ToEntitys<TempExample>();
                var jtid = objs["TempInvoiceDetail"].ToString().ToEntitys<TempInvoiceDetail>();
                var jtim = objs["TempInvoiceMain"].ToString().ToEntitys<TempInvoiceMain>();

                Tsb = db.SysButton;
                Tsd = db.SysDictionary;
                Tsm = db.SysMenu;
                Tsr = db.SysRole;
                Tstc = db.SysTableConfig;
                Tsu = db.SysUser;
                Tte = db.TempExample;
                Ttid = db.TempInvoiceDetail;
                Ttim = db.TempInvoiceMain;

                var num = 0;
                if (isClear)
                {
                    Tsb.RemoveRange(Tsb.ToList());
                    Tsd.RemoveRange(Tsd.ToList());
                    Tsm.RemoveRange(Tsm.ToList());
                    Tsr.RemoveRange(Tsr.ToList());
                    Tstc.RemoveRange(Tstc.ToList());
                    Tsu.RemoveRange(Tsu.ToList());

                    Tte.RemoveRange(Tte.ToList());
                    Ttid.RemoveRange(Ttid.ToList());
                    Ttim.RemoveRange(Ttim.ToList());

                    num = db.SaveChanges();
                }

                Tsb.AddRange(jsb);
                Tsd.AddRange(jsd);
                Tsm.AddRange(jsm);
                Tsr.AddRange(jsr);
                Tstc.AddRange(jstc);
                Tsu.AddRange(jsu);

                Tte.AddRange(jte);
                Ttid.AddRange(jtid);
                Ttim.AddRange(jtim);

                db.AddRange(jsb);

                num += db.SaveChanges();

                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
