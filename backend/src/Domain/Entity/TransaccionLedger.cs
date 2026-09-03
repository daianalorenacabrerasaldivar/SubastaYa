using Domain.Enum;

namespace Domain.Entity
{
    public class TransaccionLedger
    {
        public int Id { get; set; }

        public int BilleteraId { get; set; }

        public TipoTransaccion Tipo { get; set; }

        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; }

        public int? SubastaId { get; set; }

        // Navegaciones
        public Billetera Billetera { get; set; } = null!;

        public Subasta? Subasta { get; set; }
    }

}
