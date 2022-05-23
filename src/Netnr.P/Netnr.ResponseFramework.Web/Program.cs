using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Netnr;
using Netnr.SharedApp;

var builder = WebApplication.CreateBuilder(args).SetGlobal();

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

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    //cookie存储需用户同意，欧盟新标准，暂且关闭，否则用户没同意无法写入
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddControllersWithViews(options =>
{
    //注册全局过滤器
    options.Filters.Add(new Netnr.ResponseFramework.Web.Apps.FilterConfigs.GlobalActionAttribute());
}).SetJson();
builder.Services.AddSwaggerGenNewtonsoftSupport();

//授权访问信息
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "__NRF_AUTH";
    options.LoginPath = "/account/login";
});

//session
builder.Services.AddSession();

//数据库连接池
builder.Services.AddDbContextPool<Netnr.ResponseFramework.Data.ContextBase>(options =>
{
    Netnr.ResponseFramework.Data.ContextBaseFactory.CreateDbContextOptionsBuilder(options);
}, 10);

//定时任务
FluentScheduler.JobManager.Initialize(new Netnr.ResponseFramework.Web.Apps.TaskService.Reg());

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
    var db = scope.ServiceProvider.GetRequiredService<Netnr.ResponseFramework.Data.ContextBase>();

    var createScript = db.Database.GenerateCreateScript();
    if (Netnr.SharedFast.GlobalTo.TDB == SharedEnum.TypeDB.PostgreSQL)
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
