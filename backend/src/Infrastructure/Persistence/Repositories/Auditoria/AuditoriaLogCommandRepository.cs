using Application.Interfaces.Persistencia;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories.Auditoria
{
    public sealed class AuditoriaLogCommandRepository : EfCommandRepository<AuditoriaLog>, IAuditoriaLogCommandRepository
    {
        public AuditoriaLogCommandRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
