using Entities.GlobalErrorHandling.Model;
using Logger;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Middleware
{
    public sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {

            _logger = logger;
            _next = next;
        }

        public RequestDelegate Next => _next;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (AccessViolationException avEx)
            {
                _logger.LogError($"A new violation exception has been thrown: {avEx}");
                await HandleExceptionAsync(httpContext, avEx);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Đã xảy ra sự cố: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }


        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = $"Lỗi Máy chủ Nội bộ từ phần mềm trung gian tùy chỉnh.{exception.Message} "
            }.ToString());
        }
    }

}
