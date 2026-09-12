using Domain.Common;
namespace Domain.Entity
{
    public class Categoria : IEntity
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? UrlIcono { get; set; }

        // Navegación
        public ICollection<Subasta> Subastas { get; set; } = new List<Subasta>();
    }

}
