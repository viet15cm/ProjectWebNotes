using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException(string message) : base(message)
        {
        }
    }
}
