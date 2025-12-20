using driver_api.Repository.Exceptions;

namespace driver_api.Middlewares;

public class HttpExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public HttpExceptionMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            await context.Response.WriteAsync(ex.Message);
        }
    }

}
