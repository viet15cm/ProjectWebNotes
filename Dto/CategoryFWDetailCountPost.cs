using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class CategoryFWDetailCountPost : CategoryDto
    {
        public int CountPost { get; set; }

        public ICollection<CategoryFWDetailCountPost> CategoryChildren { get; set; }
    }
}
