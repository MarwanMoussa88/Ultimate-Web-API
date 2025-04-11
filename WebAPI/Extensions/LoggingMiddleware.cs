using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Domain.Log;
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
            context.Request.EnableBuffering();

            Stream orignalResposneBody = context.Response.Body;
            using MemoryStream responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var requestLog = await LogRequest(context);
            await _next.Invoke(context);
            var responseLog = await LogResponse(context, requestLog);

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
                RequestBody = await GetBody(context.Request.Body),
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
                ResponseBody = await GetBody(context.Response.Body),
                RequestPath = context.Request.GetDisplayUrl(),
                StackTrace = Environment.StackTrace,
                StatusCode = context.Response.StatusCode
            };
            return log;
        }

        private async Task<string> GetBody(Stream body)
        {
            body.Position = 0;
            using StreamReader streamReader = new StreamReader(body, Encoding.UTF8, leaveOpen: true, bufferSize: 8192);

            return await streamReader.ReadToEndAsync();
        }

    }
}
