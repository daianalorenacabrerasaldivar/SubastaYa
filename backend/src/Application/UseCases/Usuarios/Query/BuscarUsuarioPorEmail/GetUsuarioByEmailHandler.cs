using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Usuarios.Query.BuscarUsuarioPorEmail
{
    public sealed class GetUsuarioByEmailHandler
        : IRequestHandler<GetUsuarioByEmailQuery, Result<GetUsuarioByEmailResponse>>
    {
        private readonly IUsuarioQueryRepository _usuarios;

        public GetUsuarioByEmailHandler(IUsuarioQueryRepository usuarios)
        {
            _usuarios = usuarios;
        }

        public async Task<Result<GetUsuarioByEmailResponse>> Handle(
            GetUsuarioByEmailQuery request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarios.GetByEmailAsync(request.Email, cancellationToken);

            if (usuario is null)
                return new Failed<GetUsuarioByEmailResponse>(
                    $"No existe un usuario con el email '{request.Email}'.",
                    DataStatus.NotFound);

            return new Success<GetUsuarioByEmailResponse>(new GetUsuarioByEmailResponse
            {
                Id     = usuario.Id,
                Email  = usuario.Email,
                Nombre = usuario.Nombre,
                Rol    = usuario.Rol.ToString(),
            });
        }
    }
}
