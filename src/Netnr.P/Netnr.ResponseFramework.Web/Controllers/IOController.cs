using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netnr.ResponseFramework.Data;
using Netnr.ResponseFramework.Application;
using Netnr.Core;
using Netnr.SharedFast;
using Netnr.SharedApp;
using Netnr.SharedNpoi;

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
        public SharedResultVM Export(QueryDataInputVM ivm, string title = "export")
        {
            var vm = new SharedResultVM();

            //虚拟路径
            string vpath = GlobalTo.GetValue("StaticResource:TmpDir");
            //物理路径
            var ppath = PathTo.Combine(GlobalTo.WebRootPath, vpath);
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
                        vm.Set(SharedEnum.RTag.invalid);
                        break;

                    //角色
                    case "sysrole":
                        {
                            using var ctl = new SettingController(db);
                            dtReport = ExportService.ModelsMapping(ivm, ctl.QuerySysRole(ivm));
                        }
                        break;

                    //用户
                    case "sysuser":
                        {
                            using var ctl = new SettingController(db);
                            dtReport = ExportService.ModelsMapping(ivm, ctl.QuerySysUser(ivm));
                        }
                        break;

                    //日志
                    case "syslog":
                        {
                            using var ctl = new SettingController(db);
                            dtReport = ExportService.ModelsMapping(ivm, ctl.QuerySysLog(ivm));
                        }
                        break;

                    //字典
                    case "sysdictionary":
                        {
                            using var ctl = new SettingController(db);
                            dtReport = ExportService.ModelsMapping(ivm, ctl.QuerySysDictionary(ivm));
                        }
                        break;
                }

                Console.WriteLine($"Export table rows : {dtReport.Rows.Count}");
                if (vm.Msg != SharedEnum.RTag.invalid.ToString())
                {
                    //生成
                    if (NpoiTo.DataTableToExcel(dtReport, PathTo.Combine(ppath, filename)))
                    {
                        vm.Data = PathTo.Combine(vpath, filename);

                        //生成的Excel继续操作
                        ExportService.ExcelDraw(PathTo.Combine(ppath, filename), ivm);

                        vm.Set(SharedEnum.RTag.success);
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.fail);
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 导出下载
        /// </summary>
        /// <param name="path">下载文件路径</param>
        [HttpGet]
        public void ExportDown(string path)
        {
            path = GlobalTo.ContentRootPath + path;
            new DownTo(Response).Stream(path, "");
        }

        #endregion
    }
}