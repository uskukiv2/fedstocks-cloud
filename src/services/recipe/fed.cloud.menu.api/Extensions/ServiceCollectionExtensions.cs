using fed.cloud.menu.api.Mappings;
using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.infrastructure;
using fed.cloud.menu.infrastructure.Utilities;
using fed.cloud.recipe.application.Behaviors;
using fed.cloud.recipe.application.Commands;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using RepoDb;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Options;

namespace fed.cloud.menu.api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection service)
    {
        return service;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection service, ConfigurationManager manager)
    {
        service.AddEntityFrameworkNpgsql()
            .AddDbContext<MenuContext>(opt =>
            {
                opt.UseNpgsql(manager.GetConnectionString("default"));
            });
        
        service.AddScoped<IUnitOfWork<NpgsqlConnection>>(sp => sp.GetRequiredService<MenuContext>());

        GlobalConfiguration.Setup(new GlobalConfigurationOptions
        {
            ConversionType = ConversionType.Automatic,
        }).UsePostgreSql();

        DbSettingMapper.Add<NpgsqlConnection>(new PostgreSqlDbSetting(), true);
        
        RepoDbUtilities.MapAllEntities();
        
        return service;
    }

    public static IServiceCollection AddMediator(this IServiceCollection service)
    {
        service.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateNewRecipeCommandHandler).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        return service;
    }

    public static IServiceCollection AddMapper(this IServiceCollection service)
    {
        service.AddTransient<IMapper, Mapper>();
        TypeAdapterConfig.GlobalSettings.Scan(typeof(RecipeRegister).Assembly);
        return service;
    }
}