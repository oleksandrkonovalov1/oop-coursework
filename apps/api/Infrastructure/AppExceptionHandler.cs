using AbiturientDirectory.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AbiturientDirectory.Infrastructure;

/// <summary>
/// Глобальний обробник винятків: перетворює виключення на відповіді у форматі
/// ProblemDetails (RFC 9457) без витоку stack trace клієнту.
/// </summary>
public class AppExceptionHandler : IExceptionHandler
{
    private readonly ILogger<AppExceptionHandler> _logger;

    /// <summary>Створює обробник із логером для запису неочікуваних помилок.</summary>
    /// <param name="logger">Логер застосунку.</param>
    public AppExceptionHandler(ILogger<AppExceptionHandler> logger) => _logger = logger;

    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem;

        switch (exception)
        {
            case ValidationException ve:
                // 400: помилки за полями у форматі errors: { field: ["msg"] }
                problem = new ValidationProblemDetails(
                    ve.Errors.ToDictionary(e => e.Key, e => new[] { e.Value }))
                {
                    Status = StatusCodes.Status400BadRequest
                };
                break;

            case KeyNotFoundException knf:
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = knf.Message
                };
                break;

            default:
                _logger.LogError(exception, "Необроблений виняток під час обробки запиту");
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Сталася внутрішня помилка. Спробуйте ще раз."
                };
                break;
        }

        httpContext.Response.StatusCode = problem.Status!.Value;
        // Серіалізуємо за фактичним типом (ValidationProblemDetails → поле errors не губиться)
        await httpContext.Response.WriteAsJsonAsync(
            problem, problem.GetType(), options: null,
            contentType: "application/problem+json", cancellationToken);
        return true;
    }
}
