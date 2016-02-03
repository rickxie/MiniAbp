namespace Yooya.Bpm.Framework.Domain.Entity
{
    public interface IFileUpload
    {
        /// <summary>
        /// file upload parameter
        /// </summary>
        /// <param name="input">所有文件列表</param>
        /// <returns></returns>
        bool UploadFiles(FileInput input);
    }
}
