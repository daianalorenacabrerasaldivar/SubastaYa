using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Context.config
{
    public class PujaConfiguration : IEntityTypeConfiguration<Puja>
    {
        public void Configure(EntityTypeBuilder<Puja> builder)
        {
            builder.ToTable("puja");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.SubastaId)
                .HasColumnName("subasta_id")
                .IsRequired();

            builder.Property(x => x.CompradorId)
                .HasColumnName("comprador_id")
                .IsRequired();

            builder.Property(x => x.Monto)
                .HasColumnName("monto")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.FechaPuja)
                .HasColumnName("fecha_puja")
                .IsRequired();

            builder.HasOne(x => x.Subasta)
                .WithMany(x => x.Pujas)
                .HasForeignKey(x => x.SubastaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Comprador)
                .WithMany(x => x.Pujas)
                .HasForeignKey(x => x.CompradorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SubastaId);
            builder.HasIndex(x => x.CompradorId);
            builder.HasIndex(x => new { x.SubastaId, x.FechaPuja });
        }
    }
}
