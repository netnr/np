var builder = WebApplication.CreateBuilder(args);

//����swagger
var swaggerVersion = "v1";
var swaggerTitle = "Netnr.GraphDemo";

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    //Actionԭ�����JSON
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    //���ڸ�ʽ��
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
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

//����swagger
app.UseSwagger().UseSwaggerUI(c =>
{
    c.DocumentTitle = swaggerTitle;
    c.SwaggerEndpoint($"{swaggerVersion}/swagger.json", c.DocumentTitle);
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
