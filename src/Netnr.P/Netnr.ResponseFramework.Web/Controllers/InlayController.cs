using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netnr.ResponseFramework.Data;
using Newtonsoft.Json.Linq;

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
            Application.CommonService.QueryJoin(query, ivm, db, ref ovm);

            return ovm;
        }

        /// <summary>
        /// 保存配置表格
        /// </summary>
        /// <param name="rows">配置JSON</param>
        /// <param name="tablename">虚拟表名</param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM SaveConfigTable(string rows, string tablename)
        {
            var vm = new SharedResultVM();

            JArray ja = JArray.Parse(rows);

            var listRow = db.SysTableConfig.Where(x => x.TableName == tablename).ToList();

            int order = 0;
            foreach (JToken jt in ja)
            {
                string id = jt["Id"].ToString();

                var mo = listRow.FirstOrDefault(x => x.Id == id);

                mo.ColTitle = jt["ColTitle"].ToStringOrEmpty();
                mo.ColAlign = string.IsNullOrWhiteSpace(jt["ColAlign"].ToStringOrEmpty()) ? 1 : Convert.ToInt32(jt["ColAlign"].ToStringOrEmpty());
                mo.ColWidth = string.IsNullOrWhiteSpace(jt["ColWidth"].ToStringOrEmpty()) ? 0 : Convert.ToInt32(jt["ColWidth"].ToStringOrEmpty());
                mo.ColHide = string.IsNullOrWhiteSpace(jt["ColHide"].ToStringOrEmpty()) ? 0 : Convert.ToInt32(jt["ColHide"].ToStringOrEmpty());
                mo.ColFrozen = string.IsNullOrWhiteSpace(jt["ColFrozen"].ToStringOrEmpty()) ? 0 : Convert.ToInt32(jt["ColFrozen"].ToStringOrEmpty());
                mo.ColExport = string.IsNullOrWhiteSpace(jt["ColExport"].ToStringOrEmpty()) ? 0 : Convert.ToInt32(jt["ColExport"].ToStringOrEmpty());
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
        public SharedResultVM SaveConfigForm(string rows, string tablename)
        {
            var vm = new SharedResultVM();

            JArray ja = JArray.Parse(rows);

            var listRow = db.SysTableConfig.Where(x => x.TableName == tablename).ToList();

            int order = 0;
            foreach (JToken jt in ja)
            {
                string field = jt["field"].ToStringOrEmpty();
                var mo = listRow.FirstOrDefault(x => x.ColField == field);

                mo.ColTitle = jt["title"].ToStringOrEmpty();
                mo.FormSpan = string.IsNullOrWhiteSpace(jt["span"].ToStringOrEmpty()) ? 1 : Convert.ToInt32(jt["span"].ToStringOrEmpty());
                mo.FormArea = string.IsNullOrWhiteSpace(jt["area"].ToStringOrEmpty()) ? 0 : Convert.ToInt32(jt["area"].ToStringOrEmpty());
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