using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WifiWarriorAPI.Infrastructure;

/// <summary>
/// Converts unhandled exceptions into ProblemDetails responses.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    /// <summary>
    /// Creates a global exception handler that logs failures and returns standardised error payloads.
    /// </summary>
    /// <param name="environment">The host environment.</param>
    /// <param name="logger">The global exception logger.</param>
    /// <param name="problemDetailsService">The problem details service.</param>
    public GlobalExceptionHandler(
        IHostEnvironment environment,
        ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _environment = environment;
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        _logger.LogError(exception, "Unhandled exception. TraceId: {traceId}", traceId);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Type = "https://httpstatuses.com/500",
            Detail = _environment.IsDevelopment()
                     || _environment.EnvironmentName == "Test"
                ? exception.Message
                : "An unexpected error occurred.",
            Instance = httpContext.Request.Path,
        };

        problem.Extensions["traceId"] = traceId;
        
        httpContext.Response.StatusCode = problem.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception,
        });
        
        return true;
    }
}