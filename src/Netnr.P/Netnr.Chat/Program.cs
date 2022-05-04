using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

Netnr.SharedFast.GlobalTo.Configuration = builder.Configuration;
Netnr.SharedFast.GlobalTo.HostEnvironment = builder.Environment;

Netnr.SharedReady.ReadyTo.EncodingReg();

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
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    //Action原样输出JSON
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    //日期格式化
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
});

//授权访问信息
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

//session
builder.Services.AddSession();

builder.Services.AddSignalR();

//数据库连接池
builder.Services.AddDbContextPool<Netnr.Chat.Data.ContextBase>(options =>
{
    Netnr.Chat.Data.ContextBaseFactory.CreateDbContextOptionsBuilder(options);
}, 10);
//数据库初始化
builder.Services.Configure<Netnr.Chat.Data.ContextBase>(db =>
{
    //数据库不存在则创建，创建后返回true
    if (db.Database.EnsureCreated())
    {

    }

    if (!db.NChatUser.Any())
    {
        db.NChatUser.Add(new Netnr.Chat.Domain.NChatUser()
        {
            CuUserId = "5757526144712703761",
            CuCreateTime = DateTime.Now,
            CuPassword = Netnr.Core.CalcTo.MD5("123"),
            CuStatus = 1,
            CuUserName = "123",
            CuUserNickname = "123",
            CuUserPhoto = "favicon.ico"

        });
        db.NChatUser.Add(new Netnr.Chat.Domain.NChatUser()
        {
            CuUserId = "5757526144712703761",
            CuCreateTime = DateTime.Now,
            CuPassword = Netnr.Core.CalcTo.MD5("123"),
            CuStatus = 1,
            CuUserName = "123",
            CuUserNickname = "123",
            CuUserPhoto = "favicon.ico"
        });

        db.SaveChanges();
    }
});

//配置swagger
builder.Services.AddSwaggerGen(c =>
{
    var name = builder.Environment.ApplicationName;
    c.SwaggerDoc(name, new Microsoft.OpenApi.Models.OpenApiInfo { Title = name });
    c.IncludeXmlComments($"{AppContext.BaseDirectory}{name}.xml", true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.UseStaticFiles();

app.UseRouting();

//授权
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<Netnr.Chat.Application.ChatHubService>("/chathub");

app.Run();