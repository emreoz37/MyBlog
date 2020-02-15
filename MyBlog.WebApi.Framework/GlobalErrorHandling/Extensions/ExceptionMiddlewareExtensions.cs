using Microsoft.AspNetCore.Builder;
using MyBlog.WebApi.Framework.Middleware.CustomException;

namespace MyBlog.WebApi.Framework.GlobalErrorHandling.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
