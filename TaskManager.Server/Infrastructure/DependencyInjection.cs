using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;
using TaskManager.Server.Infrastructure.Repositories;

namespace TaskManager.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {

        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserRefreshTokenRepository, UserRefreshTokenRepository>();
        services.AddSingleton<IGroupsFromUserRepository, GroupsFromUserRepository>();
        services.AddSingleton<IGroupsFromTeamRepository, GroupsFromTeamRepository>();
        services.AddSingleton<IListRepository, ListRepository>();
        services.AddSingleton<ITaskRepository, TaskRepository>();
        services.AddSingleton<ITeamRepository, TeamRepository>();
        services.AddSingleton<ITeamInvitationRepository, TeamInvitationRepository>();
        services.AddSingleton<IUserTeamRepository, UserTeamRepository>();
        services.AddSingleton<ITagRepository, TagRepository>();

        services.AddAuth(configuration);

        return services;
    }

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        return services;
    }
}
