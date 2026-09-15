using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuarios.Query.MisPujas
{
    public sealed class GetUserBidActivityHandler : IRequestHandler<GetUserBidActivityQuery, Result<IReadOnlyList<UserBidActivityItem>>>
    {
        private readonly IUsuarioQueryRepository _usuarioRepository;

        public GetUserBidActivityHandler(IUsuarioQueryRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result<IReadOnlyList<UserBidActivityItem>>> Handle(
            GetUserBidActivityQuery request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, cancellationToken);

            if (usuario is null)
                return new Failed<IReadOnlyList<UserBidActivityItem>>(
                    $"No existe un usuario con Id {request.UsuarioId}.",
                    DataStatus.NotFound);

            var items = await _usuarioRepository.ListBidActivityAsync(request.UsuarioId, cancellationToken);

            return new Success<IReadOnlyList<UserBidActivityItem>>(items);
        }
    }
}
