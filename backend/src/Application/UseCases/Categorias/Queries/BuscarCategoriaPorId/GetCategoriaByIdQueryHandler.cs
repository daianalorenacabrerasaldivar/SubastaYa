using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Categorias.Queries.BuscarCategoriaPorId
{
    public sealed class GetCategoriaByIdQueryHandler
     : IRequestHandler<
         GetCategoriaByIdQuery,
         Result<GetCategoriaByIdResponse>>
    {
        private readonly ICategoriaQueryRepository _categoriaRepository;

        public GetCategoriaByIdQueryHandler(
            ICategoriaQueryRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<Result<GetCategoriaByIdResponse>> Handle(
            GetCategoriaByIdQuery request,
            CancellationToken cancellationToken)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(
                request.CategoriaId,
                cancellationToken);

            if (categoria is null)
            {
                return new Failed<GetCategoriaByIdResponse>(
                    $"No existe la categoría con Id {request.CategoriaId}.",
                    DataStatus.NotFound);
            }

            var response = new GetCategoriaByIdResponse(
                categoria.Id,
                categoria.Nombre,
                categoria.UrlIcono);

            return new Success<GetCategoriaByIdResponse>(response);
        }
    }

}