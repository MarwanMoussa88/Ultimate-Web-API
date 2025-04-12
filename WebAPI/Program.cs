using System.Reflection.Metadata;
using Presentation;
using WebAPI.Extensions;
//Add config using builder.configuration
//registering services using builder.services
//logging configuration using builder.logging
//IHost and IWebHostConfigs

var builder = WebApplication.CreateBuilder(args);

//builder.Services.ConfigureSerilog(builder.Configuration);

//registers controllers only in the IServiceCollection and not views
builder.Services.AddControllers().AddApplicationPart(typeof(PresentationAssemblyReference).Assembly);
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureAppsettingsOptions(builder.Configuration);
builder.Services.ConfigureServices();
//IHost which starts or stops the host
//IApplicationBuilder which we use to build Middleware pipeline
//IEndpointRouteBuilder adds endpoints to our app
var app = builder.Build();

app.UseLogMiddleware();

if (!app.Environment.IsDevelopment())
    //tells the browser that the site should be accessed only from https 
    app.UseHsts();

//redirects http to https
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseForwardHeaderConfiguration();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
