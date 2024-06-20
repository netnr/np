using System.Collections.Concurrent;
using System.Xml;

namespace Netnr.ResponseFramework.Web.Services
{
    public class LoggingService
    {
        private static Dictionary<string, string> _dicDescription;

        /// <summary>
        /// 提取 XML 注释为字典
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

                                var action = "/" + (conll.Name.Replace("Controller", "/") + item.Name).ToLower();
                                dic.TryAdd(action, remark);
                            }
                        }
                    }
                    _dicDescription = dic;
                }

                return _dicDescription;
            }
        }

        /// <summary>
        /// 当前缓存日志
        /// </summary>
        public static ConcurrentQueue<SysLog> CurrentCacheLog { get; set; } = [];

    }
}
