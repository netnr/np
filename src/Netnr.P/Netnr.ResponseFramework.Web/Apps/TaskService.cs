using FluentScheduler;
using Netnr.Core;

namespace Netnr.ResponseFramework.Web.Apps
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
                //每间隔2天在4:4 重置数据库
                _ = Schedule<DatabaseResetJob>().ToRunEvery(2).Days().At(4, 4);

                //每间隔2天在3:3 清理临时目录
                _ = Schedule<ClearTmpJob>().ToRunEvery(2).Days().At(3, 3);
            }
        }

        /// <summary>
        /// 重置数据库
        /// </summary>
        public class DatabaseResetJob : IJob
        {
            void IJob.Execute()
            {
                var vm = new Controllers.ServicesController().DatabaseReset();
                Console.WriteLine(vm.ToJson(true));
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
                var vm = new Controllers.ServicesController().ClearTmp();
                Console.WriteLine(vm.ToJson(true));
                ConsoleTo.Log(vm);
            }
        }
    }
}
