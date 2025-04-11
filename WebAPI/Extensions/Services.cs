using Contracts;
using Domain.Appsettings;
using Repository;
using Services;

namespace WebAPI.Extensions
{
    public static class Services
    {
        //Cors is the ability to give or restrict access to application of certain domains
        //To Register Services in DI we must use the IServiceCollection interface
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
        }

        //IIS is the web server our .NET application is going to be deployed on
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        public static void ConfigureAppsettingsOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionString>(configuration.GetSection(ConnectionString.Key));
            services.Configure<AllowedHost>(configuration.GetSection(AllowedHost.Key));

        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ISqlHelper, SqlHelper>();
            services.AddScoped<IDapperHelper, DapperHelper>();
            services.AddScoped<IEmployeeService, EmployeeService>();

        }

    }
}
