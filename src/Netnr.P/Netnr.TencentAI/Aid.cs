using Netnr.TencentAI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Netnr.TencentAI
{
    /// <summary>
    /// 
    /// </summary>
    public class Aid
    {
        /// <summary>
        /// APPID
        /// </summary>
        [Description("APPID")]
        public static int APPID { get; set; }

        /// <summary>
        /// APPKEY
        /// </summary>
        [Description("APPKEY")]
        public static string APPKEY { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="pairs">参数字典</param>
        /// <param name="appkey">APPKEY</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        [Description("签名")]
        public static string Sign(Dictionary<string, string> pairs, string appkey, string charset = "utf-8")
        {
            var dic = pairs.OrderBy(x => x.Key);

            var pair = "";
            foreach (var kv in dic)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    pair += kv.Key + "=" + Encode(kv.Value, charset) + "&";
                }
            }

            pair += "app_key=" + appkey;

            var sign = MD5(pair).ToUpper();

            return sign;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">参数实体</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        [Description("签名")]
        public static string Sign<T>(T model, string charset = "utf-8") where T : class, new()
        {
            var m = model.GetType();
            var pis = m.GetProperties();

            var pair = new Dictionary<string, string>();

            foreach (var pi in pis)
            {
                if (pi.Name.ToLower() != "sign")
                {
                    object val = pi.GetValue(model, null);
                    pair.Add(pi.Name, val?.ToString() ?? "");
                }
            }

            var sign = Sign(pair, APPKEY, charset);

            return sign;
        }

        /// <summary>
        /// 签名编码，编码格式需要转成大写
        /// </summary>
        /// <param name="uri">字符串</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        [Description("签名编码，编码格式需要转成大写")]
        public static string Encode(string uri, string charset = "utf-8")
        {
            string URL_ALLOWED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            if (string.IsNullOrEmpty(uri))
                return string.Empty;

            const string escapeFlag = "%";
            var encodedUri = new StringBuilder(uri.Length * 2);
            var bytes = Encoding.GetEncoding(charset).GetBytes(uri);
            foreach (var b in bytes)
            {
                char ch = (char)b;
                if (URL_ALLOWED_CHARS.IndexOf(ch) != -1)
                    encodedUri.Append(ch);
                else
                {
                    encodedUri.Append(escapeFlag).Append(string.Format(CultureInfo.InstalledUICulture, "{0:X2}", (int)b).ToUpper());
                }
            }

            return encodedUri.ToString();
        }

        /// <summary>
        /// 验证实体值是否有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="err">错误项</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        [Description("验证实体值是否有效")]
        public static bool IsValid<T>(T entity, ref List<string> err, string charset = "utf-8") where T : class, new()
        {
            bool b = true;

            var gt = entity.GetType();
            var pis = gt.GetProperties();

            var reqName = typeof(Required).Name;

            foreach (var pi in pis)
            {
                string value = pi.GetValue(entity, null)?.ToString();

                if (string.IsNullOrWhiteSpace(value) || value == "0")
                {
                    switch (pi.Name)
                    {
                        //赋值 APPID
                        case "app_id":
                            pi.SetValue(entity, APPID, null);
                            break;

                        //赋值 当前时间戳
                        case "time_stamp":
                            pi.SetValue(entity, (int)DateTime.Now.ToTimestamp(), null);
                            break;

                        //赋值 随机字符串
                        case "nonce_str":
                            pi.SetValue(entity, Guid.NewGuid().ToString("N"), null);
                            break;
                    }
                }

                //是必填
                if (pi.Name != "sign")
                {
                    var isReq = false;
                    object[] attrs = pi.GetCustomAttributes(true);
                    foreach (var attr in attrs)
                    {
                        var agt = attr.GetType();
                        if (agt.Name == reqName)
                        {
                            isReq = true;
                            break;
                        }
                    }
                    if (isReq && string.IsNullOrEmpty(pi.GetValue(entity, null)?.ToString()))
                    {
                        b = false;
                        err.Add(pi.Name);
                    }
                }
            }

            //赋值签名
            gt.GetProperty("sign").SetValue(entity, Sign(entity, charset), null);

            return b;
        }

        /// <summary>
        /// 验证实体值是否有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        [Description("验证实体值是否有效")]
        public static bool IsValid<T>(T entity, string charset = "utf-8") where T : class, new()
        {
            var err = new List<string>();
            return IsValid(entity, ref err, charset);
        }

        /// <summary>
        /// 验证实体值失败返回信息
        /// </summary>
        /// <returns></returns>
        [Description("验证实体值失败返回信息")]
        public static string ValidFail()
        {
            return "{\"ret\":4096,\"msg\":\"paramter invalid\",\"data\":{}}";
        }

        /// <summary>
        /// 公用请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="uri">接口</param>
        /// <param name="type">请求类型</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        [Description("公用请求")]
        public static string Request<T>(T entity, string uri, string type = "POST", string charset = "utf-8") where T : class, new()
        {
            if (IsValid(entity, charset))
            {
                string result;
                if (type == "POST")
                {
                    result = Core.HttpTo.Post(uri, Parameter(entity, charset), charset);
                }
                else
                {
                    result = Core.HttpTo.Get(uri + "?" + Parameter(entity, charset), charset);
                }
                return result;
            }
            return ValidFail();
        }

        /// <summary>
        /// 实体转Pars
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        [Description("实体转Pars")]
        public static string Parameter<T>(T entity, string charset = "utf-8") where T : class, new()
        {
            string result = string.Empty;
            var pis = entity.GetType().GetProperties();
            foreach (var pi in pis)
            {
                string value = pi.GetValue(entity, null)?.ToString();
                if (value != null)
                {
                    result += "&" + pi.Name + "=" + value.ToEncode(charset);
                }
            }
            return result.TrimStart('&');
        }

        /// <summary>
        /// MD5加密（小写）
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        [Description("MD5加密（小写）")]
        public static string MD5(string s, int len = 32)
        {
            var md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(s));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            string result = sb.ToString();
            return len == 32 ? result : result.Substring(8, 16);
        }
    }
}