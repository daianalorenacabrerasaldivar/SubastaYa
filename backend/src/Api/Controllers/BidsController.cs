using Application.UseCases.Pujas.Command.Ofertar;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/auctions/{auctionId:int}/bids")]
    public sealed class BidsController : ApiControllerBase
    {
        private readonly ISender _sender;

        public BidsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Registra una oferta en la subasta.
        /// </summary>
        /// <remarks>
        /// Retiene el monto en la billetera del postor y libera la retención del líder anterior en una sola transacción.
        /// Una oferta en los últimos 60 segundos extiende el cierre 2 minutos (anti-sniping).
        /// </remarks>
        /// <param name="auctionId">Id de la subasta.</param>
        /// <param name="command">Comprador y monto de la oferta.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="201">Oferta registrada.</response>
        /// <response code="400">Datos inválidos.</response>
        /// <response code="404">La subasta o la billetera no existen.</response>
        /// <response code="409">La subasta no está abierta o hubo un conflicto de concurrencia.</response>
        /// <response code="422">Saldo insuficiente, monto menor al mínimo, el vendedor ofertando o el postor ya lidera.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PlaceBidResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PlaceBidResponse>> PlaceBid(
            int auctionId,
            [FromBody] PlaceBidCommand command,
            CancellationToken cancellationToken)
        {
            command.SubastaId = auctionId;
            var result = await _sender.Send(command, cancellationToken);
            return FromResult(result, StatusCodes.Status201Created);
        }
    }
}
