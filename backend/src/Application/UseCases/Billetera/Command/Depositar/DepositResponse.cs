namespace Application.UseCases.Billetera.Command.Depositar
{
    public sealed record DepositResponse(
        int UsuarioId,
        decimal MontoAcreditado,
        decimal SaldoTotal,
        decimal SaldoRetenido,
        decimal SaldoDisponible,
        DateTime Fecha);
}
