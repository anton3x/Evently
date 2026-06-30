using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Ticketing.Domain.Customers;
using FluentValidation;

namespace Evently.Modules.Ticketing.Application.Carts;

public sealed record ClearCartCommand(Guid CustomerId) : ICommand;

internal sealed class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
{
    public ClearCartCommandValidator()
    {
        RuleFor(c => c.CustomerId).NotEmpty();
    }
}

internal sealed class ClearCartCommandHandler(ICustomerRepository customerRepository, CartService cartService)
    : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        Customer? customer = await customerRepository.GetAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure(CustomerErrors.NotFound(request.CustomerId));
        }

        await cartService.ClearAsync(customer.Id, cancellationToken);

        return Result.Success();
    }
}
