namespace ProjectWebNotes.FileManager
{
    public class BannerPost : ObjectFolder
    {
        private BannerPost() { }

        private static BannerPost _instance = null;

        public static BannerPost GetBannerPost()
        {
            if (_instance is null)
            {
                return new BannerPost();
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
