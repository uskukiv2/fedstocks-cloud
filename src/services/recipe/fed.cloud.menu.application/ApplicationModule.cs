using System.Reflection;
using Autofac;
using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;
using fed.cloud.recipe.application.Abstract;
using fed.cloud.recipe.application.Query;
using fed.cloud.recipe.application.Query.Queries;

namespace fed.cloud.recipe.application;

public class ApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<QueryProcessor>()
            .As<IQueryProcessor>()
            .SingleInstance();

        builder.RegisterAssemblyTypes(typeof(GetUserByAuthIdQuery).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IQueryHandler<,>));
    }
}