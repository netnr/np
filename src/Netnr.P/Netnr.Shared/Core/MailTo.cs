﻿#if Full || Core

using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Netnr;

/// <summary>
/// 邮件
/// </summary>
public partial class MailTo
{
    /// <summary>
    /// 发送
    /// </summary>
    /// <param name="model">发送参数</param>
    public static async Task Send(SendModel model)
    {
        using var client = new SmtpClient(model.Host, model.Port)
        {
            Credentials = new System.Net.NetworkCredential(model.FromMail, model.FromPassword),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = true,
            Timeout = 10000
        };

        using var message = new MailMessage
        {
            From = new MailAddress(model.FromMail, model.FromName, model.Coding)
        };
        model.ToMail.ForEach(message.To.Add);
        model.CcMail.ForEach(message.CC.Add);

        message.Subject = model.Subject;
        message.SubjectEncoding = model.Coding;

        message.Body = model.Body;
        message.IsBodyHtml = true;
        message.BodyEncoding = model.Coding;

        await client.SendMailAsync(message).ConfigureAwait(false);
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    public class SendModel
    {
        /// <summary>
        /// 服务
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 587;
        /// <summary>
        /// 发送者邮箱
        /// </summary>
        public string FromMail { get; set; }
        /// <summary>
        /// 发送者名称
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// 发送者邮箱密码
        /// </summary>
        public string FromPassword { get; set; }
        /// <summary>
        /// 发送主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 发送内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 内容编码
        /// </summary>
        public Encoding Coding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 接收者邮箱
        /// </summary>
        public List<string> ToMail { get; set; } = [];
        /// <summary>
        /// 抄送给邮箱
        /// </summary>
        public List<string> CcMail { get; set; } = [];
    }
}

#endif