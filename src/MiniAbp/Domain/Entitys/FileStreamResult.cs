using System.Collections.Generic;
using System.IO;
using System.Web;

namespace MiniAbp.Domain.Entitys
{
    /// <summary>
    /// 文件流对象
    /// </summary>
    public class FileStreamOutput : FileOutput
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }

        public FileStreamOutput(Stream stream, string contentType, string downloadName)
        {
            this.Stream = stream;
            ContentType = contentType;
            DownloadName = downloadName;
        }
    }
    /// <summary>
    /// 文件路径对象
    /// </summary>
    public class FilePathOutput : FileOutput
    {
        public string Path { get; set; }
        public string ContentType { get; set; }
        public FilePathOutput(string stream, string contentType, string downloadName)
        {
            this.Path = stream;
            ContentType = contentType;
            DownloadName = downloadName;
        }
    }

    public abstract class FileOutput
    {
        public string DownloadName { get; set; }
    }
}
