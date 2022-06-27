using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;

namespace Netnr.SMS.Application
{
    /// <summary>
    /// 发送短信
    /// </summary>
    public class AliyunService
    {
        /// <summary>
        /// 短信批量发送
        /// </summary>
        /// <param name="dicAddQuery">参数项</param>
        /// <returns></returns>
        public static SharedResultVM SendBatchSms(Dictionary<string, string> dicAddQuery)
        {
            return SharedResultVM.Try(vm =>
            {
                var ci = new Domain.ConfigInit();

                var profile = DefaultProfile.GetProfile(ci.GetValue("Aliyun:RegionId"), ci.GetValue("Aliyun:AccessKeyId"), ci.GetValue("Aliyun:Secret"));
                var client = new DefaultAcsClient(profile);
                var request = new CommonRequest
                {
                    Method = MethodType.POST,
                    Protocol = ProtocolType.HTTP,
                    Action = "SendBatchSms",
                    Domain = ci.GetValue("Aliyun:Domain"),
                    Version = ci.GetValue("Aliyun:Version")
                };

                foreach (var key in dicAddQuery.Keys)
                {
                    request.AddQueryParameters(key, dicAddQuery[key]);
                }

                try
                {
                    var response = client.GetCommonResponse(request);
                    var result = Encoding.UTF8.GetString(response.HttpResponse.Content);

                    var ro = result.ToJObject();
                    vm.Data = ro;
                    if (ro["Code"].ToStringOrEmpty() == "OK")
                    {
                        vm.Set(SharedEnum.RTag.success);
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.fail);
                    }
                }
                catch (Exception ex)
                {
                    vm.Set(ex);
                }

                return vm;
            });
        }
    }
}
