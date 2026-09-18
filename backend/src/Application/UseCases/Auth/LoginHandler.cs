using Application.Interfaces.Persistencia;
using Application.Interfaces.Services;
using Domain.Common.ResultPattern;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Application.UseCases.Auth
{
    public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IUsuarioQueryRepository _usuarioRepository;
        private readonly IJwtTokenService _jwtService;

        public LoginHandler(IUsuarioQueryRepository usuarioRepository, IJwtTokenService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return new Failed<LoginResponse>("Email y contraseña son requeridos.", DataStatus.RequestValidation);

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email.Trim().ToLower(), cancellationToken);

            if (usuario is null)
                return new Failed<LoginResponse>("Credenciales incorrectas.", DataStatus.NotFound);

            var hash = HashPassword(request.Password);

            if (usuario.PasswordHash != hash)
                return new Failed<LoginResponse>("Credenciales incorrectas.", DataStatus.NotFound);

            var token = _jwtService.GenerarToken(usuario);
            var expira = DateTime.UtcNow.AddHours(24);

            return new Success<LoginResponse>(new LoginResponse(
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol,
                token,
                expira));
        }

        private static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
