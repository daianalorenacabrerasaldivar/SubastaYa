using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Billetera.Query.ListarTransacciones
{
    public sealed class ListWalletTransactionsHandler : IRequestHandler<ListWalletTransactionsQuery, Result<IReadOnlyList<LedgerTransactionItem>>>
    {
        private readonly IBilleteraQueryRepository _billeteraRepository;

        public ListWalletTransactionsHandler(IBilleteraQueryRepository billeteraRepository)
        {
            _billeteraRepository = billeteraRepository;
        }

        public async Task<Result<IReadOnlyList<LedgerTransactionItem>>> Handle(ListWalletTransactionsQuery request, CancellationToken cancellationToken)
        {
            var balance = await _billeteraRepository.GetBalanceByUsuarioIdAsync(request.UsuarioId, cancellationToken);

            if (balance is null)
                return new Failed<IReadOnlyList<LedgerTransactionItem>>(
                    $"No existe una billetera para el usuario con Id {request.UsuarioId}.",
                    DataStatus.NotFound);

            var transacciones = await _billeteraRepository.ListTransactionsByUsuarioIdAsync(request.UsuarioId, cancellationToken);

            return new Success<IReadOnlyList<LedgerTransactionItem>>(transacciones);
        }
    }
}
