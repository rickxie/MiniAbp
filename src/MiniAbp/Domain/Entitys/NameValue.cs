namespace MiniAbp.Domain.Entitys
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
}
