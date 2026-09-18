namespace Application.UseCases.Usuarios.Query.BuscarUsuarioPorEmail
{
    public sealed class GetUsuarioByEmailResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
