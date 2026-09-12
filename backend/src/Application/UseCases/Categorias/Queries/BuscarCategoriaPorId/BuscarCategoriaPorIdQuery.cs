using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Categorias.Queries.BuscarCategoriaPorId
{
    public sealed record BuscarCategoriaPorIdQuery(
    int CategoriaId
) : IRequest<Result<BuscarCategoriaPorIdResponse>>;

}
