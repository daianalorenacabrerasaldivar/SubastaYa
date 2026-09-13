using Domain.Enum;

namespace Application.UseCases.Subastas.Query.GetAuctionById
{
    public sealed record GetAuctionByIdResponse(
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
        decimal ProximaPujaMinima,
        DateTime FechaInicio,
        DateTime FechaFin,
        EstadoSubasta Estado,
        DateTime FechaServidor,
        byte[] Version);
}
