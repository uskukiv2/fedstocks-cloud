using System.Collections.Concurrent;
using fed.cloud.menu.data.Abstract;
using fed.cloud.recipe.application.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace fed.cloud.recipe.application.Query;

public class QueryProcessor : IQueryProcessor
{
    private readonly IServiceProvider _serviceProvider;
    
    public QueryProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public Task<TResult> Process<TResult>(IQuery<TResult> query) where TResult : class
    {
        var handler = GetHandler(query);

        return handler.Handle((dynamic)query)!;
    }

    private dynamic GetHandler<TResult>(IQuery<TResult> query) where TResult : class
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

        dynamic implType = _serviceProvider.GetRequiredService(handlerType)!;

        return implType;
    }
}