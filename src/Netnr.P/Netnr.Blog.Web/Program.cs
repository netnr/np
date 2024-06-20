using Microsoft.AspNetCore.Authentication.Cookies;

PMScriptTo.Init();

var builder = WebApplication.CreateBuilder(args);
if (!BaseTo.CommandLineArgs.Contains("--urls"))
{
    builder.WebHost.ConfigureKestrel((context, serverOptions) =>
    {
        //随机端口
        serverOptions.Listen(System.Net.IPAddress.Any, 0);
    });
}

BaseTo.ReadyEncoding();
BaseTo.ReadyLegacyTimestamp();

//（上传）主体大小限制
builder.SetMaxRequestData();

//第三方登录
if (AppTo.GetValue<bool?>("ProgramParameters:DisableDatabaseWrite") != true && AppTo.GetValue<bool?>("OAuthLogin:enable") == true)
{
    Netnr.Login.LoginTo.InitConfig((loginType, field) =>
    {
        object val = null;
        if (field == "Redirect_Uri")
        {
            val = string.Format(AppTo.GetValue($"OAuthLogin:Redirect_Uri"), loginType.ToString().ToLower());
        }
        else if (field.StartsWith("Is"))
        {
            val = AppTo.GetValue<bool?>($"OAuthLogin:{loginType}:{field}");
        }
        else if (!field.StartsWith("API_"))
        {
            val = AppTo.GetValue($"OAuthLogin:{loginType}:{field}");
        }
        return val;
    });
}

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    //cookie存储需用户同意，欧盟新标准，暂且关闭，否则用户没同意无法写入
    options.CheckConsentNeeded = context => false;

    //允许其他站点携带授权Cookie访问，会出现伪造
    //Chrome新版本必须启用HTTPS，安装命令：dotnet dev-certs https
    //options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddControllersWithViews(options =>
{
    //注册全局过滤器
    options.Filters.Add(new FilterConfigs.GlobalFilter());

    //注册全局授权访问单一在线
    options.Filters.Add(new FilterConfigs.SingleOnlineFilter());
}).SetJsonConfig();

//授权访问信息
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    //允许其他站点携带授权Cookie访问，会出现伪造
    //Chrome新版本必须启用HTTPS，安装命令：dotnet dev-certs https
    //options.Cookie.SameSite = SameSiteMode.None;

    options.Cookie.Name = "netnr_auth";
    options.LoginPath = "/account/login";
});

//session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); //有效期
    options.Cookie.HttpOnly = true; //服务端访问
    options.Cookie.IsEssential = true; //绕过同意使用
});

//数据库连接池
builder.Services.AddDbContextPool<ContextBase>(options =>
{
    AppTo.DBT = AppTo.GetValue<DBTypes>("ConnectionStrings:DBTypes");
    DbContextTo.CreateDbContextOptionsBuilder<ContextBase>(AppTo.DBT, options);
}, 10);

//配置swagger
builder.Services.AddSwaggerGen(c =>
{
    var name = builder.Environment.ApplicationName;
    c.SwaggerDoc(name, new Microsoft.OpenApi.Models.OpenApiInfo { Title = name, Version = BaseTo.Version });

    c.CustomSchemaIds(type => type.FullName.Replace("+", "."));

    Directory.EnumerateFiles(AppContext.BaseDirectory, "Netnr.*.xml").ForEach(file =>
    {
        c.IncludeXmlComments(file, true);
    });
});

// Add Hangfire services.
HangfireService.InitServer(builder);

var app = builder.Build();

//ERROR
app.UseExceptionHandler(options => options.SetExceptionHandler());

// Configure the HTTP request pipeline.
if (!(BaseTo.IsDev = app.Environment.IsDevelopment()))
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//配置swagger
app.UseSwagger().UseSwaggerUI(c =>
{
    c.DocumentTitle = builder.Environment.ApplicationName;
    c.SwaggerEndpoint($"{c.DocumentTitle}/swagger.json", c.DocumentTitle);
});

//app.UseHttpsRedirection();

