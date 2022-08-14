﻿using Microsoft.AspNetCore.Authorization;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 公共组件、视图
    /// </summary>
    [Authorize]
    [Route("[controller]/[action]")]
    public class InlayController : Controller
    {
        public ContextBase db;

        public InlayController(ContextBase cb)
        {
            db = cb;
        }

        #region 配置表格

        /// <summary>
        /// 查询配置表格
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public QueryDataOutputVM QueryConfigTable(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var query = db.SysTableConfig.Where(x => x.ColHide != 2);
            CommonService.QueryJoin(query, ivm, db, ref ovm);

            return ovm;
        }

        /// <summary>
        /// 保存配置表格
        /// </summary>
        /// <param name="rows">配置JSON</param>
        /// <param name="tablename">虚拟表名</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM SaveConfigTable(string rows, string tablename)
        {
            var vm = new ResultVM();

            var ja = rows.DeJson().EnumerateArray();

            var listRow = db.SysTableConfig.Where(x => x.TableName == tablename).ToList();

            int order = 0;
            foreach (var jt in ja)
            {
                string id = jt.GetValue("Id");

                var mo = listRow.FirstOrDefault(x => x.Id == id);

                mo.ColTitle = jt.GetValue("ColTitle");
                mo.ColAlign = jt.GetValue<int?>("ColAlign") ?? 1;
                mo.ColWidth = jt.GetValue<int>("ColWidth");
                mo.ColHide = jt.GetValue<int>("ColHide");
                mo.ColFrozen = jt.GetValue<int>("ColFrozen");
                mo.ColExport = jt.GetValue<int>("ColExport");

                mo.ColOrder = order++;
            }

            db.SysTableConfig.UpdateRange(listRow);

            int num = db.SaveChanges();
            vm.Set(num > 0);

            return vm;
        }

        #endregion

        #region 配置表单

        /// <summary>
        /// 保存配置表单
        /// </summary>
        /// <param name="rows">配置JSON</param>
        /// <param name="tablename">虚拟表名</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM SaveConfigForm(string rows, string tablename)
        {
            var vm = new ResultVM();

            var ja = rows.DeJson().EnumerateArray();

            var listRow = db.SysTableConfig.Where(x => x.TableName == tablename).ToList();

            int order = 0;
            foreach (var jt in ja)
            {
                string field = jt.GetValue("field");
                var mo = listRow.FirstOrDefault(x => x.ColField == field);

                mo.ColTitle = jt.GetValue("title");
                mo.FormSpan = jt.GetValue<int?>("span") ?? 1;
                mo.FormSpan = jt.GetValue<int>("FormArea");
                mo.FormOrder = order++;
            }

            db.SysTableConfig.UpdateRange(listRow);

            int num = db.SaveChanges();
            vm.Set(num > 0);

            return vm;
        }

        #endregion
    }
}