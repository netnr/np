#if Full || Public

using System;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Generic;

namespace Netnr
{
    /// <summary>
    /// 通用请求方法返回对象
    /// </summary>
    public class SharedResultVM
    {
        private readonly Stopwatch sw;
        private double et = 0;

        /// <summary>
        /// 构造
        /// </summary>
        public SharedResultVM()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        /// <summary>
        /// 错误码，200 表示成功，-1 表示异常，其它自定义建议从 1 开始累加
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; } = 0;

        /// <summary>
        /// 消息
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }

        /// <summary>
        /// 主体数据
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        [JsonProperty("log")]
        public List<object> Log { get; set; } = new List<object>();

        /// <summary>
        /// 用时，毫秒
        /// </summary>
        [JsonProperty("useTime")]
        public double UseTime
        {
            get
            {
                return sw.Elapsed.TotalMilliseconds;
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
        /// 设置快捷标签，赋值code、msg
        /// </summary>
        /// <param name="tag">快捷标签枚举</param>
        public void Set(SharedEnum.RTag tag)
        {
            Code = Convert.ToInt32(tag);
            Msg = tag.ToString();
        }

        /// <summary>
        /// 设置快捷标签，赋值code、msg
        /// </summary>
        /// <param name="isyes"></param>
        public void Set(bool isyes)
        {
            if (isyes)
            {
                Set(SharedEnum.RTag.success);
            }
            else
            {
                Set(SharedEnum.RTag.fail);
            }
        }

        /// <summary>
        /// 设置快捷标签，赋值code、msg
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="showException">显示异常信息，默认true</param>
        public void Set(Exception ex, bool showException = true)
        {
            Set(SharedEnum.RTag.exception);
            if (showException)
            {
                Msg = ex.Message;
            }
        }
    }
}

#endif