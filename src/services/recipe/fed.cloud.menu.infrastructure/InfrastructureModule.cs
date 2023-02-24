using System.Reflection;
using Autofac;
using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.infrastructure.Services;

namespace fed.cloud.menu.infrastructure;

public class InfrastructureModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        builder.RegisterAssemblyOpenGenericTypes(assembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        RegisterFactories(builder, assembly);

        RegisterServices(builder);
    }

    private void RegisterFactories(ContainerBuilder builder, Assembly assembly)
    {
        builder.RegisterAssemblyOpenGenericTypes(assembly)
            .Where(t => t.Name.EndsWith("Factory"))
            .AsImplementedInterfaces()
            .SingleInstance();
    }

    private void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<PostgresFetchManager>()
            .As<IFetchManager>()
            .SingleInstance();

        builder.RegisterType<AuditManager>()
            .As<IAuditManager>()
            .SingleInstance();
    }
}