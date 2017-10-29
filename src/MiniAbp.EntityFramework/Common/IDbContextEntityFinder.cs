using System;
using System.Collections.Generic;
using MiniAbp.Domain.Entities;

namespace MiniAbp.EntityFramework
{
    public interface IDbContextEntityFinder
    {
        IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType);
    }
}