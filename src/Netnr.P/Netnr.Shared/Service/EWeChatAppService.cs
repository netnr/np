#if Full || Service

using System.Net.Http;

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
    /// <param name="corpSecret">应用凭证密钥，注意应用需要是启用状态</param>
    /// <returns></returns>
    public static async Task<string> GetToken(string corpId, string corpSecret)
    {
        var ckey = $"EWeChatApp-{corpId}";
        var cval = CacheTo.Get<string>(ckey);

        var hc = new HttpClient();
        var uri = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpId}&corpsecret={corpSecret}";
        var resp = await hc.GetAsync(uri);
        if (resp.IsSuccessStatusCode)
        {
            var read = await resp.Content.ReadAsStringAsync();
            cval = read.DeJson().GetValue("access_token");

            CacheTo.Set(ckey, cval, 3600, false);
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
    /// <param name="content">内容</param>
    /// <param name="msgType">消息类型</param>
    /// <returns></returns>
    public static async Task<ResultVM> MessageSend(string accessToken, string toUser, string agentId, string content, string msgType = "text")
    {
        var vm = new ResultVM();

        var hc = new HttpClient();
        var uri = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={accessToken}";

        var postJson = new JsonObject
        {
            ["touser"] = toUser,
            ["msgtype"] = msgType,
            ["agentid"] = agentId,
            [msgType] = new JsonObject
            {
                ["content"] = content
            }
        };

        var resp = await hc.PostAsync(uri, new StringContent(postJson.ToJson()));
        if (resp.IsSuccessStatusCode)
        {
            var read = await resp.Content.ReadAsStringAsync();
            var result = read.DeJson();

            vm.Data = result;
            vm.Set(result.GetValue("errcode") == "0");
        }
        else
        {
            vm.Data = resp;
            vm.Set(EnumTo.RTag.failure);
        }

        return vm;
    }
}

#endif