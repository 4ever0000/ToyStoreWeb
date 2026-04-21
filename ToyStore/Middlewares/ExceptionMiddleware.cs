using System.Net;
using System.Text.Json;
using ToyStore.Application.Common.Exceptions;

namespace ToyStore.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var response = new { error = exception.Message, details = (object?)null };

        switch (exception)
        {
            case BadRequestException badRequestEx:
                statusCode = HttpStatusCode.BadRequest;
                response = new { error = badRequestEx.Message, details = (object?)badRequestEx.Errors };
                break;
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;
            case ApiException:
                statusCode = HttpStatusCode.BadRequest;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}