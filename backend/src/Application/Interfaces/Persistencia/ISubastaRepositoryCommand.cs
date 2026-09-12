using Domain.Common.ResultPattern;
using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface ISubastaRepositoryCommand
    {
        void Add(Subasta subasta);

        void Update(Subasta subasta);

        void Remove(Subasta subasta);

    }

}
