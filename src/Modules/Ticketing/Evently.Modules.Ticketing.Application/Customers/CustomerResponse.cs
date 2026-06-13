namespace Evently.Modules.Ticketing.Application.Customers;

public sealed record CustomerResponse(Guid Id, string Email, string FirstName, string LastName);
