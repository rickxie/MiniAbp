using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace MiniAbp.Domain.Entitys
{
    public class FileInput
    {
        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileInfo> Files { get; set; }
        /// <summary>
        /// 表单数据
        /// </summary>
        public NameValueCollection Form { get; set; }
    }

  
}
