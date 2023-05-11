using Entities.Models;

namespace ProjectWebNotes.FileManager
{
    public class ImagePost : ObjectFolder
    {
        public ImagePost() { }

        public ImagePost(string folderImage) : base(folderImage)
        {
        }

        public ImagePost(string folderImage, string folderRootDirectory) : base(folderImage, folderRootDirectory)
        {
        }

        public override string GetFolderImage()
        {
            return base.GetFolderImage();
        }

        public override string GetFolderRootDirectory()
        {
            return base.GetFolderRootDirectory();
        }
    }
}
