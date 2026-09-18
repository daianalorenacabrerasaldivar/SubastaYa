using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class Subasta : IEntity
    {
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

        public bool PuedeActivar(DateTime ahora)
        {
            return Estado == EstadoSubasta.PROGRAMADA && FechaInicio <= ahora;
        }

        public void Activar()
        {
            if (Estado != EstadoSubasta.PROGRAMADA)
                throw new InvalidOperationException("Solo una subasta PROGRAMADA puede activarse.");

            Estado = EstadoSubasta.ACTIVA;
        }

        public bool PuedeFinalizar(DateTime ahora)
        {
            return Estado == EstadoSubasta.ACTIVA && FechaFin <= ahora && Pujas.Count > 0;
        }

        public void Finalizar(DateTime ahora)
        {
            if (Estado != EstadoSubasta.ACTIVA)
            {
                throw new InvalidOperationException("Solo una subasta ACTIVA puede pasar a FINALIZADA.");
            }

            if (FechaFin > ahora)
            {
                throw new InvalidOperationException("La subasta todavía no venció.");
            }

            if (Pujas.Count == 0)
            {
                throw new InvalidOperationException("Una subasta sin pujas no puede finalizar con ganador; corresponde MarcarDesierta.");
            }

            Estado = EstadoSubasta.FINALIZADA;
        }

        public bool PuedeMarcarDesierta(DateTime ahora)
        {
            return Estado == EstadoSubasta.ACTIVA && FechaFin <= ahora && Pujas.Count == 0;
        }

        public void MarcarDesierta(DateTime ahora)
        {
            if (Estado != EstadoSubasta.ACTIVA)
            {
                throw new InvalidOperationException("Solo una subasta ACTIVA puede pasar a DESIERTA.");
            }

            if (FechaFin > ahora)
            {
                throw new InvalidOperationException("La subasta todavía no venció.");
            }

            if (Pujas.Count > 0)
            {
                throw new InvalidOperationException("Una subasta con pujas no puede quedar desierta; corresponde Finalizar.");
            }

            Estado = EstadoSubasta.DESIERTA;
        }

        public static readonly TimeSpan VentanaAntiSniping = TimeSpan.FromSeconds(60);

        public static readonly TimeSpan ExtensionAntiSniping = TimeSpan.FromMinutes(2);

        public Puja? PujaLider => Pujas
            .OrderByDescending(p => p.Monto)
            .ThenByDescending(p => p.FechaPuja)
            .FirstOrDefault();

        public static decimal CalcularMontoMinimo(decimal? ofertaMasAlta, decimal precioBase, decimal incrementoMinimo)
        {
            return ofertaMasAlta.HasValue ? ofertaMasAlta.Value + incrementoMinimo : precioBase;
        }

        public decimal MontoMinimoProximaPuja()
        {
            return CalcularMontoMinimo(PujaLider?.Monto, PrecioBase, IncrementoMinimo);
        }

        public bool EstaAbiertaParaPujas(DateTime ahora)
        {
            return Estado == EstadoSubasta.ACTIVA && FechaInicio <= ahora && ahora < FechaFin;
        }

        public Puja RegistrarPuja(int compradorId, decimal monto, DateTime ahora)
        {
            if (!EstaAbiertaParaPujas(ahora))
            {
                throw new InvalidOperationException("La subasta no está abierta para pujas.");
            }

            if (compradorId == VendedorId)
            {
                throw new InvalidOperationException("El vendedor no puede pujar en su propia subasta.");
            }

            if (PujaLider?.CompradorId == compradorId)
            {
                throw new InvalidOperationException("El postor ya es el líder de la subasta.");
            }

            if (monto < MontoMinimoProximaPuja())
            {
                throw new InvalidOperationException($"El monto mínimo para pujar es {MontoMinimoProximaPuja()}.");
            }

            var puja = new Puja
            {
                SubastaId = Id,
                CompradorId = compradorId,
                Monto = monto,
                FechaPuja = ahora
            };

            Pujas.Add(puja);
            return puja;
        }

        public bool AplicarAntiSniping(DateTime ahora)
        {
            if (!EstaAbiertaParaPujas(ahora))
            {
                throw new InvalidOperationException("La subasta no está abierta para pujas.");
            }

            if (FechaFin - ahora > VentanaAntiSniping)
            {
                return false;
            }

            FechaFin = FechaFin.Add(ExtensionAntiSniping);
            return true;
        }
    }

}
