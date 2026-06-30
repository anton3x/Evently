using System.Data.Common;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Ticketing.Domain.Orders;

namespace Evently.Modules.Ticketing.Application.Orders;

public sealed record GetOrdersQuery(Guid CustomerId) : IQuery<IReadOnlyCollection<OrderSummaryResponse>>;

internal sealed class GetOrdersQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetOrdersQuery, IReadOnlyCollection<OrderSummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<OrderSummaryResponse>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 id AS {nameof(OrderSummaryResponse.Id)},
                 customer_id AS {nameof(OrderSummaryResponse.CustomerId)},
                 status AS {nameof(OrderSummaryResponse.Status)},
                 total_price AS {nameof(OrderSummaryResponse.TotalPrice)},
                 created_at_utc AS {nameof(OrderSummaryResponse.CreatedAtUtc)}
             FROM ticketing.orders
             WHERE customer_id = @CustomerId
             """;

        List<OrderSummaryResponse> orders = (await connection.QueryAsync<OrderSummaryResponse>(sql, request)).AsList();

        return orders;
    }
}

public sealed record OrderSummaryResponse(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    decimal TotalPrice,
    DateTime CreatedAtUtc);
