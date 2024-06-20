#if Full || Core

using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Netnr;

/// <summary>
/// 队列
/// </summary>
public static partial class QueueTo
{
    /// <summary>
    /// 队列
    /// </summary>
    public static ConcurrentQueue<Func<Task>> QueueAction { get; set; } = [];

    /// <summary>
    /// 标记
    /// </summary>
    static int SafeMark = 0;

    /// <summary>
    /// 消费
    /// </summary>
    public static void QueueRun()
    {
        if (SafeMark == 0)
        {
            Interlocked.Exchange(ref SafeMark, 1);

            Task.Run(async () =>
            {
                while (QueueAction.TryDequeue(out var action))
                {
                    try
                    {
                        await action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        ConsoleTo.LogError(ex, nameof(QueueTo));
                    }

                    ConsoleTo.WriteCard($"{nameof(QueueTo)} count {QueueAction.Count}");
                    await Task.Delay(0);
                }

                Interlocked.Exchange(ref SafeMark, 0);
            });
        }
    }

    /// <summary>
    /// 添加作业，已处理异常
    /// </summary>
    /// <param name="workItem"></param>
    /// <param name="workRemark">作业备注，添加日志输出</param>
    public static void AddWork(Func<Task> workItem, string workRemark = null)
    {
        QueueAction.Enqueue(workItem);
        if (!string.IsNullOrEmpty(workRemark))
        {
            ConsoleTo.WriteCard($"{nameof(QueueTo)}-{nameof(AddWork)}", workRemark);
        }

        QueueRun();
    }
}

#endif