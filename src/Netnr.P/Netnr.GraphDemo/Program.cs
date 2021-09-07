
var builder = WebApplication.CreateBuilder(args);

//配置swagger
var swaggerVersion = "v1";
var swaggerTitle = "Netnr.GraphDemo";

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    //Action原样输出JSON
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    //日期格式化
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(swaggerVersion, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = swaggerTitle,
        Version = swaggerVersion
    });

    c.IncludeXmlComments(AppContext.BaseDirectory + swaggerTitle + ".xml", true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//配置swagger
app.UseSwagger().UseSwaggerUI(c =>
{
    c.DocumentTitle = swaggerTitle;
    c.SwaggerEndpoint($"{swaggerVersion}/swagger.json", c.DocumentTitle);
});

app.UseHttpsRedirection();
//静态资源允许跨域
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = (x) =>
    {
        x.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        x.Context.Response.Headers.Add("Cache-Control", "public, max-age=604800");
    },
    ServeUnknownFileTypes = true
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
