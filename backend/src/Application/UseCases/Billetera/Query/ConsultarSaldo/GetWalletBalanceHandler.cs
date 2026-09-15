using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Billetera.Query.ConsultarSaldo
{
    public sealed class GetWalletBalanceHandler : IRequestHandler<GetWalletBalanceQuery, Result<WalletBalanceResult>>
    {
        private readonly IBilleteraQueryRepository _billeteraRepository;

        public GetWalletBalanceHandler(IBilleteraQueryRepository billeteraRepository)
        {
            _billeteraRepository = billeteraRepository;
        }

        public async Task<Result<WalletBalanceResult>> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            var balance = await _billeteraRepository.GetBalanceByUsuarioIdAsync(request.UsuarioId, cancellationToken);

            if (balance is null)
                return new Failed<WalletBalanceResult>(
                    $"No existe una billetera para el usuario con Id {request.UsuarioId}.",
                    DataStatus.NotFound);

            return new Success<WalletBalanceResult>(balance);
        }
    }
}
