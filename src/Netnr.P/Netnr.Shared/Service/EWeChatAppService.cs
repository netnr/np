#if Full || Service

using System.Drawing.Drawing2D;

namespace Netnr;

/// <summary>
/// 推送：企业微信应用
/// </summary>
public class EWeChatAppService
{
    /// <summary>
    /// 获取 access_token
    /// https://developer.work.weixin.qq.com/document/path/91039
    /// </summary>
    /// <param name="corpId">企业ID</param>
    /// <param name="corpSecret">应用凭证密钥，注意应用需要是启用状态（按应用凭证缓存）</param>
    /// <returns></returns>
    public static async Task<string> GetToken(string corpId, string corpSecret)
    {
        var ckey = $"EWeChatApp-{corpSecret}";
        var cval = CacheTo.Get<string>(ckey);

        if (string.IsNullOrWhiteSpace(cval))
        {
            var hc = new HttpClient();
            var uri = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpId}&corpsecret={corpSecret}";
            var resp = await hc.GetAsync(uri);
            if (resp.IsSuccessStatusCode)
            {
                var read = await resp.Content.ReadAsStringAsync();
                cval = read.DeJson().GetValue("access_token");

                CacheTo.Set(ckey, cval, 3600, false);
            }
        }

        return cval;
    }

    /// <summary>
    /// 发送应用消息
    /// https://developer.work.weixin.qq.com/document/path/90236
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="toUser">指定接收消息的成员，多个用‘|’分隔，或指定为 @all </param>
    /// <param name="agentId">企业应用id</param>
    /// <param name="content">【以 2048 字节分批】消息内容</param>
    /// <param name="title">消息标题，可选，追加到消息内容前面</param>
    /// <param name="msgType">消息类型</param>
    /// <returns></returns>
    public static async Task<ResultVM> MessageSend(string accessToken, string toUser, string agentId, string content, string title = null, string msgType = "text")
    {
        var vm = new ResultVM();

        try
        {
            var hc = new HttpClient();
            var uri = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={accessToken}";

            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss dddd");
            var successCount = 0;
            var listResult = new List<JsonElement>();

            if (string.IsNullOrWhiteSpace(title))
            {
                content = $"{now}\r\n\r\n{content}";
            }
            else
            {
                content = $"{title}\r\n{now}\r\n\r\n{content}";
            }

            var groups = ParsingTo.SplitBySize(content, 2048);
            await LockTo.RunAsync(nameof(MessageSend), 5000 * groups.Count, async () =>
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    if (i != 0)
                    {
                        await Task.Delay(1000);
                    }

                    var postJson = new JsonObject
                    {
                        ["touser"] = toUser,
                        ["msgtype"] = msgType,
                        ["agentid"] = agentId,
                        [msgType] = new JsonObject
                        {
                            ["content"] = groups[i]
                        }
                    };

                    var resp = await hc.PostAsync(uri, new StringContent(postJson.ToJson()));
                    if (resp.IsSuccessStatusCode)
                    {
                        var read = await resp.Content.ReadAsStringAsync();
                        var outjson = read.DeJson();
                        listResult.Add(outjson);
                        if (outjson.GetValue("errcode") == "0")
                        {
                            successCount++;
                        }
                    }
                }
            });

            if (successCount == groups.Count)
            {
                vm.Set(RCodeTypes.success);
            }
            else if (successCount > 0)
            {
                vm.Set(RCodeTypes.partialSuccess);
            }
            else
            {
                vm.Set(RCodeTypes.failure);
            }
            vm.Data = listResult.Count == 1 ? listResult[0] : listResult;
        }
        catch (Exception ex)
        {
            vm.Set(ex);
        }

        return vm;
    }
}

#endif