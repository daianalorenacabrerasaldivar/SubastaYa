namespace Application.UseCases.Categorias.Queries.BuscarCategoriaPorId
{
    public sealed record GetCategoriaByIdResponse(int Id, string Nombre, string? UrlIcono );

}
