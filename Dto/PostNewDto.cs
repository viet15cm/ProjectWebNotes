using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class PostNewDto : PostDto
    {
        public string AuthorName { get; set; }

        public string IconCategory { get; set; }
    }
}
