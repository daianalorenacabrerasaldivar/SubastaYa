namespace Application.UseCases.Categorias.Queries.BuscarCategoriaPorId
{
    public sealed record BuscarCategoriaPorIdResponse(
    int Id,
    string Nombre,
    string? UrlIcono
);

}
