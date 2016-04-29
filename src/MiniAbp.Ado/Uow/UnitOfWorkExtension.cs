using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Domain.Uow;

namespace MiniAbp.Ado.Uow
{
    public static class UnitOfWorkExtension
    {
        public static IDbContext GetDbContext(this IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }
            var adoUow = unitOfWork as AdoUnitOfWork;
            if (adoUow == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }
            return adoUow.GetOrCreateDbContext();
        }

    }
}
