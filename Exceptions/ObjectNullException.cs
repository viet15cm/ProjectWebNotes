using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions
{
    public class ObjectNullException : BadRequestException
    {
        public ObjectNullException(string message) : base(message)
        {
        }
    }
    

    
}
