using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Context.config
{
    public class AuditoriaLogConfiguration : IEntityTypeConfiguration<AuditoriaLog>
    {
        public void Configure(EntityTypeBuilder<AuditoriaLog> builder)
        {
            builder.ToTable("auditoria_log");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Entidad)
                .HasColumnName("entidad")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.EntidadId)
                .HasColumnName("entidad_id")
                .IsRequired();

            builder.Property(x => x.Accion)
                .HasColumnName("accion")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.UsuarioId)
                .HasColumnName("usuario_id");

            builder.Property(x => x.DetalleJson)
                .HasColumnName("detalle_json")
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Fecha)
                .HasColumnName("fecha")
                .IsRequired();

            builder.HasOne(x => x.Usuario)
                .WithMany(x => x.AuditoriaLogs)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => new { x.Entidad, x.EntidadId });
            builder.HasIndex(x => x.UsuarioId);
            builder.HasIndex(x => x.Fecha);
        }
    }
}
