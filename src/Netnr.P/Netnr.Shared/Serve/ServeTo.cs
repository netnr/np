#if Full || Serve

using System.Net;
using System.Text;
using System.IO.Compression;
using System.Net.Sockets;

namespace Netnr;

/// <summary>
/// 服务
/// </summary>
public class ServeTo
{
    public HttpListener _listener;
    public string _defaultUri = "http://*:7713/"; // 避免linux系统低于1024需要root权限
    public string _defaultRoot = Environment.CurrentDirectory;
    public string _defaultIndexFile = "index.html";
    public string _defaultExtension = ".html";
    public string _defaultCharset = "utf-8";
    public Encoding _defaultEncode = Encoding.UTF8;
    public string _defaultErrorPage = "404.html";

    public string _defaultReadOnly = "false";
    public string _defaultBasicAuth;
    public string _defaultHeaders = $"access-control-allow-origin:*||x-server:Netnr||x-version:{Environment.Version}";
    public Dictionary<string, string> _headers = [];

    public class ServeModel
    {
        public string UriPrefix { get; set; }
        public string DefaultRoot { get; set; }
        public string DefaultIndexFile { get; set; }
        public string DefaultExtension { get; set; }
        public string DefaultCharset { get; set; }
        public string DefaultErrorPage { get; set; }
        public string DefaultReadOnly { get; set; }
        public string DefaultBasicAuth { get; set; }
        public string DefaultHeaders { get; set; }
    }

    public void Init(ServeModel model)
    {
        _listener = new HttpListener();

        if (!string.IsNullOrWhiteSpace(model.UriPrefix))
        {
            if (!model.UriPrefix.EndsWith('/'))
            {
                model.UriPrefix += '/';
            }
            _defaultUri = model.UriPrefix;
        }
        _listener.Prefixes.Add(_defaultUri);

        if (!string.IsNullOrWhiteSpace(model.DefaultRoot))
        {
            _defaultRoot = model.DefaultRoot;
        }
        _defaultRoot = new DirectoryInfo(_defaultRoot).FullName;

        if (!string.IsNullOrWhiteSpace(model.DefaultIndexFile))
        {
            _defaultIndexFile = model.DefaultIndexFile;
        }

        if (!string.IsNullOrWhiteSpace(model.DefaultExtension))
        {
            _defaultExtension = model.DefaultExtension;
        }

        if (!string.IsNullOrWhiteSpace(model.DefaultCharset))
        {
            _defaultCharset = model.DefaultCharset;
        }
        _defaultEncode = Encoding.GetEncoding(_defaultCharset);

        if (!string.IsNullOrWhiteSpace(model.DefaultErrorPage))
        {
            _defaultErrorPage = model.DefaultErrorPage;
        }

        if (!string.IsNullOrWhiteSpace(model.DefaultReadOnly))
        {
            _defaultReadOnly = model.DefaultReadOnly;
        }
        _defaultBasicAuth = model.DefaultBasicAuth;

        if (!string.IsNullOrEmpty(model.DefaultHeaders))
        {
            _defaultHeaders = model.DefaultHeaders;
        }
        _defaultHeaders.Split("||").Where(x => x.Contains(':')).ToList().ForEach(item =>
        {
            var kv = item.Split(":");
            _headers.Add(kv[0], kv[1]);
        });
    }

    public async Task StartAsync()
    {
        Console.WriteLine($"\r\nStarting Serve [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\r\n");

        try
        {
            _listener.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        Console.WriteLine(string.Join(" , ", [
            $"Visit {string.Join(" ", _listener.Prefixes)}",
            $"Auth {(string.IsNullOrWhiteSpace(_defaultBasicAuth) ? "anonymous" : _defaultBasicAuth)}",
            $"ReadOnly {_defaultReadOnly}\r\n"
        ]));

        var localIP = await GetAddressInterNetwork();
        var url = _listener.Prefixes.First().Replace("*", localIP?.Address.ToString() ?? "localhost");
        Console.WriteLine($"----- Silent startup\r\n--urls {_defaultUri} --root {_defaultRoot} --index {_defaultIndexFile} --404 {_defaultErrorPage} --suffix {_defaultExtension} --charset {_defaultCharset} --readonly {_defaultReadOnly} --auth user:pass --headers {_defaultHeaders}\r\n");
        Console.WriteLine($"----- List\r\ncurl {url}\r\ncurl {url} -u user:pass\r\n(iwr {url}).content\r\n");
        Console.WriteLine($"----- Download\r\ncurl {url}file.exe -O\r\ncurl {url}dir/?zip\r\niwr {url}file.exe -outfile file.exe\r\n");
        if (!_defaultReadOnly.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"----- Upload\r\ncurl {url} -T file.ext\r\ncurl {url}dir/rename.ext -T file.ext\r\niwr {url}dir/rename.ext -method put -infile file.ext\r\n");
            Console.WriteLine($"----- Delete\r\ncurl {url}file.ext -X delete\r\niwr {url}file.ext -method delete\r\n");
        }

        while (true)
        {
            var context = await _listener.GetContextAsync();
            _ = Task.Run(() => ProcessRequestAsync(context)); // Fire and forget
            //_ = ProcessRequestAsync(context); // Fire and forget 不支持并发
        }
    }

