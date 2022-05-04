var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var fso = new FileServerOptions()
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(builder.Configuration.GetValue<string>("Root")),
    RequestPath = new PathString($"/{builder.Configuration.GetValue<string>("Password")}"),
    EnableDirectoryBrowsing = true
};
fso.StaticFileOptions.ServeUnknownFileTypes = true;
fso.StaticFileOptions.OnPrepareResponse = (resp) =>
{
    resp.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    resp.Context.Response.Headers.Add("Cache-Control", "public, max-age=604800");
};
app.UseFileServer(fso);

app.Map("/", () => $"{builder.Environment.ApplicationName}\r\n{DateTime.Now}\r\n");

app.Run();