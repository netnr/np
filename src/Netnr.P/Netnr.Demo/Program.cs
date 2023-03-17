var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//ÅäÖÃswagger
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
    app.UseHsts();
}

//ÅäÖÃswagger
app.UseSwagger().UseSwaggerUI(c =>
{
    c.DocumentTitle = builder.Environment.ApplicationName;
    c.SwaggerEndpoint($"{c.DocumentTitle}/swagger.json", c.DocumentTitle);
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
