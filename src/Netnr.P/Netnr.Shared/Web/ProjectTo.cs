#if Full || Web

using System.Xml;
using System.Reflection;

namespace Netnr
{
    /// <summary>
    /// 项目
    /// </summary>
    public class ProjectTo
    {
        /// <summary>
        /// 方法
        /// </summary>
        public class MethodModel
        {
            /// <summary>
            /// 模块名称（可选）
            /// </summary>
            public string ModuleName { get; set; }
            /// <summary>
            /// 控制器名称
            /// </summary>
            public string ControllerName { get; set; }
            /// <summary>
            /// 方法名称
            /// </summary>
            public string ActionName { get; set; }
            /// <summary>
            /// 方法注释
            /// </summary>
            public string ActionComment { get; set; }
            /// <summary>
            /// 方法参数
            /// </summary>
            public List<ParameterModel> ActionParameter { get; set; } = [];
        }

        /// <summary>
        /// 参数
        /// </summary>
        public class ParameterModel
        {
            /// <summary>
            /// 参数类型
            /// </summary>
            public string ParameterType { get; set; }
            /// <summary>
            /// 参数类型
            /// </summary>
            public string ParameterFullType { get; set; }
            /// <summary>
            /// 参数名称
            /// </summary>
            public string ParameterName { get; set; }
            /// <summary>
            /// 参数注释
            /// </summary>
            public string ParameterComment { get; set; }
        }

        /// <summary>
        /// 获取项目 XML 注释
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <returns></returns>
        public static List<XmlNode> GetDocumentationFile(string projectName)
        {
            var ckey = $"Project-DocumentationFile-{projectName}";

            var cval = CacheTo.Get<List<XmlNode>>(ckey);
            if (cval == null)
            {
                //读取 XML 注释
                XmlDocument xmlDoc = new();
                xmlDoc.Load($"{AppContext.BaseDirectory}{projectName}.xml");
                var memberNodes = xmlDoc.DocumentElement.SelectSingleNode("members").SelectNodes("member");
                var listMemberNode = new List<XmlNode>();
                for (int i = 0; i < memberNodes.Count; i++)
                {
                    listMemberNode.Add(memberNodes[i]);
                }

                cval = listMemberNode;
                CacheTo.Set(ckey, cval, sliding: false);
            }

            return cval;
        }

        /// <summary>
        /// 获取项目 XML 注释
        /// </summary>
        /// <typeparam name="T">项目里的某个类</typeparam>
        /// <param name="len">命名空间按点分隔的数组长度</param>
        /// <returns></returns>
        public static List<XmlNode> GetDocumentationFile<T>(int len = 3)
        {
            var projectName = string.Join(".", typeof(T).FullName.Split('.').Take(len));
            return GetDocumentationFile(projectName);
        }

        /// <summary>
        /// 获取全部方法（接口名、注释）
        /// </summary>
        /// <returns></returns>
        public static List<MethodModel> GetAllAction(bool force = false)
        {
            var ckey = "Project-Action";
            var cval = CacheTo.Get<List<MethodModel>>(ckey);
            if (force || cval == null)
            {
                var asm = Assembly.GetExecutingAssembly();
                var ctrlType = typeof(Controller);
                var naaType = typeof(NonActionAttribute);
                var listController = asm.GetTypes().Where(ctrlType.IsAssignableFrom);

                //读取 XML 注释
                var listMemberNode = GetDocumentationFile(asm.FullName.Split(',').First());

                var result = new List<MethodModel>();
                foreach (var ctrl in listController)
                {
                    var authAttr = ctrl.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();

                    var listAction = ctrl.GetMethods().Where(m => m.IsPublic && !m.IsStatic && !m.IsSpecialName && !m.IsDefined(naaType) && m.DeclaringType == ctrl);
                    foreach (var action in listAction)
                    {
                        var model = new MethodModel
                        {
                            ModuleName = authAttr?.Policy,
                            ControllerName = ctrl.Name[..^10], //移除 Controller
                            ActionName = action.Name,
                        };

                        //方法完整命名空间及名称
                        var mname = $"M:{ctrl.FullName}.{action.Name}";
                        var listParameterTypeFullName = new List<string>();

                        //方法参数
                        var getParameters = action.GetParameters();
                        foreach (var x in getParameters)
                        {
                            var isNullable = Nullable.GetUnderlyingType(x.ParameterType) != null;
                            if (isNullable)
                            {
                                var parameterTypeFullName = x.ParameterType.FullName.Split(',').First().Split('[').Last();
                                listParameterTypeFullName.Add("System.Nullable{" + parameterTypeFullName + "}");

                                var parameterTypeName = x.ParameterType.GenericTypeArguments.FirstOrDefault()?.Name ?? parameterTypeFullName;
                                model.ActionParameter.Add(new ParameterModel
                                {
                                    ParameterType = $"{parameterTypeName}?",
                                    ParameterFullType = parameterTypeFullName,
                                    ParameterName = x.Name
                                });
                            }
                            else
                            {
                                listParameterTypeFullName.Add(x.ParameterType.FullName);
                                model.ActionParameter.Add(new ParameterModel
                                {
                                    ParameterType = x.ParameterType.Name,
                                    ParameterFullType = x.ParameterType.FullName,
                                    ParameterName = x.Name
                                });
                            }
                        }
                        if (listParameterTypeFullName.Count > 0)
                        {
                            mname = $"{mname}({string.Join(",", listParameterTypeFullName).Replace("+", ".")})";
                        }

                        //方法注释
                        var memberNode = listMemberNode.FirstOrDefault(x => mname.Equals(x.Attributes["name"].Value));
                        if (memberNode != null)
                        {
                            model.ActionComment = memberNode.SelectSingleNode("summary").InnerText.ToString().Trim();

                            //参数注释
                            if (model.ActionParameter.Count > 0)
                            {
                                model.ActionParameter.ForEach(ap =>
                                {
                                    var listParams = memberNode.SelectNodes("param");
                                    for (int pi = 0; pi < listParams.Count; pi++)
                                    {
                                        if (listParams[pi].Attributes["name"].Value == ap.ParameterName)
                                        {
                                            ap.ParameterComment = listParams[pi].InnerText.ToString().Trim();
                                            break;
                                        }
                                    }
                                });
                            }
                        }

                        result.Add(model);
                    }
                }

                cval = result;
                CacheTo.Set(ckey, cval, sliding: false);
            }

            return cval;
        }
    }
}

#endif