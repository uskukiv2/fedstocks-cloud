using System.Linq.Expressions;
using System.Runtime.InteropServices;
using fed.cloud.common.Infrastructure;
using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Factories;
using fed.cloud.menu.infrastructure.Factories;
using RepoDb;
using RepoDb.Enumerations;

namespace fed.cloud.menu.infrastructure.Services;

public class PostgresFetchManager : IFetchManager
{
    private readonly ITraceFactory _traceFactory;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly string _connectionString;

    public PostgresFetchManager(IServiceConfiguration serviceConfiguration, ITraceFactory traceFactory,
        IDbConnectionFactory connectionFactory)
    {
        _traceFactory = traceFactory;
        _connectionFactory = connectionFactory;
        _connectionString = serviceConfiguration.Database.Connection;
    }

    public async Task<TResult> GetOneAsync<TResult>(Expression<Func<TResult, bool>> where) where TResult : class
    {
        await using var conn = _connectionFactory.CreateConnection(_connectionString);
        return (await conn.QueryAsync(where, trace: _traceFactory.Create<TResult>())).FirstOrDefault()!;
    }

    public async Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<TResult, bool>> where)
        where TResult : class
    {
        await using var conn = _connectionFactory.CreateConnection(_connectionString);
        var genericType = typeof(TResult);
        if (genericType.BaseType?.Name != nameof(ReadModel))
        {
            return await conn.QueryAsync(where, trace: _traceFactory.Create<TResult>());
        }

        var whereParsed = QueryGroup.Parse(where);
        var query = conn.GetStatementBuilder().CreateQuery(GetTableName(genericType),
            GetFieldsFromGeneric<TResult>(), where: whereParsed);

        return await conn.ExecuteQueryAsync<TResult>(query, whereParsed, trace: _traceFactory.Create<TResult>());
    }

    public async Task<IEnumerable<TResult>> GetByPageAsync<TResult>(int take, int offset,
        Expression<Func<TResult, object>> orderBy, bool isAsc, Expression<Func<TResult, bool>> where = null)
        where TResult : class
    {
        await using var conn = _connectionFactory.CreateConnection(_connectionString);
        var genericType = typeof(TResult);
        if (genericType.BaseType?.Name != nameof(ReadModel))
        {
            return await conn.BatchQueryAsync(take, offset, Array.Empty<OrderField>(), where,
                trace: _traceFactory.Create<TResult>());
        }
        
        var parsedWhere = QueryGroup.Parse(where);
        var query = conn.GetStatementBuilder().CreateBatchQuery(GetTableName(genericType),
            GetFieldsFromGeneric<TResult>(), offset, take,
            new[] { OrderField.Parse(orderBy, isAsc ? Order.Ascending : Order.Descending) }, where: parsedWhere);

        return await conn.ExecuteQueryAsync<TResult>(query, parsedWhere, trace: _traceFactory.Create<TResult>());
    }

    private static IEnumerable<Field> GetFieldsFromGeneric<T>() where T : class
    {
        var properties = typeof(T).GetProperties().Where(x =>
            x.GetAccessors().All(y => y is { IsAbstract: false, IsPublic: true, IsStatic: false, IsVirtual: false }));
        foreach (var property in properties)
        {
            yield return new Field(PropertyMapper.Get<T>(property.Name));
        }
    }

    private static string GetTableName(Type generic)
    {
        return ClassMapper.Get(generic);
    }
}