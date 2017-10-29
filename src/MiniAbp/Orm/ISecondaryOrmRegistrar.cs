using MiniAbp.Dependency;
using MiniAbp.Domain.Repositories;

namespace MiniAbp.Orm
{
    public interface ISecondaryOrmRegistrar
    {
        string OrmContextKey { get; }

        void RegisterRepositories(IIocManager iocManager, AutoRepositoryTypesAttribute defaultRepositoryTypes);
    }
}
