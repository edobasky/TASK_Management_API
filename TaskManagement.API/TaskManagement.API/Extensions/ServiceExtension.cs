using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.API.Services.Implementation;
using TaskManagement.API.Services.Interface;
using TaskManagement.API.Services.Logger;
using TaskManagement.API.Utility;

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

        public static void ConfigureJWTAuth(this IServiceCollection services,IConfiguration configuration) =>
             services.AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               //  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
             }).AddJwtBearer(options =>
             {
                // options.RequireHttpsMetadata = false; // change on prod env
                // options.SaveToken = true;
                options.IncludeErrorDetails = true;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidIssuer = configuration["JwtConfig:Issuer"],
                     ValidAudience = configuration["JwtConfig:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Key"]!)),
                     ValidateAudience = false,
                     ValidateIssuer = false,
                     ValidateIssuerSigningKey = false,
                     ValidateLifetime = false,
                     ClockSkew = TimeSpan.FromMinutes(3), // 1 min for sensitive systems
                 };
             });

        public static void ConfigureTokenServ(this IServiceCollection services) =>
            services.AddScoped<IJWTService, JWTService>();

        public static void ConfigureTaskService(this IServiceCollection services) =>
            services.AddScoped<ITaskService,TaskServiceImp>();
    }
}
