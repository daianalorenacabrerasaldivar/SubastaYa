using Domain.Enum;

namespace Domain.Entity
{
    public class Subasta
    {
        public int Id { get; set; }

        public int VendedorId { get; set; }

        public int CategoriaId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string? UrlImagen { get; set; }

        public decimal PrecioBase { get; set; }

        public decimal IncrementoMinimo { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public EstadoSubasta Estado { get; set; }

        public int Version { get; set; }

        // Navegaciones
        public Usuario Vendedor { get; set; } = null!;

        public Categoria Categoria { get; set; } = null!;

        public ICollection<Puja> Pujas { get; set; } = new List<Puja>();

        public ICollection<TransaccionLedger> Transacciones { get; set; } = new List<TransaccionLedger>();
    }

}
