using System.Xml;

namespace Netnr.Blog.Web.Services
{
    /// <summary>
    /// 日志
    /// </summary>
    public class LoggingService
    {
        private static Dictionary<string, string> _dicDescription;

        /// <summary>
        /// 根据生成的注释文件XML获取Action的注释
        /// </summary>
        public static Dictionary<string, string> DicDescription
        {
            get
            {
                if (_dicDescription == null)
                {
                    var ass = System.Reflection.Assembly.GetExecutingAssembly();
                    var listController = ass.ExportedTypes.Where(x => x.BaseType?.FullName == "Microsoft.AspNetCore.Mvc.Controller").ToList();

                    //载入xml注释
                    var cp = AppContext.BaseDirectory + ass.FullName.Split(',').FirstOrDefault() + ".xml";
                    XmlDocument xmldoc = new();
                    xmldoc.Load(cp);
                    var xns = xmldoc.DocumentElement.SelectSingleNode("members").SelectNodes("member");
                    var listMember = new List<XmlNode>();
                    for (int i = 0; i < xns.Count; i++)
                    {
                        listMember.Add(xns[i]);
                    }

                    var dic = new Dictionary<string, string>();
                    foreach (var conll in listController)
                    {
                        var methods = conll.GetMethods();
                        foreach (var item in methods)
                        {
                            if (item.DeclaringType == conll)
                            {
                                string remark = "未备注说明";

                                //方法完整命名空间及名称
                                var cname = "M:" + conll.FullName + "." + item.Name;
                                //方法参数
                                var cparam = item.GetParameters();
                                if (cparam.Length > 0)
                                {
                                    var listParam = new List<string>();
                                    foreach (var par in cparam)
                                    {
                                        listParam.Add(par.ParameterType.FullName);
                                    }
                                    cname += "(" + string.Join(",", listParam) + ")";
                                }

                                var xnm = listMember.FirstOrDefault(x => x.Attributes["name"].Value.ToString() == cname);
                                if (xnm != null)
                                {
                                    remark = xnm.SelectSingleNode("summary").InnerText.ToString().Trim();
                                }

                                var action = (conll.Name.Replace("Controller", "/") + item.Name).ToLower();
                                if (!dic.ContainsKey(action))
                                {
                                    dic.Add(action, remark);
                                }
                            }
                        }
                    }
                    _dicDescription = dic;
                }

                return _dicDescription;
            }
        }

        /// <summary>
        /// 日志 构建实体
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LoggingModel Build(HttpContext context)
        {
            string reqPath = context.Request.Path.ToString();
            string reqQueryString = context.Request.QueryString.ToString();

            //客户端信息
            var ci = new ClientInfoTo(context);

            //用户信息
            var uinfo = IdentityService.Get(context);

            //日志保存
            var mo = new LoggingModel()
            {
                LogApp = AppTo.GetValue("Common:EnglishName"),
                LogUid = uinfo?.UserName,
                LogNickname = uinfo?.Nickname,
                LogAction = reqPath,
                LogUrl = reqPath + reqQueryString,
                LogIp = ci.IP,
                LogReferer = ci.Headers.Referer,
                LogCreateTime = DateTime.Now,
                LogUserAgent = ci.Headers.UserAgent,
                LogGroup = "1",
                LogLevel = "I"
            };

            var ddk = reqPath.ToLower().TrimStart('/');
            if (DicDescription.ContainsKey(ddk))
            {
                mo.LogContent = DicDescription[ddk];
            }

            return mo;
        }

        /// <summary>
        /// 日志 写入异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public static void Write(HttpContext context, Exception exception)
        {
            var mo = Build(context);

            mo.LogLevel = "E";
            mo.LogGroup = "-1";
            mo.LogContent = exception.Message;

            LoggingTo.Add(mo);
        }
    }
}
