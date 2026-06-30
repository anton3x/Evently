using Evently.Common.Application.Messaging;
using Evently.Common.Domain;

namespace Evently.Modules.Ticketing.Application.Carts;

public sealed record GetCartQuery(Guid CustomerId) : IQuery<Cart>;

internal sealed class GetCartQueryHandler(CartService cartService) : IQueryHandler<GetCartQuery, Cart>
{
    public async Task<Result<Cart>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        return await cartService.GetAsync(request.CustomerId, cancellationToken);
    }
}
