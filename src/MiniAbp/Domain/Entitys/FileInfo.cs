using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Entitys
{
    public class FileInfo
    {
        public string FileName { get; set; }
        public string ExtensionName { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public byte[] FileBytes { get; set; }

        public void SaveAs(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            fs.Write(FileBytes, 0, ContentLength);
            fs.Close();
        }

    }
}
