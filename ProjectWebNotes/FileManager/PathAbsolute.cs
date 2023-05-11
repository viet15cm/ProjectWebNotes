namespace ProjectWebNotes.FileManager
{
    public class PathAbsolute
    {
        public static string PathRepresentContentRootPath(IWebHostEnvironment webHostEnvironment, IObjectFolder objectFolder)
        {
            if (objectFolder != null)
            {
                return Path.Combine(webHostEnvironment.ContentRootPath, objectFolder.GetFolderRootDirectory(), objectFolder.GetFolderImage());
            }

            return null;
        }

        public static string HttpContextAccessorPathImgSrcIndex(IHttpContextAccessor httpContextAccessor, IObjectFolder objectFolder, string UrlImg)
        {
            if (UrlImg != null && objectFolder != null)
            {
                return string.Format("{0}://{1}/{2}/{3}/{4}", httpContextAccessor.HttpContext.Request.Scheme, httpContextAccessor.HttpContext.Request.Host.ToString(), objectFolder.GetFolderRootDirectory(), objectFolder.GetFolderImage() , UrlImg);
            }
            return null;
        }
    }
}
