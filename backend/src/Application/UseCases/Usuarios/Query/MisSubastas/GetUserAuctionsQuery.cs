using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuarios.Query.MisSubastas
{
    public sealed record GetUserAuctionsQuery(int UsuarioId) : IRequest<Result<IReadOnlyList<UserAuctionActivityItem>>>;
}
