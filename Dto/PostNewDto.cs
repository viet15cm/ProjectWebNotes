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

        public string TitleCategory { get; set; }
        public string IconCategory { get; set; }

        public string SlugCategory { get; set; }

        public string DescriptionCollapse { get; set; }
    }
}
