using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MiniAbp.Domain.Entitys;

namespace MiniAbp.Web.Mvc
{
    public static class FileOutputExtension
    {
        /// <summary>
        /// 输出文件
        /// </summary>
        /// <param name="response"></param>
        /// <param name="fileStream"></param>
        public static void WriteFile(this HttpResponse response, FileStreamOutput fileStream)
        {
            using (fileStream)
            {
                Stream outputStream = response.OutputStream;
                using (fileStream.Stream)
                {
                    byte[] buffer = new byte[4096];
                    while (true)
                    {
                        int count = fileStream.Stream.Read(buffer, 0, 4096);
                        if (count != 0)
                            outputStream.Write(buffer, 0, count);
                        else
                            break;
                    }
                }
            }
        }
    }
}
