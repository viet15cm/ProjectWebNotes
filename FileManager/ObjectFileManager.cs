using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class ObjectFileManager
    {

        private static ObjectFileManager FileManager = null;
        private ObjectFileManager()
        {

        }
        public static ObjectFileManager Instance()
        {
            if (FileManager == null)
            {
                return new ObjectFileManager();
            }

            return FileManager;
        }
    }
}
