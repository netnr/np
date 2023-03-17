using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
        public async Task<QueryDataOutputVM> QueryConfigTable(QueryDataInputVM ivm)
        {
            var query = db.SysTableConfig.Where(x => x.ColHide != 2);
            var ovm = await CommonService.QueryJoin(query, ivm, db);

            return ovm;
        }

        /// <summary>
        /// 保存配置表格
        /// </summary>
        /// <param name="rows">配置JSON</param>
        /// <param name="tablename">虚拟表名</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> SaveConfigTable(string rows, string tablename)
        {
            var vm = new ResultVM();

            var ja = rows.DeJson().EnumerateArray();

            var listRow = await db.SysTableConfig.Where(x => x.TableName == tablename).ToListAsync();

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

            int num = await db.SaveChangesAsync();
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
        public async Task<ResultVM> SaveConfigForm(string rows, string tablename)
        {
            var vm = new ResultVM();

            var ja = rows.DeJson().EnumerateArray();

            var listRow = await db.SysTableConfig.Where(x => x.TableName == tablename).ToListAsync();

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

            int num = await db.SaveChangesAsync();
            vm.Set(num > 0);

            return vm;
        }

        #endregion
    }
}