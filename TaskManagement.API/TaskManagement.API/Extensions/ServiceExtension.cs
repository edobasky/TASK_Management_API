using System.Runtime.CompilerServices;
using TaskManagement.API.Services.Implementation;
using TaskManagement.API.Services.Interface;
using TaskManagement.API.Services.Logger;

namespace TaskManagement.API.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(opt =>
           {
               opt.AddPolicy("Corspolicy", builder =>
               {
                   builder.AllowAnyOrigin();
                   builder.AllowAnyMethod();
                   builder.AllowAnyHeader();
               });
           });
        }

        // Deploying to IIS default configuration
        public static void ConfigureIISIntegration(this IServiceCollection service)
        {
            service.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services) => 
            services.AddSingleton<ILoggerServ, LoggerServImp>();

        public static void ConfigureAuthService(this IServiceCollection services) =>
            services.AddScoped<IAuthenticationService,AuthenticationServiceImp>();

       // public static void 
    }
}
