
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//ÅäÖÃswagger
var ns = Path.GetFileNameWithoutExtension(System.Reflection.MethodBase.GetCurrentMethod().Module.Name);
var ver = "v1";
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(ver, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = ns,
        Version = ver
    });

    //×¢ÊÍ
    c.IncludeXmlComments($"{AppContext.BaseDirectory}{ns}.xml", true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//ÅäÖÃswagger
app.UseSwagger().UseSwaggerUI(c =>
{
    c.DocumentTitle = ns;
    c.SwaggerEndpoint("/swagger/v1/swagger.json", c.DocumentTitle);
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
