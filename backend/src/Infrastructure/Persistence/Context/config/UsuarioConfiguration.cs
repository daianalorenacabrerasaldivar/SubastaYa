using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Context.config
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuario");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.FechaRegistro)
                .HasColumnName("fecha_registro")
                .IsRequired();

            // Usuario 1 ---- N Subastas
            builder.HasMany(x => x.Subastas)
                .WithOne(x => x.Vendedor)
                .HasForeignKey(x => x.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario 1 ---- 1 Billetera
            builder.HasOne(x => x.Billetera)
                .WithOne(x => x.Usuario)
                .HasForeignKey<Billetera>(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario 1 ---- N Puja
            builder.HasMany(x => x.Pujas)
                .WithOne(x => x.Comprador)
                .HasForeignKey(x => x.CompradorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario 1 ---- N AuditoriaLog
            // usuario_id es opcional
            builder.HasMany(x => x.AuditoriaLogs)
                .WithOne(x => x.Usuario)
                .HasForeignKey(x => x.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
