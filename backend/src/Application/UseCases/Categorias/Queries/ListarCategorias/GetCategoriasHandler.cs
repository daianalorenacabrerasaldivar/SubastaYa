using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Categorias.Queries.ListarCategorias
{
    public sealed class GetCategoriasHandler : IRequestHandler<GetCategoriasQuery, Result<IReadOnlyList<CategoriaItem>>>
    {
        private readonly ICategoriaQueryRepository _categoriaRepository;

        public GetCategoriasHandler(ICategoriaQueryRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<Result<IReadOnlyList<CategoriaItem>>> Handle(
            GetCategoriasQuery request,
            CancellationToken cancellationToken)
        {
            var categorias = await _categoriaRepository.ListAllAsync(cancellationToken);

            var items = categorias
                .Select(c => new CategoriaItem(c.Id, c.Nombre, c.UrlIcono))
                .ToList();

            return new Success<IReadOnlyList<CategoriaItem>>(items);
        }
    }
}
