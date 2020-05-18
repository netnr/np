using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentScheduler;

namespace Netnr.ResponseFramework.Application
{
    /// <summary>
    /// 任务
    /// </summary>
    public class TaskService
    {
        /// <summary>
        /// 清理临时文件
        /// </summary>
        /// <returns></returns>
        public static ActionResultVM ClearTemp()
        {
            var vm = new ActionResultVM();

            try
            {
                string directoryPath = Path.Combine(GlobalTo.WebRootPath, "upload/temp");

                var listLog = new List<string>();

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
                            listLog.Add("删除文件异常：" + ex.Message);
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
                        listLog.Add("删除文件夹异常：" + ex.Message);
                    }
                }

                listLog.Insert(0, $"删除文件{delFileCount}个，删除{delFolderCount}个文件夹");

                vm.Data = listLog;
                vm.Set(ARTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

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
                Schedule<ResetDataBaseJob>().ToRunEvery(1).Days().At(4, 4);
                //每间隔两天在3:3 清理临时目录
                Schedule<ClearTempJob>().ToRunEvery(2).Days().At(3, 3);
            }
        }

        /// <summary>
        /// 重置数据库
        /// </summary>
        public class ResetDataBaseJob : IJob
        {
            void IJob.Execute()
            {
                var vm = new DataMirrorService().AddForJson();
                Core.ConsoleTo.Log(vm);
            }
        }

        /// <summary>
        /// 清理临时目录
        /// </summary>
        public class ClearTempJob : IJob
        {
            void IJob.Execute()
            {
                var vm = ClearTemp();
                Core.ConsoleTo.Log(vm);
            }
        }

    }
}
