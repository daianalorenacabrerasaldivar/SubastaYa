using Application.Interfaces.Persistencia.Lectura;
using Application.UseCases.Billetera.Command.Depositar;
using Application.UseCases.Billetera.Query.ConsultarSaldo;
using Application.UseCases.Billetera.Query.ListarTransacciones;
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
        /// Consulta el saldo de la billetera de un usuario.
        /// </summary>
        /// <param name="usuarioId">Id del usuario.</param>
        /// <param name="cancellationToken">Token de cancelacion.</param>
        /// <response code="200">Saldo total, retenido y disponible del usuario.</response>
        /// <response code="404">No existe billetera para el usuario indicado.</response>
        [HttpGet("balance")]
        [ProducesResponseType(typeof(WalletBalanceResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WalletBalanceResult>> GetBalance(
            [FromQuery] int usuarioId,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetWalletBalanceQuery(usuarioId), cancellationToken);
            return FromResult(result);
        }

        /// <summary>
        /// Retorna el historial de movimientos del Ledger de un usuario.
        /// </summary>
        /// <remarks>
        /// Incluye depositos, retenciones, liberaciones, pagos y cobros, ordenados por fecha descendente.
        /// </remarks>
        /// <param name="usuarioId">Id del usuario.</param>
        /// <param name="cancellationToken">Token de cancelacion.</param>
        /// <response code="200">Lista de transacciones del Ledger.</response>
        /// <response code="404">No existe billetera para el usuario indicado.</response>
        [HttpGet("transactions")]
        [ProducesResponseType(typeof(IReadOnlyList<LedgerTransactionItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<LedgerTransactionItem>>> GetTransactions(
            [FromQuery] int usuarioId,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new ListWalletTransactionsQuery(usuarioId), cancellationToken);
            return FromResult(result);
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
