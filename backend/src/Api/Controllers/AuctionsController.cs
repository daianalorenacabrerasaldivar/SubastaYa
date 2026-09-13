using Application.UseCases.Subastas.Command.Creacion;
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
        /// Crea y publica una subasta como vendedor.
        /// </summary>
        /// <remarks>
        /// Una fecha de inicio futura crea la subasta como PROGRAMADA; en otro caso se crea como ACTIVA.
        /// </remarks>
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
    }
}
