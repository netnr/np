#if Full || ConsoleX

using System.Data;
using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// 辅助类
/// </summary>
public partial class ConsoleXTo
{
    /// <summary>
    /// 解析路径变量
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ParsePathVar(string str)
    {
        var ci = new ConfigInit();
        var now = DateTime.Now;

        var pattern = @"({\w+})";
        var path = new Regex(pattern).Replace(str, o =>
        {
            var format = o.Groups[1].Value[1..^1];
            return now.ToString(format);
        }).Replace("~", ci.DXHub);

        return path;
    }

    /// <summary>
    /// [Remark](MySQL://Conn)
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\[(.*?)\]\((.*?):\/\/(.*?)\)$", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex MatchConnUriFull();

    /// <summary>
    /// MySQL://Conn
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^(.*?):\/\/(.*?)$", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex MatchConnUriShort();

    /// <summary>
    /// 输入数据库
    /// </summary>
    /// <param name="configOption"></param>
    /// <param name="tip"></param>
    /// <param name="isSelectDatabase">选择数据库，非Oracle默认是</param>
    /// <returns></returns>
    public static async Task<DbKitConnectionOption> ConsoleReadDatabase(ConfigOption configOption, string tip = "请选择数据库连接", bool isSelectDatabase = true)
    {
        var connOption = new DbKitConnectionOption();

        await ConsoleTo.ReadRetry(async () =>
        {
            var allDbConns = configOption.ListConnectionInfo;

            var ckey = "Database-Conns";
            var tmpDbConns = CacheTo.Get<List<DbKitConnectionOption>>(ckey);
            if (tmpDbConns != null)
            {
                allDbConns = configOption.ListConnectionInfo.Concat(tmpDbConns).ToList();
            }
            else
            {
                tmpDbConns = [];
            }

            Console.WriteLine($"\r\n{0,5}. 输入数据库连接信息");
            for (int i = 0; i < allDbConns.Count; i++)
            {
                var obj = allDbConns[i];
                Console.WriteLine($"{i + 1,5}. [{obj.ConnectionRemark}]({obj.ConnectionType}://{obj.GetSafeConnectionString()})");
            }
            Console.Write(TipSymbol(tip));

            //读取选择的连接序号
            var connIndex = Convert.ToInt32(Console.ReadLine().Trim());

            //输入新的连接
            if (connIndex == 0)
            {
                await ConsoleTo.ReadRetry(() =>
                {
                    Console.Write(TipSymbol("连接格式 [Remark](MySQL://Conn)"));
                    var readConnUri = Console.ReadLine().Trim();

                    var mr = MatchConnUriFull().Match(readConnUri);
                    if (mr.Success || (mr = MatchConnUriShort().Match(readConnUri)).Success)
                    {
                        if (mr.Groups.Count == 3)
                        {
                            connOption.ConnectionRemark = $"TMP_{RandomTo.NewNumber()}";
                            connOption.ConnectionType = mr.Groups[1].ToString().DeEnum<DBTypes>();
                            connOption.ConnectionString = mr.Groups[2].ToString();
                        }
                        else
                        {
                            connOption.ConnectionRemark = mr.Groups[1].ToString();
                            connOption.ConnectionType = mr.Groups[2].ToString().DeEnum<DBTypes>();
                            connOption.ConnectionString = mr.Groups[3].ToString();
                        }
                        //解密
                        connOption.ConnectionString = DbKitExtensions.SqlConnEncryptOrDecrypt(connOption.ConnectionString);

                        //缓存
                        tmpDbConns.Add(connOption);
                        CacheTo.Set(ckey, tmpDbConns);
                    }

                    return Task.FromResult(mr.Success);
                });
            }
            else
            {
                //选择连接序号
                connOption = allDbConns[connIndex - 1];
            }

            //深拷贝构建新实例
            connOption.DeepCopyNewInstance = true;

            //选择数据库名
            if (connOption.ConnectionType != DBTypes.Oracle && isSelectDatabase)
            {
                var dataKit = DataKitTo.CreateDataKitInstance(connOption);
                var listDatabaseName = await dataKit.GetDatabaseNameOnly();

                var dv = 1;
                if (!string.IsNullOrWhiteSpace(connOption.DatabaseName))
                {
                    dv = listDatabaseName.IndexOf(connOption.DatabaseName) + 1;
                }

                var cri = ConsoleReadItem(TipSymbol($"选择数据库名"), listDatabaseName, dv);
                connOption.DatabaseName = listDatabaseName[cri - 1];
                connOption.SetConnDatabaseName(connOption.DatabaseName);
            }

            ConsoleTo.LogColor($"\r\n[{connOption.ConnectionRemark}]({connOption.ConnectionType}://{connOption.GetSafeConnectionString()})\r\n", ConsoleColor.Cyan);
            return true;
        });

        //复制一个新对象返回
        var newOption = new DbKitConnectionOption();
        newOption.ToDeepCopy(connOption);

        return newOption;
    }

