using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuarios.Query.MisPujas
{
    public sealed record GetUserBidActivityQuery(int UsuarioId) : IRequest<Result<IReadOnlyList<UserBidActivityItem>>>;
}
