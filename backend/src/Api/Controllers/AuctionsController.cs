using Application.Common;
using Application.Interfaces.Persistencia.Lectura;
using Application.UseCases.Subastas.Command.Creacion;
using Application.UseCases.Subastas.Query.GetAuctionById;
using Application.UseCases.Subastas.Query.Listar;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/auctions")]
    public sealed class AuctionsController : ApiControllerBase
    {
        private readonly ISender _sender;

        public AuctionsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Lista subastas con filtros, orden y paginación.
        /// </summary>
        /// <remarks>
        /// Filtros: status (Activas, Proximas, Finalizadas), categoryId, minPrice y maxPrice sobre el precio actual.
        /// Orden: sortBy TiempoRestante (default) o MayorPuja. Paginación: page desde 1, pageSize hasta 100.
        /// </remarks>
        /// <param name="query">Filtros, orden y paginación.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Página de subastas con metadata de paginación.</response>
        /// <response code="400">Parámetros de filtro o paginación inválidos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<AuctionListItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<AuctionListItem>>> GetAuctions(
            [FromQuery] ListAuctionsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return FromResult(result);
        }

        /// <summary>
        /// Crea y publica una subasta como vendedor.
        /// </summary>
        /// <remarks>
        /// Una fecha de inicio futura crea la subasta como PROGRAMADA; en otro caso se crea como ACTIVA.
        /// </remarks>
        /// <param name="command">Datos de la subasta a crear.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="201">La subasta fue creada correctamente.</response>
        /// <response code="400">Los datos enviados no cumplen las validaciones.</response>
        /// <response code="404">El vendedor o la categoría no existen.</response>
        /// <response code="409">Conflicto de estado o de concurrencia.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AuctionDetailResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuctionDetailResponse>> CreateAuction(
            [FromBody] CreateAuctionCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return FromResult(result, StatusCodes.Status201Created);
        }

        /// <summary>
        /// Detalle completo de una subasta con su estado y puja actual.
        /// </summary>
        /// <param name="id">Id de la subasta.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Detalle de la subasta.</response>
        /// <response code="404">La subasta no existe.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(GetAuctionByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetAuctionByIdResponse>> GetAuctionById(int id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAuctionByIdQuery(id), cancellationToken);
            return FromResult(result);
        }
    }
}
