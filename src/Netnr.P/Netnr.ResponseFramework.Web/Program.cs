using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

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

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    //cookie存储需用户同意，欧盟新标准，暂且关闭，否则用户没同意无法写入
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddControllersWithViews(options =>
{
    //注册全局过滤器
    options.Filters.Add(new FilterConfigs.GlobalActionAttribute());
}).SetJsonConfig();

//授权访问信息
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "netnrf_auth";
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
app.UseStaticFiles();

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
        ConsoleTo.WriteCard("GenerateCreateScript", createScript);

        //重置数据库
        ConsoleTo.WriteCard("DatabaseReset");
        var vm = await new Netnr.ResponseFramework.Web.Controllers.ServicesController(db).DatabaseReset(deleteData: false, realTimePrint: true);
        Console.WriteLine($"{vm.Code} {vm.Msg}");
    }
}

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
