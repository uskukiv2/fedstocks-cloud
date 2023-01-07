using gen.fed.application.Abstract;
using gen.fedstocks.web.server.Abstract;
using Microsoft.AspNetCore.Components;

namespace gen.fedstocks.web.server.Services;

public interface IPageManager : IFedService
{
    string GetPageUrlByViewModel<T>(T viewModel) where T : BaseViewModel;
}

public class PageManager : IPageManager
{
    private readonly IServiceProvider _serviceProvider;

    public PageManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string GetPageUrlByViewModel<T>(T viewModel) where T : BaseViewModel
    {
        var page = GetPageType(viewModel);

        var routeLink = (RouteAttribute)Attribute.GetCustomAttribute(page, typeof(RouteAttribute))!;

        return routeLink!.Template;
    }

    private Type GetPageType<T>(T viewModel) where T : BaseViewModel
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => x.BaseType != null && x.BaseType.Name == typeof(FedComponentBase<>).Name);
        foreach (var type in types)
        {
            if (type.BaseType.GenericTypeArguments[0].Name == viewModel.GetType().Name)
            {
                return type;
            }
        }

        return null!;
    }
}