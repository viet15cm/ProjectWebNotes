using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions
{
    public class CategoryIdNullException : BadRequestException
    {
        public CategoryIdNullException(string message) : base(message)
        {
        }
    }
}
