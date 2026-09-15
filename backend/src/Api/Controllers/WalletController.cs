using Application.UseCases.Billetera.Query.ConsultarSaldo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/wallet")]
    public sealed class WalletController : ApiControllerBase
    {
        private readonly ISender _sender;

        public WalletController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Consulta el desglose de saldos de la billetera de un usuario.
        /// </summary>
        /// <remarks>
        /// Retorna tres métricas: SaldoTotal (fondos totales depositados),
        /// SaldoRetenido (monto bloqueado en garantía por subastas activas donde el usuario lidera)
        /// y SaldoDisponible (SaldoTotal - SaldoRetenido), único dinero habilitado para nuevas pujas.
        /// </remarks>
        /// <param name="usuarioId">Id del usuario dueño de la billetera.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Desglose de saldos de la billetera.</response>
        /// <response code="404">No existe billetera para el usuario indicado.</response>
        [HttpGet("balance")]
        [ProducesResponseType(typeof(GetWalletBalanceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetWalletBalanceResponse>> GetBalance(
            [FromQuery] int usuarioId,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetWalletBalanceQuery(usuarioId), cancellationToken);
            return FromResult(result);
        }
    }
}
