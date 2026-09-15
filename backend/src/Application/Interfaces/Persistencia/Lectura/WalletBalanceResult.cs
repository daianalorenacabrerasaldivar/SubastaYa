namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record WalletBalanceResult(
        int UsuarioId,
        decimal SaldoTotal,
        decimal SaldoRetenido,
        decimal SaldoDisponible);
}
