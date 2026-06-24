using Evently.Common.Application.Exceptions;
using Evently.Common.Domain;
using Evently.Modules.Ticketing.Application.Customers;
using Evently.Modules.Users.IntegrationEvents;
using MassTransit;
using MediatR;

namespace Evently.Modules.Ticketing.Presentation.Customers;

public class UserRegisteredIntegrationEventConsumer(ISender sender): IConsumer<UserRegisteredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context)
    {
        Result result = await sender.Send(
            new CreateCustomerCommand(context.Message.UserId, context.Message.Email, context.Message.FirstName, context.Message.LastName)
            );

        if (result.IsFailure)
        {
            throw new EventlyException(nameof(CreateCustomerCommand), result.Error);
        }
    }
}
