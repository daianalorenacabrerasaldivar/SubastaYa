using Application.Interfaces.Persistencia.Lectura;
using Application.UseCases.Categorias.Queries.ListarCategorias;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/categories")]
    public sealed class CategoriesController : ApiControllerBase
    {
        private readonly ISender _sender;

        public CategoriesController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Retorna el listado completo de categorias disponibles.
        /// </summary>
        /// <remarks>
        /// Utilizado principalmente para poblar el dropdown de categorias
        /// en el formulario de publicacion de subasta.
        /// </remarks>
        /// <param name="cancellationToken">Token de cancelacion.</param>
        /// <response code="200">Lista de categorias ordenadas alfabeticamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CategoriaItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CategoriaItem>>> GetAll(
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetCategoriasQuery(), cancellationToken);
            return FromResult(result);
        }
    }
}
