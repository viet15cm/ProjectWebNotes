using Entities.Models;

namespace ProjectWebNotes.FileManager
{
    public class ImagePost : ObjectFolder
    {
        private ImagePost() { }

        private static ImagePost _instance;

        public static ImagePost GetImagePost()
        {
            if (_instance is null)
            {
                return new ImagePost();
            }

            return _instance;
        }
        public override string GetFileImage()
        {
            return base.GetFileImage();
        }

        public override string GetFolderRootDirectory()
        {
            return base.GetFolderRootDirectory();
        }
    }
}
