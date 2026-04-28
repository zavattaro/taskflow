using System.Net;
using System.Text.Json;
using TaskFlow.Api.Errors;

namespace TaskFlow.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = ErrorMessages.UnexpectedError
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}