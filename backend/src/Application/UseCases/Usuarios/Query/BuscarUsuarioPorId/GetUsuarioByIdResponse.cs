namespace Application.UseCases.Usuarios.Query.BuscarUsuarioPorId
{
    public class GetUsuarioByIdResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}
