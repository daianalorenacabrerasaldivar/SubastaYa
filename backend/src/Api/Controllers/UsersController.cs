using Application.Interfaces.Persistencia.Lectura;
using Application.UseCases.Usuarios.Query.MisPujas;
using Application.UseCases.Usuarios.Query.MisSubastas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/v1/users")]
    public sealed class UsersController : ApiControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Retorna las subastas en las que el usuario ha pujado (Mis Pujas).
        /// </summary>
        /// <remarks>
        /// Para cada subasta incluye la mejor puja del usuario, la oferta actual del lider,
        /// el estado de la subasta y si el usuario es el lider actual.
        /// </remarks>
        /// <param name="id">Id del usuario.</param>
        /// <param name="cancellationToken">Token de cancelacion.</param>
        /// <response code="200">Lista de actividad de pujas del usuario.</response>
        /// <response code="404">No existe un usuario con el Id indicado.</response>
        [HttpGet("{id}/bids")]
        [ProducesResponseType(typeof(IReadOnlyList<UserBidActivityItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<UserBidActivityItem>>> GetBids(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetUserBidActivityQuery(id), cancellationToken);
            return FromResult(result);
        }

        /// <summary>
        /// Retorna las subastas publicadas por el usuario (Mis Subastas).
        /// </summary>
        /// <remarks>
        /// Para cada subasta incluye el estado, la oferta actual, la cantidad de pujas
        /// y el rango de fechas de la subasta.
        /// </remarks>
        /// <param name="id">Id del usuario.</param>
        /// <param name="cancellationToken">Token de cancelacion.</param>
        /// <response code="200">Lista de subastas publicadas por el usuario.</response>
        /// <response code="404">No existe un usuario con el Id indicado.</response>
        [HttpGet("{id}/auctions")]
        [ProducesResponseType(typeof(IReadOnlyList<UserAuctionActivityItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<UserAuctionActivityItem>>> GetAuctions(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetUserAuctionsQuery(id), cancellationToken);
            return FromResult(result);
        }
    }
}
