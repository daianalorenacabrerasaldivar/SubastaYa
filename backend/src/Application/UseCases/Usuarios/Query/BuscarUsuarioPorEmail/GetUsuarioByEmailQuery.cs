using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuarios.Query.BuscarUsuarioPorEmail
{
    public sealed record GetUsuarioByEmailQuery(string Email)
        : IRequest<Result<GetUsuarioByEmailResponse>>;
}
