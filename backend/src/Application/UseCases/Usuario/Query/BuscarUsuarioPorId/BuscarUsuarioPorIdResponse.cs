namespace Application.UseCases.Usuario.Query.BuscarUsuarioPorId
{
    public class BuscarUsuarioPorIdResponse
    {
        public int Id { get;  set;}
        public string Email { get;  set;}
        public string Nombre { get;  set;}
        public DateTime FechaRegistro { get;  set;}

    }
}
