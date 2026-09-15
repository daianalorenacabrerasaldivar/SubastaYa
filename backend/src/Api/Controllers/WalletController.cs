using Application.UseCases.Billetera.Command.Depositar;
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
        /// Acredita saldo ficticio en la billetera del usuario.
        /// </summary>
        /// <remarks>
        /// El monto se agrega al SaldoTotal y al SaldoDisponible.
        /// Escribe un movimiento de tipo Deposito en el Ledger y un registro en AuditoriaLog.
        /// Monto maximo por operacion: $1.000.000.
        /// </remarks>
        /// <param name="command">Id del usuario y monto a acreditar.</param>
        /// <param name="cancellationToken">Token de cancelacion.</param>
        /// <response code="201">Deposito registrado. Retorna los saldos actualizados.</response>
        /// <response code="400">Datos invalidos (monto cero, Id negativo, monto mayor al maximo).</response>
        /// <response code="404">No existe billetera para el usuario indicado.</response>
        [HttpPost("deposit")]
        [ProducesResponseType(typeof(DepositResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepositResponse>> Deposit(
            [FromBody] DepositCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return FromResult(result, StatusCodes.Status201Created);
        }
    }
}
