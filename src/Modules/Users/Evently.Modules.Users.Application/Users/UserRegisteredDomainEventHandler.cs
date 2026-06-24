using Evently.Common.Application.EventBus;
using Evently.Common.Application.Exceptions;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Users.Domain.Users;
using Evently.Modules.Users.IntegrationEvents;
using MediatR;

namespace Evently.Modules.Users.Application.Users;

internal sealed class UserRegisteredDomainEventHandler(
    IEventBus eventBus,
    ISender sender
    ): IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await sender.Send(new GetUserQuery(notification.UserId), cancellationToken);

        if (result.IsFailure)
        {
            throw new EventlyException(nameof(GetUserQuery), result.Error);
        }
        
        await eventBus.PublishAsync(
            new UserRegisteredIntegrationEvent(
                notification.Id,
                notification.OccurredOnUtc,
                result.Value.Id, 
                result.Value.Email, 
                result.Value.FirstName, 
                result.Value.LastName),
            cancellationToken);
    }
}
