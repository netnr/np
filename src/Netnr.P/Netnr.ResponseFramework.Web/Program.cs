using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Netnr;

var builder = WebApplication.CreateBuilder(args);

ReadyTo.EncodingReg();
ReadyTo.LegacyTimestamp();

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
    options.Cookie.Name = "AUTH_NRF";
    options.LoginPath = "/account/login";
});

//session
builder.Services.AddSession();

//数据库连接池
builder.Services.AddDbContextPool<ContextBase>(options =>
{
    AppTo.TDB = AppTo.GetValue<EnumTo.TypeDB>("TypeDB");
    DbContextTo.CreateDbContextOptionsBuilder<ContextBase>(AppTo.TDB, options);
}, 10);

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
if (!(GlobalTo.IsDev = app.Environment.IsDevelopment()))
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//定时任务 https://github.com/fluentscheduler/FluentScheduler
{
    FluentScheduler.JobManager.Initialize();

    var sc = new Netnr.ResponseFramework.Web.Controllers.ServicesController();

    //每2天在2:2 重置数据库
    FluentScheduler.JobManager.AddJob(() =>
    {
        var vm = sc.DatabaseReset();
        Console.WriteLine(vm.ToJson(true));
        ConsoleTo.Log(vm.ToJson());
    }, s =>
    {
        s.WithName("Job_DatabaseReset");
        s.ToRunEvery(2).Days().At(2, 2);
    });

    //每2天在3:3 清理临时目录
    FluentScheduler.JobManager.AddJob(() =>
    {
        var vm = sc.ClearTmp();
        Console.WriteLine(vm.ToJson(true));
        ConsoleTo.Log(vm.ToJson());
    }, s => s.ToRunEvery(2).Days().At(3, 3));
}

//数据库初始化
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContextBase>();

    var createScript = db.Database.GenerateCreateScript();
    if (AppTo.TDB == EnumTo.TypeDB.PostgreSQL)
    {
        createScript = createScript.Replace(" datetime ", " timestamp ");
    }
    Console.WriteLine(createScript);

    //数据库不存在则创建，创建后返回true
    if (db.Database.EnsureCreated())
    {
        //重置数据库
        var vm = new Netnr.ResponseFramework.Web.Controllers.ServicesController().DatabaseReset();
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
app.UseStaticFiles();

app.UseRouting();

//身份验证·解 JWT/Cookie 设置 HttpContext.User
app.UseAuthentication();
//身份验证·授权访问校验
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
