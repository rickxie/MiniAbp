using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{
    public interface IUnitOfWorkCompleteHandle
    {
        void Complete();
    }
}