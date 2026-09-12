using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Context.config
{
    public class TransaccionLedgerConfiguration : IEntityTypeConfiguration<TransaccionLedger>
    {
        public void Configure(EntityTypeBuilder<TransaccionLedger> builder)
        {
            builder.ToTable("transaccion_ledger");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.BilleteraId)
                .HasColumnName("billetera_id")
                .IsRequired();

            builder.Property(x => x.Tipo)
                .HasColumnName("tipo")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Monto)
                .HasColumnName("monto")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Fecha)
                .HasColumnName("fecha")
                .IsRequired();

            builder.Property(x => x.SubastaId)
                .HasColumnName("subasta_id");

            builder.HasOne(x => x.Billetera)
                .WithMany(x => x.Transacciones)
                .HasForeignKey(x => x.BilleteraId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Subasta)
                .WithMany(x => x.Transacciones)
                .HasForeignKey(x => x.SubastaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.BilleteraId);
            builder.HasIndex(x => x.SubastaId);
            builder.HasIndex(x => x.Fecha);
        }
    }
}
