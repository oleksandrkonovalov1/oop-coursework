using AbiturientDirectory.Infrastructure;
using AbiturientDirectory.Services;
using AbiturientDirectory.Storage;

var builder = WebApplication.CreateBuilder(args);

// AddNewtonsoftJson за замовчуванням серіалізує camelCase (збігається з TS-типами фронтенда);
// enum віддаємо рядками: "FullTime" / "Evening" / "PartTime"
builder.Services.AddControllers(o =>
        // Відсутній результат віддаємо як 200 з тілом null (узгоджений JSON-контракт),
        // а не як 204 No Content — інакше fetch(...).json() на фронтенді впав би
        o.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.HttpNoContentOutputFormatter>())
    .AddNewtonsoftJson(o =>
        o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));

// Єдина обробка помилок у форматі ProblemDetails (RFC 9457)
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<AppExceptionHandler>();

// Опис API за стандартом OpenAPI (вбудований у .NET 10)
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IDirectoryRepository>(_ =>
{
    var repo = new JsonDirectoryRepository(Path.Combine(AppContext.BaseDirectory, "Data"));
    repo.Load();
    return repo;
});
builder.Services.AddSingleton<DirectoryService>();

var app = builder.Build();

// Перетворення винятків на відповіді ProblemDetails через AppExceptionHandler
app.UseExceptionHandler();

app.MapOpenApi();

// Стан сховища — для повідомлення про пошкоджені файли даних (сценарій 9, альтернативний потік)
app.MapGet("/api/status", (IDirectoryRepository repo) => new { loadProblem = repo.LoadProblem });

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
