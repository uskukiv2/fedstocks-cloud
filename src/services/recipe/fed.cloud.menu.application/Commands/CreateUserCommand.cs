using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fed.cloud.recipe.application.Commands;

public class CreateUserNotificationCommand : INotification
{
    public CreateUserNotificationCommand(Guid userId, string authId)
    {
        UserId = userId;
        AuthId = authId;
    }
    
    public Guid UserId { get; }
    
    public string AuthId { get; }
}

public class CreateUserNotificationCommandHandler : INotificationHandler<CreateUserNotificationCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserNotificationCommandHandler> _logger;

    public CreateUserNotificationCommandHandler(IUserRepository userRepository, ILogger<CreateUserNotificationCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public Task Handle(CreateUserNotificationCommand notification, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = notification.UserId,
            AuthenticationId = notification.AuthId,
            IsActive = true
        };

        _userRepository.Add(user, cancellationToken);
        
        return Task.CompletedTask;
    }
}