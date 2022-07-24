﻿#if Full || Web

using System.Web;

namespace Netnr
{
    /// <summary>
    /// 下载
    /// </summary>
    public class DownTo
    {
        private readonly HttpResponse Response;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="httpResponse"></param>
        public DownTo(HttpResponse httpResponse)
        {
            Response = httpResponse;
        }

        /// <summary>
        /// 流的方式下载
        /// </summary>
        public void Stream(string path, string fileName)
        {
            FileStream fileStream = new(path + fileName, FileMode.Open);
            byte[] bytes = new byte[(int)fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();

            Stream(bytes, fileName);
        }

        /// <summary>
        /// 流的方式下载
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName">下载文件名</param>
        public void Stream(byte[] bytes, string fileName)
        {
            Response.ContentType = "application/octet-stream";

            // 通知浏览器下载而不是打开  
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
            Response.Body.WriteAsync(bytes, 0, bytes.Length).Wait();
            Response.Body.FlushAsync().Wait();
        }
    }
}

#endif