namespace Netnr.Blog.Application.Services
{
    /// <summary>
    /// 推送服务
    /// </summary>
    public class PushService
    {
        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content">内容</param>
        /// <param name="touser">可选</param>
        /// <returns></returns>
        public async static Task<ResultVM> PushAsync(string title, string content = "", string touser = "@all")
        {
            var vm = new ResultVM();

            try
            {
                var list = new List<string>();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    list.Add(title);
                }
                list.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                if (!string.IsNullOrWhiteSpace(content))
                {
                    list.Add("\r\n" + content);
                }

                var corpId = AppTo.GetValue("ApiKey:EWeixinApp:CorpId");
                var corpSecret = AppTo.GetValue("ApiKey:EWeixinApp:CorpSecret");
                var agentId = AppTo.GetValue("ApiKey:EWeixinApp:AgentId");
                if (string.IsNullOrWhiteSpace(touser))
                {
                    touser = "@all";
                }

                var accessToken = await EWeChatAppService.GetToken(corpId, corpSecret);
                vm = await EWeChatAppService.MessageSend(accessToken, touser, agentId, string.Join("\r\n", list));
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
