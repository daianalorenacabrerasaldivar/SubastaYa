using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuarios.Query.BuscarUsuarioPorId
{
    public sealed class GetUsuarioByIdQueryHandler
    : IRequestHandler<
        GetUsuarioByIdQuery,
        Result<GetUsuarioByIdResponse>>
    {
        private readonly IUsuarioQueryRepository _usuarioRepository;

        public GetUsuarioByIdQueryHandler(
            IUsuarioQueryRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result<GetUsuarioByIdResponse>> Handle(
            GetUsuarioByIdQuery request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(
                request.UsuarioId,
                cancellationToken);

            if (usuario is null)
            {
                return new Failed<GetUsuarioByIdResponse>(
                    $"No existe el usuario con Id {request.UsuarioId}.",
                    DataStatus.NotFound);
            }

            var response = new GetUsuarioByIdResponse
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                FechaRegistro = usuario.FechaRegistro
            };

            return new Success<GetUsuarioByIdResponse>(response);
        }
    }

}
