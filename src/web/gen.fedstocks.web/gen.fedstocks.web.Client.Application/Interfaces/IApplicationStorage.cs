using System.Linq.Expressions;

namespace gen.fedstocks.web.Client.Application.Interfaces;

public interface IApplicationStorage
{
    Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> where) where T : notnull;

    Task<IEnumerable<T>> GetItemsAsync<T>() where T : notnull;

    Task SetItemAsync<T>(T item) where T : notnull;
}