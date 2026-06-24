using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Users.Application.Abstractions.Data;
using Evently.Modules.Users.Domain.Users;
using FluentValidation;

namespace Evently.Modules.Users.Application.Users;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName) : ICommand<Guid>;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.Email, request.FirstName, request.LastName);
        
        userRepository.Insert(user);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return user.Id;
    }
}

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Email).EmailAddress();
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty();
    }
}
