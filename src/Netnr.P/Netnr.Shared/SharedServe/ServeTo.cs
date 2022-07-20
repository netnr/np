#if Full || Serve

using System.Net;
using System.Text;

namespace Netnr;

/// <summary>
/// 服务
/// </summary>
public class ServeTo
{
    /// <summary>
    /// 启动参数
    /// </summary>
    public static List<string> Args { get; set; } = Environment.GetCommandLineArgs().ToList();

    /// <summary>
    /// 获取启动参数值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetArgsVal(string key)
    {
        var keyIndex = Args.IndexOf(key);
        if (keyIndex != -1 && ++keyIndex < Args.Count)
        {
            var val = Args[keyIndex];
            if (!val.StartsWith("--"))
            {
                return val;
            }
        }
        return null;
    }

    const string version = "1.0.1"; // 2022-07-17
    public HttpListener Listener;
    public ServeOptions so;

    /// <summary>
    /// 服务配置
    /// </summary>
    public class ServeOptions
    {
        /// <summary>
        /// 服务端口，多个分号分隔
        /// </summary>
        public string Urls = "http://*:713/";
        /// <summary>
        /// 字符集
        /// </summary>
        public string Charset = "utf-8";
        /// <summary>
        /// 根目录
        /// </summary>
        public string Root;
        /// <summary>
        /// 根目录（自动）
        /// </summary>
        public DirectoryInfo RootDir;
        /// <summary>
        /// 默认页面
        /// </summary>
        public string Index = "index.html";
        /// <summary>
        /// 错误页面，history 模式配置 index.html
        /// </summary>
        public string Error404 = "404.html";
        /// <summary>
        /// 缺省后缀
        /// </summary>
        public string Suffix = ".html";
        /// <summary>
        /// 添加头部（跨域等）多个 || 分隔，例：access-control-allow-headers:*||access-control-allow-origin:*
        /// </summary>
        public string Headers;
        /// <summary>
        /// 头部解析（自动）
        /// </summary>
        public Dictionary<string, string> HeadersDic = new();
        /// <summary>
        /// Basic认证，格式为 user:pass
        /// </summary>
        public string Auth;
        /// <summary>
        /// 认证码（自动）
        /// </summary>
        public string AuthCode;

