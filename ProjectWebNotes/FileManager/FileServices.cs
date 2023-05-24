using Contracts;
using Services.Abstractions;
using Services;

namespace ProjectWebNotes.FileManager
{
    public class FileServices : IFileServices
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly IHttpContextAccessor _contextAccessor;
       
        public FileServices(IWebHostEnvironment webHost , IHttpContextAccessor contextAccessor)
        {
            _webHost = webHost;
            _contextAccessor = contextAccessor;
        }


        public string PathRepresentWebRootPath(IObjectFolder objectFolder)
        {
            if (objectFolder != null)
            {
                return Path.Combine(_webHost.WebRootPath, objectFolder.GetFolderRootDirectory(), objectFolder.GetFileImage());
            }

            return null;
        }

        public  async Task<bool> CreateFileAsync(IObjectFolder objectFolder, IFormFile formFile, string fileName)
        {
            try
            {
                if (!Directory.Exists(PathRepresentWebRootPath(objectFolder)))
                {
                    Directory.CreateDirectory(PathRepresentWebRootPath(objectFolder));
                }

                var uniqueFileName = fileName;

                var uploads = PathRepresentWebRootPath(objectFolder);

                var filePath = Path.Combine(uploads, uniqueFileName);

                using (var stream = File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(IObjectFolder objectFolder, string Url)
        {

            var task = new Task<bool>(() => {

                try
                {
                    if (Url != null)
                    {
                        var path = Path.Combine(PathRepresentWebRootPath(objectFolder), Url);

                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            });

            task.Start();

            return await task;

        }

        public  string PathRepresentContentRootPath(IObjectFolder objectFolder)
        {
            if (objectFolder != null)
            {
                return Path.Combine(_webHost.ContentRootPath, objectFolder.GetFolderRootDirectory(), objectFolder.GetFileImage());
            }

            return null;
        }

        public  string HttpContextAccessorPathImgSrcIndex(IObjectFolder objectFolder, string UrlImg)
        {
            if (UrlImg != null && objectFolder != null)
            {
                return string.Format("{0}://{1}/{2}/{3}/{4}", _contextAccessor.HttpContext.Request.Scheme, _contextAccessor.HttpContext.Request.Host.ToString(), objectFolder.GetFolderRootDirectory(), objectFolder.GetFileImage(), UrlImg);
            }
            return null;
        }

        public static string GetUniqueFileName(string fileName)
        {

            return DateTime.Now.ToString("yymmssfff")
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 8)
                      + Path.GetExtension(fileName);
        }
    }
}
