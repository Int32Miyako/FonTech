using System.Net;
using FonTech.Domain.Result;
using ILogger = Serilog.ILogger;

namespace FonTech.Api.Middlewares;

/// <summary>
/// обработчик ошибок для устранения необходимости использования try/catch во всех сервисах
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        // Логирование с полями для удобной диагностики
        _logger.Error(exception, "Unhandled exception caught. Path: {Path}, Method: {Method}, Query: {QueryString}",
            httpContext.Request.Path,
            httpContext.Request.Method,
            httpContext.Request.QueryString);

        // Подготовка стандартного ответа
        var response = new BaseResult
        {
            ErrorMessage = "An unexpected error occurred. Please try again later.",
            ErrorCode = (int)HttpStatusCode.InternalServerError
        };

        // Переключение для обработки определенных типов исключений
        response = exception switch
        {
            UnauthorizedAccessException => new BaseResult
            {
                ErrorMessage = exception.Message,
                ErrorCode = (int)HttpStatusCode.Unauthorized
            },
            ArgumentException => new BaseResult
            {
                ErrorMessage = exception.Message,
                ErrorCode = (int)HttpStatusCode.BadRequest
            },
            _ => response
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = response.ErrorCode ?? (int)HttpStatusCode.InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(response);
    }
}