﻿using gen.fedstocks.web.Client.Application.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace gen.fedstocks.web.Client.Application.Factories.Impl;

public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _provider;

    public ViewModelFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public T Create<T>() where T : BaseViewModel
    {
        return (T)Create(typeof(T));
    }

    public BaseViewModel Create(Type type)
    {
        return (BaseViewModel)CreateByType(type);
    }

    private object CreateByType(Type type)
    {
        return _provider.GetRequiredService(type);
    }

}