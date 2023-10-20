namespace Netnr.Blog.Application.Services
{
    /// <summary>
    /// 推送服务
    /// </summary>
    public class PushService
    {
        /// <summary>
        /// 企业微信应用消息推送
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="touser">可选</param>
        /// <returns></returns>
        public async static Task<ResultVM> PushWeChat(string title, string content, string touser = "@all")
        {
            var vm = new ResultVM();

            try
            {
                var corpId = AppTo.GetValue("ApiKey:EWeixinApp:CorpId");
                var corpSecret = AppTo.GetValue("ApiKey:EWeixinApp:CorpSecret");
                var agentId = AppTo.GetValue("ApiKey:EWeixinApp:AgentId");
                if (string.IsNullOrWhiteSpace(touser))
                {
                    touser = "@all";
                }

                var accessToken = await EWeChatAppService.GetToken(corpId, corpSecret);
                vm = await EWeChatAppService.MessageSend(accessToken, touser, agentId, content, title);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content">内容</param>
        /// <param name="toMails">接收邮箱，多个逗号分割</param>
        /// <returns></returns>
        public async static Task<ResultVM> SendEmail(string title, string content, string toMails)
        {
            var vm = new ResultVM();

            try
            {
                var sendModel = new MailTo.SendModel()
                {
                    Host = AppTo.GetValue("ApiKey:Mail:Host"),
                    Port = AppTo.GetValue<int>("ApiKey:Mail:Port"),
                    FromMail = AppTo.GetValue("ApiKey:Mail:FromMail"),
                    FromPassword = AppTo.GetValue("ApiKey:Mail:FromPassword"),
                    FromName = AppTo.GetValue("Common:EnglishName"),
                    Subject = title ?? AppTo.GetValue("Common:EnglishName"),
                    Body = content.Replace("\r\n", "<br/>"),
                    ToMail = toMails.Split(',').ToList()
                };

                await MailTo.Send(sendModel);

                vm.Set(RCodeTypes.success);
                vm.Msg = "已发送";
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
