namespace ProjectWebNotes.FileManager
{
    public class Avatar : ObjectFolder
    {
        private Avatar() { }

        private static Avatar _instance = null;

        public static Avatar GetAvartar()
        {
            if (_instance is null)
            {
                return new Avatar();
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
