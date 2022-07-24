using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Netnr;
using Netnr.Core;
using Netnr.Login;

var builder = WebApplication.CreateBuilder(args);

ReadyTo.EncodingReg();
ReadyTo.LegacyTimestamp();

//（上传）主体大小限制
var srms = builder.Configuration.GetValue<int>("StaticResource:MaxSize");
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = srms * 1024 * 1024;
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = srms * 1024 * 1024;
});

//结巴词典路径
var jbPath = Path.Combine(GlobalTo.ContentRootPath, "db/jieba");
if (!Directory.Exists(jbPath))
{
    Directory.CreateDirectory(jbPath);
    try
    {
        var dhost = "https://raw.githubusercontent.com/anderscui/jieba.NET/master/src/Segmenter/Resources/";
        "prob_trans.json,prob_emit.json,idf.txt,pos_prob_start.json,pos_prob_trans.json,pos_prob_emit.json,char_state_tab.json".Split(',').ToList().ForEach(file =>
        {
            var fullPath = Path.Combine(jbPath, file);
            HttpTo.DownloadSave(dhost + file, fullPath);
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}
JiebaNet.Segmenter.ConfigManager.ConfigFileBaseDir = jbPath;

//第三方登录
new List<Type>
{
    typeof(QQConfig),
    typeof(WeChatConfig),
    typeof(WeiboConfig),
    typeof(GitHubConfig),
    typeof(GiteeConfig),
    typeof(TaoBaoConfig),
    typeof(MicroSoftConfig),
    typeof(DingTalkConfig),
    typeof(GoogleConfig),
    typeof(AliPayConfig),
    typeof(StackOverflowConfig)
}.ForEach(lc =>
{
    var fields = lc.GetFields();
    foreach (var field in fields)
    {
        if (!field.Name.StartsWith("API_"))
        {
            var cv = GlobalTo.GetValue($"OAuthLogin:{lc.Name.Replace("Config", "")}:{field.Name}");
            field.SetValue(lc, cv);
        }
    }
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    //cookie存储需用户同意，欧盟新标准，暂且关闭，否则用户没同意无法写入
    options.CheckConsentNeeded = context => false;
    if (!GlobalTo.GetValue<bool>("ReadOnly"))
    {
        //允许其他站点携带授权Cookie访问，会出现伪造
        //Chrome新版本必须启用HTTPS，安装命令：dotnet dev-certs https
        options.MinimumSameSitePolicy = SameSiteMode.None;
    }
});

builder.Services.AddControllersWithViews(options =>
{
    //注册全局过滤器
    options.Filters.Add(new Netnr.Blog.Web.Apps.FilterConfigs.GlobalFilter());

    //注册全局授权访问时登录标记是否有效
    options.Filters.Add(new Netnr.Blog.Web.Apps.FilterConfigs.LoginSignValid());
}).SetJson();
builder.Services.AddSwaggerGenNewtonsoftSupport();

//授权访问信息
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    if (!GlobalTo.GetValue<bool>("ReadOnly"))
    {
        //允许其他站点携带授权Cookie访问，会出现伪造
        //Chrome新版本必须启用HTTPS，安装命令：dotnet dev-certs https
        options.Cookie.SameSite = SameSiteMode.None;
    }

    options.Cookie.Name = "__NETNR_AUTH";
    options.LoginPath = "/account/login";
});

//session
builder.Services.AddSession();

//数据库连接池
builder.Services.AddDbContextPool<Netnr.Blog.Data.ContextBase>(options =>
{
    Netnr.Blog.Data.ContextBaseFactory.CreateDbContextOptionsBuilder(options);
}, 10);

//定时任务 https://github.com/fluentscheduler/FluentScheduler
if (!GlobalTo.GetValue<bool>("ReadOnly"))
{
    FluentScheduler.JobManager.Initialize();

    var sc = new Netnr.Blog.Web.Controllers.ServicesController();

    //Gist 同步任务
    FluentScheduler.JobManager.AddJob(() => sc.GistSync(), s =>
    {
        s.WithName("Job_GistSync");
        s.ToRunEvery(8).Hours();
    });

    //处理操作记录
    FluentScheduler.JobManager.AddJob(() => sc.HandleOperationRecord(), s => s.ToRunEvery(6).Hours());

    //数据库备份到 Git
    FluentScheduler.JobManager.AddJob(() => sc.DatabaseBackupToGit(), s => s.ToRunEvery(7).Days().At(16, 16));

    //监控
    FluentScheduler.JobManager.AddJob(() => sc.Monitor("http"), s => s.ToRunEvery(1).Minutes());
    FluentScheduler.JobManager.AddJob(() => sc.Monitor("tcp"), s => s.ToRunEvery(1).Minutes());
    FluentScheduler.JobManager.AddJob(() => sc.Monitor("ssl"), s => s.ToRunEvery(1).Days().At(10, 10));
}

//配置swagger
builder.Services.AddSwaggerGen(c =>
{
    var name = builder.Environment.ApplicationName;
    c.SwaggerDoc(name, new Microsoft.OpenApi.Models.OpenApiInfo { Title = name });
    c.IncludeXmlComments($"{AppContext.BaseDirectory}{name}.xml", true);
});

var app = builder.Build();

//ERROR
app.UseExceptionHandler(options => options.SetExceptionHandler());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//数据库初始化
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Netnr.Blog.Data.ContextBase>();

    var createScript = db.Database.GenerateCreateScript();
    if (GlobalTo.TDB == EnumTo.TypeDB.PostgreSQL)
    {
        createScript = createScript.Replace(" datetime ", " timestamp ");
    }
    Console.WriteLine(createScript);

    //数据库不存在则创建，创建后返回true
    if (db.Database.EnsureCreated())
    {
        //导入数据库示例数据
        var vm = new Netnr.Blog.Web.Controllers.ServicesController().DatabaseImport("db/backup_demo.zip");
        Console.WriteLine(vm.ToJson(true));
    }
}

