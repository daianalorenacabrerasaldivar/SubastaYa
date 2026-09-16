namespace Application.UseCases.Auth
{
    public sealed record LoginResponse(
        int UsuarioId,
        string Nombre,
        string Email,
        string Token,
        DateTime ExpiraEn);
}
