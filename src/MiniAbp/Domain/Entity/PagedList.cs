using System.Collections.Generic;

namespace Yooya.Bpm.Framework.Domain.Entity
{
    public class PagedList<T> 
    {
        public List<T> Model { get; set; }
        public int TotalCount { get; set; }
        /// <summary>
        /// 用以以JSON格式返回一些辅助数据
        /// </summary>
        public string OptionalOutputs { get; set; }
    }
}
