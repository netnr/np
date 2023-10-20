using Netnr;

var builder = WebApplication.CreateBuilder(args);

BaseTo.ReadyEncoding();
BaseTo.ReadyLegacyTimestamp();

// Add services to the container.
builder.Services.AddControllersWithViews().SetJsonConfig();

//ÅäÖÃswagger
builder.Services.AddSwaggerGen(c =>
{
    var name = builder.Environment.ApplicationName;
    c.SwaggerDoc(name, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = name,
        Version = BaseTo.Version,
        Description = "https://github.com/netnr"
    });

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
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//ÅäÖÃswagger
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

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();