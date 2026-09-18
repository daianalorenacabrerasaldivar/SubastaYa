using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Auth
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
}
