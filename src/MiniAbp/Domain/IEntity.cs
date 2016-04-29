using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
