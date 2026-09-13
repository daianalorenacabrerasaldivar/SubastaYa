namespace Application.UseCases.Subastas.Command.Creacion
{
    public record AuctionDetailResponse(
    int Id,
    int VendedorId,
    int CategoriaId,
    string Titulo,
    string Descripcion,
    string UrlImagen,
    decimal PrecioBase,
    decimal IncrementoMinimo,
    DateTime FechaInicio,
    DateTime FechaFin,
    string Estado,
    byte[] Version
);

}
