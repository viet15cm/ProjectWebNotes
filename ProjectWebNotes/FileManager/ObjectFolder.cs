using System.Runtime.CompilerServices;

namespace ProjectWebNotes.FileManager
{
    public abstract class ObjectFolder : IObjectFolder
    {
        public string FolderImage { get; set; }

        public string FolderRootDirectory { get; set; }

        public ObjectFolder() { }
        
        public ObjectFolder(string folderImage)
        {
            FolderImage = folderImage;    
        }

        public ObjectFolder(string folderImage, string folderRootDirectory)
        {
            FolderImage = folderImage;
            FolderRootDirectory = folderRootDirectory;
        }

        public virtual string GetFolderImage()
        {
            if (FolderImage is null)
            {
                return this.GetType().Name;
            }

            return FolderImage;
        }

        public virtual string GetFolderRootDirectory()
        {
            if (FolderRootDirectory is null)
            {
                return "ImageManager";
            }

            return FolderRootDirectory;
        }
    }
}