    /// <summary>
    /// 快速启动
    /// </summary>
    public static async Task QuickStart()
    {
        var title = $"NS(Netnr.Serve)";
        Console.Title = title;

        //参数
        List<KeyValuePair<string, string>> args = [];
        var clArgs = Environment.GetCommandLineArgs();
        for (int i = 0; i < clArgs.Length; i++)
        {
            var key = clArgs[i];
            if (key.StartsWith('-'))
            {
                var val = i + 1 < clArgs.Length ? clArgs[i + 1] : "";
                args.Add(new KeyValuePair<string, string>(key, val.StartsWith('-') ? "" : val));
            }
        }

        var model = new ServeModel();
        var serverInstance = new ServeTo();

        //静默
        if (clArgs.Length > 1)
        {
            model.UriPrefix = args.FirstOrDefault(x => x.Key == "--urls").Value;
            model.DefaultRoot = args.FirstOrDefault(x => x.Key == "--root").Value;
            model.DefaultIndexFile = args.FirstOrDefault(x => x.Key == "--index").Value;
            model.DefaultExtension = args.FirstOrDefault(x => x.Key == "--suffix").Value;
            model.DefaultCharset = args.FirstOrDefault(x => x.Key == "--charset").Value;
            model.DefaultErrorPage = args.FirstOrDefault(x => x.Key == "--404").Value;

            model.DefaultReadOnly = args.FirstOrDefault(x => x.Key == "--readonly").Value;
            model.DefaultBasicAuth = args.FirstOrDefault(x => x.Key == "--auth").Value;
            model.DefaultHeaders = args.FirstOrDefault(x => x.Key == "--headers").Value;
        }
        else
        {
            Console.Write($"urls(default: {serverInstance._defaultUri}): ");
            model.UriPrefix = Console.ReadLine().Trim();

            Console.Write($"root(default: {serverInstance._defaultRoot}): ");
            model.DefaultRoot = Console.ReadLine().Trim();

            Console.Write($"index(default: {serverInstance._defaultIndexFile}): ");
            model.DefaultIndexFile = Console.ReadLine().Trim();

            Console.Write($"404(default: {serverInstance._defaultErrorPage}): ");
            model.DefaultErrorPage = Console.ReadLine().Trim();

            Console.Write($"suffix(default: {serverInstance._defaultExtension}): ");
            model.DefaultExtension = Console.ReadLine().Trim();

            Console.Write($"charset(default: {serverInstance._defaultCharset}): ");
            model.DefaultCharset = Console.ReadLine().Trim();

            Console.Write($"readonly(default: {serverInstance._defaultReadOnly}): ");
            model.DefaultReadOnly = Console.ReadLine().Trim();

            Console.Write($"auth(user:pass): ");
            model.DefaultBasicAuth = Console.ReadLine();

            Console.Write($"headers(k1:v1||k2:v2): ");
            model.DefaultHeaders = Console.ReadLine().Trim();
        }

        serverInstance.Init(model);
        await serverInstance.StartAsync();
    }

    public void Stop()
    {
        Console.WriteLine("\r\nStop Serve ...");
        _listener.Stop();
    }

