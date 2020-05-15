using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netnr.AliyunSMS.Application
{
    /// <summary>
    /// 发送短信
    /// </summary>
    public class SmsService
    {
        /// <summary>
        /// 短信批量发送
        /// </summary>
        /// <param name="dicAddQuery">参数项</param>
        /// <returns></returns>
        public static ActionResultVM SendBatchSms(Dictionary<string, string> dicAddQuery)
        {
            var vm = new ActionResultVM();

            try
            {
                var regionId = GlobalTo.GetValue("Sms:regionId");
                var accessKeyId = GlobalTo.GetValue("Sms:accessKeyId");
                var secret = GlobalTo.GetValue("Sms:secret");
                var Domain = GlobalTo.GetValue("Sms:Domain");

                IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKeyId, secret);
                DefaultAcsClient client = new DefaultAcsClient(profile);
                CommonRequest request = new CommonRequest
                {
                    Method = MethodType.POST,
                    Domain = Domain,
                    Version = "2017-05-25",
                    Action = "SendBatchSms",
                    Protocol = ProtocolType.HTTP
                };

                foreach (var key in dicAddQuery.Keys)
                {
                    request.AddQueryParameters(key, dicAddQuery[key]);
                }

                try
                {
                    CommonResponse response = client.GetCommonResponse(request);
                    var result = Encoding.UTF8.GetString(response.HttpResponse.Content);

                    var ro = result.ToJObject();
                    vm.Data = ro;
                    if (ro["Code"].ToStringOrEmpty() == "OK")
                    {
                        vm.Set(ARTag.success);
                    }
                    else
                    {
                        vm.Set(ARTag.fail);
                    }
                }
                catch (ServerException e)
                {
                    vm.Code = -1;
                    vm.Msg = e.Message;
                    vm.Data = "Server Exception";
                }
                catch (ClientException e)
                {
                    vm.Code = -1;
                    vm.Msg = e.Message;
                    vm.Data = "Server Exception";
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
