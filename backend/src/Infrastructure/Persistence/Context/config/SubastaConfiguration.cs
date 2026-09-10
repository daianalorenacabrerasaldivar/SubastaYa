using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Context.config
{
    public class SubastaConfiguration : IEntityTypeConfiguration<Subasta>
    {
        public void Configure(EntityTypeBuilder<Subasta> builder)
        {
            builder.ToTable("subasta");
            // Clave primaria
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

            // Propiedades 
            builder.Property(x => x.Titulo)
                .HasColumnName("titulo")
               .HasMaxLength(200)
               .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasColumnName("descripcion")
                .HasMaxLength(1000);

            builder.Property(x => x.UrlImagen)
                .HasColumnName("url_imagen")
                .HasMaxLength(500);

            builder.Property(x => x.PrecioBase)
                 .HasColumnName("precio_base")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.IncrementoMinimo)
                .HasColumnName("incremento_minimo")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.FechaInicio)
                .HasColumnName("fecha_inicio")
                .IsRequired();

            builder.Property(x => x.FechaFin)
                .HasColumnName("fecha_fin")
                .IsRequired();

            builder.Property(x => x.Estado)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
             .HasComment("Estado actual de la subasta (PROGRAMADA, ACTIVA, FINALIZADA, DESIERTA)");

            // Optimistic Locking - Control de concurrencia
            builder.Property(x => x.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired();




            // Relación: Vendedor (Usuario) 1 -> N Subasta
            builder.HasOne(x => x.Vendedor)
                .WithMany(x => x.Subastas)
                .HasForeignKey(x => x.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);
           

            // Relación: Categoría -> Subasta
            builder.HasOne(x => x.Categoria)
                .WithMany(x => x.Subastas)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
         

            // Relación: Subasta -> Pujas
            builder.HasMany(x => x.Pujas)
                .WithOne(p => p.Subasta)
                .HasForeignKey(p => p.SubastaId)
                .OnDelete(DeleteBehavior.Cascade);
          
            // Relación: Subasta -> Transacciones
            builder.HasMany(x => x.Transacciones)
                .WithOne(t => t.Subasta)
                .HasForeignKey(t => t.SubastaId)
                .OnDelete(DeleteBehavior.Cascade);
            //.HasConstraintName("FK_TRANSACCION_SUBASTA");

            // Índices para optimización de consultas


            builder.HasIndex(x => x.VendedorId);
            

            builder.HasIndex(x => x.CategoriaId);

            builder.HasIndex(x => x.Estado);
            //.HasName("IX_SUBASTA_ESTADO");

            builder.HasIndex(x => x.FechaInicio);
            //.HasName("IX_SUBASTA_FECHA_INICIO");

            builder.HasIndex(x => x.FechaFin);
            //.HasName("IX_SUBASTA_FECHA_FIN");
        }
    }
}
