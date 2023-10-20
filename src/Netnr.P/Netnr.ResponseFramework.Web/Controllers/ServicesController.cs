using Microsoft.EntityFrameworkCore;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 服务
    /// </summary>
    [Route("[controller]/[action]")]
    public class ServicesController : Controller
    {
        public ContextBase db;
        public ServicesController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 数据库重置
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public async Task<ResultVM> DatabaseReset(string zipName = "static/sample.zip")
        {
            var vm = new ResultVM();

            try
            {
                if (HttpContext != null && new UAParsers(HttpContext.Request.Headers.UserAgent).GetBot() != null)
                {
                    vm.Set(RCodeTypes.refuse);
                    vm.Msg = "are you human？";
                }
                else
                {
                    var idb = new DataKitTransfer.ImportDatabase
                    {
                        WriteConnectionInfo = new DbKitConnectionOption
                        {
                            ConnectionType = AppTo.DBT,
                            Connection = db.Database.GetDbConnection()
                        },
                        PackagePath = ParsingTo.Combine(AppTo.ContentRootPath, zipName),
                        WriteDeleteData = true
                    };

                    vm = await DataKitTo.ImportDatabase(idb);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 数据库导出（开发环境）
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public async Task<ResultVM> DatabaseExport(string zipName = "static/sample.zip")
        {
            var vm = new ResultVM();

            try
            {
                if (BaseTo.IsDev)
                {
                    var edb = new DataKitTransfer.ExportDatabase
                    {
                        PackagePath = Path.Combine(AppTo.ContentRootPath, zipName),
                        ReadConnectionInfo = new DbKitConnectionOption
                        {
                            ConnectionType = AppTo.DBT,
                            Connection = db.Database.GetDbConnection()
                        },
                        ExportType = "onlyData"
                    };

                    vm = await DataKitTo.ExportDataTable(await edb.AsExportDataTable());
                }
                else
                {
                    vm.Set(RCodeTypes.refuse);
                    vm.Msg = "仅限开发环境使用";
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public ResultVM ClearTmp()
        {
            var vm = new ResultVM();

            try
            {
                string directoryPath = ParsingTo.Combine(AppTo.WebRootPath, AppTo.GetValue("StaticResource:TmpDir"));

                vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 清理临时目录：{directoryPath}");

                if (!Directory.Exists(directoryPath))
                {
                    vm.Set(RCodeTypes.failure);
                    vm.Msg = "文件路径不存在";
                }
                else
                {
                    int delFileCount = 0;
                    int delFolderCount = 0;

                    //删除文件
                    Directory.EnumerateFiles(directoryPath).ForEach(file =>
                    {
                        if (!file.Contains("README"))
                        {
                            try
                            {
                                System.IO.File.Delete(file);
                                delFileCount++;
                            }
                            catch (Exception ex)
                            {
                                vm.Log.Add($"删除文件异常：{ex.Message}");
                            }
                        }
                    });

                    //删除文件夹
                    Directory.EnumerateDirectories(directoryPath).ForEach(dir =>
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                            delFolderCount++;
                        }
                        catch (Exception ex)
                        {
                            vm.Log.Add($"删除文件夹异常：{ex.Message}");
                        }
                    });

                    vm.Log.Insert(0, $"删除文件{delFileCount}个，删除{delFolderCount}个文件夹");
                    vm.Set(RCodeTypes.success);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}