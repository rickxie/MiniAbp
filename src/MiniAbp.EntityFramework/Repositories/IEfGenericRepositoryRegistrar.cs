using System;
using MiniAbp.Dependency;
using MiniAbp.Domain.Repositories;

namespace MiniAbp.EntityFramework.Repositories
{
    public interface IEfGenericRepositoryRegistrar
    {
        void RegisterForDbContext(Type dbContextType, IIocManager iocManager, AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute);
    }
}