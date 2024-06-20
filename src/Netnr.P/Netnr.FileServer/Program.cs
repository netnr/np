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

//（上传）主体大小限制
builder.SetMaxRequestData();

// Add services to the container.
builder.Services.AddControllersWithViews().SetJsonConfig();

//配置swagger
builder.Services.AddSwaggerGen(c =>
{
    var name = builder.Environment.ApplicationName;

    c.SwaggerDoc(name, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = name,
        Version = BaseTo.Version,
        Description = "<b>GitHub</b>：<a target='_blank' href='https://github.com/netnr'>https://github.com/netnr</a>"
    });

    c.CustomSchemaIds(type => type.FullName.Replace("+", "."));

    Directory.EnumerateFiles(AppContext.BaseDirectory, "Netnr.*.xml").ForEach(file =>
    {
        c.IncludeXmlComments(file, true);
    });
});

var app = builder.Build();
//全局错误处理
app.UseExceptionHandler(options =>
{
    options.Run(async context =>
    {
        context.Response.StatusCode = 200;

        var vm = new ResultVM();
        vm.Set(RCodeTypes.exception);

        var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (ex != null)
        {
            ConsoleTo.LogError(ex.Error, "Server-Error");
            vm.Msg = ex.Error.Message;
            vm.Data = ex.Error.ToTree();
        }

        context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Append("Access-Control-Allow-Methods", context.Request.Method);
        context.Response.Headers.Append("Access-Control-Allow-Headers", "*");
        await context.Response.WriteAsync(vm.ToJson(true));
    });
});

//配置swagger
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        var listAction = ProjectTo.GetAllAction();
        var listDomainMember = ProjectTo.GetDocumentationFile("Netnr.FileServer");
        var listDomainType = typeof(HomeController).Assembly.GetTypes();

        //完善注释
        swagger.Paths.ForEach(path =>
        {
            path.Value.Operations.ForEach(httpMethod =>
            {
                if (httpMethod.Value.RequestBody != null)
                {
                    if (string.IsNullOrWhiteSpace(httpMethod.Value.RequestBody?.Description))
                    {
                        //方法对象
                        var methodModel = listAction.FirstOrDefault(x => path.Key.EndsWith($"{x.ControllerName}/{x.ActionName}"));
                        if (methodModel != null)
                        {
                            //Content-Type 字典
                            httpMethod.Value.RequestBody.Content.ForEach(contentType =>
                            {
                                //Properties 字典
                                contentType.Value.Schema.Properties.ForEach(propItem =>
                                {
                                    //参数对象
                                    var parameterModel = methodModel.ActionParameter.FirstOrDefault(x => x.ParameterName == propItem.Key);
                                    if (parameterModel == null)
                                    {
                                        // 尝试找实体
                                        foreach (var ap in methodModel.ActionParameter)
                                        {
                                            var domainType = listDomainType.FirstOrDefault(x => x.FullName == ap.ParameterFullType);
                                            if (domainType != null)
                                            {
                                                //属性注释
                                                var prop = domainType.GetProperty(propItem.Key);
                                                var propMember = listDomainMember.FirstOrDefault(x => x.Attributes["name"].Value == $"P:{ap.ParameterFullType}.{propItem.Key}");
                                                if (propMember != null)
                                                {
                                                    propItem.Value.Description = propMember.InnerText.Trim();
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (!string.IsNullOrWhiteSpace(parameterModel.ParameterComment))
                                    {
                                        // 单个参数
                                        propItem.Value.Description = parameterModel.ParameterComment;
                                    }
                                });
                            });
                        }
                    }
                }
            });
        });
    });
}).UseSwaggerUI(c =>
{
    c.DocumentTitle = builder.Environment.ApplicationName;
    c.SwaggerEndpoint($"{c.DocumentTitle}/swagger.json", c.DocumentTitle);
});

//初始化库
AppService.CollSysApp.EnsureIndex(x => x.AppId, true);
AppService.CollSysApp.EnsureIndex(x => x.AppKey, true);
AppService.CollSysApp.EnsureIndex(x => x.AppOwner, true);
AppService.CollSysApp.EnsureIndex(x => x.AppToken, true);
AppService.CollFileRecord.EnsureIndex(x => x.Id, true);
AppService.CollFileRecord.EnsureIndex(x => x.OwnerUser);

//启用临时上传
if (AppTo.GetValue<bool>("Safe:EnableUploadTmp"))
{
    //启动清理临时目录线程
    var thread = new Thread(() =>
    {
        while (true)
        {
            AppService.ClearTmp(AppTo.GetValue<int>("StaticResource:TmpExpire"));
            Thread.Sleep(1000 * 60);
        }
    })
    {
        IsBackground = true
    };
    thread.Start();
    GC.KeepAlive(thread);
}

//静态资源
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = (resp) =>
    {
        resp.Context.Response.Headers.AccessControlAllowOrigin = "*";

        var maxAge = AppTo.GetValue<int>("StaticResource:TmpExpire");
        maxAge = maxAge == 0 ? 604800 : maxAge * 60;
        resp.Context.Response.Headers.CacheControl = $"public, max-age={maxAge}";
    }
});

//目录浏览&&公开访问
if (builder.Configuration.GetValue<bool>("Safe:EnableDirectoryBrowsing") && builder.Configuration.GetValue<bool>("Safe:PublicAccess"))
{
    string vrootdir = builder.Configuration.GetValue<string>("StaticResource:RootDir");
    string prootdir = AppService.StaticVrPathAsPhysicalPath(vrootdir);
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
    fso.FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(prootdir);
    //目录浏览链接
    fso.RequestPath = new PathString(vrootdir);
    app.UseFileServer(fso);
}

app.UseRouting();

//配置 CORS
var corsDomain = AppTo.GetValue<string>("Safe:AllowCrossDomain");
if (!string.IsNullOrWhiteSpace(corsDomain))
{
    app.UseMiddleware<AllowCorsMiddleware>(new AllowCorsMiddleware.MiddlewareOptions { CustomOrigin = corsDomain });
    app.UseCors();
}

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();