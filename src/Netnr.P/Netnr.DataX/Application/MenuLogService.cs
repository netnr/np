using System.Text.RegularExpressions;

namespace Netnr.DataX.Application
{
    /// <summary>
    /// Log
    /// </summary>
    public partial class MenuItemService
    {
        [Display(Name = "Nginx Log Write", Description = "读取 Nginx 日志写入 ClickHouse", GroupName = "Log",
            ShortName = "nlw --file [files] --format [format] --conn [conn] --table [table]", Prompt = @"ndx nlw --file '/package/log/access_*.log' --format '^\[$time_local(\S+ \S+)\] ""$remote_addr(.*?)"" ""$request(.*?)"" $status(\d+) $body_bytes_sent(\d+) ""$http_user_agent(.*?)""$' --conn 'Host=local.host;Port=8123;Username=root;Password=Abc1230..;Database=default' --table access_more")]
        public static async Task NginxLogWrite()
        {
            var logFiles = DXService.VarName("--file", "读取日志文件(如 /package/access_*.log)");

            var dvFormat = @"^\[$time_local(\S+ \S+)\] ""$remote_addr(.*?)"" ""$host(.*?)"" ""$request(.*?)"" $status(\d+) $body_bytes_sent(\d+) $request_body(.*) ""$http_referer(.*?)"" ""$http_user_agent(.*?)"" ""$http_x_forwarded_for(.*?)""$";
            var logFormat = DXService.VarName("--format", $"日志格式及匹配, 变量格式为 $var(regex), default\r\n{dvFormat}\r\n");
            if (string.IsNullOrWhiteSpace(logFormat))
            {
                logFormat = dvFormat;
            }

            //解析对应规则和索引
            string patternFormat = @"(\$[a-z_]+)";
            var formatIndexs = Regex.Matches(logFormat, patternFormat).ToList(); //变量对应的索引（用于取值）
            formatIndexs.Select(x => x.Groups[1].ToString())
                .OrderByDescending(x => x.Length).ToList()
                .ForEach(formatField =>
                {
                    //移除变量字段 $remote_addr(.*?) => (.*?)
                    logFormat = logFormat.Replace(formatField, "");
                });
            //预编译
            var regex = new Regex(logFormat, RegexOptions.Singleline | RegexOptions.Compiled);

            var dvConn = "Host=local.host;Port=8123;Username=root;Password=Abc1230..;Database=default";
            var writeConn = DXService.VarName("--conn", $"写入 ClickHouse 连接字符串, 如\r\n{dvConn}\r\n");

            var dvTable = "access_more";
            var writeTable = DXService.VarName("--table", $"写入表名(default {dvTable})");
            if (string.IsNullOrWhiteSpace(writeTable))
            {
                writeTable = dvTable;
            }

            //读取日志文件并排序
            var logPath = new DirectoryInfo(Path.GetDirectoryName(logFiles));
            if (logPath.Exists)
            {
                var files = logPath.GetFiles(Path.GetFileName(logFiles)).OrderBy(x => x.Name).ToList();
                DXService.Log($"找到日志文件 {files.Count} 个");

                if (files.Count > 0)
                {
                    var ipv4Path = Path.Combine(BaseTo.ProjectRootPath, "ud/qqwry.dat");
                    var ipv6Path = Path.Combine(BaseTo.ProjectRootPath, "ud/ipv6wry.db");

                    var readyIPQuery = File.Exists(ipv4Path) && File.Exists(ipv6Path);
                    if (!readyIPQuery)
                    {
                        DXService.Log($"IP 查询数据库文件不存在 {ipv4Path} {ipv6Path}", ConsoleColor.Red);
                    }
                    var ipq = new IPQuery(ipv4Path, ipv6Path);

                    //写入表数据
                    var isMore = true;
                    var dt = DXService.NginxLogEmptyTable(writeTable, isMore);

                    foreach (var file in files)
                    {
                        DXService.Log($"开始读取分析 {file.FullName}");

                        using var fs = file.OpenRead();
                        using var sr = new StreamReader(fs);

                        var st = Stopwatch.StartNew();
                        var st2 = Stopwatch.StartNew();

                        var badMatch = 0;
                        var rowCount = 0;
                        var writeCount = 0;
                        var listRow = new List<string>();
                        var listBad = new List<string>();

                        string row;
                        while ((row = await sr.ReadLineAsync()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(row))
                            {
                                continue;
                            }

                            rowCount++;
                            var rm = regex.Match(row);
                            if (rm.Success)
                            {
                                //填充行
                                var dr = dt.NewRow();
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    var gi = formatIndexs.FindIndex(x => x.Groups[1].ToString() == $"${dc.ColumnName}") + 1;
                                    if (gi > 0)
                                    {
                                        if (dc.DataType == typeof(string))
                                        {
                                            var val = rm.Groups[gi].ToString();
                                            dr[dc.ColumnName] = val;
                                        }
                                        else if (dc.DataType == typeof(DateTime))
                                        {
                                            dr[dc.ColumnName] = DateTime.ParseExact(rm.Groups[gi].ToString(), "dd/MMM/yyyy:HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture);
                                        }
                                        else if (dc.DataType == typeof(int))
                                        {
                                            var val = Convert.ToInt32(rm.Groups[gi].ToString());
                                            dr[dc.ColumnName] = val;
                                        }
                                        else if (dc.DataType == typeof(long))
                                        {
                                            var val = Convert.ToInt64(rm.Groups[gi].ToString());
                                            dr[dc.ColumnName] = val;
                                        }
                                    }
                                }

                                //拆分某项
                                var requests = rm.Groups[formatIndexs.FindIndex(x => x.Groups[1].ToString() == "$request") + 1].ToString().Split(' ');
                                if (requests.Length == 3)
                                {
                                    var http_method = requests[0];
                                    var http_protocol = requests[2];
                                    //删除恶意请求
                                    if (http_method.Contains("\\x") || http_protocol.Contains("\\x"))
                                    {
                                        http_method = "";
                                        http_protocol = "";
                                    }
                                    else
                                    {
                                        dr["url"] = requests[1];
                                        dr["path_name"] = requests[1].Split('?').First();
                                    }

                                    dr["http_method"] = http_method;
                                    dr["http_protocol"] = http_protocol;
                                }

                                //查询 IP 地址
                                if (readyIPQuery && isMore)
                                {
                                    var ipres = ipq.Search(dr["remote_addr"].ToString());
                                    if (!string.IsNullOrWhiteSpace(ipres.Addr))
                                    {
                                        var addrs = ipres.Addr.Split('\t');
                                        dr["ip_country"] = addrs[0];
                                        if (addrs.Length > 1)
                                        {
                                            dr["ip_city1"] = addrs[1];
                                        }
                                        if (addrs.Length > 2)
                                        {
                                            dr["ip_city2"] = addrs[2];
                                        }
                                    }
                                    dr["ip_isp"] = ipres.ISP;
                                }

                                dt.Rows.Add(dr.ItemArray);

                                //分批写入数据库
                                if (dt.Rows.Count == 1000000)
                                {
                                    writeCount += await DXService.NginxLogWriteTable(dt, writeConn, isMore);
                                    dt.Clear();
                                }
                            }
                            else
                            {
                                if (badMatch++ < 10)
                                {
                                    DXService.Log(row);
                                }

                                listBad.Add(row);
                                if (listBad.Count % 1000 == 0)
                                {
                                    if (!DXService.ConsoleReadBool($"已匹配失败 {listBad.Count} 条，是否继续"))
                                    {
                                        DXService.Log(string.Join(Environment.NewLine, listBad.Take(100)));
                                        break;
                                    }
                                }
                            }

                            if (st.Elapsed.TotalSeconds > 2)
                            {
                                st.Restart();
                                DXService.Log($"已读取 {rowCount} 条，已写入 {writeCount} 条，解析失败 {badMatch} 条，已耗时 {st2.Elapsed}");
                            }
                        }

                        //最后剩余
                        if (dt.Rows.Count > 0)
                        {
                            writeCount += await DXService.NginxLogWriteTable(dt, writeConn, isMore);
                            dt.Clear();
                        }

                        DXService.Log($"已读取 {rowCount} 条，已写入 {writeCount} 条，解析失败 {badMatch} 条");
                        DXService.Log($"Done! 分析 {file.FullName} 完成，共耗时 {st2.Elapsed}");

                        //当有存档目录 archive 时，移动该日志文件
                        var archivePath = Path.Combine(logPath.FullName, "archive");
                        if (Directory.Exists(archivePath))
                        {
                            var archiveFile = Path.Combine(archivePath, file.Name);
                            DXService.Log($"移动 {file.FullName} 到 {archiveFile}");
                            file.MoveTo(archiveFile, true);
                        }
                    }
                }
            }
            else
            {
                DXService.Log($"未找到日志文件 {logFiles}", ConsoleColor.Red);
            }
        }
    }
}
