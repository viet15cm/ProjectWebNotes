using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions
{
    public class CategoryIdDuplicatePatentIdException : BadRequestException
    {
        public CategoryIdDuplicatePatentIdException(string message) : base(message)
        {
        }
    }
}
