using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace MiniAbp.Domain.Entitys
{
    /// <summary>
    /// 文件流对象
    /// </summary>
    [Serializable]
    public class FileStreamOutput : FileOutput, IDisposable
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }

        public FileStreamOutput(Stream stream, string contentType, string downloadName)
        {
            this.Stream = stream;
            ContentType = contentType;
            DownloadName = downloadName;
        }

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
    /// <summary>
    /// 文件路径对象
    /// </summary>
    [Serializable]
    public class FilePathOutput : FileOutput
    {
        public string Path { get; set; }
        public string ContentType { get; set; }
        public FilePathOutput(string path, string contentType, string downloadName)
        {
            this.Path = path;
            ContentType = contentType;
            DownloadName = downloadName;
        }
    }

    [Serializable]
    public abstract class FileOutput
    {
        public string DownloadName { get; set; }
    }
}
