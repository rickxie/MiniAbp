using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.DataAccess;

namespace MiniAbp.Configuration
{
    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
        public Dialect Dialect { get; set; }
    }
}
