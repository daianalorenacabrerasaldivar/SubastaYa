using Domain.Enum;

namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record AuctionDetail(
        int Id,
        int VendedorId,
        string VendedorNombre,
        int CategoriaId,
        string CategoriaNombre,
        string Titulo,
        string Descripcion,
        string UrlImagen,
        decimal PrecioBase,
        decimal IncrementoMinimo,
        decimal? OfertaMasAlta,
        int CantidadOfertas,
        int? PostorLiderId,
        DateTime FechaInicio,
        DateTime FechaFin,
        EstadoSubasta Estado,
        byte[] Version);
}
