using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// 辅助类
    /// </summary>
    public partial class DXService
    {
        /// <summary>
        /// 调用菜单
        /// </summary>
        /// <param name="ctype"></param>
        /// <param name="isAgain"></param>
        public static void InvokeMenu(Type ctype, bool isAgain = true)
        {
            var cms = ctype.GetMethods().ToList();
            var mm = cms.First().Module;
            cms = cms.Where(x => x.Module == mm).ToList();

            var dicMethod = new Dictionary<string, MethodInfo>();
            for (int i = 0; i < cms.Count; i++)
            {
                var mi = cms[i];
                var attrs = mi.CustomAttributes.LastOrDefault()?.NamedArguments;
                if (attrs?.Count > 0)
                {
                    var attrGroupName = attrs.FirstOrDefault(x => x.MemberName == "GroupName").TypedValue.Value?.ToString();
                    var attrName = attrs.FirstOrDefault(x => x.MemberName == "Name").TypedValue.Value?.ToString();
                    var attrShortName = attrs.FirstOrDefault(x => x.MemberName == "ShortName").TypedValue.Value?.ToString();
                    var attrDescription = attrs.FirstOrDefault(x => x.MemberName == "Description").TypedValue.Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(attrShortName))
                    {
                        attrName = $"{attrShortName.Split(' ')[0]} ({attrName})";
                    }

                    dicMethod.Add($"{attrName} -> {attrDescription}{attrGroupName}", mi);
                }
            }
            var listSilent = dicMethod.Keys.ToList();

            var cri = ConsoleReadItem("Please choose", listSilent, isAgain ? null : 1);
            var method = dicMethod[listSilent[cri - 1]];
            method.Invoke(ctype, null);

            if (isAgain)
            {
                Thread.Sleep(1500);
                InvokeMenu(ctype, isAgain);
            }
        }

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            Log(ex.ToJson(true), ConsoleColor.Red);
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cc"></param>
        public static void Log(string msg, ConsoleColor? cc = null)
        {
            if (cc != null)
            {
                Console.ForegroundColor = cc.Value;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine(msg);
            }

            ConsoleTo.Log(msg);
        }

        /// <summary>
        /// 相似匹配
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool SimilarMatch(string s1, string s2)
        {
            s1 = s1.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
            s2 = s2.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();
            return s1 == s2;
        }

        /// <summary>
        /// 提示符号
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string TipSymbol(string tip, string symbol = ": ")
        {
            return $"\r\n{tip.TrimEnd('：').TrimEnd(':')}{symbol}";
        }

        /// <summary>
        /// 重试
        /// </summary>
        /// <param name="action"></param>
        public static void TryAgain(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Log(ex.Message);

                if (ConsoleReadBool("\r\nTry Again"))
                {
                    TryAgain(action);
                }
            }
        }

        /// <summary>
        /// 输入数据库
        /// </summary>
        /// <param name="model"></param>
        public static DataKitTransferVM.ConnectionInfo ConsoleReadDatabase(ConfigJson model, string tip = "请选择数据库连接")
        {
            var mo = new DataKitTransferVM.ConnectionInfo();

        Flag1:
            var allDbConns = model.ListConnectionInfo;

            var ckey = "tmp_Database_Conns";
            var tmpDbConns = CacheTo.Get<List<DataKitTransferVM.ConnectionInfo>>(ckey);
            if (tmpDbConns != null)
            {
                allDbConns = model.ListConnectionInfo.Concat(tmpDbConns).ToList();
            }
            else
            {
                tmpDbConns = new List<DataKitTransferVM.ConnectionInfo>();
            }

            Console.WriteLine($"\n{0,5}. 输入数据库连接信息");
            for (int i = 0; i < allDbConns.Count; i++)
            {
                var obj = allDbConns[i];
                Console.WriteLine($"{i + 1,5}. {obj.ConnectionRemark} -> {obj.ConnectionType} => {obj.ConnectionString}");
            }
            Console.Write(TipSymbol(tip));

            var rdi = int.TryParse(Console.ReadLine().Trim(), out int ed);

            if (rdi && ed == 0)
            {
            Flag2:
                Console.Write(TipSymbol("数据库连接信息（MySQL => Conn）"));
                var tcs = Console.ReadLine().Trim().Split(" => ");
                var tctype = tcs[0].Split(" -> ").Last();
                if (Enum.TryParse(tctype, true, out EnumTo.TypeDB tdb))
                {
                    mo.ConnectionType = tdb;
                    mo.ConnectionString = tcs[1];
                    mo.ConnectionRemark = $"[TMP] {DateTime.Now:yyyy-MM-dd HH:mm:ss} {tdb}";
                    if (mo.ConnectionString.Length < 10)
                    {
                        Log("数据库连接字符串无效");
                        goto Flag2;
                    }

                    mo.ConnectionString = DbHelper.SqlConnPreCheck(mo.ConnectionType, mo.ConnectionString);

                    tmpDbConns.Add(mo);
                    CacheTo.Set(ckey, tmpDbConns);
                }
                else
                {
                    Log("无效数据库类型");
                    goto Flag2;
                }
            }
            else if (ed > 0 && ed <= allDbConns.Count)
            {
                mo = allDbConns[ed - 1];
            }
            else
            {
                Log($"无效选择，请重新选择");
                goto Flag1;
            }

            //选择数据库名
            switch (mo.ConnectionType)
            {
                case EnumTo.TypeDB.MySQL:
                case EnumTo.TypeDB.MariaDB:
                case EnumTo.TypeDB.SQLServer:
                case EnumTo.TypeDB.PostgreSQL:
                    {
                        var dk = DataKitTo.Init(mo.ConnectionType, mo.ConnectionString);
                        var listDatabaseName = dk.GetDatabaseName();
                        var dv = 1;
                        if (!string.IsNullOrWhiteSpace(mo.DatabaseName))
                        {
                            dv = listDatabaseName.IndexOf(mo.DatabaseName) + 1;
                        }

                        var cri = ConsoleReadItem(TipSymbol($"选择数据库名"), listDatabaseName, dv);
                        mo.DatabaseName = listDatabaseName[cri - 1];
                    }
                    break;
            }

            Log($"\n已选择 {mo.ConnectionType} => {mo.ConnectionString}\n", ConsoleColor.Cyan);

            return mo;
        }

        /// <summary>
        /// 输入文件（夹）
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="type">默认（0：都可以；1：文件；2：文件夹）</param>
        /// <param name="dv">默认文件（夹）</param>
        /// <param name="mustExist">是否必须存在</param>
        public static string ConsoleReadPath(string tip, int type = 1, string dv = null, bool mustExist = true)
        {
        Flag1:
            var dtip = string.IsNullOrWhiteSpace(dv) ? TipSymbol(tip) : TipSymbol(tip, $"(default: {dv}): ");
            Console.Write(dtip);
            var path = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(dv) && string.IsNullOrWhiteSpace(path))
            {
                path = dv;
            }
            else if (mustExist)
            {
                if ((type == 1 && !File.Exists(path)) || (type == 2 && !Directory.Exists(path)) || (type == 0 && !File.Exists(path) && !Directory.Exists(path)))
                {
                    Log($"{path} 无效文件（夹）");
                    goto Flag1;
                }
            }

            return path;
        }

        /// <summary>
        /// 输入选择项
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="items">项</param>
        /// <param name="dv">默认（从 1 开始）</param>
        public static int ConsoleReadItem(string tip, IList<string> items, int? dv = 1)
        {
            var outIndex = 0;
            bool isAgain;
            do
            {
                isAgain = false;
                Console.WriteLine("");
                for (int j = items.Count - 1; j >= 0; j--)
                {
                    Console.WriteLine($"{j + 1,5}. {items[j]}");
                }

                Console.Write(TipSymbol(tip, dv.HasValue ? $"(default: {dv}. {items[dv.Value - 1].Trim()}): " : ": "));
                var ii = Console.ReadLine()?.Trim();
                if (dv.HasValue && string.IsNullOrWhiteSpace(ii))
                {
                    Log($"\r\nChosen {dv}. {items[dv.Value - 1].Trim()}", ConsoleColor.Cyan);
                    outIndex = dv.Value;
                }
                else
                {
                    _ = int.TryParse(ii, out int i);

                    if (i > 0 && i <= items.Count)
                    {
                        Log($"\r\nChosen {i}. {items[i - 1].Trim()}", ConsoleColor.Cyan);
                        outIndex = i;
                    }
                    else
                    {
                        isAgain = true;
                    }
                }
            } while (isAgain);

            return outIndex;
        }

        /// <summary>
        /// 输入是否
        /// </summary>
        /// <param name="tip">提示文字</param>
        public static bool ConsoleReadBool(string tip)
        {
            Console.Write($"{tip}? [y(1)/N(default)]: ");
            return new[] { "y", "1" }.Contains(Console.ReadLine().ToLower().Trim());
        }

        /// <summary>
        /// 输入数字
        /// </summary>
        /// <param name="tip">提示文字</param>
        /// <param name="dv">默认 1 </param>
        public static int ConsoleReadNumber(string tip, int dv = 1)
        {
            Console.Write(TipSymbol(tip, $"(default: {dv}): "));

            var ii = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(ii))
            {
                Log($"\nNumber {dv}\n", ConsoleColor.Cyan);
                return dv;
            }
            else
            {
                _ = int.TryParse(ii, out int i);
                Log($"\nNumber {i}\n", ConsoleColor.Cyan);
                return i;
            }
        }

        /// <summary>
        /// 输出标题
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public static void OutputTitle(string title) => Log($"\r\n------- {title}", ConsoleColor.Cyan);

        /// <summary>
        /// 证书信息
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        public static void CertificateInformation(X509Certificate2 certificate, X509Chain chain = null)
        {
            if (chain == null)
            {
                chain = new X509Chain();
                chain.Build(certificate);
            }

            var now = DateTime.Now;
            var cert = certificate;
            var pk = cert.PublicKey;

            OutputTitle(cert.PublicKey.Oid.FriendlyName);
            Log($"颁发给: {cert.Subject}");
            Log($"颁发着: {cert.Issuer}");

            RSA pkrsa;
            ECDsa pkecdsa;
            ECDiffieHellman pkecdiffiie;
            int keySize = 0;
            if ((pkrsa = pk.GetRSAPublicKey()) != null)
            {
                keySize = pkrsa.KeySize;
            }
            else if ((pkecdsa = pk.GetECDsaPublicKey()) != null)
            {
                keySize = pkecdsa.KeySize;
            }
            else if ((pkecdiffiie = pk.GetECDiffieHellmanPublicKey()) != null)
            {
                keySize = pkecdiffiie.KeySize;
            }
            Log($"加密算法: {cert.PublicKey.Oid.FriendlyName} {keySize} bits");
            Log($"签名算法: {cert.SignatureAlgorithm.FriendlyName}");
            Log($"SHA1指纹: {cert.Thumbprint}");

            var sha256 = CalcTo.HashString(CalcTo.HashType.SHA256, certificate.GetRawCertData().ToStream()).ToUpper();
            Log($"SHA256指纹: {sha256}");
            Log($"有效期: {cert.NotBefore} 至 {cert.NotAfter} (剩余 {(int)((cert.NotAfter - now).TotalDays)} 天)");

            var isRevoked = chain.ChainStatus.Any(x => x.Status.HasFlag(X509ChainStatusFlags.Revoked))
                ? string.Join(", ", chain.ChainStatus.Select(x => x.Status)) : "正常";
            Log($"吊销状态: {isRevoked}");

            var san = cert.Extensions.FirstOrDefault(x => x.Oid.Value == "2.5.29.17").RawData.ToText(Encoding.Latin1);
            san = Regex.Replace(san, @"\p{C}+", " ");
            san = string.Join(" ", san.Split(' ').Where(x => x.Length > 3).Select(x => RemoveSpecialCharacters(x)));
            Log($"备用名称: {san}");
            Log($"版本: {cert.Version}");
            Log($"序列号: {cert.SerialNumber}");

            for (int i = 0; i < chain.ChainElements.Count; i++)
            {
                if (i == 0)
                {
                    OutputTitle($"证书链信息");
                }
                else
                {
                    Log("");
                }

                var cc = chain.ChainElements[i].Certificate;
                var certPk = cc.PublicKey;
                Log($"颁发给: {cc.Subject}");
                Log($"颁发着: {cc.Issuer}");

                RSA certrsa;
                ECDsa certecdsa;
                ECDiffieHellman certecdiffiie;
                int certKeySize = 0;
                if ((certrsa = certPk.GetRSAPublicKey()) != null)
                {
                    certKeySize = certrsa.KeySize;
                }
                else if ((certecdsa = pk.GetECDsaPublicKey()) != null)
                {
                    certKeySize = certecdsa.KeySize;
                }
                else if ((certecdiffiie = pk.GetECDiffieHellmanPublicKey()) != null)
                {
                    certKeySize = certecdiffiie.KeySize;
                }
                Log($"加密算法: {cc.PublicKey.Oid.FriendlyName} {certKeySize} bits");
                Log($"签名算法: {cc.SignatureAlgorithm.FriendlyName}");
                Log($"有效期: {cc.NotBefore} 至 {cc.NotAfter} (剩余 {(int)((cc.NotAfter - now).TotalDays)} 天)");
            }
        }

        /// <summary>
        /// 解析主机名、端口
        /// </summary>
        /// <param name="hostnameAndPorts"></param>
        /// <returns></returns>
        public static ValueTuple<string, List<int>> ParseHostnameAndPorts(string hostnameAndPorts)
        {
            string hostname = hostnameAndPorts.Trim();
            var listPort = new List<int>();

            var hnp = hostname.Split(hostname.Contains(' ') ? ' ' : ':');
            hostname = hnp[0];
            if (hnp.Length > 1)
            {
                var ports = hnp[1].Split(',');
                foreach (var item in ports)
                {
                    if (item.Contains('-'))
                    {
                        var rport = item.Split('-');
                        var r1 = Convert.ToInt32(rport.First());
                        r1 = Math.Max(r1, 1);
                        var r2 = Convert.ToInt32(rport.Last());
                        r2 = Math.Min(r2, 65535);
                        for (int i = r1; i <= r2; i++)
                        {
                            listPort.Add(i);
                        }
                    }
                    else
                    {
                        listPort.Add(Convert.ToInt32(item));
                    }
                }
            }

            return new ValueTuple<string, List<int>>(hostname, listPort);
        }

        /// <summary>
        /// 查询域名、IP对应的地址信息
        /// </summary>
        /// <param name="domainOrIP"></param>
        /// <returns></returns>
        public static Dictionary<string, string> DomainOrIPInfo(string domainOrIP)
        {
            var result = new Dictionary<string, string>();

            try
            {
                //空，获取本地公网出口IP
                if (string.IsNullOrWhiteSpace(domainOrIP))
                {
                    var res = HttpTo.Get("https://api.bilibili.com/x/web-interface/zone");
                    var data = res.DeJson().GetProperty("data");
                    result.Add(data.GetValue("addr"), $"{data.GetValue("country")} {data.GetValue("province")} {data.GetValue("isp")}");
                }
                //域名
                else
                {
                    var listIp = new List<string>();
                    if (!IPAddress.TryParse(domainOrIP, out _))
                    {
                        var addresses = Dns.GetHostAddresses(domainOrIP);
                        foreach (var item in addresses)
                        {
                            listIp.Add($"{item}");
                        }
                    }
                    else
                    {
                        listIp.Add(domainOrIP);
                    }

                    foreach (var item in listIp)
                    {
                        if (IPAddress.TryParse(item, out var addr))
                        {
                            if (addr.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                var url = $"https://ip.useragentinfo.com/ipv6/{addr}";
                                var res = HttpTo.Get(url);
                                var data = res.DeJson();
                                result.Add(data.GetValue("ipv6"), $"{data.GetValue("country")} {data.GetValue("region")} {data.GetValue("city")}");
                            }
                            else
                            {
                                var url = $"https://opendata.baidu.com/api.php?query={item}&resource_id=6006&oe=utf8";
                                var res = HttpTo.Get(url);
                                var data = res.DeJson().GetProperty("data").EnumerateArray().First();
                                result.Add(data.GetValue("origipquery"), data.GetValue("location"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 新建文件名
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="ext">后缀，如 .zip</param>
        /// <returns></returns>
        public static string NewFileName(object prefix, string ext)
        {
            return $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
        }

        /// <summary>
        /// 解析路径变量
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ParsePathVar(string str)
        {
            string pattern = @"({\w+})";
            var now = DateTime.Now;

            var ci = new ConfigInit();

            var path = new Regex(pattern).Replace(str, o =>
            {
                var format = o.Groups[1].Value[1..^1];
                return DateTime.Now.ToString(format);
            }).Replace("~", ci.DXHub);

            return path;
        }

        /// <summary>
        /// 移除乱码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string content)
        {
            var sb = new StringBuilder();
            foreach (char c in content)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= 0x20000 && c <= 0xFA2D) || "-_.*@".Contains(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
