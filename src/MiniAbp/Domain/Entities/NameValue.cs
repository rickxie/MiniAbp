using System.Collections.Generic;
using System.Linq;

namespace MiniAbp.Domain.Entities
{
    public class NameValue
    {
        /// <summary>
        /// Name.
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value.
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="T:Abp.NameValue"/>.
        /// 
        /// </summary>
        public NameValue()
        {
        }

        /// <summary>
        /// Creates a new <see cref="T:Abp.NameValue"/>.
        /// 
        /// </summary>
        public NameValue(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public class NameValueList: List<NameValue>
    {
        /// <summary>
        /// 根据索引获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                var item = this?.FirstOrDefault(r => r.Name == name);
                if (string.IsNullOrWhiteSpace(item?.Value))
                {
                    item = null;
                }
                return item?.Value;
            }
        }
    }
}
