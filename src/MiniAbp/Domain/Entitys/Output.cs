using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Entitys
{
    public class Output<T>
    {
        public  T Data { get; set; }
    }
    public class Input<T>
    {
        public  T Data { get; set; }
    }
}
