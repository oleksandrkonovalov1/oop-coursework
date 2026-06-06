using AbiturientDirectory.Infrastructure;
using AbiturientDirectory.Services;
using AbiturientDirectory.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o =>
        o.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.HttpNoContentOutputFormatter>())
    .AddNewtonsoftJson(o =>
        o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<AppExceptionHandler>();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IDirectoryRepository>(_ =>
{
    var repo = new JsonDirectoryRepository(Path.Combine(AppContext.BaseDirectory, "Data"));
    repo.Load();
    return repo;
});
builder.Services.AddSingleton<DirectoryService>();

var app = builder.Build();

app.UseExceptionHandler();

app.MapOpenApi();

app.MapGet("/api/status", (IDirectoryRepository repo) => new { loadProblem = repo.LoadProblem });

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
