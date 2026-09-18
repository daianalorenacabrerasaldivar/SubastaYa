using Domain.Enum;

namespace Application.UseCases.Auth
{
    public sealed record LoginResponse(
        int UsuarioId,
        string Nombre,
        string Email,
        RolUsuario Rol,
        string Token,
        DateTime ExpiraEn);
}
