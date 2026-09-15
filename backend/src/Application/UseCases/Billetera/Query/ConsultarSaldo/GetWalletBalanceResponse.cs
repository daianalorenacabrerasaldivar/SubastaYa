namespace Application.UseCases.Billetera.Query.ConsultarSaldo
{
    public sealed record GetWalletBalanceResponse(
        int UsuarioId,
        decimal SaldoTotal,
        decimal SaldoRetenido,
        decimal SaldoDisponible);
}
