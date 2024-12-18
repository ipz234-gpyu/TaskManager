using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;
using TaskManager.Server.Infrastructure.Repositories;

namespace TaskManager.Server.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            ConfigurationManager configuration)
        {

            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

            services.AddSingleton<IUserRepository, UserRepository>();

            //services.AddAuth(configuration);

            return services;
        }
    }
}
