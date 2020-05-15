using System;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace Netnr
{
    /// <summary>
    /// 常用方法拓展
    /// </summary>
    public static class Extend
    {
        /// <summary>
        /// object 转 JSON 字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="DateTimeFormat">时间格式化</param>
        /// <returns></returns>
        public static string ToJson(this object obj, string DateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            Newtonsoft.Json.Converters.IsoDateTimeConverter dtFmt = new Newtonsoft.Json.Converters.IsoDateTimeConverter
            {
                DateTimeFormat = DateTimeFormat
            };
            return JsonConvert.SerializeObject(obj, dtFmt);
        }

        /// <summary>
        /// 解析 JSON字符串 为JObject对象
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>JObject对象</returns>
        public static JObject ToJObject(this string json)
        {
            return JObject.Parse(json);
        }

        /// <summary>
        /// 解析 JSON字符串 为JArray对象
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>JArray对象</returns>
        public static JArray ToJArray(this string json)
        {
            return JArray.Parse(json);
        }

        /// <summary>
        /// JSON字符串 转 实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="json">JSON字符串</param>
        public static T ToEntity<T>(this string json)
        {
            var mo = JsonConvert.DeserializeObject<T>(json);
            return mo;
        }

        /// <summary>
        /// JSON字符串 转 实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="json">JSON字符串</param>
        public static List<T> ToEntitys<T>(this string json)
        {
            var list = JsonConvert.DeserializeObject<List<T>>(json);
            return list;
        }

        /// <summary>
        /// 把jArray里面的json对象转为字符串
        /// </summary>
        /// <param name="jt">JToken对象</param>
        /// <returns></returns>
        public static string ToStringOrEmpty(this JToken jt)
        {
            try
            {
                return jt == null ? "" : jt.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 字符串 JSON转义
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static string OfJson(this string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// SQL单引号转义
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static string OfSql(this string s)
        {
            return s.Replace("'", "''");
        }

        /// <summary>
        /// 实体转表
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="list">对象</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            var t = new DataTable();
            elementType.GetProperties().ToList().ForEach(propInfo => t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType));
            foreach (T item in list)
            {
                var row = t.NewRow();
                elementType.GetProperties().ToList().ForEach(propInfo => row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value);
                t.Rows.Add(row);
            }
            return t;
        }

        /// <summary>
        /// 表转为实体
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="table">表</param>
        /// <returns></returns>
        public static List<T> ToModel<T>(this DataTable table) where T : class, new()
        {
            var list = new List<T>();
            foreach (DataRow dr in table.Rows)
            {
                var model = new T();
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    object drValue = dr[dc.ColumnName];

                    var pi = model.GetType().GetProperties().Where(x => x.Name.ToLower() == dc.ColumnName.ToLower()).FirstOrDefault();

                    Type type = pi.PropertyType;
                    if (pi.PropertyType.FullName.Contains("System.Nullable"))
                    {
                        type = Type.GetType("System." + pi.PropertyType.FullName.Split(',')[0].Split('.')[2]);
                    }

                    if (pi != null && pi.CanWrite && (drValue != null && !Convert.IsDBNull(drValue)))
                    {
                        try
                        {
                            drValue = Convert.ChangeType(drValue, type);
                            pi.SetValue(model, drValue, null);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="uri">内容</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        public static string ToEncode(this string uri, string charset = "utf-8")
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
                    encodedUri.Append(escapeFlag).Append(string.Format(CultureInfo.InstalledUICulture, "{0:X2}", (int)b));
                }
            }
            return encodedUri.ToString();
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="uriToDecode">内容</param>
        /// <returns></returns>
        public static string ToDecode(this string uriToDecode)
        {
            if (!string.IsNullOrEmpty(uriToDecode))
            {
                uriToDecode = uriToDecode.Replace("+", " ");
                return Uri.UnescapeDataString(uriToDecode);
            }

            return string.Empty;
        }

        /// <summary>
        /// 将Datetime转换成时间戳，10位：秒 或 13位：毫秒
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="isms">毫秒，默认false为秒，设为true，返回13位，毫秒</param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime datetime, bool isms = false)
        {
            var t = datetime.ToUniversalTime().Ticks - 621355968000000000;
            var tc = t / (isms ? 10000 : 10000000);
            return tc;
        }
    }
}