namespace Application.UseCases.Subasta.Command.Creacion
{
    public record AuctionDetailResponse(
    int Id,
    int VendedorId,
    int CategoriaId,
    string Titulo,
    string Descripcion,
    string Url_imagen,
    decimal Precio_base,
    decimal Incremento_minimo,
    DateTime Fecha_inicio,
    DateTime Fecha_fin,
    string Estado,
    byte[] Version
);

}
