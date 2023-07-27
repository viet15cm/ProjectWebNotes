using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Middleware;
using System.Net;

namespace Domain.Extensions
{
    public static class MiddlewareExtensions
    {

        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            //app.UseMiddleware<ExceptionMiddleware>();
        }

    }
}
