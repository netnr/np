using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Netnr
{
    /// <summary>
    /// 锁
    /// </summary>
    public class LockTo
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        public static ConcurrentDictionary<string, object> DictLockObj { get; set; } = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 锁运行，不支持异步
        /// </summary>
        /// <param name="lockKey">锁标识</param>
        /// <param name="lockTimeout">锁超时，单位：毫秒</param>
        /// <param name="lockAction">锁成功执行</param>
        /// <param name="timeoutAction">锁超时执行</param>
        public static void Run(string lockKey, double lockTimeout, Action lockAction, Action timeoutAction = null)
        {
            var lockObj = DictLockObj.GetOrAdd(lockKey, new object());

            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(lockObj, TimeSpan.FromMilliseconds(lockTimeout), ref lockTaken);
                if (lockTaken)
                {
                    lockAction.Invoke();
                }
                else
                {
                    timeoutAction?.Invoke();
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(lockObj);
                }
            }
        }
    }
}
