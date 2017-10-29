using System;

namespace MiniAbp.EntityFramework
{
    public class EfBasedSecondaryOrmRegistrar : SecondaryOrmRegistrarBase
    {
        public EfBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
            : base(dbContextType, dbContextEntityFinder)
        {
        }

        public override string OrmContextKey => MAbpConsts.Orms.EntityFramework;
    }
}
