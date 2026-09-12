using Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistencia
{
    public interface IUnitOfWork
    {
        Task<Result<string>> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }

}
