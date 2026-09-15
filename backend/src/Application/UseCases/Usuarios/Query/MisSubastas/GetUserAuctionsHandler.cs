using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuarios.Query.MisSubastas
{
    public sealed class GetUserAuctionsHandler : IRequestHandler<GetUserAuctionsQuery, Result<IReadOnlyList<UserAuctionActivityItem>>>
    {
        private readonly IUsuarioQueryRepository _usuarioRepository;

        public GetUserAuctionsHandler(IUsuarioQueryRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result<IReadOnlyList<UserAuctionActivityItem>>> Handle(
            GetUserAuctionsQuery request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, cancellationToken);

            if (usuario is null)
                return new Failed<IReadOnlyList<UserAuctionActivityItem>>(
                    $"No existe un usuario con Id {request.UsuarioId}.",
                    DataStatus.NotFound);

            var items = await _usuarioRepository.ListAuctionActivityAsync(request.UsuarioId, cancellationToken);

            return new Success<IReadOnlyList<UserAuctionActivityItem>>(items);
        }
    }
}
