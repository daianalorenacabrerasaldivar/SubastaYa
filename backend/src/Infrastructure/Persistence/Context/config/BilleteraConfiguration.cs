using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Context.config
{
    public class BilleteraConfiguration : IEntityTypeConfiguration<Billetera>
    {
        public void Configure(EntityTypeBuilder<Billetera> builder)
        {
            builder.ToTable("billetera");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UsuarioId)
                .HasColumnName("usuario_id")
                .IsRequired();

            builder.Property(x => x.SaldoTotal)
                .HasColumnName("saldo_total")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.SaldoRetenido)
                .HasColumnName("saldo_retenido")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.SaldoDisponible)
                .HasColumnName("saldo_disponible")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Version)
                .HasColumnName("version")
                .IsRowVersion();

            builder.HasIndex(x => x.UsuarioId)
                .IsUnique();

            builder.HasMany(x => x.Transacciones)
                .WithOne(x => x.Billetera)
                .HasForeignKey(x => x.BilleteraId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
