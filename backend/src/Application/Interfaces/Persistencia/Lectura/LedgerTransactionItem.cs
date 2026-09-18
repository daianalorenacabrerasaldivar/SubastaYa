using Domain.Enum;

namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record LedgerTransactionItem(
        int Id,
        TipoTransaccion Tipo,
        decimal Monto,
        DateTime Fecha,
        int? SubastaId);
}
