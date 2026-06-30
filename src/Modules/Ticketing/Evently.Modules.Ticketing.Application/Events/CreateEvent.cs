using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Ticketing.Application.Abstractions.Data;
using Evently.Modules.Ticketing.Domain.Events;
using FluentValidation;

namespace Evently.Modules.Ticketing.Application.Events;

public sealed record CreateEventCommand(
    Guid EventId,
    string Title,
    string Description,
    string Location,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    List<CreateEventCommand.TicketTypeRequest> TicketTypes) : ICommand
{
    public sealed record TicketTypeRequest(
        Guid TicketTypeId,
        Guid EventId,
        string Name,
        decimal Price,
        string Currency,
        decimal Quantity);
}

internal sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(c => c.EventId).NotEmpty();
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.Location).NotEmpty();
        RuleFor(c => c.StartsAtUtc).NotEmpty();
        RuleFor(c => c.EndsAtUtc).Must((cmd, endsAt) => endsAt > cmd.StartsAtUtc).When(c => c.EndsAtUtc.HasValue);

        RuleForEach(c => c.TicketTypes)
            .ChildRules(t =>
            {
                t.RuleFor(r => r.EventId).NotEmpty();
                t.RuleFor(r => r.Name).NotEmpty();
                t.RuleFor(r => r.Price).GreaterThan(decimal.Zero);
                t.RuleFor(r => r.Currency).NotEmpty();
                t.RuleFor(r => r.Quantity).GreaterThan(decimal.Zero);
            });
    }
}

internal sealed class CreateEventCommandHandler(
    IEventRepository eventRepository,
    ITicketTypeRepository ticketTypeRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateEventCommand>
{
    public async Task<Result> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = Event.Create(
            request.EventId,
            request.Title,
            request.Description,
            request.Location,
            request.StartsAtUtc,
            request.EndsAtUtc);

        eventRepository.Insert(@event);

        IEnumerable<TicketType> ticketTypes = request.TicketTypes
            .Select(t => TicketType.Create(t.TicketTypeId, t.EventId, t.Name, t.Price, t.Currency, t.Quantity));

        ticketTypeRepository.InsertRange(ticketTypes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
