using System.Data;
using MiniAbp.Runtime;

namespace MiniAbp.Domain
{
    public class BaseService
    {
        protected YSession Session = YSession.GetInstance();
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }
    }
}
