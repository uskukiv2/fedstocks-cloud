using gen.fed.web.domain.Abstract;
using gen.fed.web.domain.Factories;
using gen.fed.web.domain.Repositories;
using gen.fed.web.infrastructure;
using gen.fed.web.infrastructure.Factories;
using gen.fed.web.infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace gen.fedstocks.web.Server.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddFed(this IServiceCollection services)
    {
        RegisterRepositories(services);

        RegisterFactories(services);

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection service, ConfigurationManager config)
    {
        service.AddEntityFrameworkNpgsql()
            .AddDbContext<ServiceContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("default"));
            });
        service.AddScoped<IUnitOfWork<NpgsqlConnection>>(sp => sp.GetRequiredService<ServiceContext>());
        return service;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void RegisterFactories(IServiceCollection services)
    {
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
    }
}
