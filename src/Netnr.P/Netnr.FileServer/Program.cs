using Netnr;

var builder = WebApplication.CreateBuilder(args);

ReadyTo.EncodingReg();

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

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    //Action原样输出JSON
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    //日期格式化
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
});

//配置swagger
builder.Services.AddSwaggerGen(c =>
{
    var name = builder.Environment.ApplicationName;

    c.SwaggerDoc(name, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = name,
        Description = $@"
<b>GitHub</b>：<a target='_blank' href='https://github.com/netnr'>https://github.com/netnr</a>
&nbsp; 
<b>Blog</b>：<a target='_blank' href='https://www.netnr.com'>https://www.netnr.com</a>
&nbsp; 
文件上传大小限制：<b>{srms}</b> MB
&nbsp; 
管理员默认密码：<b>nr</b>
"
    });

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
    c.InjectStylesheet("/Home/SwaggerCustomStyle");
});

//初始化库
using var db = new SQLite.SQLiteConnection(Netnr.FileServer.Application.FileServerService.SQLiteConn);
db.CreateTable<Netnr.FileServer.Domain.SysApp>();
db.CreateTable<Netnr.FileServer.Domain.FileRecord>();

//静态资源
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = (resp) =>
    {
        resp.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        resp.Context.Response.Headers.Add("Cache-Control", "public, max-age=604800");
    }
});

//目录浏览&&公开访问
if (builder.Configuration.GetValue<bool>("Safe:EnableDirectoryBrowsing") && builder.Configuration.GetValue<bool>("Safe:PublicAccess"))
{
    string vrootdir = builder.Configuration.GetValue<string>("StaticResource:RootDir");
    string prootdir = Netnr.FileServer.Application.FileServerService.StaticVrPathAsPhysicalPath(vrootdir);
    if (!Directory.Exists(prootdir))
    {
        Directory.CreateDirectory(prootdir);
    }
    
    var fso = new FileServerOptions()
    {
        EnableDirectoryBrowsing = true,
        EnableDefaultFiles = false
    };
    fso.StaticFileOptions.ServeUnknownFileTypes = true;
    fso.StaticFileOptions.OnPrepareResponse = (resp) =>
    {
        resp.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        resp.Context.Response.Headers.Add("Cache-Control", "public, max-age=604800");
    };
    fso.FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(prootdir);
    //目录浏览链接
    fso.RequestPath = new PathString(vrootdir);
    app.UseFileServer(fso);
}

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();