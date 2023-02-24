namespace fed.cloud.menu.data.Abstract;

public interface IQuery<TResult> where TResult : notnull
{
}

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult> where TResult : notnull
{
    Task<TResult?> Handle(TQuery query);
}