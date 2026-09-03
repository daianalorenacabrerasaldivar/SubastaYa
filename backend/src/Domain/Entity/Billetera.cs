namespace Domain.Entity
{
    public class Billetera
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public decimal SaldoTotal { get; set; }

        public decimal SaldoRetenido { get; set; }

        public decimal SaldoDisponible { get; set; }

        public int Version { get; set; }

        // Navegaciones
        public Usuario Usuario { get; set; } = null!;

        public ICollection<TransaccionLedger> Transacciones { get; set; } = new List<TransaccionLedger>();
    }

}
