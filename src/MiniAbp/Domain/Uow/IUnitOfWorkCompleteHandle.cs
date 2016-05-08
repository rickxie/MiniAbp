using System;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        void Complete();
    }
}