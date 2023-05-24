using System.Runtime.CompilerServices;

namespace ProjectWebNotes.FileManager
{
    public abstract class ObjectFolder  : IObjectFolder
    {

        public virtual string GetFileImage()
        {
          
            return this.GetType().Name;
            
        }

        public virtual string GetFolderRootDirectory()
        {  
                return "ImageManager";           
        }
    }
}
