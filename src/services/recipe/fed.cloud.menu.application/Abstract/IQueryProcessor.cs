using fed.cloud.menu.data.Abstract;

namespace fed.cloud.recipe.application.Abstract;

#nullable disable

public interface IQueryProcessor
{
    Task<TResult> Process<TResult>(IQuery<TResult> query) where TResult : class;
}