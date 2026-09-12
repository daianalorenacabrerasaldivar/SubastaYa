using Domain.Enum;

namespace Domain.Entity
{
    public class Subasta
    {
        private Subasta()
        {
        }

        public int Id { get; set; }

        public int VendedorId { get; set; }

        public int CategoriaId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string UrlImagen { get; set; } = string.Empty;

        public decimal PrecioBase { get; set; }

        public decimal IncrementoMinimo { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public EstadoSubasta Estado { get; set; }

        public byte[] Version { get; set; } = Array.Empty<byte>();

        // Navegaciones
        public Usuario Vendedor { get; set; } = null!;

        public Categoria Categoria { get; set; } = null!;

        public ICollection<Puja> Pujas { get; set; } = new List<Puja>();

        public ICollection<TransaccionLedger> Transacciones { get; set; } = new List<TransaccionLedger>();

        public static Subasta Create(
            int vendedorId,
            int categoriaId,
            string titulo,
            string descripcion,
            string urlImagen,
            decimal precioBase,
            decimal incrementoMinimo,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            if (vendedorId <= 0)
            {
                throw new ArgumentException("El Id del vendedor debe ser mayor que cero.", nameof(vendedorId));
            }

            if (categoriaId <= 0)
            {
                throw new ArgumentException("El Id de la categoría debe ser mayor que cero.", nameof(categoriaId));
            }

            if (string.IsNullOrWhiteSpace(titulo))
            {
                throw new ArgumentException("El título es obligatorio.", nameof(titulo));
            }

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new ArgumentException("La descripción es obligatoria.", nameof(descripcion));
            }

            if (string.IsNullOrWhiteSpace(urlImagen))
            {
                throw new ArgumentException("La URL de la imagen es obligatoria.", nameof(urlImagen));
            }

            if (precioBase <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precioBase), "El precio base debe ser mayor que cero.");
            }

            if (incrementoMinimo <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(incrementoMinimo), "El incremento mínimo debe ser mayor que cero.");
            }

            if (fechaFin <= fechaInicio)
            {
                throw new ArgumentException("La fecha de finalización debe ser posterior a la fecha de inicio.", nameof(fechaFin));
            }

            return new Subasta
            {
                VendedorId = vendedorId,
                CategoriaId = categoriaId,
                Titulo = titulo.Trim(),
                Descripcion = descripcion.Trim(),
                UrlImagen = urlImagen.Trim(),
                PrecioBase = precioBase,
                IncrementoMinimo = incrementoMinimo,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Estado = fechaInicio > DateTime.UtcNow ? EstadoSubasta.PROGRAMADA : EstadoSubasta.ACTIVA
            };
        }
    }

}
