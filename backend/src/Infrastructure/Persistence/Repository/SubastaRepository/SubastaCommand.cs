using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repository
{
   public class SubastaRepositoryCommand: ISubastaRepositoryCommand
    {
        private readonly IRepositoryCommand _repositoryCommand;

        public SubastaRepositoryCommand(IRepositoryCommand repositoryCommand)
        {
            _repositoryCommand = repositoryCommand;
        }
        public void Add(Subasta subasta)
        {
            _repositoryCommand.Add(subasta);
        }

        public void Update(Subasta subasta)
        {
            _repositoryCommand.Update(subasta);
        }

        public void Remove(Subasta subasta)
        {
            _repositoryCommand.Remove(subasta);
        }

    }

}
