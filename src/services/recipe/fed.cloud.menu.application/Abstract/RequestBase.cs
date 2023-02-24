using MediatR;

namespace fed.cloud.recipe.application.Abstract;

public class RequestBase : IRequest
{
    protected RequestBase(Guid userId){ UserId = userId; }
    
    public Guid UserId { get; }
}

public class RequestBase<T> : IRequest<T>
{
    protected RequestBase(Guid userId){ UserId = userId; }
    
    public Guid UserId { get; }
}