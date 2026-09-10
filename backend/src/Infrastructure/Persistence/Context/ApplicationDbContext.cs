using Domain.Entity;
using Infrastructure.Persistence.Context.config;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Subasta> Subastas => Set<Subasta>();
        public DbSet<Billetera> Billeteras => Set<Billetera>();
        public DbSet<Puja> Pujas => Set<Puja>();
        public DbSet<TransaccionLedger> TransaccionesLedger => Set<TransaccionLedger>();
        public DbSet<AuditoriaLog> AuditoriaLogs => Set<AuditoriaLog>();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }
        private static void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        }
    }
}
