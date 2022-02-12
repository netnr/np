using Netnr.SharedUserAgent;
using Netnr.SharedApp;
using Netnr.SharedFast;
using Netnr.Core;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 服务
    /// </summary>
    [Route("[controller]/[action]")]
    public class ServicesController : Controller
    {
        /// <summary>
        /// 服务
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return Ok("Normal Service!");
        }

        /// <summary>
        /// 数据库重置
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public SharedResultVM DatabaseReset(string zipName = "db/backup.zip")
        {
            return SharedResultVM.Try(vm =>
            {
                if (HttpContext != null && new UserAgentTo(new ClientTo(HttpContext).UserAgent).IsBot)
                {
                    vm.Set(SharedEnum.RTag.refuse);
                    vm.Msg = "are you human？";
                }
                else
                {
                    var idb = new SharedDataKit.TransferVM.ImportDatabase
                    {
                        WriteConnectionInfo = new SharedDataKit.TransferVM.ConnectionInfo
                        {
                            ConnectionType = GlobalTo.TDB,
                            ConnectionString = SharedDbContext.FactoryTo.GetConn().Replace("Filename=", "Data Source=")
                        },
                        ZipPath = PathTo.Combine(GlobalTo.ContentRootPath, zipName),
                        WriteDeleteData = true
                    };

                    vm = SharedDataKit.DataKit.ImportDatabase(idb);
                }

                return vm;
            });
        }

        /// <summary>
        /// 数据库导出
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public SharedResultVM DatabaseExport(string zipName = "db/backup.zip")
        {
            return SharedResultVM.Try(vm =>
            {
                //是否覆盖备份，默认不覆盖，避免线上重置功能被破坏
                var CoverBack = false;

                if (CoverBack)
                {
                    var edb = new SharedDataKit.TransferVM.ExportDatabase
                    {
                        ZipPath = PathTo.Combine(GlobalTo.ContentRootPath, zipName),
                        ReadConnectionInfo = new SharedDataKit.TransferVM.ConnectionInfo()
                        {
                            ConnectionString = SharedDbContext.FactoryTo.GetConn().Replace("Filename=", "Data Source="),
                            ConnectionType = GlobalTo.TDB
                        }
                    };

                    vm = SharedDataKit.DataKit.ExportDatabase(edb);
                }
                else
                {
                    vm.Set(SharedEnum.RTag.refuse);
                    vm.Msg = "已被限制导出覆盖";
                }

                return vm;
            });
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public SharedResultVM ClearTmp()
        {
            return SharedResultVM.Try(vm =>
            {
                string directoryPath = PathTo.Combine(GlobalTo.WebRootPath, GlobalTo.GetValue("StaticResource:TmpDir"));

                vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 清理临时目录：{directoryPath}");

                if (!Directory.Exists(directoryPath))
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "文件路径不存在";
                }
                else
                {
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
                                System.IO.File.Delete(path);
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

                return vm;
            });
        }
    }
}