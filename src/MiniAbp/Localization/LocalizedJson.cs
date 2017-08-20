using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Domain.Entities;

namespace MiniAbp.Localization
{
    public class LocalizedJson
    {
        public string Culture { get; set; }
        public List<NameValue> Texts { get; set; } 
    }
}
