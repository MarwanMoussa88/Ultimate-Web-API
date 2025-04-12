using System.Net;
using System.Text.Json;
using Domain.Log;
using Entities.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace WebAPI.Extensions
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            bool isExceptionThrown = false;
            context.Request.EnableBuffering();

            Stream orignalResposneBody = context.Response.Body;
            using MemoryStream responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var requestLog = await LogRequest(context);
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                isExceptionThrown = true;
                await HandleException(context, ex);
            }

            if (!isExceptionThrown)
            {
                var responseLog = await LogResponse(context, requestLog);
            }

            responseBody.Position = 0;
            await responseBody.CopyToAsync(orignalResposneBody);
            context.Response.Body = orignalResposneBody;
        }

        private async Task<RequestLogDetails> LogRequest(HttpContext context)
        {
            RequestLogDetails log = new RequestLogDetails()
            {
                RequestId = string.IsNullOrEmpty(context.Request.Headers?.RequestId.ToString()) ? Guid.NewGuid().ToString() : context.Request.Headers?.RequestId,
                Headers = JsonSerializer.Serialize(context.Request.Headers),
                Method = context.Request.Method,
                UserAgent = context.Request.Headers.UserAgent,
                ClientIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                StartTime = DateTime.Now,
                QueryParams = JsonSerializer.Serialize(context.Request.Query),
                UserDetails = JsonSerializer.Serialize(context.User),
                RequestBody = await context.Request.Body.GetBody(),
                RequestPath = context.Request.GetDisplayUrl(),
            };
            return log;
        }

        private async Task<ResponseLogDetails> LogResponse(HttpContext context, RequestLogDetails requestLog)
        {
            ResponseLogDetails log = new ResponseLogDetails()
            {
                ResponseId = requestLog.RequestId,
                Method = context.Request.Method,
                UserAgent = context.Request.Headers.UserAgent,
                ClientIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                ElapsedTime = requestLog.StartTime - DateTime.Now,
                UserDetails = JsonSerializer.Serialize(context.User),
                Body = await context.Response.Body.GetBody(),
                RequestPath = context.Request.GetDisplayUrl(),
                StackTrace = Environment.StackTrace,
                StatusCode = context.Response.StatusCode
            };
            return log;
        }

        private async Task<ResponseLogDetails> HandleException(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ResponseLogDetails()
            {
                ResponseId = string.IsNullOrEmpty(context.Request.Headers?.RequestId.ToString()) ? Guid.NewGuid().ToString() : context.Request.Headers?.RequestId,
                Method = context.Request.Method,
                UserAgent = context.Request.Headers.UserAgent,
                ClientIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                UserDetails = JsonSerializer.Serialize(context.User),
                Body = await context.Request.Body.GetBody(),
                RequestPath = context.Request.GetDisplayUrl(),
                StackTrace = exception.StackTrace,
                StatusCode = context.Response.StatusCode,
                ErrorMessage = exception.Message,
            };

            await context.Response.WriteAsync(response.ToString());
            return response;

        }

    }
}
