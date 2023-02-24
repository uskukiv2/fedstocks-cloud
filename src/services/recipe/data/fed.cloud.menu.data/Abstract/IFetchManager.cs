using System.Linq.Expressions;

namespace fed.cloud.menu.data.Abstract;

public interface IFetchManager
{
    /// <summary>
    /// Returns only one searching instance. SingleOrDefault will be used inside
    /// </summary>
    /// <param name="where"></param>
    /// <typeparam name="TResult">EntityType</typeparam>
    /// <returns>Single instance</returns>
    Task<TResult> GetOneAsync<TResult>(Expression<Func<TResult, bool>> where) where TResult : class;

    Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<TResult, bool>> where) where TResult : class;

    Task<IEnumerable<TResult>> GetByPageAsync<TResult>(int take, int offset,
        Expression<Func<TResult, object>> orderBy, bool isAsc, Expression<Func<TResult, bool>> where = null) where TResult : class;
}