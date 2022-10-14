namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 服务
    /// </summary>
    [Route("[controller]/[action]")]
    public class ServicesController : Controller
    {
        /// <summary>
        /// 数据库重置
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public ResultVM DatabaseReset(string zipName = "db/backup.zip") => ResultVM.Try(vm =>
        {
            if (HttpContext != null && new UAParser.Parsers(HttpContext.Request.Headers.UserAgent).GetBot() != null)
            {
                vm.Set(EnumTo.RTag.refuse);
                vm.Msg = "are you human？";
            }
            else
            {
                var idb = new DataKitTransferVM.ImportDatabase
                {
                    WriteConnectionInfo = new DataKitTransferVM.ConnectionInfo
                    {
                        ConnectionType = AppTo.TDB,
                        ConnectionString = DbContextTo.GetConn().Replace("Filename=", "Data Source=")
                    },
                    PackagePath = PathTo.Combine(AppTo.ContentRootPath, zipName),
                    WriteDeleteData = true
                };

                vm = DataKitTo.ImportDatabase(idb);
            }

            return vm;
        });

        /// <summary>
        /// 数据库导出（开发环境）
        /// </summary>
        /// <param name="zipName">文件名</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public ResultVM DatabaseExport(string zipName = "db/backup.zip") => ResultVM.Try(vm =>
        {
            if (GlobalTo.IsDev)
            {
                var edb = new DataKitTransferVM.ExportDatabase
                {
                    PackagePath = Path.Combine(AppTo.ContentRootPath, zipName),
                    ReadConnectionInfo = new DataKitTransferVM.ConnectionInfo()
                    {
                        ConnectionString = DbContextTo.GetConn().Replace("Filename=", "Data Source="),
                        ConnectionType = AppTo.TDB
                    }
                };

                vm = DataKitTo.ExportDatabase(edb);
            }
            else
            {
                vm.Set(EnumTo.RTag.refuse);
                vm.Msg = "仅限开发环境使用";
            }

            return vm;
        });

        /// <summary>
        /// 清理临时目录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public ResultVM ClearTmp()
        {
            return ResultVM.Try(vm =>
            {
                string directoryPath = PathTo.Combine(AppTo.WebRootPath, AppTo.GetValue("StaticResource:TmpDir"));

                vm.Log.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 清理临时目录：{directoryPath}");

                if (!Directory.Exists(directoryPath))
                {
                    vm.Set(EnumTo.RTag.lack);
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
                    vm.Set(EnumTo.RTag.success);
                }

                return vm;
            });
        }
    }
}