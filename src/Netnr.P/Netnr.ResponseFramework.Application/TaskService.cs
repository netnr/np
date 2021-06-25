using System;
using System.IO;
using System.Linq;
using FluentScheduler;
using Netnr.ResponseFramework.Data;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.ResponseFramework.Application
{
    /// <summary>
    /// 任务
    /// 帮助文档：https://github.com/fluentscheduler/FluentScheduler
    /// </summary>
    public class TaskService
    {
        /// <summary>
        /// 任务注册
        /// </summary>
        public class Reg : Registry
        {
            /// <summary>
            /// 构造
            /// </summary>
            public Reg()
            {
                //每间隔一天在4:4 重置数据库
                _ = Schedule<ResetDataBaseJob>().ToRunEvery(1).Days().At(4, 4);
                //每间隔两天在3:3 清理临时目录
                _ = Schedule<ClearTmpJob>().ToRunEvery(2).Days().At(3, 3);
            }
        }

        /// <summary>
        /// 重置数据库
        /// </summary>
        public class ResetDataBaseJob : IJob
        {
            void IJob.Execute()
            {
                var vm = ContextBase.ImportDataBase(PathTo.Combine(GlobalTo.WebRootPath, "db/data.json"));
                ConsoleTo.Log(vm);
            }
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        public class ClearTmpJob : IJob
        {
            void IJob.Execute()
            {
                var vm = ClearTmp();
                ConsoleTo.Log(vm);
            }
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        /// <returns></returns>
        public static SharedResultVM ClearTmp()
        {
            var vm = new SharedResultVM();

            try
            {
                string directoryPath = PathTo.Combine(GlobalTo.WebRootPath, GlobalTo.GetValue("StaticResource:TmpDir"));

                vm.Log.Add($"清理临时目录：{directoryPath}");
                if (!Directory.Exists(directoryPath))
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "文件路径不存在";
                    return vm;
                }

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
                            File.Delete(path);
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
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
