using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yooya.Bpm.Framework.Domain.Entity
{
    public class IdInput<T> : IInputDto
    {
        public T Id { get; set; }
    }
}
