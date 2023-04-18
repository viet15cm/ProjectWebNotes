using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dto
{
    public class PostsFWDImagesDto : PostDto
    {
        public ICollection<ImageDto> Images { get; set; }
    }
}
