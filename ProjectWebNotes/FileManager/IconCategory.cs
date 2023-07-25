namespace ProjectWebNotes.FileManager
{
    public class IconCategory : ObjectFolder
    {
        private IconCategory() { }

        private static IconCategory _instance = null;

        public static IconCategory GetIcon()
        {
            if (_instance is null)
            {
                return new IconCategory();
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
