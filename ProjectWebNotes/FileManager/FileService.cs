using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebNotes.FileManager
{
    public static class FileService
    {

        private static string PathRepresentWebRootPath(IWebHostEnvironment webHostEnvironment, IObjectFolder objectFolder)
        {
            if (objectFolder != null)
            {
                return Path.Combine(webHostEnvironment.WebRootPath, objectFolder.GetFolderRootDirectory(), objectFolder.GetFolderImage());
            }

            return null;
        }

        public static async Task<bool> CreateFileAsync(IWebHostEnvironment webHostEnvironment , IObjectFolder objectFolder, IFormFile formFile, string fileName)
        {
            try
            {
                if (!Directory.Exists(PathRepresentWebRootPath(webHostEnvironment, objectFolder)))
                {
                    Directory.CreateDirectory(PathRepresentWebRootPath(webHostEnvironment, objectFolder));
                }

                var uniqueFileName = fileName;

                var uploads = PathRepresentWebRootPath(webHostEnvironment, objectFolder);

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

        public static async Task<bool> DeleteFileAsync(IWebHostEnvironment webHostEnvironment, IObjectFolder objectFolder ,  string Url)
        {

            var task = new Task<bool>(() => {

                try
                {
                    if (Url != null)
                    {
                        var path = Path.Combine(PathRepresentWebRootPath(webHostEnvironment, objectFolder), Url);

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

        public static string GetUniqueFileName(string fileName)
        {

            return DateTime.Now.ToString("yymmssfff")
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 8)
                      + Path.GetExtension(fileName);
        }
    }
}
