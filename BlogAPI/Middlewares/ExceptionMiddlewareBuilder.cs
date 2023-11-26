namespace BlogAPI.Middlewares;

public static class ExceptionMiddlewareBuilder
{
    public static void UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}