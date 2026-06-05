using AbiturientDirectory.Services;
using AbiturientDirectory.Storage;

var builder = WebApplication.CreateBuilder(args);

// AddNewtonsoftJson за замовчуванням серіалізує camelCase (збігається з TS-типами фронтенда);
// enum віддаємо рядками: "FullTime" / "Evening" / "PartTime"
builder.Services.AddControllers().AddNewtonsoftJson(o =>
    o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));
builder.Services.AddSingleton<JsonDataStore>(_ =>
{
    var store = new JsonDataStore(Path.Combine(AppContext.BaseDirectory, "Data"));
    store.Load();
    return store;
});
builder.Services.AddSingleton<DirectoryService>();

var app = builder.Build();

// Перетворення помилок на зрозумілі клієнту відповіді без stack traces:
// ValidationException → 400 {errors}; будь-яка інша → 500 із загальним повідомленням
app.Use(async (context, next) =>
{
    try { await next(); }
    catch (ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { errors = ex.Errors });
    }
    catch (Exception)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { message = "Сталася внутрішня помилка. Спробуйте ще раз." });
    }
});

// Стан сховища — для повідомлення про пошкоджені файли даних (сценарій 9, альтернативний потік)
app.MapGet("/api/status", (JsonDataStore store) => new { loadProblem = store.LoadProblem });

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