//静态资源
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = (resp) =>
    {
        if (!resp.File.IsDirectory && (resp.File.Name.EndsWith(".js") || resp.File.Name.EndsWith(".css")))
        {
            resp.Context.Response.Headers.ContentType += "; charset=utf-8";
        }
        resp.Context.Response.Headers.AccessControlAllowOrigin = "*";
        resp.Context.Response.Headers.CacheControl = "public, max-age=604800";
    }
});
//目录浏览&&公开访问
if (AppTo.GetValue<bool?>("ProgramParameters:DisableDatabaseWrite") == true)
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
            resp.Context.Response.Headers.ContentType += "; charset=utf-8";
        }
    };
    fso.FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(AppTo.WebRootPath);
    fso.RequestPath = new PathString($"/_");

    app.UseFileServer(fso);
}

app.UseRouting();

app.UseMiddleware<AllowCorsMiddleware>();
// Call UseCors after UseRouting https://learn.microsoft.com/zh-cn/aspnet/core/security/cors
app.UseCors();

//身份验证·解 JWT/Cookie 设置 HttpContext.User
app.UseAuthentication();
//身份验证·授权访问校验
app.UseAuthorization();

// Call UseSession after UseRouting and before MapRazorPages and MapDefaultControllerRoute 
app.UseSession();

//获取注入对象
using (var scope = app.Services.CreateScope())
{
    //数据库初始化
    var db = scope.ServiceProvider.GetRequiredService<ContextBase>();

    //数据库不存在则创建，创建后返回true
    if (db.Database.EnsureCreated())
    {
        var createScript = db.Database.GenerateCreateScript();
        if (AppTo.DBT == DBTypes.PostgreSQL)
        {
            createScript = createScript.Replace(" datetime ", " timestamp ");
        }
        Console.WriteLine(createScript);

        //导入数据库示例数据
        ConsoleTo.WriteCard("DatabaseImport");
        var vm = await new Netnr.Blog.Web.Controllers.ServiceController(db).DatabaseImport("static/sample.zip", realTimePrint: true);
        Console.WriteLine($"{vm.Code} {vm.Msg}");
    }

    //定时任务
    HangfireService.InitManager(app);
}

//default index.html
//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis
app.MapGet("/app/{appName}/", (string appName) => Results.File($"{AppTo.WebRootPath}/app/{appName}/index.html", "text/html; charset=utf-8"));

//app.Map("/{xid:int}", (int xid) => Results.Ok(xid));
app.Map("/generate_200", () => Results.Ok());
app.Map("/generate_204", () => Results.NoContent());
app.Map("/generate_400", () => Results.BadRequest());
app.Map("/generate_401", () => Results.Unauthorized());
app.Map("/generate_404", () => Results.NotFound());
app.Map("/generate_418", () => Results.StatusCode(418));

//curl HOST -T file.txt
if (AppTo.GetValue<bool?>("ProgramParameters:DisableDatabaseWrite") == true)
{
    app.MapPut("/{id}", async ([FromRoute] string id, HttpContext context) =>
    {
        string msg = string.Empty;
        try
        {
            //接收文件
            using var ms = new MemoryStream();

            var tmpDir = Path.Combine(AppTo.WebRootPath, "tmp");
            if (!Directory.Exists(tmpDir))
            {
                Directory.CreateDirectory(tmpDir);
            }
            var fileName = $"{RandomTo.NewString()}{Path.GetExtension(id)}";
            using var stream = File.OpenWrite(Path.Combine(tmpDir, fileName));
            await context.Request.Body.CopyToAsync(stream);

            var download = $"wget {context.Request.Scheme}://{context.Request.Host}/tmp/{fileName}";
            msg = $"uploaded successfully\r\n\r\n{download}";
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            msg = $"upload failed\r\n\r\n{ex.Message}";
        }

        await context.Response.Body.WriteAsync($"\r\n\r\n{msg}\r\n".ToByte());
    });
}

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "sid", pattern: "{controller=Home}/{action=Index}/{id?}/{sid?}");

app.Run();
