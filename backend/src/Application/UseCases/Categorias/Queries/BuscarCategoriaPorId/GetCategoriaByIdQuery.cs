using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Categorias.Queries.BuscarCategoriaPorId
{
    public sealed record GetCategoriaByIdQuery(int CategoriaId) : IRequest<Result<GetCategoriaByIdResponse>>;

}
