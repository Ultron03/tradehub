using System.Net;
using System.Text.Json;
using TradingPlatform.Application.Exceptions;

namespace TradingPlatform.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, ve.Message),
            NotFoundException ne => (HttpStatusCode.NotFound, ne.Message),
            UnauthorizedException ue => (HttpStatusCode.Unauthorized, ue.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(payload);
    }
}
