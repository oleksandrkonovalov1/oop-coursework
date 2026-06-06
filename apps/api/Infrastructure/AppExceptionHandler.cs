using AbiturientDirectory.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AbiturientDirectory.Infrastructure;

public class AppExceptionHandler : IExceptionHandler
{
    private readonly ILogger<AppExceptionHandler> _logger;

    public AppExceptionHandler(ILogger<AppExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem;

        switch (exception)
        {
            case ValidationException ve:
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
        await httpContext.Response.WriteAsJsonAsync(
            problem, problem.GetType(), options: null,
            contentType: "application/problem+json", cancellationToken);
        return true;
    }
}
