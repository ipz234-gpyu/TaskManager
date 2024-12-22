using System.Numerics;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Models;
using TaskManager.Server.Application.Services;
using TaskManager.Server.Application.Services.Authentication;

namespace TaskManager.Server.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
           this IServiceCollection services,
           ConfigurationManager configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JWT"));
            services.Configure<PasswordHasherSettings>(configuration.GetSection("PasswordHasher"));

            services.AddSingleton<IJwtTokenUtils, JwtTokenUtils>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddSingleton<IEmailSender, EmailSender>();

            return services;
        }
    }
}
