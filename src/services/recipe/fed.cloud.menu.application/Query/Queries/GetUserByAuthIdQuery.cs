using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;

#nullable enable

namespace fed.cloud.recipe.application.Query.Queries;

public class GetUserByAuthIdQuery : IQuery<User>
{
    public GetUserByAuthIdQuery(string authId)
    {
        AuthId = authId;
    }

    public string AuthId { get; }
}

public class GetUserByAuthIdQueryHandler : IQueryHandler<GetUserByAuthIdQuery, User>
{
    private readonly IFetchManager _fetchManager;

    public GetUserByAuthIdQueryHandler(IFetchManager fetchManager)
    {
        _fetchManager = fetchManager;
    }
    
    public async Task<User?> Handle(GetUserByAuthIdQuery query)
    {
        return await _fetchManager.GetOneAsync<User>(user => user.AuthenticationId == query.AuthId && user.IsActive);
    }
}