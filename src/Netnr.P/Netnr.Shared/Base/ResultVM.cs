#if Full || Base

namespace Netnr;

/// <summary>
/// 通用请求方法返回对象
/// </summary>
public class ResultVM
{
    readonly Stopwatch sw;
    private double et = 0;

    /// <summary>
    /// 构造
    /// </summary>
    public ResultVM()
    {
        sw = Stopwatch.StartNew();
    }

    /// <summary>
    /// 错误码，200 表示成功，-1 表示异常，其它自定义建议从 1 开始累加
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; } = 0;

    /// <summary>
    /// 消息
    /// </summary>
    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    /// <summary>
    /// 主体数据
    /// </summary>
    [JsonPropertyName("data")]
    public object Data { get; set; }

    /// <summary>
    /// 日志
    /// </summary>
    [JsonPropertyName("log")]
    public ObservableCollection<object> Log { get; set; } = new ObservableCollection<object>();

    /// <summary>
    /// 日志事件
    /// </summary>
    /// <param name="le"></param>
    public void LogEvent(Action<NotifyCollectionChangedEventArgs> le)
    {
        if (le != null)
        {
            Log.CollectionChanged += new NotifyCollectionChangedEventHandler(delegate (object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    le(e);
                }
            });
        }
    }

    /// <summary>
    /// 用时，毫秒
    /// </summary>
    [JsonPropertyName("useTime")]
    public double UseTime
    {
        get
        {
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }
    }

    /// <summary>
    /// 用时，可视化
    /// </summary>
    [JsonPropertyName("useTimeFormat")]
    public string UseTimeFormat
    {
        get
        {
            sw.Stop();
            return sw.Elapsed.ToString(@"hh\:mm\:ss\.fff");
        }
    }

    /// <summary>
    /// 片段耗时，毫秒
    /// </summary>
    /// <returns></returns>
    public double PartTime()
    {
        var pt = sw.Elapsed.TotalMilliseconds - et;
        et = sw.Elapsed.TotalMilliseconds;
        return pt;
    }

    /// <summary>
    /// 片段耗时，毫秒，可视化
    /// </summary>
    /// <param name="format">格式化</param>
    /// <returns></returns>
    public string PartTimeFormat(string format = @"hh\:mm\:ss\.fff")
    {
        var pt = sw.Elapsed.TotalMilliseconds - et;
        et = sw.Elapsed.TotalMilliseconds;
        return TimeSpan.FromMilliseconds(pt).ToString(format);
    }

    /// <summary>
    /// 设置快捷标签，赋值code、msg
    /// </summary>
    /// <param name="tag">快捷标签枚举</param>
    public void Set(EnumTo.RTag tag)
    {
        Code = Convert.ToInt32(tag);
        Msg = tag.ToString();
    }

    /// <summary>
    /// 设置快捷标签，赋值code、msg
    /// </summary>
    /// <param name="isSuccess"></param>
    public void Set(bool isSuccess)
    {
        if (isSuccess)
        {
            Set(EnumTo.RTag.success);
        }
        else
        {
            Set(EnumTo.RTag.failure);
        }
    }

    /// <summary>
    /// 设置快捷标签，赋值code、msg
    /// </summary>
    /// <param name="ex"></param>
    public void Set(Exception ex)
    {
        ConsoleTo.Title("Exception", ex);
        Set(EnumTo.RTag.exception);
        Msg = ex.ToJson();
    }

    /// <summary>
    /// 通用的异常处理
    /// </summary>
    /// <param name="resTry"></param>
    /// <returns></returns>
    public static async Task<ResultVM> Try(Func<ResultVM, Task<ResultVM>> resTry)
    {
        var vm = new ResultVM();

        try
        {
            vm = await resTry.Invoke(vm);
        }
        catch (Exception ex)
        {
            vm.Set(ex);
        }

        return vm;
    }
}

#endif