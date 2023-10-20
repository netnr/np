using Microsoft.AspNetCore.Authorization;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 输入输出
    /// </summary>
    [Authorize]
    [Route("[controller]/[action]")]
    public class IOController : Controller
    {
        public ContextBase db;
        public IOController(ContextBase cb)
        {
            db = cb;
        }

        #region 导出

        /// <summary>
        /// 公共导出
        /// </summary>
        /// <param name="ivm"></param>
        /// <param name="title">标题，文件名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> Export(QueryDataInputVM ivm, string title = "export")
        {
            var vm = new ResultVM();

            //虚拟路径
            string vpath = AppTo.GetValue("StaticResource:TmpDir");
            //物理路径
            var ppath = ParsingTo.Combine(AppTo.WebRootPath, vpath);
            if (!Directory.Exists(ppath))
            {
                Directory.CreateDirectory(ppath);
            }

            //文件名
            string filename = $"{title.Replace(" ", "").Trim()}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            //导出的表数据
            var dtReport = new DataTable();

            try
            {
                switch (ivm.TableName?.ToLower())
                {
                    default:
                        vm.Set(RCodeTypes.failure);
                        break;

                    //角色
                    case "sysrole":
                        {
                            using var ctl = new SettingController(db);
                            dtReport = ExportService.ModelsMapping(ivm, await ctl.QuerySysRole(ivm));
                        }
                        break;

                    //用户
                    case "sysuser":
                        {
                            using var ctl = new SettingController(db);
                            dtReport = ExportService.ModelsMapping(ivm, await ctl.QuerySysUser(ivm));
                        }
                        break;

                    //日志
                    case "syslog":
                        {
                            using var ctl = new SettingController(db);
                            dtReport = ExportService.ModelsMapping(ivm, await ctl.QuerySysLog(ivm));
                        }
                        break;

                    //字典
                    case "sysdictionary":
                        {
                            using var ctl = new SettingController(db);
                            dtReport = ExportService.ModelsMapping(ivm, await ctl.QuerySysDictionary(ivm));
                        }
                        break;
                }

                Console.WriteLine($"Export table rows : {dtReport.Rows.Count}");
                if (vm.Code != (int)RCodeTypes.failure)
                {
                    //生成
                    if (NpoiTo.DataTableToExcel(dtReport, ParsingTo.Combine(ppath, filename)))
                    {
                        vm.Data = ParsingTo.Combine(vpath, filename);

                        //生成的Excel继续操作
                        ExportService.ExcelDraw(ParsingTo.Combine(ppath, filename), ivm);

                        vm.Set(RCodeTypes.success);
                    }
                    else
                    {
                        vm.Set(RCodeTypes.failure);
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        #endregion
    }
}