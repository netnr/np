using FluentScheduler;
using Netnr.Core;

namespace Netnr.Blog.Web.Apps
{
    /// <summary>
    /// 定时任务
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
                //Gist同步任务，每 2 小时一次
                Schedule<GistSyncJob>().ToRunEvery(2).Hours();

                //处理操作记录，每 30 分钟一次
                Schedule<HandleOperationRecordJob>().ToRunEvery(30).Minutes();

                //数据库备份到Git，每天 16:16 一次
                Schedule<DatabaseBackupToGitJob>().ToRunEvery(1).Days().At(16, 16);
            }
        }

        /// <summary>
        /// Gist同步任务
        /// </summary>
        public class GistSyncJob : IJob
        {
            void IJob.Execute()
            {
                var vm = new Controllers.ServicesController().GistSync();
                ConsoleTo.Log(vm.ToJson(true));
                Console.WriteLine(vm);
            }
        }

        /// <summary>
        /// 处理操作记录
        /// </summary>
        public class HandleOperationRecordJob : IJob
        {
            void IJob.Execute()
            {
                var vm = new Controllers.ServicesController().HandleOperationRecord();
                ConsoleTo.Log(vm.ToJson(true));
                Console.WriteLine(vm);
            }
        }

        /// <summary>
        /// 数据库备份到Git
        /// </summary>
        public class DatabaseBackupToGitJob : IJob
        {
            void IJob.Execute()
            {
                var vm = new Controllers.ServicesController().DatabaseBackupToGit();
                ConsoleTo.Log(vm.ToJson(true));
                Console.WriteLine(vm);
            }
        }
    }
}