        /// <summary>
        /// 输出文件
        /// </summary>
        /// <param name="response"></param>
        /// <param name="file"></param>
        public void OutputFile(HttpListenerResponse response, FileInfo file)
        {
            response.ContentType = GetMIMEType(file.Extension);

            // charset
            if (response.ContentType.StartsWith("text/")
                || response.ContentType.EndsWith("/json")
                || response.ContentType.EndsWith("/javascript")
                || response.ContentType.EndsWith("/xml"))
            {
                response.ContentType = $"{response.ContentType}; charset={Charset}";
            }

            //output stream
            using var fs = File.OpenRead(file.FullName);
            fs.Position = 0;
            byte[] buffer = new byte[2048];
            int bytesRead;
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                response.OutputStream.Write(buffer, 0, bytesRead);
            }
        }

    }

    /// <summary>
    /// 构造实例
    /// </summary>
    /// <param name="options"></param>
    public ServeTo(ServeOptions options)
    {
        Listener = new HttpListener();

        if (string.IsNullOrWhiteSpace(options.Urls))
        {
            options.Urls = "http://*:713/";
        }
        if (string.IsNullOrWhiteSpace(options.Charset))
        {
            options.Charset = "utf-8";
        }
        if (string.IsNullOrWhiteSpace(options.Root) || !Directory.Exists(options.Root))
        {
            options.Root = Environment.CurrentDirectory;
        }
        options.RootDir = new DirectoryInfo(options.Root);

        if (string.IsNullOrWhiteSpace(options.Index))
        {
            options.Index = "index.html";
        }

        if (string.IsNullOrWhiteSpace(options.Error404))
        {
            options.Error404 = "404.html";
        }

        if (string.IsNullOrWhiteSpace(options.Suffix))
        {
            options.Suffix = ".html";
        }

        Console.WriteLine(string.Join("\r\n", Args));
        if (!string.IsNullOrWhiteSpace(options.Headers))
        {
            Console.WriteLine(options.Headers);
            options.Headers.Split("||").ToList().ForEach(item =>
            {
                Console.WriteLine(item);
                var kv = item.Split(':');
                options.HeadersDic.Add(kv[0].Trim(), kv[1].Trim());
            });
        }

        if (!string.IsNullOrWhiteSpace(options.Auth))
        {
            options.AuthCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(options.Auth));
        }

        options.Urls.Split(';').ToList().ForEach(url =>
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                Listener.Prefixes.Add(url.EndsWith('/') ? url : url + '/');
            }
        });

        so = options;
    }

    public void Start()
    {
        Console.WriteLine("\r\nStarting Serve ...\r\n");
        Listener.Start();
        Receive();
    }

    public void Stop()
    {
        Console.WriteLine("\r\nStop Serve ...");
        Listener.Stop();
    }

    private void Receive()
    {
        Listener.BeginGetContext(new AsyncCallback(ListenerCallback), Listener);
    }

    private void ListenerCallback(IAsyncResult result)
    {
        var context = Listener.EndGetContext(result);
        var request = context.Request;
        var response = context.Response;

        try
        {
            var ip = request.Headers.GetValues("X-Forwarded-For")?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = request.RemoteEndPoint.Address.ToString();
            }

            //record
            Console.WriteLine($"[{DateTime.Now}] {ip} {request.HttpMethod} {request.Url}");

            //headers
            if (so.HeadersDic.Count > 0)
            {
                foreach (var key in so.HeadersDic.Keys)
                {
                    response.Headers.Remove(key);
                    response.Headers.Add(key, so.HeadersDic[key]);
                }
            }

            //auth
            var isAuth = true;
            if (!string.IsNullOrWhiteSpace(so.Auth))
            {
                var auth = request.Headers.GetValues("Authorization")?.FirstOrDefault() ?? "";
                isAuth = auth.EndsWith(so.AuthCode);
            }

            if (isAuth)
            {
                response.StatusCode = (int)HttpStatusCode.OK;

                var pathname = request.Url?.LocalPath ?? "/";
                var path = pathname.Trim('/');
                switch (request.HttpMethod.ToUpper())
                {
                    case "HEAD":
                        {
                            response.OutputStream.Write(Array.Empty<byte>(), 0, 0);
                        }
                        break;
                    case "GET":
                    case "POST":
                        {
                            var isDir = pathname.EndsWith("/");

                            //index
                            if (isDir && !string.IsNullOrWhiteSpace(so.Index))
                            {
                                if (so.RootDir.GetFiles(Path.Combine(path, so.Index)).Any())
                                {
                                    path = Path.Combine(path, so.Index);
                                    isDir = false;
                                }
                            }

                            if (isDir)
                            {
                                // directory/
                                var listOut = new List<string>();

                                var ua = request.Headers["User-Agent"]?.ToString();
                                var isText = ua.StartsWith("curl/") || ua.Contains("WindowsPowerShell/");
                                if (isText != true)
                                {
                                    listOut.Add($"<html><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" /><title>Index of {pathname}</title>");
                                    listOut.Add(@"<style>table{border-collapse:collapse}table td,table th{padding:3px 10px;white-space:nowrap;border:1px #ddd solid}table td{text-align:right}table td:first-child{text-align:left}a{color:#1ba1e2;text-decoration:none}a:hover{color:#13709e;text-decoration:underline}</style>");
                                    listOut.Add($"</head><body>");
                                    listOut.Add($"<h1>Index of {pathname}</h1><table>");
                                    listOut.Add($"<tr><th>Name</th><th>Size</th><th>Last Modified</th></tr>");
                                    listOut.Add($"<tr><td colspan=\"3\"><a href=\"../\">../</a></td>");
                                }
                                else
                                {
                                    listOut.Add($"\r\n\r\nIndex of {pathname}");
                                }

                                var fsInfos = so.RootDir.GetFileSystemInfos(string.IsNullOrWhiteSpace(path) ? "" : path + '/', SearchOption.TopDirectoryOnly);
                                foreach (var fsInfo in fsInfos)
                                {
                                    if (!fsInfo.Attributes.HasFlag(FileAttributes.Hidden))
                                    {
                                        if (fsInfo.Attributes.HasFlag(FileAttributes.Directory))
                                        {
                                            if (isText == true)
                                            {
                                                listOut.Add($"{fsInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}  {"-",10}  {fsInfo.Name}/");
                                            }
                                            else
                                            {
                                                listOut.Add($"<tr><td><a href=\"{fsInfo.Name}/\">{fsInfo.Name}/</a></td><td>-</td><td>{fsInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}</td></tr>");
                                            }
                                        }
                                    }
                                }
                                foreach (var fsInfo in fsInfos)
                                {
                                    if (!fsInfo.Attributes.HasFlag(FileAttributes.Hidden))
                                    {
                                        if (!fsInfo.Attributes.HasFlag(FileAttributes.Directory))
                                        {
                                            var itemSize = FormatByteSize(new FileInfo(fsInfo.FullName).Length).TrimEnd('B').TrimEnd('i');

                                            if (isText == true)
                                            {
                                                listOut.Add($"{fsInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}  {itemSize,10}  {fsInfo.Name}");
                                            }
                                            else
                                            {
                                                listOut.Add($"<tr><td><a href=\"{fsInfo.Name}\">{fsInfo.Name}</a></td><td>{itemSize}</td><td>{fsInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}</td></tr>");
                                            }
                                        }
                                    }
                                }
                                if (isText != true)
                                {
                                    listOut.Add($"</table></body></html>");
                                }

                                OutputText(response, string.Join("\r\n", listOut));
                            }
                            else
                            {
                                // file

                                var file = so.RootDir.GetFiles(path).FirstOrDefault();
                                if (file != null)
                                {
                                    so.OutputFile(response, file);
                                }
                                else if (so.RootDir.GetDirectories(path).Any())
                                {
                                    // 301 directory/

                                    response.StatusCode = (int)HttpStatusCode.Redirect;
                                    response.Redirect($"/{path}/");
                                }
                                else
                                {
                                    response.StatusCode = (int)HttpStatusCode.NotFound;
                                }
                            }
                        }
                        break;
                    case "PUT":
                        {
                            //upload

                            var saveFile = path.Replace("*", "");
                            if (request.HasEntityBody && !string.IsNullOrWhiteSpace(saveFile))
                            {
                                while (File.Exists(Path.Combine(so.RootDir.FullName, saveFile)))
                                {
                                    saveFile = $"{Guid.NewGuid().ToString("N").ToLower()[..4]}_{path}";
                                }
                                var saveFull = Path.Combine(so.RootDir.FullName, saveFile);

                                var saveDir = Path.GetDirectoryName(saveFull);
                                if (!Directory.Exists(saveDir))
                                {
                                    Directory.CreateDirectory(saveDir);
                                }

                                using var fs = File.Create(saveFull);
                                request.InputStream.CopyTo(fs);
                                fs.Flush();

                                var origin = request.Url.AbsoluteUri.TrimEnd(request.Url.AbsolutePath.ToCharArray());
                                OutputText(response, $"\r\nSaved {origin}/{saveFile}");
                            }
                        }
                        break;
                    case "DELETE":
                        {
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                //delete directory

                                if (pathname.EndsWith('/') && request.Url.Query == "?force")
                                {
                                    var delPath = so.RootDir.GetDirectories(path, SearchOption.AllDirectories).FirstOrDefault();
                                    if (delPath != null)
                                    {
                                        delPath.Delete(true);
                                    }
                                }
                                else
                                {
                                    //delete file

                                    var delPath = Path.Combine(so.RootDir.FullName, path);
                                    if (File.Exists(delPath))
                                    {
                                        File.Delete(delPath);

                                        OutputText(response, $"\r\nDeleted {request.Url}");
                                    }
                                    else
                                    {
                                        throw new DirectoryNotFoundException();
                                    }
                                }
                            }
                        }
                        break;
                }

                //404
                if (response.StatusCode == 404)
                {
                    FileInfo file = null;

                    if (!string.IsNullOrEmpty(so.Suffix) && !path.EndsWith(so.Suffix))
                    {
                        file = so.RootDir.GetFiles(path + so.Suffix).FirstOrDefault();
                    }

                    if (file == null && !string.IsNullOrWhiteSpace(so.Error404))
                    {
                        file = so.RootDir.GetFiles(so.Error404).FirstOrDefault();
                    }

                    if (file != null)
                    {
                        so.OutputFile(response, file);
                    }
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ForegroundColor = ConsoleColor.White;

            OutputText(response, "404 Not Found", HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ForegroundColor = ConsoleColor.White;

            OutputText(response, ex.Message, HttpStatusCode.ServiceUnavailable);
        }

        //response.OutputStream.Write(Array.Empty<byte>(), 0, 0);
        response.OutputStream.Close();

        Receive();
    }

    /// <summary>
    /// 快速启动
    /// </summary>
    /// <param name="urls"></param>
    /// <param name="rootDir"></param>
    /// <param name="user_pass"></param>
    public static void FastStart()
    {
        var title = $"NS(Netnr.Serve) v{version}";
        Console.Title = title;

        var options = new ServeOptions();

        //静默
        if (Args.Count > 1)
        {
            options.Urls = GetArgsVal("--urls");
            options.Root = GetArgsVal("--root");
            options.Index = GetArgsVal("--index");
            options.Error404 = GetArgsVal("--404");
            options.Suffix = GetArgsVal("--suffix");
            options.Charset = GetArgsVal("--charset");
            options.Headers = GetArgsVal("--headers");
            options.Auth = GetArgsVal("--auth");
        }
        else
        {
            Console.Write($"urls(default: {options.Urls}): ");
            options.Urls = Console.ReadLine().Trim();

            Console.Write($"root(default: {Environment.CurrentDirectory}): ");
            options.Root = Console.ReadLine().Trim();

            Console.Write($"index(default: {options.Index}): ");
            options.Index = Console.ReadLine().Trim();

            Console.Write($"404(default: {options.Error404}): ");
            options.Error404 = Console.ReadLine().Trim();

            Console.Write($"suffix(default: {options.Suffix}): ");
            options.Suffix = Console.ReadLine().Trim();

            Console.Write($"charset(default: {options.Charset}): ");
            options.Charset = Console.ReadLine().Trim();

            Console.Write($"headers: ");
            options.Headers = Console.ReadLine().Trim();

            Console.Write($"auth: ");
            options.Auth = Console.ReadLine();
        }

        var serve = new ServeTo(options);
        serve.Start();

        var url = serve.Listener.Prefixes.First();
        Console.WriteLine($"\r\nPlease visit {string.Join(" ", serve.Listener.Prefixes)}");
        var authMode = string.IsNullOrWhiteSpace(options.Auth) ? "anonymous" : "Basic " + options.Auth;
        Console.WriteLine($"Authorization mode: {authMode}");
        Console.WriteLine($"Enter \"exit\" stop\r\n");

        string line;
        while ((line = Console.ReadLine()) != "exit")
        {
            switch (line.ToLower())
            {
                default:
                    {
                        Console.WriteLine($"{title}\r\n");
                        Console.WriteLine($"Silent start\r\n--urls {options.Urls} --root {options.Root} --index {options.Index} --404 {options.Error404} --suffix {options.Suffix} --charset {options.Charset} --headers access-control-allow-headers:*||access-control-allow-origin:* --auth user:pass\r\n");
                        Console.WriteLine($"List\r\ncurl {url}\r\ncurl {url} -u user:pass\r\n(iwr {url}).content\r\n");
                        Console.WriteLine($"Download\r\ncurl {url}/file.exe -O\r\niwr {url}/file.exe -outfile file.exe\r\n");
                        Console.WriteLine($"Upload\r\ncurl {url} -T file.ext\r\ncurl {url}dir/rename.ext -T file.ext\r\n");
                        Console.WriteLine($"Delete\r\ncurl {url}file.ext -X delete\r\ncurl {url}dir/?force -X delete\r\niwr {url}file.ext -method delete\r\n");
                    }
                    break;
            }
        }

        serve.Stop();
    }

    /// <summary>
    /// 字节可视化
    /// </summary>
    /// <param name="size">字节大小</param>
    /// <param name="keep">保留</param>
    /// <param name="rate"></param>
    /// <param name="space">间隔</param>
    /// <returns></returns>
    public static string FormatByteSize(double size, int keep = 2, int rate = 1024, string space = "")
    {
        if (Math.Abs(size) < rate)
        {
            return $"{Math.Round(size, keep)}{space}B";
        }

        string[] units = rate == 1000
            ? new[] { "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" }
            : new[] { "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

        var u = -1;
        var r = Math.Pow(10, keep);

        do
        {
            size /= rate;
            ++u;
        } while (Math.Round(Math.Abs(size) * r) / r >= rate && u < units.Length - 1);

        var result = $"{Math.Round(size, keep)}{space}{units[u]}";
        return result;
    }

    /// <summary>
    /// 输出文本
    /// </summary>
    /// <param name="response"></param>
    /// <param name="text"></param>
    /// <param name="statusCode"></param>
    /// <param name="contentType"></param>
    public static void OutputText(HttpListenerResponse response, string text, HttpStatusCode statusCode = HttpStatusCode.OK, string contentType = "text/html; charset=utf-8")
    {
        response.StatusCode = (int)statusCode;
        response.ContentType = contentType;
        if (!string.IsNullOrEmpty(text))
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }

    /// <summary>
    /// GET MIME-Type
    /// </summary>
    /// <param name="pathOrExtension"></param>
    /// <returns></returns>
    public static string GetMIMEType(string pathOrExtension)
    {
        if (string.IsNullOrWhiteSpace(pathOrExtension))
        {
            return "";
        }
        if (!pathOrExtension.StartsWith('.'))
        {
            pathOrExtension = Path.GetExtension(pathOrExtension);
        }

        if (!MIMEType.TryGetValue(pathOrExtension, out string mimeType))
        {
            mimeType = "application/octet-stream";
        }

        return mimeType;
    }

    /// <summary>
    /// MIME Type
    /// ref https://github.com/samuelneff/MimeTypeMap
    /// </summary>
    public static Dictionary<string, string> MIMEType { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".ai", "application/postscript"},
        {".aif", "audio/aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/mpeg"},
        {".anx", "application/annodex"},
        {".apk", "application/vnd.android.package-archive"},
        {".apng", "image/apng"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avci", "image/avci"},
        {".avcs", "image/avcs"},
        {".avi", "video/x-msvideo"},
        {".avif", "image/avif"},
        {".avifs", "image/avif-sequence"},
        {".axa", "audio/annodex"},
        {".axs", "application/olescript"},
        {".axv", "video/annodex"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".cfg", "text/plain"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmd", "text/plain"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".czx", "application/x-czx"},
        {".cxx", "text/plain"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".divx", "video/divx"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwg", "application/acad"},
        {".dxf", "application/x-dxf"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emf", "image/emf"},
        {".eot", "application/vnd.ms-fontobject"},
        {".eps", "application/postscript"},
        {".es", "application/ecmascript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/vnd.microsoft.portable-executable"},
        {".exe.config", "text/xml"},
        {".f4v", "video/mp4"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "application/xml"},
        {".flac", "audio/flac"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".geojson", "application/geo+json"},
        {".gif", "image/gif"},
        {".gpx", "application/gpx+xml"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".heic", "image/heic"},
        {".heics", "image/heic-sequence"},
        {".heif", "image/heif"},
        {".heifs", "image/heif-sequence"},
        {".hhc", "application/x-oleobject"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxk", "application/xml"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ical", "text/calendar"},
        {".icalendar", "text/calendar"},
        {".ico", "image/x-icon"},
        {".ics", "text/calendar"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".ifb", "text/calendar"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".ini", "text/plain"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".key", "application/vnd.apple.keynote"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".log", "text/plain"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mbox", "application/mbox"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mk", "text/plain"},
        {".mk3d", "video/x-matroska-3d"},
        {".mka", "audio/x-matroska"},
        {".mkv", "video/x-matroska"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msg", "application/vnd.ms-outlook"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxf", "application/mxf"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".numbers", "application/vnd.apple.numbers"},
        {".nws", "message/rfc822"},
        {".oda", "application/oda"},
        {".odb", "application/vnd.oasis.opendocument.database"},
        {".odc", "application/vnd.oasis.opendocument.chart"},
        {".odf", "application/vnd.oasis.opendocument.formula"},
        {".odg", "application/vnd.oasis.opendocument.graphics"},
        {".odh", "text/plain"},
        {".odi", "application/vnd.oasis.opendocument.image"},
        {".odl", "text/plain"},
        {".odm", "application/vnd.oasis.opendocument.text-master"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/vnd.oasis.opendocument.spreadsheet"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".oga", "audio/ogg"},
        {".ogg", "audio/ogg"},
        {".ogv", "video/ogg"},
        {".ogx", "application/ogg"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".opus", "audio/ogg"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".otf", "application/font-sfnt"},
        {".otg", "application/vnd.oasis.opendocument.graphics-template"},
        {".oth", "application/vnd.oasis.opendocument.text-web"},
        {".otp", "application/vnd.oasis.opendocument.presentation-template"},
        {".ots", "application/vnd.oasis.opendocument.spreadsheet-template"},
        {".ott", "application/vnd.oasis.opendocument.text-template"},
        {".oxps", "application/oxps"},
        {".oxt", "application/vnd.openofficeorg.extension"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pages", "application/vnd.apple.pages"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pdf", "application/pdf"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psess", "application/xml"},
        {".pst", "application/vnd.ms-outlook"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".reg", "text/plain"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".rmvb", "application/vnd.rn-realmedia-vbr"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".scr", "text/plain"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".skp", "application/x-koan"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".sql", "application/sql"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".spx", "audio/ogg"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".svg", "image/svg+xml"},
        {".swf", "application/x-shockwave-flash"},
        {".step", "application/step"},
        {".stp", "application/step"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/font-sfnt"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtt", "text/vtt"},
        {".vtx", "application/vnd.visio"},
        {".wasm", "application/wasm"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webm", "video/webm"},
        {".webp", "image/webp"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".woff", "application/font-woff"},
        {".woff2", "application/font-woff2"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xss", "application/xml"},
        {".xspf", "application/xspf+xml"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/zip"}
    };
}

#endif