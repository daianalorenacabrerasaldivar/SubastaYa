using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Categorias.Queries.ListarCategorias
{
    public sealed record GetCategoriasQuery() : IRequest<Result<IReadOnlyList<CategoriaItem>>>;
}
