using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using Domain.Entity;
using MediatR;

namespace Application.UseCases.Categorias.Queries.BuscarCategoriaPorId
{
    public sealed class BuscarCategoriaPorIdQueryHandler
     : IRequestHandler<
         BuscarCategoriaPorIdQuery,
         Result<BuscarCategoriaPorIdResponse>>
    {
        private readonly IRepositoryQuery _repositoryQuery;

        public BuscarCategoriaPorIdQueryHandler(
            IRepositoryQuery repositoryQuery)
        {
            _repositoryQuery = repositoryQuery;
        }

        public async Task<Result<BuscarCategoriaPorIdResponse>> Handle(
            BuscarCategoriaPorIdQuery request,
            CancellationToken cancellationToken)
        {
            var categoria = _repositoryQuery
            .Query<Categoria>()
            .FirstOrDefault(x => x.Id == request.CategoriaId);

            if (categoria is null)
            {
                return new Failed<BuscarCategoriaPorIdResponse>(
                    $"No existe la categoría con Id {request.CategoriaId}.",
                    DataStatus.NotFound);
            }

            var response = new BuscarCategoriaPorIdResponse(
                categoria.Id,
                categoria.Nombre,
                categoria.UrlIcono);

            return new Success<BuscarCategoriaPorIdResponse>(response);
        }
    }

}