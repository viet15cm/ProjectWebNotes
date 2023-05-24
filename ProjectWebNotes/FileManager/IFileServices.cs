namespace ProjectWebNotes.FileManager
{
    public interface  IFileServices
    {
        string PathRepresentWebRootPath(IObjectFolder objectFolder);
        string PathRepresentContentRootPath(IObjectFolder objectFolder);

        string HttpContextAccessorPathImgSrcIndex(IObjectFolder objectFolder, string UrlImg);

        Task<bool> CreateFileAsync(IObjectFolder objectFolder, IFormFile formFile, string fileName);

        Task<bool> DeleteFileAsync(IObjectFolder objectFolder, string Url);
    }
}
