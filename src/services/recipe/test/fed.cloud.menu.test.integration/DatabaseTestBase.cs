using System.Data;
using fed.cloud.common;
using fed.cloud.common.Infrastructure;
using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Factories;
using fed.cloud.menu.infrastructure.Factories;
using fed.cloud.menu.infrastructure.Services;
using fed.cloud.menu.infrastructure.Utilities;
using fed.cloud.menu.test.core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using RepoDb;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Options;


namespace fed.cloud.menu.test.integration;

public class DatabaseTestBase
{
    protected TestDatabaseManager DatabaseManager { get; private set; }

    protected IServiceProvider ServiceProvider { get; private set; }

    protected async Task BaseSetUp(Action<IServiceCollection>? action = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json")
            .AddEnvironmentVariables()
            .Build();

        GlobalConfiguration.Setup(new GlobalConfigurationOptions
        {
            ConversionType = ConversionType.Automatic,
        }).UsePostgreSql();

        DbSettingMapper.Add<NpgsqlConnection>(new PostgreSqlDbSetting(), true);
        
        RepoDbUtilities.MapAllEntities();

        LoadServices(configuration, action);

        DatabaseManager = new TestDatabaseManager(ServiceProvider.GetRequiredService<IDbConnectionFactory>());
    }

    protected async Task ExecuteDatabaseChangingScripts(Func<IDbTransaction, Task> action)
    {
        using var conn = DatabaseManager.CreateConnection();
        conn.Open();
        using var tr = DatabaseManager.BeginTransaction(conn);
        if (tr.Connection is null)
        {
            tr.Rollback();
            return;
        }

        await action.Invoke(tr);
        tr.Commit();
    }

    private void LoadServices(IConfigurationRoot configuration, Action<IServiceCollection>? action = null)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging(l =>
        {
            l.AddConsole();
            l.SetMinimumLevel(LogLevel.Trace);
        });

        serviceCollection.AddSingleton<IServiceConfiguration>(new ServiceConfiguration(GetDatabase(configuration),
            DatabaseType.RepoDb, string.Empty));

        serviceCollection.AddSingleton<IDbConnectionFactory>(sp =>
            new NpgsqlDbConnectionFactory(sp.GetRequiredService<IServiceConfiguration>()));
        
        serviceCollection.AddSingleton<IFetchManager, PostgresFetchManager>();

        serviceCollection.AddSingleton<ITraceFactory, TraceFactory>();

        action?.Invoke(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    private static IDatabase GetDatabase(IConfigurationRoot configurationRoot)
    {
        var connectionString = configurationRoot.GetConnectionString("default");
        return new Database(connectionString, string.Empty);
    }
}