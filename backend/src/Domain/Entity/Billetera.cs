using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class Billetera : IEntity
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public decimal SaldoTotal { get; set; }

        public decimal SaldoRetenido { get; set; }

        public decimal SaldoDisponible { get; set; }

        public byte[] Version { get; set; } = Array.Empty<byte>();

        public Usuario Usuario { get; set; } = null!;

        public ICollection<TransaccionLedger> Transacciones { get; set; } = new List<TransaccionLedger>();

        public bool PuedeRetener(decimal monto)
        {
            return monto > 0 && SaldoDisponible >= monto;
        }

        public TransaccionLedger Retener(decimal monto, int subastaId, DateTime ahora)
        {
            if (!PuedeRetener(monto))
            {
                throw new InvalidOperationException("Saldo disponible insuficiente para retener el monto.");
            }

            SaldoRetenido += monto;
            SaldoDisponible = SaldoTotal - SaldoRetenido;

            return RegistrarMovimiento(TipoTransaccion.Retencion, monto, subastaId, ahora);
        }

        public bool PuedeLiberar(decimal monto)
        {
            return monto > 0 && monto <= SaldoRetenido;
        }

        public TransaccionLedger Liberar(decimal monto, int subastaId, DateTime ahora)
        {
            if (!PuedeLiberar(monto))
            {
                throw new InvalidOperationException("El monto a liberar supera el saldo retenido.");
            }

            SaldoRetenido -= monto;
            SaldoDisponible = SaldoTotal - SaldoRetenido;

            return RegistrarMovimiento(TipoTransaccion.Liberacion, monto, subastaId, ahora);
        }

        private TransaccionLedger RegistrarMovimiento(TipoTransaccion tipo, decimal monto, int subastaId, DateTime ahora)
        {
            var transaccion = new TransaccionLedger
            {
                BilleteraId = Id,
                Tipo = tipo,
                Monto = monto,
                Fecha = ahora,
                SubastaId = subastaId
            };

            Transacciones.Add(transaccion);
            return transaccion;
        }
    }
}
