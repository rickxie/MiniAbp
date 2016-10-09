using System.Collections.Generic;
using System.Linq;

namespace MiniAbp.Domain.Entitys
{
    public class PageInput : IPaging
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string OrderByProperty { get; set; }
        public bool Ascending { get; set; }
        public List<NameValue> Filters { get; set; }

        public string this[string name]
        {
            get
            {
                var item = Filters?.FirstOrDefault(r => r.Name == name);
                if (item?.Value == null)
                {
                    return null;
                }
                return item.Value.Replace("'", "''");
            }
        }

        public NameValue Find(string name)
        {
            return Filters.FirstOrDefault(r => r.Name.Equals(name));
        }
    }
}
