using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Netnr.TencentAI.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //所有方法
            string msg = GetMethodInfo();
            //Console.WriteLine(msg);

            //示例
            Aid.APPID = 2112919356;
            Aid.APPKEY = "Vqe3e9EBzi0r5wJK";

            string result = string.Empty;

            result = Ocr.Ocr_GeneralOcr();

            Console.WriteLine(result);
            Console.ReadKey();
        }

        /// <summary>
        /// 反射得到方法列表，JSON格式
        /// </summary>
        /// <returns></returns>
        public static string GetMethodInfo()
        {
            Dictionary<string, string> dicClass = new Dictionary<string, string>();
            Dictionary<string, string> dicMethod = new Dictionary<string, string>();

            string ns = "Netnr.TencentAI";
            string nssub = "fcgi_bin";

            Assembly asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + $"/{ns}.dll");
            foreach (TypeInfo dt in asm.DefinedTypes)
            {
                if (dt.FullName.Contains(ns) && dt.FullName.Contains(nssub))
                {
                    Type t = asm.GetType($"{ns}.{nssub}.{dt.Name}");
                    DescriptionAttribute attrClass = Attribute.GetCustomAttribute(t, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    dicClass.Add(dt.Name, attrClass?.Description ?? "");

                    MethodInfo[] mhs = t.GetMethods();
                    foreach (MethodInfo mh in mhs)
                    {
                        if (Attribute.GetCustomAttribute(mh, typeof(DescriptionAttribute)) is DescriptionAttribute attrMethod)
                        {
                            string isc = string.Empty;
                            if (mh.ToString().Contains(ns))
                            {
                                isc = "";
                            }
                            dicMethod.Add(mh.Name, attrMethod.Description + isc);
                        }
                    }
                }
            }
            List<string> listKey = dicMethod.Keys.ToList();

            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();

            foreach (string methodKey in listKey)
            {
                string key = methodKey.Split('_')[0];
                string classDesc = dicClass[key];
                string desc = dicMethod[methodKey];
                if (!string.IsNullOrWhiteSpace(classDesc))
                {
                    desc = classDesc + ">" + desc;
                    dicMethod[methodKey] = desc;
                }

                List<string> dcs = desc.Split('>').ToList();
                string dcskey = dcs[0];
                dcs.RemoveAt(0);
                string dcsval = string.Join(" > ", dcs);

                if (dic.ContainsKey(dcskey))
                {
                    dic[dcskey].Add(methodKey, dcsval);
                }
                else
                {
                    Dictionary<string, string> dicitem = new Dictionary<string, string>
                    {
                        { methodKey, dcsval }
                    };
                    dic[dcskey] = dicitem;
                }
            }

            return dic.ToJson();
        }
    }
}