    private async Task ProcessRequestAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        var ip = request.Headers.GetValues("X-Forwarded-For")?.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = request.RemoteEndPoint.Address.ToString();
        }
        //record
        Console.WriteLine($"[{DateTime.Now}] - {ip} - {request.HttpMethod} - {request.Url}");

        // Basic authentication
        if (!string.IsNullOrEmpty(_defaultBasicAuth))
        {
            var authHeader = request.Headers["Authorization"];
            if (authHeader == null || !authHeader.StartsWith("Basic ") || authHeader.Substring(6) != Convert.ToBase64String(Encoding.UTF8.GetBytes(_defaultBasicAuth)))
            {
                response.AddHeader("WWW-Authenticate", "Basic realm=\"Authorization Required\"");
                ResponseText(response, "Unauthorized", 401);
                return;
            }
        }

        // Add custom headers
        foreach (var header in _headers)
        {
            response.Headers.Add(header.Key, header.Value);
        }

        var pathname = request.Url.LocalPath.TrimStart('/');
        var filePath = pathname == "" ? _defaultRoot : Path.Combine(_defaultRoot, pathname);
        var directoryPath = pathname == "" ? _defaultRoot : Path.GetDirectoryName(filePath);

        // Check if the directory path is safe
        if (!Path.GetFullPath(directoryPath).StartsWith(_defaultRoot))
        {
            ResponseText(response, $"Unsafe directory path: {directoryPath}");
            return;
        }

        if (request.HttpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase))
        {
            if (_defaultReadOnly.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                ResponseText(response, $"Read-only: {request.Url.LocalPath}");
                return;
            }

            var fileName = Path.GetFileName(filePath);
            if (string.IsNullOrEmpty(fileName) || fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || directoryPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                ResponseText(response, $"Path error: {request.Url.LocalPath}");
                return;
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await request.InputStream.CopyToAsync(fs);
            }

            ResponseText(response, $"Upload success: {request.Url.LocalPath}", 200);
            return;
        }

        // Directory
        if (Directory.Exists(filePath))
        {
            //zip
            if (request.Url.PathAndQuery.EndsWith("?zip"))
            {
                var zipName = $"{request.Url.Host}_{request.Url.LocalPath.Trim('/')}".Replace('/', '_');
                response.ContentType = "application/zip";
                response.AddHeader("Content-Disposition", $"attachment; filename={zipName}.zip");

                using (var zipArchive = new ZipArchive(response.OutputStream, ZipArchiveMode.Create, true))
                {
                    var directoryInfo = new DirectoryInfo(filePath);
                    var files = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        zipArchive.CreateEntryFromFile(file.FullName, file.FullName.Substring(_defaultRoot.Length + 1), CompressionLevel.NoCompression);
                    }
                }

                response.Close();
                return;
            }

            var indexFilePath = Path.Combine(filePath, _defaultIndexFile);
            if (File.Exists(indexFilePath))
            {
                filePath = indexFilePath;
            }
            else
            {
                var html = GenerateDirectoryListingHtml(filePath, request);

                ResponseText(response, html, 200, htmlEncode: false);
                return;
            }
        }

        // File
        if (File.Exists(filePath) || (Path.GetExtension(filePath) == "" && File.Exists(filePath += _defaultExtension)))
        {
            if (!MIMEType.TryGetValue(Path.GetExtension(filePath), out string mimeType))
            {
                mimeType = "application/octet-stream";
            }
            response.ContentType = mimeType;

            // charset
            if (response.ContentType.StartsWith("text/")
                || response.ContentType.EndsWith("/json")
                || response.ContentType.EndsWith("/javascript")
                || response.ContentType.EndsWith("/xml"))
            {
                response.ContentType += $"; charset={_defaultCharset}";
            }

            try
            {
                if (request.HttpMethod.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
                {
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    response.ContentLength64 = fs.Length;
                }
                else if (request.HttpMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                {
                    if (_defaultReadOnly.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        ResponseText(response, $"Read-only: {request.Url.LocalPath}");
                        return;
                    }

                    File.Delete(filePath);

                    ResponseText(response, $"Delete success: {request.Url.LocalPath}", 200);
                    return;
                }
                else
                {
                    // GET POST ...
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var rangeHeader = request.Headers["Range"];
                    if (string.IsNullOrEmpty(rangeHeader))
                    {
                        response.ContentLength64 = fs.Length;
                        try
                        {
                            await fs.CopyToAsync(response.OutputStream);
                        }
                        catch (HttpListenerException ex)
                        {
                            if (ex.ErrorCode != 64)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    else
                    {
                        var range = ParseRangeHeader(rangeHeader, fs.Length);
                        response.StatusCode = 206;
                        long lengthToRead = range.Item2 - range.Item1 + 1;
                        response.ContentLength64 = lengthToRead;
                        response.AddHeader("Content-Range", $"bytes {range.Item1}-{range.Item2}/{fs.Length}");
                        var buffer = new byte[8192];
                        fs.Seek(range.Item1, SeekOrigin.Begin);
                        int bytesRead;
                        long totalBytesRead = 0;
                        while ((bytesRead = await fs.ReadAsync(buffer)) > 0 && totalBytesRead < lengthToRead)
                        {
                            totalBytesRead += bytesRead;
                            if (totalBytesRead > lengthToRead)
                            {
                                bytesRead -= (int)(totalBytesRead - lengthToRead);
                            }
                            await response.OutputStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                        }
                    }
                }
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode != 64)
                {
                    Console.WriteLine(ex);
                    ResponseText(response, ex.Message);
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ResponseText(response, ex.Message);
                return;
            }
        }
        else
        {
            var errorFilePath = Path.Combine(_defaultRoot, _defaultErrorPage);
            if (File.Exists(errorFilePath))
            {
                using var fs = new FileStream(errorFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                response.StatusCode = 404;
                response.ContentLength64 = fs.Length;
                await fs.CopyToAsync(response.OutputStream);
            }
            else
            {
                ResponseText(response, $"Not found: {request.Url.LocalPath}", 404);
                return;
            }
        }

        response.Close();
    }

    /// <summary>
    /// 输出文本
    /// </summary>
    /// <param name="response"></param>
    /// <param name="text"></param>
    /// <param name="statusCode"></param>
    /// <param name="htmlEncode"></param>
    /// <param name="contentType"></param>
    private void ResponseText(HttpListenerResponse response, string text, int statusCode = 500, bool htmlEncode = true, string contentType = null)
    {
        response.StatusCode = statusCode;
        response.ContentType = string.IsNullOrWhiteSpace(contentType) ? $"text/html; charset={_defaultCharset}" : contentType;
        if (!string.IsNullOrEmpty(text))
        {
            if (statusCode >= 300)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(text);
                Console.WriteLine("");
                Console.ResetColor();
            }

            var buffer = _defaultEncode.GetBytes($"\r\n{(htmlEncode ? WebUtility.HtmlEncode(text) : text)}\r\n");
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
        response.Close();
    }

    private static ValueTuple<long, long> ParseRangeHeader(string rangeHeader, long fileSize)
    {
        var range = rangeHeader.Replace("bytes=", "").Split('-');
        var start = string.IsNullOrEmpty(range[0]) ? 0 : Convert.ToInt64(range[0]);
        var end = string.IsNullOrEmpty(range[1]) ? fileSize - 1 : Convert.ToInt64(range[1]);
        return new ValueTuple<long, long>(start, end);
    }

    private static string GenerateDirectoryListingHtml(string directoryPath, HttpListenerRequest request)
    {
        var files = Directory.GetFiles(directoryPath);
        var directories = Directory.GetDirectories(directoryPath);

        var userAgent = request.Headers["User-Agent"];
        if (string.IsNullOrEmpty(userAgent) || userAgent.StartsWith("curl/") || userAgent.StartsWith("WindowsPowerShell/"))
        {
            // Command line friendly list
            var sb = new StringBuilder();
            sb.AppendLine($"\r\n\r\nIndex of {request.Url.LocalPath}");
            foreach (var directory in directories)
            {
                var dirName = Path.GetFileName(directory);
                var lastModified = Directory.GetLastWriteTime(directory).ToString("yyyy-MM-dd HH:mm:ss");
                sb.AppendLine($"{lastModified}  {"-",10}  {dirName}/");
            }
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var lastModified = File.GetLastWriteTime(file).ToString("yyyy-MM-dd HH:mm:ss");
                var size = new FileInfo(file).Length;

                sb.AppendLine($"{lastModified}  {FormatByte(size).TrimEnd('B').TrimEnd('i'),10}  {fileName}");
            }
            return sb.ToString();
        }
        else
        {
            // FTP style HTML list
            var html = new StringBuilder("<html><head>");
            html.Append($"<title>Index of {request.Url.LocalPath}</title>");
            html.Append("<style>table{border-collapse:collapse}table td,table th{padding:3px 10px;white-space:nowrap;border:1px #ddd solid}table td{text-align:right}table td:first-child{text-align:left}a{color:#1ba1e2;text-decoration:none}a:hover{color:#13709e;text-decoration:underline}</style>");
            html.Append("</head><body>");
            html.Append($"<h1>Index of {request.Url.LocalPath}</h1>");
            html.Append("<table><tr><th>Name</th><th>Size</th><th>Last Modified</th></tr>");
            html.Append("<tr><td colspan=\"3\"><a href=\"../\">../</a></td>");
            foreach (var directory in directories)
            {
                var dirName = Path.GetFileName(directory);
                var lastModified = Directory.GetLastWriteTime(directory).ToString("yyyy-MM-dd HH:mm:ss");
                html.Append($"<tr><td><a href=\"{dirName}/\">{dirName}/</a></td><td>-</td><td>{lastModified}</td></tr>");
            }
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var lastModified = File.GetLastWriteTime(file).ToString("yyyy-MM-dd HH:mm:ss");
                var size = new FileInfo(file).Length;
                html.Append($"<tr><td><a href=\"{fileName}\">{fileName}</a></td><td>{FormatByte(size).TrimEnd('B').TrimEnd('i')}</td><td>{lastModified}</td></tr>");
            }
            html.Append("</table></body></html>");

            return html.ToString();
        }
    }

    private static string FormatByte(double size, int keep = 2, int rate = 1024, string space = "")
    {
        if (Math.Abs(size) < rate)
        {
            return $"{Math.Round(size, keep)}{space}B";
        }

        string[] units = rate == 1000
            ? ["KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"]
            : ["KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"];

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
    /// 获取联网地址
    /// </summary>
    /// <returns></returns>
    public static async Task<IPEndPoint> GetAddressInterNetwork()
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            await socket.ConnectAsync("114.114.114.114", 65530).ConfigureAwait(false);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint;
        }
        catch (Exception) { }
        return null;
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
        {".AddIn", "text/xml"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".ai", "application/postscript"},
        {".aif", "audio/aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".amc", "application/mpeg"},
        {".anx", "application/annodex"},
        {".apng", "image/apng"},
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
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".cfg", "text/plain"},
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
        {".disco", "text/xml"},
        {".divx", "video/divx"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dwf", "drawing/x-dwf"},
        {".eml", "message/rfc822"},
        {".emf", "image/emf"},
        {".etx", "text/x-setext"},
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
        {".hdml", "text/x-hdml"},
        {".heic", "image/heic"},
        {".heics", "image/heic-sequence"},
        {".heif", "image/heif"},
        {".heifs", "image/heif-sequence"},
        {".hpp", "text/plain"},
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
        {".inc", "text/plain"},
        {".ini", "text/plain"},
        {".inl", "text/plain"},
        {".ipproj", "text/plain"},
        {".iqy", "text/x-ms-iqy"},
        {".IVF", "video/x-ivf"},
        {".jfif", "image/pjpeg"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".loadtest", "application/xml"},
        {".log", "text/plain"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
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
        {".map", "text/plain"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mk", "text/plain"},
        {".mk3d", "video/x-matroska-3d"},
        {".mka", "audio/x-matroska"},
        {".mkv", "video/x-matroska"},
        {".mno", "text/xml"},
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
        {".mpg", "video/mpeg"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".oga", "audio/ogg"},
        {".ogg", "audio/ogg"},
        {".ogv", "video/ogg"},
        {".opus", "audio/ogg"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".otf", "application/font-sfnt"},
        {".oxps", "application/oxps"},
        {".oxt", "application/vnd.openofficeorg.extension"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
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
        {".pls", "audio/scpls"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".ppm", "image/x-portable-pixmap"},
        {".psess", "application/xml"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".ras", "image/x-cmu-raster"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".reg", "text/plain"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rmi", "audio/mid"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".scr", "text/plain"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".settings", "application/xml"},
        {".sgml", "text/sgml"},
        {".shtml", "text/html"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sln", "text/plain"},
        {".smd", "audio/x-smd"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".sql", "application/sql"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spx", "audio/ogg"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".svg", "image/svg+xml"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".tr", "application/x-troff"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
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
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vstemplate", "text/xml"},
        {".vtt", "text/vtt"},
        {".wasm", "application/wasm"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wdp", "image/vnd.ms-photo"},
        {".webm", "video/webm"},
        {".webp", "image/webp"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wml", "text/vnd.wap.wml"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".woff", "application/font-woff"},
        {".woff2", "application/font-woff2"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
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
        {".zip", "application/zip"}
    };
}

#endif