using Domain.Entity;

namespace Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerarToken(Usuario usuario);
    }
}