//配置swagger
app.UseSwagger().UseSwaggerUI(c =>
{
    c.DocumentTitle = builder.Environment.ApplicationName;
    c.SwaggerEndpoint($"{c.DocumentTitle}/swagger.json", c.DocumentTitle);
    c.InjectStylesheet("/Home/SwaggerCustomStyle");
});

app.UseHttpsRedirection();

//静态资源
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = (resp) =>
    {
        if (!resp.File.IsDirectory && (resp.File.Name.EndsWith(".js") || resp.File.Name.EndsWith(".css")))
        {
            resp.Context.Response.Headers["Content-Type"] += "; charset=utf-8";
        }
        resp.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        resp.Context.Response.Headers.Add("Cache-Control", "public, max-age=604800");
    }
});
//目录浏览&&公开访问
if (GlobalTo.GetValue<bool>("ReadOnly"))
{
    var fso = new FileServerOptions()
    {
        EnableDirectoryBrowsing = true,
        EnableDefaultFiles = false
    };
    fso.StaticFileOptions.ServeUnknownFileTypes = true;
    fso.StaticFileOptions.OnPrepareResponse = (resp) =>
    {
        if (!resp.File.IsDirectory && (resp.File.Name.EndsWith(".js") || resp.File.Name.EndsWith(".css")))
        {
            resp.Context.Response.Headers["Content-Type"] += "; charset=utf-8";
        }
        resp.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        resp.Context.Response.Headers.Add("Cache-Control", "public, max-age=604800");
    };
    fso.FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(GlobalTo.WebRootPath);
    fso.RequestPath = new PathString($"/_");

    app.UseFileServer(fso);
}

app.UseRouting();

//身份验证·解 JWT/Cookie 设置 HttpContext.User
app.UseAuthentication();
//身份验证·授权访问校验
app.UseAuthorization();

//session
app.UseSession();

//https://docs.microsoft.com/en-us/aspnet/core/tutorials/min-web-api
app.Map("/app/{appName}/", (string appName) => Results.File($"{GlobalTo.WebRootPath}/app/{appName}/index.html", "text/html; charset=utf-8"));

//app.Map("/{xid:int}", (int xid) => Results.Ok(xid));
app.Map("/generate_200", () => Results.Ok());
app.Map("/generate_204", () => Results.NoContent());
app.Map("/generate_400", () => Results.BadRequest());
app.Map("/generate_401", () => Results.Unauthorized());
app.Map("/generate_404", () => Results.NotFound());
app.Map("/generate_418", () => Results.StatusCode(418));

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "sid", pattern: "{controller=Home}/{action=Index}/{id?}/{sid?}");

app.Run();