    /// <summary>
    /// 显示读写数据库配置
    /// </summary>
    /// <param name="connOptionRead">读取源数据库</param>
    /// <param name="connOptionWrite">写入目标数据库</param>
    public static void ViewConnectionOption(DbKitConnectionOption connOptionRead, DbKitConnectionOption connOptionWrite)
    {
        ConsoleTo.LogColor($"\r\n[读取]: {connOptionRead.ConnectionType}://{connOptionRead.GetSafeConnectionString()}", ConsoleColor.Green);
        ConsoleTo.LogColor($"[写入]: {connOptionWrite.ConnectionType}://{connOptionWrite.GetSafeConnectionString()}\r\n", ConsoleColor.Yellow);
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="conn"></param>
    /// <param name="parserUA"></param>
    /// <returns></returns>
    public static async Task<int> NginxLogWriteTable(DataTable dt, string conn, bool parserUA = false)
    {
        var st = Stopwatch.StartNew();

        if (parserUA && dt.Columns.Contains("http_user_agent"))
        {
            var lockObject = new object();

            var dte = dt.AsEnumerable();
            var po = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
            };
            if (dt.Rows.Count > 100)
            {
                ConsoleTo.LogColor($"正在并行（{po.MaxDegreeOfParallelism}）解析 User-Agent，共 {dt.Rows.Count} 条");
            }
            Parallel.ForEach(dte, po, dr =>
            {
                var http_user_agent = dr["http_user_agent"].ToString();
                if (http_user_agent.Length > 1)
                {
                    var uap = new UAParsers(http_user_agent);
                    var botModel = uap.GetBot();
                    if (botModel != null)
                    {
                        lock (lockObject)
                        {
                            dr["ua_bot"] = botModel.Name;
                        }
                    }
                    else
                    {
                        lock (lockObject)
                        {
                            dr["ua_bot"] = "user";
                        }

                        var clientModel = uap.GetClient();
                        if (clientModel != null)
                        {
                            lock (lockObject)
                            {
                                dr["ua_browser_name"] = clientModel.Name;
                                dr["ua_browser_version"] = clientModel.Version;
                            }
                        }

                        var osModel = uap.GetOS();
                        if (osModel != null)
                        {
                            lock (lockObject)
                            {
                                dr["ua_system_name"] = osModel.Name;
                                dr["ua_system_version"] = osModel.Version;
                            }
                        }
                    }
                }
                else
                {
                    lock (lockObject)
                    {
                        dr["ua_bot"] = "bot";
                    }
                }
            });

            if (dt.Rows.Count > 100)
            {
                ConsoleTo.LogColor($"解析完成，解析耗时 {st.Elapsed}");
            }
        }

        st.Restart();

        if (dt.Rows.Count > 100)
        {
            ConsoleTo.LogColor($"开始写入 {dt.Rows.Count} 条");
        }

        var connOption = new DbKitConnectionOption
        {
            ConnectionType = DBTypes.ClickHouse,
            ConnectionString = conn
        };
        var dbKit = connOption.CreateDbInstance();
        var num = await dbKit.BulkCopy(dt);

        ConsoleTo.LogColor($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 已写入 {num} 条，当前写入耗时 {st.Elapsed}");

        return num;
    }

    /// <summary>
    /// 构建空表
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="isMore"></param>
    /// <returns></returns>
    public static DataTable NginxLogEmptyTable(string tableName = "access_more", bool isMore = false)
    {
        var dict = new Dictionary<string, Type>
            {
                { "time_local", typeof(DateTime) },
                { "remote_addr", typeof(string) },
                { "host", typeof(string) },
                { "path_name", typeof(string) },
                { "url", typeof(string) },
                { "http_method", typeof(string) },
                { "http_protocol", typeof(string) },
                { "status", typeof(int) },
                { "body_bytes_sent", typeof(long) },
                { "request_body", typeof(string) },
                { "http_referer", typeof(string) },
                { "http_user_agent", typeof(string) }
            };
        if (isMore)
        {
            dict.Add("ip_country", typeof(string));
            dict.Add("ip_city1", typeof(string));
            dict.Add("ip_city2", typeof(string));
            dict.Add("ip_isp", typeof(string));
            dict.Add("ua_bot", typeof(string));
            dict.Add("ua_browser_name", typeof(string));
            dict.Add("ua_browser_version", typeof(string));
            dict.Add("ua_system_name", typeof(string));
            dict.Add("ua_system_version", typeof(string));
        }

        var dt = new DataTable();
        foreach (var key in dict.Keys)
        {
            dt.Columns.Add(key, dict[key]);
        }
        dt.TableName = tableName;

        return dt;
    }
}

#endif