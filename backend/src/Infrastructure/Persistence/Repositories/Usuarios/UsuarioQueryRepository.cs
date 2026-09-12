using Application.Interfaces.Persistencia;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories.Usuarios
{
    public sealed class UsuarioQueryRepository : EfQueryRepository<Usuario>, IUsuarioQueryRepository
    {
        public UsuarioQueryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
