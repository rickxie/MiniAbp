using System.Collections.Generic;

namespace MiniAbp.Domain.Entitys
{
    public interface IFileUpload
    {
        /// <summary>
        /// file upload parameter
        /// </summary>
        /// <param name="input">所有文件列表</param>
        /// <returns></returns>
        List<FileModel> UploadFiles(FileInput input);
    }
}
