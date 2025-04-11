using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.HttpOverrides;

namespace WebAPI.Extensions
{
    public static class Middlewares
    {
        //middlewares are registered in the IApplicationBuilder interface
        //app.run is a terminating middleware so it should be the last middleware
        public static void TestRun(this IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello middleware 2!\n");
            });
        }

        //app.use chains middlewares together and accepts the next function to invoke the next middleware
        public static void TestUse(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("hello before TestMiddleware1 1\n");
                await next();
                await context.Response.WriteAsync("hello after TestMiddleware1 1\n");

            });
        }
        //app.map enters he middleware based on a route
        public static void TestMap(this IApplicationBuilder app)
        {
            app.Map("/testmap", builder =>
            {
                builder.Run(async (context) =>
                {
                    await context.Response.WriteAsync(JsonSerializer.Serialize(context.User));
                });
            });
        }

        //app.map enters he middleware based on a condition

        public static void TestMapWhen(this IApplicationBuilder app)
        {
            app.MapWhen(context => context.Request.Query.ContainsKey("abc"), builder =>
            {
                builder.Run(async (context) =>
                {
                    await context.Response.WriteAsync(JsonSerializer.Serialize(context.Request.Host));
                });
            });
        }

        public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }

        public static void UseForwardHeaderConfiguration(this IApplicationBuilder builder)
        {
            //forwards proxy/load-balancer headers to the current request
            //proxy is a server that intercepts the request and forwards them to a server
            builder.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
        }
    }
}
