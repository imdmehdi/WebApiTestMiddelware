namespace WebApiTestMiddelware
{
    public class MyCustomMiddleware
    {
        private readonly RequestDelegate _next;

        public MyCustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMessageWriter is injected into InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext, IMessageWriter svc)
        {
            svc.Write(DateTime.Now.Ticks.ToString());
            await _next(httpContext);
            svc.Write(DateTime.Now.Ticks.ToString()+" Out");

        }
    }

    public static class MyCustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyCustomMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyCustomMiddleware>();
        }
    }
}
